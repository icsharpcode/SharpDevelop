// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Debugger;
using Debugger.AddIn.Pads.ParallelPad;
using Debugger.AddIn.TreeModel;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui.Pads;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public enum ParallelStacksView
	{
		Threads,
		Tasks
	}
	
	public class ParallelStackPad : DebuggerPad
	{
		DrawSurface surface;
		Process debuggedProcess;
		ParallelStacksGraph graph;
		List<ThreadStack> currentThreadStacks = new List<ThreadStack>();
		ParallelStacksView parallelStacksView;
		StackFrame selectedFrame;
		bool isMethodView;
		
		#region Overrides
		
		protected override void InitializeComponents()
		{
			surface = new DrawSurface();
			
			panel.Children.Add(surface);
		}
		
		protected override void SelectProcess(Process process)
		{
			if (debuggedProcess != null) {
				debuggedProcess.Paused -= OnProcessPaused;
			}
			debuggedProcess = process;
			if (debuggedProcess != null) {
				debuggedProcess.Paused += OnProcessPaused;
			}
			
			DebuggerService.DebugStarted += OnReset;
			DebuggerService.DebugStopped += OnReset;
			
			InvalidatePad();
		}
		
		protected override void RefreshPad()
		{
			if (debuggedProcess == null || debuggedProcess.IsRunning) {
				return;
			}
			
			LoggingService.InfoFormatted("Start refresh: {0}" + Environment.NewLine, parallelStacksView);
			
			currentThreadStacks.Clear();
			
			using(new PrintTimes("Create stacks")) {
				try {
					// create all simple ThreadStacks
					foreach (Thread thread in debuggedProcess.Threads) {
						var t = thread;
						debuggedProcess.EnqueueWork(Dispatcher.CurrentDispatcher, () => CreateThreadStack(t));
					}
				}
				catch(AbortedBecauseDebuggeeResumedException) { }
				catch(System.Exception) {
					if (debuggedProcess == null || debuggedProcess.HasExited) {
						// Process unexpectedly exited
					} else {
						throw;
					}
				}
			}
			using(new PrintTimes("Run algorithm")) {
				debuggedProcess.EnqueueWork(Dispatcher.CurrentDispatcher, () => 
				{
					if (isMethodView)
					{
						// build method view for threads
						CreateMethodViewStacks();
					}
					else
					{
						// normal view
						CreateCommonStacks();
					}
				});
			}
			
			using(new PrintTimes("Graph refresh")) {
				debuggedProcess.EnqueueWork(Dispatcher.CurrentDispatcher, () => 
				{
					// paint the ThreadStaks
					graph = new ParallelStacksGraph();
					foreach (var stack in this.currentThreadStacks.FindAll(ts => ts.ThreadStackParents == null ))
					{
						graph.AddVertex(stack);
						
						// add the children
						AddChildren(stack);
					}
					
					if (graph.VertexCount > 0)
						surface.SetGraph(graph);
				});
			}
		}
		
		protected override ToolBar BuildToolBar()
		{
			return ToolBarService.CreateToolBar(panel, this, "/SharpDevelop/Pads/ParallelStacksPad/ToolBar");
		}

		#endregion
		
		#region Public Properties
		
		public ParallelStacksView ParallelStacksView {
			get { return parallelStacksView; }
			set {
				parallelStacksView = value;
				InvalidatePad();
			}
		}
		
		public bool IsMethodView {
			get { return isMethodView; }
			set {
				isMethodView = value;
				InvalidatePad();
			}
		}
		
		public bool IsZoomControlVisible {
			get { return surface.IsZoomControlVisible; }
			set { surface.IsZoomControlVisible = value; }
		}
		
		#endregion
		
		#region Private Methods
		
		void OnReset(object sender, EventArgs e)
		{
			currentThreadStacks.Clear();
			selectedFrame = null;
			
			// remove all
			BookmarkManager.RemoveAll(b => b is SelectedFrameBookmark);
		}

		void OnProcessPaused(object sender, ProcessEventArgs e)
		{
			InvalidatePad();
		}
		
		void AddChildren(ThreadStack parent)
		{
			if(parent.ThreadStackChildren == null || parent.ThreadStackChildren.Count == 0)
				return;
			
			foreach (var ts in parent.ThreadStackChildren)
			{
				if (ts == null) continue;
				
				graph.AddVertex(ts);
				graph.AddEdge(new ParallelStacksEdge(parent, ts));
				
				if (ts.ThreadStackChildren == null || ts.ThreadStackChildren.Count == 0)
					continue;
				
				AddChildren(ts);
			}
		}
		
		void CreateCommonStacks()
		{
			// stack.ItemCollection     order
			// 0 -> top of stack 		= S.C
			// 1 -> ............ 		= S.B
			// .......................
			// n -> bottom of stack 	= [External Methods]
			
			// ThreadStacks with common frame
			var commonFrameThreads = new Dictionary<string, List<ThreadStack>>();
			
			bool isOver = false;
			while (true) {
				
				for (int i = currentThreadStacks.Count - 1; i >=0; --i) {
					var stack = currentThreadStacks[i];
					if (stack.ItemCollection.Count == 0) {
						currentThreadStacks.Remove(stack);
						continue;
					}
				}
				//get all thread stacks with common start frame
				foreach (var stack in currentThreadStacks) {
					int count = stack.ItemCollection.Count;
					ParallelStackFrameModel frame = stack.ItemCollection[count - 1];
					string fullname = frame.MethodName + stack.Level.ToString();
					
					if (!commonFrameThreads.ContainsKey(fullname))
						commonFrameThreads.Add(fullname, new List<ThreadStack>());
					
					if (!commonFrameThreads[fullname].Contains(stack))
						commonFrameThreads[fullname].Add(stack);
				}
				
				// for every list of common stacks, find split place and add them to currentThreadStacks
				foreach (var frameName in commonFrameThreads.Keys) {
					var listOfCurrentStacks = commonFrameThreads[frameName];
					
					if (listOfCurrentStacks.Count == 1) // just skip the parents
					{
						isOver = true; // we finish when all are pseodo-parents: no more spliting
						continue;
					}
					
					isOver = false;
					
					// find the frameIndex where we can split
					int frameIndex = 0;
					string fn = string.Empty;
					bool canContinue = true;
					
					while(canContinue) {
						for (int i = 0; i < listOfCurrentStacks.Count; ++i) {
							var stack = listOfCurrentStacks[i];
							if (stack.ItemCollection.Count == frameIndex)
							{
								canContinue = false;
								break;
							}
							
							ParallelStackFrameModel item = stack.ItemCollection[stack.ItemCollection.Count - frameIndex - 1];
							
							string currentName = item.MethodName;
							
							if (i == 0) {
								fn = currentName;
								continue;
							}
							
							if (fn == currentName)
								continue;
							else {
								canContinue = false;
								break;
							}
						}
						if (canContinue)
							frameIndex++;
					}
					
					// remove last [frameIndex] and create a new ThreadStack as the parent of what remained in the children
					var threadIds = new List<uint>();
					var parentItems = new Stack<ParallelStackFrameModel>();

					while (frameIndex > 0) {
						for (int i = 0 ; i < listOfCurrentStacks.Count; ++i) {
							var stack = listOfCurrentStacks[i];
							int indexToRemove = stack.ItemCollection.Count - 1;
							
							#if DEBUG
							ParallelStackFrameModel d_item = stack.ItemCollection[indexToRemove];
							string name = d_item.MethodName;
							#endif
							if (i == 0)
								parentItems.Push(stack.ItemCollection[indexToRemove]);
							
							stack.ItemCollection.RemoveAt(indexToRemove);
						}
						
						frameIndex--;
					}
					
					// update thread ids
					for (int i = 0 ; i < listOfCurrentStacks.Count; ++i)
						threadIds.AddRange(listOfCurrentStacks[i].ThreadIds);
					
					// remove stacks with no items
					for (int i = listOfCurrentStacks.Count - 1; i >= 0; --i) {
						var stack = listOfCurrentStacks[i];
						if (stack.ItemCollection.Count == 0)
							listOfCurrentStacks.Remove(stack);
					}

					// increase the Level
					for (int i = 0 ; i < listOfCurrentStacks.Count; ++i)
						listOfCurrentStacks[i].Level++;
					
					// create new parent stack
					ThreadStack commonParent = new ThreadStack();
					commonParent.UpdateThreadIds(parallelStacksView == ParallelStacksView.Tasks, threadIds.ToArray());
					commonParent.ItemCollection = parentItems.ToObservable();
					commonParent.Process = debuggedProcess;
					commonParent.StackSelected += OnThreadStackSelected;
					commonParent.FrameSelected += OnFrameSelected;
					commonParent.IsSelected = commonParent.ThreadIds.Contains(debuggedProcess.SelectedThread.ID);
					// add new children
					foreach (var stack in listOfCurrentStacks) {
						if (stack.ItemCollection.Count == 0)
						{
							currentThreadStacks.Remove(stack);
							continue;
						}
						ParallelStackFrameModel item = stack.ItemCollection[stack.ItemCollection.Count - 1];
						// add the parent to the parent
						if (stack.ThreadStackParents != null) {
							// remove stack from it's parent because it will have the commonParent as parent
							stack.ThreadStackParents[0].ThreadStackChildren.Remove(stack);
							commonParent.ThreadStackParents = stack.ThreadStackParents.Clone();
							commonParent.ThreadStackParents[0].ThreadStackChildren.Add(commonParent);
							// set level
							commonParent.Level = stack.Level - 1;
						}
						else
							stack.ThreadStackParents = new List<ThreadStack>();
						
						// update thread ids
						if (commonParent.ThreadStackParents != null) {
							commonParent.ThreadStackParents[0].UpdateThreadIds(
								parallelStacksView == ParallelStacksView.Tasks,
								commonParent.ThreadIds.ToArray());
						}
						
						stack.ThreadStackParents.Clear();
						stack.ThreadStackParents.Add(commonParent);
						string currentName = item.MethodName + stack.Level.ToString();;
						
						// add name or add to list
						if (!commonFrameThreads.ContainsKey(currentName)) {
							var newList = new List<ThreadStack>();
							newList.Add(stack);
							commonFrameThreads.Add(currentName, newList);
						}
						else {
							var list = commonFrameThreads[currentName];
							list.Add(stack);
						}
					}
					
					commonParent.ThreadStackChildren = listOfCurrentStacks.Clone();
					commonFrameThreads[frameName].Clear();
					commonFrameThreads[frameName].Add(commonParent);
					currentThreadStacks.Add(commonParent);
					
					// exit and retry
					break;
				}
				
				if (isOver || currentThreadStacks.Count == 0)
					break;
			}
		}

		void CreateMethodViewStacks()
		{
			var list = new List<Tuple<ObservableCollection<ParallelStackFrameModel>, ObservableCollection<ParallelStackFrameModel>, List<uint>>>();
			
			// find all threadstacks that contains the selected frame
			for (int i = currentThreadStacks.Count - 1; i >= 0; --i) {
				var tuple = currentThreadStacks[i].ItemCollection.SplitStack(selectedFrame, currentThreadStacks[i].ThreadIds);
				if (tuple != null)
					list.Add(tuple);
			}
			
			currentThreadStacks.Clear();
			
			// common
			ThreadStack common = new ThreadStack();
			var observ = new ObservableCollection<ParallelStackFrameModel>();
			bool dummy = false;
			ParallelStackFrameModel obj = CreateItemForFrame(selectedFrame, ref dummy);
			obj.Image = PresentationResourceService.GetImage("Icons.48x48.CurrentFrame").Source;
			observ.Add(obj);
			common.ItemCollection = observ;
			common.StackSelected += OnThreadStackSelected;
			common.FrameSelected += OnFrameSelected;
			common.UpdateThreadIds(parallelStacksView == ParallelStacksView.Tasks, selectedFrame.Thread.ID);
			common.Process = debuggedProcess;
			common.ThreadStackChildren = new List<ThreadStack>();
			common.ThreadStackParents = new List<ThreadStack>();
			
			// for all thread stacks, split them in 2 :
			// one that invokes the method frame, second that get's invoked by current method frame
			foreach (var tuple in list) {
				// add top
				if (tuple.Item1.Count > 0)
				{
					ThreadStack topStack = new ThreadStack();
					topStack.ItemCollection = tuple.Item1;
					topStack.StackSelected += OnThreadStackSelected;
					topStack.FrameSelected += OnFrameSelected;
					topStack.UpdateThreadIds(parallelStacksView == ParallelStacksView.Tasks, tuple.Item3.ToArray());
					topStack.Process = debuggedProcess;
					topStack.ThreadStackParents = new List<ThreadStack>();
					topStack.ThreadStackParents.Add(common);
					
					currentThreadStacks.Add(topStack);
					common.ThreadStackChildren.Add(topStack);
				}
				
				// add bottom
				if(tuple.Item2.Count > 0)
				{
					ThreadStack bottomStack = new ThreadStack();
					bottomStack.ItemCollection = tuple.Item2;
					bottomStack.StackSelected += OnThreadStackSelected;
					bottomStack.FrameSelected += OnFrameSelected;
					bottomStack.UpdateThreadIds(parallelStacksView == ParallelStacksView.Tasks, tuple.Item3.ToArray());
					bottomStack.Process = debuggedProcess;
					bottomStack.ThreadStackChildren = new List<ThreadStack>();
					bottomStack.ThreadStackChildren.Add(common);
					common.UpdateThreadIds(parallelStacksView == ParallelStacksView.Tasks, tuple.Item3.ToArray());
					common.ThreadStackParents.Add(bottomStack);
					currentThreadStacks.Add(bottomStack);
				}
			}
			
			currentThreadStacks.Add(common);
			common.IsSelected = true;
		}
		
		void CreateThreadStack(Thread thread)
		{
			var items = CreateItems(thread);
			if (items == null || items.Count == 0)
				return;
			
			ThreadStack threadStack = new ThreadStack();
			threadStack.StackSelected += OnThreadStackSelected;
			threadStack.FrameSelected += OnFrameSelected;
			threadStack.Process = debuggedProcess;
			threadStack.ItemCollection = items;
			threadStack.UpdateThreadIds(parallelStacksView == ParallelStacksView.Tasks, thread.ID);
			
			if (debuggedProcess.SelectedThread != null) {
				threadStack.IsSelected = threadStack.ThreadIds.Contains(debuggedProcess.SelectedThread.ID);
				if (selectedFrame == null)
					selectedFrame = debuggedProcess.SelectedStackFrame;
			}
			
			currentThreadStacks.Add(threadStack);
		}
		
		ObservableCollection<ParallelStackFrameModel> CreateItems(Thread thread)
		{
			bool lastItemIsExternalMethod = false;
			int noTasks = 0;
			var result = new ObservableCollection<ParallelStackFrameModel>();
			var callstack = thread.GetCallstack(100);
			
			if (parallelStacksView == ParallelStacksView.Threads) {
				foreach (StackFrame frame in callstack) {
					ParallelStackFrameModel obj = CreateItemForFrame(frame, ref lastItemIsExternalMethod);
					
					if (obj != null)
						result.Add(obj);
				}
			} else {
				for (int i = 0 ; i < callstack.Length; ++i) {
					StackFrame frame = callstack[i];
					ParallelStackFrameModel obj = CreateItemForFrame(frame, ref lastItemIsExternalMethod);
					
					if (frame.MethodInfo.FullName.IndexOf("System.Threading.Tasks.Task.ExecuteEntry") != -1) {
						noTasks++;
					}
					
					if (noTasks == 1) {
						if (frame.HasSymbols) {
							// create thread stack for the items collected until now
							ThreadStack threadStack = new ThreadStack();
							threadStack.StackSelected += OnThreadStackSelected;
							threadStack.FrameSelected += OnFrameSelected;
							threadStack.Process = debuggedProcess;
							threadStack.ItemCollection = result.Clone();
							threadStack.UpdateThreadIds(true, frame.Thread.ID);
							
							if (debuggedProcess.SelectedThread != null) {
								threadStack.IsSelected = threadStack.ThreadIds.Contains(debuggedProcess.SelectedThread.ID);
								if (selectedFrame == null)
									selectedFrame = debuggedProcess.SelectedStackFrame;
							}
							
							currentThreadStacks.Add(threadStack);
							// reset
							result.Clear();
							noTasks = 0;
						}
					}
					
					if (obj != null)
						result.Add(obj);
				}
				
				// return null if we are dealing with a simple thread
				return noTasks == 0 ? null : result;
			}

			return result;
		}
		
		ParallelStackFrameModel CreateItemForFrame(StackFrame frame, ref bool lastItemIsExternalMethod)
		{
			ParallelStackFrameModel model = new ParallelStackFrameModel();
			string fullName;
			if (frame.HasSymbols) {
				// Show the method in the list
				fullName = frame.GetMethodName();
				lastItemIsExternalMethod = false;
				model.FontWeight = FontWeights.Normal;
				model.Foreground = Brushes.Black;
			} else {
				// Show [External methods] in the list
				if (lastItemIsExternalMethod) return null;
				fullName = ResourceService.GetString("MainWindow.Windows.Debug.CallStack.ExternalMethods").Trim();
				model.FontWeight = FontWeights.Normal;
				model.Foreground = Brushes.Gray;
				lastItemIsExternalMethod = true;
			}
			
			if (frame.Thread.SelectedStackFrame != null &&
			    frame.Thread.ID == debuggedProcess.SelectedThread.ID &&
			    frame.Thread.SelectedStackFrame.IP == frame.IP &&
			    frame.Thread.SelectedStackFrame.GetMethodName() == frame.GetMethodName()) {
				model.Image = PresentationResourceService.GetImage("Bookmarks.CurrentLine").Source;
				model.IsRunningStackFrame = true;
			} else {
				if (selectedFrame != null && frame.Thread.ID == selectedFrame.Thread.ID &&
				    frame.GetMethodName() == selectedFrame.GetMethodName())
					model.Image = PresentationResourceService.GetImage("Icons.48x48.CurrentFrame").Source;
				else
					model.Image = null;
				model.IsRunningStackFrame = false;
			}
			
			model.MethodName = fullName;
			
			return model;
		}

		void ToggleSelectedFrameBookmark(Location location)
		{
			// remove all
			BookmarkManager.RemoveAll(b => b is SelectedFrameBookmark);
			
			ITextEditorProvider provider = WorkbenchSingleton.Workbench.ActiveContent as ITextEditorProvider;
			if (provider != null) {
				ITextEditor editor = provider.TextEditor;
				BookmarkManager.AddMark(new SelectedFrameBookmark(editor.FileName, location));
			}
		}
		
		void OnThreadStackSelected(object sender, EventArgs e)
		{
			foreach (var ts in this.currentThreadStacks) {
				if (ts.IsSelected)
					ts.IsSelected = false;
				ts.ClearImages();
			}
		}
		
		void OnFrameSelected(object sender, FrameSelectedEventArgs e)
		{
			selectedFrame = e.Item;
			
			ToggleSelectedFrameBookmark(e.Location);
			
			if (isMethodView)
				InvalidatePad();
		}
		
		#endregion
	}
	
	static class StackFrameExtensions
	{
		internal static string GetMethodName(this StackFrame frame)
		{
			if (frame == null)
				return null;
			
			StringBuilder name = new StringBuilder();
			name.Append(frame.MethodInfo.DeclaringType.FullName);
			name.Append('.');
			name.Append(frame.MethodInfo.Name);
			
			return name.ToString();
		}
	}
	
	static class ParallelStackExtensions
	{
		internal static List<T> Clone<T>(this List<T> listToClone)
		{
			if (listToClone == null)
				return null;
			
			var result = new List<T>();
			foreach (var item in listToClone)
				result.Add(item);
			
			return result;
		}
		
		internal static ObservableCollection<T> Clone<T>(this ObservableCollection<T> collectionToClone)
		{
			if (collectionToClone == null)
				return null;
			
			var result = new ObservableCollection<T>();
			foreach (var item in collectionToClone)
				result.Add(item);
			
			return result;
		}
		
		internal static ObservableCollection<T> ToObservable<T>(this Stack<T> stack)
		{
			if (stack == null)
				throw new NullReferenceException("Stack is null!");
			
			var result = new ObservableCollection<T>();
			while (stack.Count > 0)
				result.Add(stack.Pop());
			
			return result;
		}
		
		internal static Tuple<ObservableCollection<T>, ObservableCollection<T>, List<uint>>
			SplitStack<T>(this ObservableCollection<T> source, StackFrame frame, List<uint> threadIds)
		{
			var bottom = new ObservableCollection<T>();
			var top = new ObservableCollection<T>();
			
			int found = 0;
			
			foreach (dynamic item in source) {
				if (item.MethodName == frame.GetMethodName())
					found = 1;
				
				if (found >= 1) {
					if(found > 1)
						bottom.Add(item);
					
					found++;
				}
				else
					top.Add(item);
			}
			
			var result =
				new Tuple<ObservableCollection<T>, ObservableCollection<T>, List<uint>>(top, bottom, threadIds);
			
			return found > 1 ? result : null;
		}
	}
}


