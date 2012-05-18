// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml.Serialization;
using Debugger;
using Debugger.AddIn;
using Debugger.AddIn.Pads.Controls;
using Debugger.AddIn.TreeModel;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Project;
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
			watchList = new WatchList(WatchListType.Watch);
			watchList.ContextMenu = MenuService.CreateContextMenu(this, "/SharpDevelop/Pads/WatchPad/ContextMenu");
			
			watchList.MouseDoubleClick += WatchListDoubleClick;
			watchList.WatchItems.CollectionChanged += OnWatchItemsCollectionChanged;
			
			panel.Children.Add(watchList);
			panel.KeyDown += PanelKeyDown;
			
			// wire events that influence the items
			LoadSavedNodes();
			ProjectService.SolutionClosed += delegate { watchList.WatchItems.Clear(); };
			ProjectService.ProjectAdded += delegate { LoadSavedNodes(); };
			ProjectService.SolutionLoaded += delegate { LoadSavedNodes(); };
		}

		#region Saved nodes
		
		void LoadSavedNodes()
		{
			var props = GetSavedVariablesProperties();
			if (props == null)
				return;
			
			foreach (var element in props.Elements) {
				watchList.WatchItems.Add(new TextNode(null, element, (SupportedLanguage)Enum.Parse(typeof(SupportedLanguage), props[element])).ToSharpTreeNode());
			}
		}

		void OnWatchItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems.Count > 0) {
				// add to saved data
				var data = e.NewItems.OfType<TextNode>().FirstOrDefault();
				if (data != null) {
					var props = GetSavedVariablesProperties();
					if (props == null) return;
					props.Set(data.FullName, data.Language.ToString());
				}
			}
			
			if (e.Action == NotifyCollectionChangedAction.Remove) {
				// remove from saved data
				var data = e.OldItems.OfType<TextNode>().FirstOrDefault();
				if (data != null) {
					var props = GetSavedVariablesProperties();
					if (props == null) return;
					props.Remove(data.FullName);
				}
			}
		}
		
		Properties GetSavedVariablesProperties()
		{
			if (ProjectService.CurrentProject == null)
				return null;
			if (ProjectService.CurrentProject.ProjectSpecificProperties == null)
				return null;
			
			var props = ProjectService.CurrentProject.ProjectSpecificProperties.Get("watchVars") as Properties;
			if (props == null) {
				ProjectService.CurrentProject.ProjectSpecificProperties.Set("watchVars", new Properties());
			}
			
			return ProjectService.CurrentProject.ProjectSpecificProperties.Get("watchVars") as Properties;
		}

		#endregion
		
		void PanelKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Insert) {
				AddNewWatch();
				e.Handled = true;
			}
		}
		
		void WatchListDoubleClick(object sender, MouseEventArgs e)
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
			var nodes = new List<TreeNodeWrapper>();
			foreach (var nod in watchList.WatchItems.OfType<TreeNodeWrapper>())
				nodes.Add(new TextNode(null, nod.Node.Name,
				                       language == "VB" || language == "VBNet" ? SupportedLanguage.VBNet : SupportedLanguage.CSharp)
				          .ToSharpTreeNode());
			
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
			InvalidatePad();
		}
		
		void debuggedProcess_Paused(object sender, ProcessEventArgs e)
		{
			InvalidatePad();
		}
		
		TreeNodeWrapper UpdateNode(TreeNodeWrapper node)
		{
			try {
				LoggingService.Info("Evaluating: " + (string.IsNullOrEmpty(node.Node.Name) ? "is null or empty!" : node.Node.Name));
				var nodExpression = debugger.GetExpression(node.Node.Name);
				//Value val = ExpressionEvaluator.Evaluate(nod.Name, nod.Language, debuggedProcess.SelectedStackFrame);
				ExpressionNode valNode = new ExpressionNode(null, null, node.Node.Name, nodExpression);
				return valNode.ToSharpTreeNode();
			} catch (GetValueException) {
				string error = String.Format(StringParser.Parse("${res:MainWindow.Windows.Debug.Watch.InvalidExpression}"), node.Node.Name);
				ErrorInfoNode infoNode = new ErrorInfoNode(node.Node.Name, error);
				return infoNode.ToSharpTreeNode();
			}
		}
		
		protected override void RefreshPad()
		{
			if (debuggedProcess == null || debuggedProcess.IsRunning)
				return;
			
			using(new PrintTimes("Watch Pad refresh")) {
				var nodes = watchList.WatchItems.OfType<TreeNodeWrapper>().ToArray();
				watchList.WatchItems.Clear();
				
				debuggedProcess.EnqueueForEach(
					Dispatcher.CurrentDispatcher,
					nodes,
					n => watchList.WatchItems.Add(UpdateNode(n))
				);
			}
		}
	}
}
