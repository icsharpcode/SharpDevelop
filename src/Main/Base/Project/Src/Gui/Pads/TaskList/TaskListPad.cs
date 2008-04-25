// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class TaskListPad : AbstractPadContent, IClipboardHandler
	{
		static TaskListPad instance;
		Dictionary<string, bool> displayedTokens;
		IClass oldClass;
		int selectedScopeIndex = 0;
		
		ToolStrip toolStrip;
		Panel contentPanel = new Panel();
		
		TaskView taskView = new TaskView();
		
		public Dictionary<string, bool> DisplayedTokens {
			get { return displayedTokens; }
		}
		
		public int SelectedScopeIndex {
			get { return selectedScopeIndex; }
			set { selectedScopeIndex = value;
				UpdateItems();
			}
		}
		
		public override Control Control {
			get {
				return contentPanel;
			}
		}
		
		public static TaskListPad Instance {
			get { return instance; }
		}
		
		public TaskListPad()
		{
			instance = this;
			this.displayedTokens = new Dictionary<string, bool>();
			
			RedrawContent();
			
			InitializeToolStrip();

			TaskService.Cleared += new EventHandler(TaskServiceCleared);
			TaskService.Added   += new TaskEventHandler(TaskServiceAdded);
			TaskService.Removed += new TaskEventHandler(TaskServiceRemoved);
			TaskService.InUpdateChanged += new EventHandler(TaskService_InUpdateChanged);
			
			WorkbenchSingleton.Workbench.ActiveViewContentChanged += new EventHandler(Workbench_ActiveViewContentChanged);
			
			if (WorkbenchSingleton.Workbench.ActiveViewContent != null) {
				UpdateItems();
				
				if (WorkbenchSingleton.Workbench.ActiveViewContent.Control is SharpDevelopTextAreaControl) {
					SharpDevelopTextAreaControl ctrl = WorkbenchSingleton.Workbench.ActiveViewContent.Control as SharpDevelopTextAreaControl;
					
					ctrl.ActiveTextAreaControl.Caret.PositionChanged += new EventHandler(Caret_PositionChanged);
				}
			}
			
			ProjectService.SolutionLoaded += OnSolutionOpen;
			ProjectService.SolutionClosed += OnSolutionClosed;
			
			InternalShowResults(null, null);
		}

		void Workbench_ActiveViewContentChanged(object sender, EventArgs e)
		{
			if (WorkbenchSingleton.Workbench.ActiveViewContent == null)
				return;
			
			UpdateItems();
			
			if (WorkbenchSingleton.Workbench.ActiveViewContent.Control is SharpDevelopTextAreaControl) {
				SharpDevelopTextAreaControl ctrl = WorkbenchSingleton.Workbench.ActiveViewContent.Control as SharpDevelopTextAreaControl;
				
				ctrl.ActiveTextAreaControl.Caret.PositionChanged += new EventHandler(Caret_PositionChanged);
			}
		}

		void Caret_PositionChanged(object sender, EventArgs e)
		{
			if (this.selectedScopeIndex > 2)
			{
				IClass current = GetCurrentClass();
				
				if (oldClass == null) oldClass = current;
				
				if ((current != null) && (current.FullyQualifiedName != oldClass.FullyQualifiedName))
					UpdateItems();
			}
		}

		void TaskService_InUpdateChanged(object sender, EventArgs e)
		{
			if (!TaskService.InUpdate)
				UpdateItems();
		}
		
		private void InitializeToolStrip()
		{
			taskView.CreateControl();
			
			contentPanel.Controls.Add(taskView);
			
			string[] tokens = PropertyService.Get<string[]>("SharpDevelop.TaskListTokens", ParserService.DefaultTaskListTokens);
			
			foreach (string token in tokens)
			{
				if (!this.displayedTokens.ContainsKey(token)) {
					this.displayedTokens.Add(token, true);
				}
			}
			
			toolStrip = ToolbarService.CreateToolStrip(this, "/SharpDevelop/Pads/TaskList/Toolbar");
			ShowTaskListTokenButton[] buttons = TaskListTokensBuilder.BuildItems(tokens);
			
			foreach (ShowTaskListTokenButton button in buttons) {
				toolStrip.Items.Add(new ToolBarSeparator());
				toolStrip.Items.Add((ToolStripItem)button.Owner);
			}
			
			toolStrip.Stretch   = true;
			toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			
			contentPanel.Controls.Add(toolStrip);
		}
		
		public override void RedrawContent()
		{
			taskView.RefreshColumnNames();
		}
		
		public void UpdateItems()
		{
			this.taskView.ClearTasks();
			
			foreach (Task t in TaskService.CommentTasks)
			{
				UpdateItem(t);
			}
			
			RedrawContent();
		}
		
		private void UpdateItem(Task item)
		{
			foreach (KeyValuePair<string, bool> pair in displayedTokens)
			{
				if (item.Description.StartsWith(pair.Key) && pair.Value && IsInScope(item))
					taskView.AddTask(item);
			}
		}
		
		bool IsInScope(Task item)
		{
			IClass current = GetCurrentClass();
			IClass itemClass = GetCurrentClass(item);
			
			switch (this.selectedScopeIndex)
			{
				case 0:
					foreach (AbstractProject proj in ProjectService.OpenSolution.Projects)
						if (proj.FindFile(item.FileName) != null)
						return true;
					
					return false;
				case 1:
					return ((WorkbenchSingleton.Workbench.ActiveViewContent != null) && (ProjectService.CurrentProject.FindFile(item.FileName) != null));
				case 2:
					return ((WorkbenchSingleton.Workbench.ActiveViewContent != null) && (WorkbenchSingleton.Workbench.ActiveViewContent.PrimaryFileName == item.FileName));
				case 3:
					return ((current != null) && (itemClass != null) && (current.Namespace == itemClass.Namespace));
				case 4:
					return ((current != null) && (itemClass != null) && (current == itemClass));
			}
			
			return true;
		}
		
		private IClass GetCurrentClass()
		{
			if (WorkbenchSingleton.Workbench.ActiveViewContent == null)
				return null;
			
			ParseInformation parseInfo = ParserService.GetParseInformation(WorkbenchSingleton.Workbench.ActiveViewContent.PrimaryFileName);
			if (parseInfo != null) {
				if (WorkbenchSingleton.Workbench.ActiveViewContent.Control is ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.SharpDevelopTextAreaControl)
				{SharpDevelopTextAreaControl ctrl = WorkbenchSingleton.Workbench.ActiveViewContent.Control
						as SharpDevelopTextAreaControl;
					IClass c = parseInfo.MostRecentCompilationUnit.GetInnermostClass(ctrl.ActiveTextAreaControl.Caret.Line, ctrl.ActiveTextAreaControl.Caret.Column);
					if (c != null) return c;
				}
			}
			
			return null;
		}
		
		private IClass GetCurrentClass(Task item)
		{
			ParseInformation parseInfo = ParserService.GetParseInformation(item.FileName);
			if (parseInfo != null) {
				IClass c = parseInfo.MostRecentCompilationUnit.GetInnermostClass(item.Line, item.Column);
				if (c != null) return c;
			}
			
			return null;
		}
		
		void OnSolutionOpen(object sender, SolutionEventArgs e)
		{
			taskView.ClearTasks();
		}
		
		void OnSolutionClosed(object sender, EventArgs e)
		{
			taskView.ClearTasks();
		}
		
		void TaskServiceCleared(object sender, EventArgs e)
		{
			taskView.ClearTasks();
		}
		
		void TaskServiceAdded(object sender, TaskEventArgs e)
		{
			if (e.Task.TaskType == TaskType.Comment) {
				UpdateItems();
			}
			
			RedrawContent();
		}
		
		void TaskServiceRemoved(object sender, TaskEventArgs e)
		{
			if (e.Task.TaskType == TaskType.Comment) {
				UpdateItems();
			}
			
			RedrawContent();
		}
		
		void InternalShowResults(object sender, EventArgs e)
		{
			UpdateItems();
			//taskView.UpdateResults(TaskService.CommentTasks);
		}
		
		public void ShowResults(object sender, EventArgs e)
		{
			taskView.Invoke(new EventHandler(InternalShowResults));
//			SelectTaskView(null, null);
		}
		
		#region IClipboardHandler interface implementation
		public bool EnableCut {
			get { return false; }
		}
		public bool EnableCopy {
			get { return taskView.TaskIsSelected; }
		}
		public bool EnablePaste {
			get { return false; }
		}
		public bool EnableDelete {
			get { return false; }
		}
		public bool EnableSelectAll {
			get { return true; }
		}
		
		public void Cut() {}
		public void Paste() {}
		public void Delete() {}
		
		public void Copy()
		{
			taskView.CopySelectionToClipboard();
		}
		public void SelectAll()
		{
			taskView.SelectAll();
		}
		#endregion
	}

}
