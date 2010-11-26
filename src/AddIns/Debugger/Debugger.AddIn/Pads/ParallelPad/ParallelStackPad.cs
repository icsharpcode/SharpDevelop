// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Debugger.AddIn.TreeModel;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Gui.Pads;
using ICSharpCode.SharpDevelop.Services;

namespace Debugger.AddIn.Pads.ParallelPad
{
	public class ParallelStackPad : DebuggerPad
	{
		private DrawSurface surface;
		private Process debuggedProcess;
		
		private List<ThreadStack> currentThreadStacks = new List<ThreadStack>();
		
		protected override void InitializeComponents()
		{
			surface = new DrawSurface();
		}
		
		public override object Control {
			get {
				return surface;
			}
		}
		
		protected override void SelectProcess(Process process)
		{
			if (debuggedProcess != null) {
				debuggedProcess.Paused -= debuggedProcess_Paused;
			}
			debuggedProcess = process;
			if (debuggedProcess != null) {
				debuggedProcess.Paused += debuggedProcess_Paused;
			}
			
			DebuggerService.DebugStarted += OnReset;
			DebuggerService.DebugStopped += OnReset;
			
			RefreshPad();
		}

		void OnReset(object sender, EventArgs e)
		{
			currentThreadStacks.Clear();
		}

		void debuggedProcess_Paused(object sender, ProcessEventArgs e)
		{
			RefreshPad();
		}
		
		public override void RefreshPad()
		{
			if (debuggedProcess == null || debuggedProcess.IsRunning) {
				return;
			}
			
			using(new PrintTimes("Parallel stack refresh")) {
				try {
					OnReset(null, EventArgs.Empty);
					// create all simple ThreadStacks
					foreach (Thread thread in debuggedProcess.Threads) {
						if (debuggedProcess.IsPaused) {
							Utils.DoEvents(debuggedProcess);
						}
						
						CreateThreadStack(thread);
					}
					
					CreateCommonStacks();
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
			
			using(new PrintTimes("Graph refresh")) {
				// paint the ThreadStaks
				graph = new ParallelStacksGraph();
				foreach (var stack in this.currentThreadStacks)
				{
					if (stack == null)
						continue;
					if (stack.ThreadStackParent != null &&
					    (stack.ThreadStackChildren == null || stack.ThreadStackChildren.Length == 0))
						continue;
					
					graph.AddVertex(stack);
					
					// add the children
					AddChildren(stack);
				}
				
				surface.SetGraph(graph);
			}
		}
		
		ParallelStacksGraph graph;
		
		void AddChildren(ThreadStack parent)
		{
			if(parent.ThreadStackChildren == null || parent.ThreadStackChildren.Length == 0)
				return;
			
			foreach (var ts in parent.ThreadStackChildren)
			{
				if (ts == null) continue;
				
				graph.AddVertex(ts);
				graph.AddEdge(new ParallelStacksEdge(parent, ts));
				
				if (ts.ThreadStackChildren == null || ts.ThreadStackChildren.Length == 0)
					continue;
				
				AddChildren(ts);
			}
		}
		
		private void CreateCommonStacks()
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
					dynamic frame = stack.ItemCollection[count - 1];
					string fullname = frame.MethodName;
					
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
							dynamic item = stack.ItemCollection[stack.ItemCollection.Count - frameIndex - 1];
							
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
					var parentItems = new Stack<ExpandoObject>();
					int j = 0;
					while (frameIndex > 0) {
						for (int i = 0 ; i < listOfCurrentStacks.Count; ++i) {
							var stack = listOfCurrentStacks[i];
							int indexToRemove = stack.ItemCollection.Count - 1;
							
							#if DEBUG
							dynamic d_item = stack.ItemCollection[indexToRemove];
							string name = d_item.MethodName;
							#endif
							if (i == 0)
								parentItems.Push(stack.ItemCollection[indexToRemove]);
							if (i == j)
								threadIds.AddRange(stack.ThreadIds);
							
							stack.ItemCollection.RemoveAt(indexToRemove);
						}
						j++;
						frameIndex--;
					}
					// remove stacks with no items
					for (int i = listOfCurrentStacks.Count - 1; i >= 0; --i) {
						var stack = listOfCurrentStacks[i];
						if (stack.ItemCollection.Count == 0)
							listOfCurrentStacks.Remove(stack);
					}
					
					// create new parent stack
					ThreadStack commonParent = new ThreadStack();
					commonParent.ThreadIds = threadIds;
					commonParent.ItemCollection = parentItems.ToObservable();
					commonParent.Process = debuggedProcess;
					commonParent.FrameSelected += threadStack_FrameSelected;
					commonParent.IsSelected = commonParent.ThreadIds.Contains(debuggedProcess.SelectedThread.ID);
					// add new children
					foreach (var stack in listOfCurrentStacks) {
						if (stack.ItemCollection.Count == 0)
						{
							currentThreadStacks.Remove(stack);
							continue;
						}
						dynamic item = stack.ItemCollection[stack.ItemCollection.Count - 1];
						stack.ThreadStackParent = commonParent;
						string currentName = item.MethodName;
						var newList = new List<ThreadStack>();
						newList.Add(stack);
						
						commonFrameThreads.Add(currentName, newList);
					}
					
					commonParent.ThreadStackChildren = listOfCurrentStacks.ToArray();
					commonFrameThreads[frameName].Clear();
					commonFrameThreads[frameName].Add(commonParent);
					currentThreadStacks.Add(commonParent);
					
					// exit and retry
					break;
				}
				
				if (isOver)
					break;
			}
		}
		
