// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Input;

using Debugger;
using Debugger.AddIn;
using Debugger.AddIn.Pads.Controls;
using Debugger.AddIn.TreeModel;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Project.SavedData;
using Exception = System.Exception;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public class WatchPad : DebuggerPad
	{
		WatchList watchList;
		Process debuggedProcess;

		static WatchPad instance;
		
		/// <remarks>Always check if Instance is null, might be null if pad is not opened!</remarks>
		public static WatchPad Instance {
			get { return instance; }
		}
		
		public WatchList WatchList {
			get {
				return watchList;
			}
		}
		
		public WatchPad()
		{
			instance = this;
		}
		
		public Process Process {
			get { return debuggedProcess; }
		}
		
		protected override void InitializeComponents()
		{
			watchList = new WatchList();
			watchList.WatchType = WatchListType.Watch;
			watchList.ParentPad = this;
			watchList.ContextMenu = MenuService.CreateContextMenu(this, "/SharpDevelop/Pads/WatchPad/ContextMenu");
			
			watchList.AllowDrop = true;
			watchList.DragEnter += watchList_DragOver;
			watchList.Drop += watchList_Drop;
			watchList.MouseDoubleClick += watchList_DoubleClick;
			watchList.KeyUp += watchList_KeyUp;
			watchList.WatchItems.CollectionChanged += OnWatchItemsCollectionChanged;
			
			panel.Children.Add(watchList);
			panel.KeyUp += new KeyEventHandler(panel_KeyUp);
			
			// wire events that influence the items
			LoadSavedNodes();
			ProjectService.SolutionClosed += delegate { watchList.WatchItems.Clear(); };
			ProjectService.SolutionPreferencesSaving += OnSolutionClosing;
			ProjectService.ProjectRemoved += OnProjectRemoved;
			ProjectService.ProjectAdded += OnProjectAdded;
			ProjectService.SolutionLoaded += delegate { LoadSavedNodes(); };
		}

		#region Saved nodes
		
		void AddNodes(Func<IProjectSavedData, bool> predicate)
		{
			if (predicate == null)
				throw new ArgumentNullException("predicate");
			
			// get nodes of current projects
			List<TextNode> temp = new List<TextNode>();
			foreach (var data in SavedDataManager.GetSavedData().Where(predicate)) {
				string[] v = data.SavedString.Split('|');

				TextNode node = new TextNode(null, v[5], (SupportedLanguage)Enum.Parse(typeof(SupportedLanguage), v[7])) { ProjectName = data.ProjectName };
				temp.Add(node);
			}
			
			// add them to watch list
			temp.ForEach(d => { if (!watchList.WatchItems.Contains(d)) watchList.WatchItems.Add(d); } );
		}
		
		void LoadSavedNodes()
		{
			AddNodes(d => d.SavedDataType == ProjectSavedDataType.WatchVariables &&
			         ProjectService.OpenSolution.Projects.Any(p => p.Name == d.ProjectName));
			
			// remove them temporarilly - they will be saved on exit
			SavedDataManager.RemoveAll(d => d.SavedDataType == ProjectSavedDataType.WatchVariables &&
			                           ProjectService.OpenSolution.Projects.Any(p => p.Name == d.ProjectName));
		}

		void OnSolutionClosing(object sender, SolutionEventArgs e)
		{
			foreach (var element in watchList.WatchItems) {
				SavedDataManager.Add((IProjectSavedData)element);
			}
		}

		void OnProjectAdded(object sender, ProjectEventArgs e)
		{
			AddNodes(d => d.SavedDataType == ProjectSavedDataType.WatchVariables && e.Project.Name == d.ProjectName);
		}

		void OnProjectRemoved(object sender, ProjectEventArgs e)
		{
			if (e.Project == null)
				return;
			
			// get the specific nodes from the list
			List<TextNode> nodes = new List<TextNode>();
			foreach (var element in watchList.WatchItems
			         .OfType<TextNode>()
			         .Where(tn => tn.ProjectName == e.Project.Name)) {
				nodes.Add(element);
			}
			
			// remove nodes from the list
			nodes.ForEach(n => watchList.WatchItems.Remove(n));
		}

		void OnWatchItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add) {
				// add to saved data
				var data = e.NewItems[0] as IProjectSavedData;
				if (data != null)
					SavedDataManager.Add(data);
			}
			
			if (e.Action == NotifyCollectionChangedAction.Remove) {
				// remove from saved data
				var data = e.OldItems[0] as IProjectSavedData;
				if (data != null)
					SavedDataManager.Remove(data);
			}
		}

		#endregion
		
		void panel_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Insert) {
				AddNewWatch();
				e.Handled = true;
			}
		}

		void watchList_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Delete) {
				RemoveWatchCommand cmd = new RemoveWatchCommand { Owner = this };
				cmd.Run();
			}
		}
		
		void watchList_Drop(object sender, DragEventArgs e)
		{
			if (ProjectService.CurrentProject == null) return;
			if (e.Data == null) return;
			if (!e.Data.GetDataPresent(DataFormats.StringFormat)) return;
			if (string.IsNullOrEmpty(e.Data.GetData(DataFormats.StringFormat).ToString())) return;
			
			string language = ProjectService.CurrentProject.Language;
			
			// FIXME languages
			TextNode text = new TextNode(null, e.Data.GetData(DataFormats.StringFormat).ToString(),
			                             language == "VB" || language == "VBNet" ? SupportedLanguage.VBNet : SupportedLanguage.CSharp);

			if (!watchList.WatchItems.Contains(text))
				watchList.WatchItems.ContainsItem(text);
			
			this.RefreshPad();
		}

		void watchList_DragOver(object sender, DragEventArgs e)
		{
			if(e.Data.GetDataPresent(DataFormats.StringFormat)) {
				e.Effects = DragDropEffects.Copy;
			}
			else {
				e.Effects = DragDropEffects.None;
				e.Handled = true;
			}
		}
		
		void watchList_DoubleClick(object sender, MouseEventArgs e)
		{
			if (watchList.SelectedNode == null)
			{
				AddNewWatch();
			}
		}
		
		void AddNewWatch()
		{
			AddWatchCommand command = new AddWatchCommand { Owner = this };
			command.Run();
		}
		
		void ResetPad(object sender, EventArgs e)
		{
			string language = "CSharp";
			
			if (ProjectService.CurrentProject != null)
				language = ProjectService.CurrentProject.Language;
			
			// rebuild list
			var nodes = new List<TreeNode>();
			foreach (var nod in watchList.WatchItems)
				nodes.Add(new TextNode(null, nod.Name,
				                       language == "VB" || language == "VBNet" ? SupportedLanguage.VBNet : SupportedLanguage.CSharp));
			
			watchList.WatchItems.Clear();
			foreach (var nod in nodes)
				watchList.WatchItems.Add(nod);
		}
		
		protected override void SelectProcess(Process process)
		{
			if (debuggedProcess != null) {
				debuggedProcess.Paused -= debuggedProcess_Paused;
				debuggedProcess.Exited -= ResetPad;
			}
			debuggedProcess = process;
			if (debuggedProcess != null) {
				debuggedProcess.Paused += debuggedProcess_Paused;
				debuggedProcess.Exited += ResetPad;
			}
			RefreshPad();
		}
		
		void debuggedProcess_Paused(object sender, ProcessEventArgs e)
		{
			RefreshPad();
		}
		
		public override void RefreshPad()
		{
			if (debuggedProcess == null || debuggedProcess.IsRunning || debuggedProcess.SelectedStackFrame == null)
				return;
			
			using(new PrintTimes("Watch Pad refresh")) {
				try {
					Utils.DoEvents(debuggedProcess);
					List<TreeNode> nodes = new List<TreeNode>();
					
					foreach (var nod in watchList.WatchItems) {
						try {
							LoggingService.Info("Evaluating: " + (string.IsNullOrEmpty(nod.Name) ? "is null or empty!" : nod.Name));
							var nodExpression = debugger.GetExpression(nod.Name);
							//Value val = ExpressionEvaluator.Evaluate(nod.Name, nod.Language, debuggedProcess.SelectedStackFrame);
							ExpressionNode valNode = new ExpressionNode(null, null, nod.Name, nodExpression);
							nodes.Add(valNode);
						}
						catch (GetValueException) {
							string error = String.Format(StringParser.Parse("${res:MainWindow.Windows.Debug.Watch.InvalidExpression}"), nod.Name);
							ErrorInfoNode infoNode = new ErrorInfoNode(nod.Name, error);
							nodes.Add(infoNode);
						}
					}
					
					// rebuild list
					watchList.WatchItems.Clear();
					foreach (var nod in nodes)
						watchList.WatchItems.Add(nod);
				}
				catch(AbortedBecauseDebuggeeResumedException) { }
				catch(Exception ex) {
					if (debuggedProcess == null || debuggedProcess.HasExited) {
						// Process unexpectedly exited
					} else {
						MessageService.ShowException(ex);
					}
				}
			}
		}
	}
}
