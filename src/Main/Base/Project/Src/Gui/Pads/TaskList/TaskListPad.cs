// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class TaskListPad : AbstractPadContent, IClipboardHandler
	{
		public const string DefaultContextMenuAddInTreeEntry = "/SharpDevelop/Pads/TaskList/TaskContextMenu";
		
		static TaskListPad instance;
		Dictionary<string, bool> displayedTokens;
		IClass oldClass;
		int selectedScopeIndex = 0;
		bool isInitialized = false;
		
		public bool IsInitialized {
			get { return isInitialized; }
		}
		
		ToolStrip toolStrip;
		Panel contentPanel = new Panel();
		
		TaskView taskView = new TaskView() { DefaultContextMenuAddInTreeEntry = TaskListPad.DefaultContextMenuAddInTreeEntry };
		
		public Dictionary<string, bool> DisplayedTokens {
			get { return displayedTokens; }
		}
		
		public int SelectedScopeIndex {
			get { return selectedScopeIndex; }
			set { selectedScopeIndex = value;
				if (this.IsInitialized)
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
			TaskService.InUpdateChanged += new EventHandler(TaskServiceInUpdateChanged);
			
			WorkbenchSingleton.Workbench.ActiveViewContentChanged += new EventHandler(WorkbenchActiveViewContentChanged);
			
			if (WorkbenchSingleton.Workbench.ActiveViewContent != null) {
				UpdateItems();
				
				if (WorkbenchSingleton.Workbench.ActiveViewContent.Control is SharpDevelopTextAreaControl) {
					SharpDevelopTextAreaControl ctrl = WorkbenchSingleton.Workbench.ActiveViewContent.Control as SharpDevelopTextAreaControl;
					
					ctrl.ActiveTextAreaControl.Caret.PositionChanged += new EventHandler(CaretPositionChanged);
				}
			}
			
			ProjectService.SolutionLoaded += OnSolutionOpen;
			ProjectService.SolutionClosed += OnSolutionClosed;
			ProjectService.CurrentProjectChanged += ProjectServiceCurrentProjectChanged;
			
			this.isInitialized = true;
		}

		void ProjectServiceCurrentProjectChanged(object sender, ProjectEventArgs e)
		{
			if (isInitialized)
				UpdateItems();
		}

		void WorkbenchActiveViewContentChanged(object sender, EventArgs e)
		{
			if (WorkbenchSingleton.Workbench.ActiveViewContent == null)
				return;
			if (isInitialized)
				UpdateItems();
			
			if (WorkbenchSingleton.Workbench.ActiveViewContent.Control is SharpDevelopTextAreaControl) {
				SharpDevelopTextAreaControl ctrl = WorkbenchSingleton.Workbench.ActiveViewContent.Control as SharpDevelopTextAreaControl;
				
				ctrl.ActiveTextAreaControl.Caret.PositionChanged += new EventHandler(CaretPositionChanged);
			}
		}

		void CaretPositionChanged(object sender, EventArgs e)
		{
			if (this.selectedScopeIndex > 2)
			{
				IClass current = GetCurrentClass();
				
				if (oldClass == null) oldClass = current;
				
				if ((current != null) && (current.FullyQualifiedName != oldClass.FullyQualifiedName))
					UpdateItems();
			}
		}

		void TaskServiceInUpdateChanged(object sender, EventArgs e)
		{
			if (!TaskService.InUpdate)
				UpdateItems();
		}
		
		void InitializeToolStrip()
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
			this.taskView.BeginUpdate();
			
			this.taskView.ClearTasks();
			
			foreach (Task t in TaskService.CommentTasks) {
				this.AddItem(t);
			}
			
			RedrawContent();
			this.taskView.EndUpdate();
		}
		
		void AddItem(Task item)
		{
			foreach (KeyValuePair<string, bool> pair in displayedTokens) {
				if (item.Description.StartsWith(pair.Key) && pair.Value && IsInScope(item))
					this.taskView.AddTask(item);
			}
		}
		
		bool IsInScope(Task item)
		{
			IClass current = GetCurrentClass();
			IClass itemClass = GetCurrentClass(item);
			
			switch (this.selectedScopeIndex) {
				case 0:
					if (ProjectService.OpenSolution != null) {
						foreach (AbstractProject proj in ProjectService.OpenSolution.Projects) {
							if (proj.FindFile(item.FileName) != null)
								return true;
						}
					}
					return false;
				case 1:
					return ProjectService.CurrentProject != null && ProjectService.CurrentProject.FindFile(item.FileName) != null;
				case 2:
					return WorkbenchSingleton.Workbench.ActiveViewContent != null && WorkbenchSingleton.Workbench.ActiveViewContent.PrimaryFileName == item.FileName;
				case 3:
					return current != null && itemClass != null && current.Namespace == itemClass.Namespace;
				case 4:
					return current != null && itemClass != null && current == itemClass;
			}
			
			return true;
		}
		
		IClass GetCurrentClass()
		{
			if (WorkbenchSingleton.Workbench.ActiveViewContent == null)
				return null;
			
			ParseInformation parseInfo = ParserService.GetParseInformation(WorkbenchSingleton.Workbench.ActiveViewContent.PrimaryFileName);
			if (parseInfo != null) {
				IPositionable positionable = WorkbenchSingleton.Workbench.ActiveViewContent as IPositionable;
				if (positionable != null) {
					IClass c = parseInfo.MostRecentCompilationUnit.GetInnermostClass(positionable.Line, positionable.Column);
					if (c != null) return c;
				}
			}
			
			return null;
		}
		
		IClass GetCurrentClass(Task item)
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
			this.taskView.BeginUpdate();

			if (e.Task.TaskType == TaskType.Comment) {
				AddItem(e.Task);
			}
			
			RedrawContent();
			
			this.taskView.EndUpdate();
		}
		
		void TaskServiceRemoved(object sender, TaskEventArgs e)
		{
			this.taskView.BeginUpdate();
			
			if (e.Task.TaskType == TaskType.Comment) {
				foreach (ListViewItem item in this.taskView.Items) {
					if (item.Tag == e.Task) {
						this.taskView.Items.Remove(item);
						break;
					}
				}
			}
			
			RedrawContent();
			this.taskView.EndUpdate();
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