		private void CreateThreadStack(Thread thread)
		{
			var items = CreateItems(thread);
			if (items.Count == 0)
				return;
			
			ThreadStack threadStack = new ThreadStack();
			threadStack.FrameSelected += threadStack_FrameSelected;
			threadStack.ThreadIds = new List<uint>();
			threadStack.ThreadIds.Add(thread.ID);
			threadStack.Process = debuggedProcess;
			currentThreadStacks.Add(threadStack);
			
			threadStack.ItemCollection = items;
			if (debuggedProcess.SelectedThread != null)
				threadStack.IsSelected = threadStack.ThreadIds.Contains(debuggedProcess.SelectedThread.ID);
		}

		void threadStack_FrameSelected(object sender, EventArgs e)
		{
			foreach (var ts in this.currentThreadStacks) {
				ts.IsSelected = false;
				ts.ClearImages();
			}
		}
		
		private ObservableCollection<ExpandoObject> CreateItems(Thread thread)
		{
			bool lastItemIsExternalMethod = false;
			var result = new ObservableCollection<ExpandoObject>();
			foreach (StackFrame frame in thread.GetCallstack(100)) {
				dynamic obj = new ExpandoObject();
				string fullName;
				if (frame.HasSymbols) {
					// Show the method in the list
					fullName = frame.GetMethodName();
					lastItemIsExternalMethod = false;
					obj.FontWeight = FontWeights.Normal;
					obj.Foreground = Brushes.Black;
				} else {
					// Show [External methods] in the list
					if (lastItemIsExternalMethod) continue;
					fullName = ResourceService.GetString("MainWindow.Windows.Debug.CallStack.ExternalMethods").Trim();
					obj.FontWeight = FontWeights.Normal;
					obj.Foreground = Brushes.Gray;
					lastItemIsExternalMethod = true;
				}
				
				if (thread.SelectedStackFrame != null &&
				    thread.ID == debuggedProcess.SelectedThread.ID &&
				    thread.SelectedStackFrame.IP == frame.IP &&
				    thread.SelectedStackFrame.GetMethodName() == frame.GetMethodName())
					obj.Image = PresentationResourceService.GetImage("Bookmarks.CurrentLine").Source;
				else
					obj.Image = null;
				
				obj.MethodName = fullName;
				
				result.Add(obj);
			}
			
			Utils.DoEvents(debuggedProcess);
			
			return result;
		}
	}
	
	internal static class StackFrameExtensions
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
	
	internal static class ParallelStackExtensions
	{
		internal static ObservableCollection<T> ToObservable<T>(this Stack<T> stack)
		{
			if (stack == null)
				throw new NullReferenceException("Stack is null!");
			
			var result = new ObservableCollection<T>();
			while (stack.Count > 0)
				result.Add(stack.Pop());
			
			return result;
		}
	}
}
