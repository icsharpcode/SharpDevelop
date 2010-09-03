// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class ErrorListPad : AbstractPadContent, IClipboardHandler
	{
		public const string DefaultContextMenuAddInTreeEntry = "/SharpDevelop/Pads/ErrorList/TaskContextMenu";
		
		static ErrorListPad instance;
		public static ErrorListPad Instance {
			get {
				return instance;
			}
		}
		
		ToolStrip toolStrip;
		Panel contentPanel = new Panel();
		TaskView taskView = new TaskView() { DefaultContextMenuAddInTreeEntry = ErrorListPad.DefaultContextMenuAddInTreeEntry };
		
		Properties properties;
		
		public bool ShowErrors {
			get {
				return properties.Get<bool>("ShowErrors", true);
			}
			set {
				properties.Set<bool>("ShowErrors", value);
				InternalShowResults();
			}
		}
		
		public bool ShowMessages {
			get {
				return properties.Get<bool>("ShowMessages", true);
			}
			set {
				properties.Set<bool>("ShowMessages", value);
				InternalShowResults();
			}
		}
		
		public bool ShowWarnings {
			get {
				return properties.Get<bool>("ShowWarnings", true);
			}
			set {
				properties.Set<bool>("ShowWarnings", value);
				InternalShowResults();
			}
		}
		
		public static bool ShowAfterBuild {
			get {
				return Project.BuildOptions.ShowErrorListAfterBuild;
			}
			set {
				Project.BuildOptions.ShowErrorListAfterBuild = value;
			}
		}
		
		public override object Control {
			get {
				return contentPanel;
			}
		}
		
		public ErrorListPad()
		{
			instance = this;
			properties = PropertyService.Get("ErrorListPad", new Properties());
			
			RedrawContent();
			ResourceService.LanguageChanged += delegate { RedrawContent(); };
			
			TaskService.Cleared += new EventHandler(TaskServiceCleared);
			TaskService.Added   += new TaskEventHandler(TaskServiceAdded);
			TaskService.Removed += new TaskEventHandler(TaskServiceRemoved);
			TaskService.InUpdateChanged += delegate {
				if (!TaskService.InUpdate)
					InternalShowResults();
			};
			
			ProjectService.BuildFinished  += ProjectServiceEndBuild;
			ProjectService.SolutionLoaded += OnSolutionOpen;
			ProjectService.SolutionClosed += OnSolutionClosed;
			
			taskView.CreateControl();
			contentPanel.Controls.Add(taskView);
			
			toolStrip = ToolbarService.CreateToolStrip(this, "/SharpDevelop/Pads/ErrorList/Toolbar");
			toolStrip.Stretch   = true;
			toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			
			contentPanel.Controls.Add(toolStrip);
			
			InternalShowResults();
		}
		
		void RedrawContent()
		{
			taskView.RefreshColumnNames();
		}
		
		void OnSolutionOpen(object sender, SolutionEventArgs e)
		{
			taskView.ClearTasks();
			UpdateToolstripStatus();
		}
		
		void OnSolutionClosed(object sender, EventArgs e)
		{
			try {
				taskView.ClearTasks();
				UpdateToolstripStatus();
			} catch (Exception ex) {
				MessageService.ShowException(ex);
			}
		}
		
		void ProjectServiceEndBuild(object sender, EventArgs e)
		{
			if (TaskService.TaskCount > 0 && ShowAfterBuild) {
				WorkbenchSingleton.Workbench.GetPad(typeof(ErrorListPad)).BringPadToFront();
			}
			UpdateToolstripStatus();
		}
		
		public BuildResults BuildResults = null;
		
		void AddTask(Task task)
		{
			switch (task.TaskType) {
				case TaskType.Warning:
					if (!ShowWarnings) {
						return;
					}
					break;
				case TaskType.Error:
					if (!ShowErrors) {
						return;
					}
					break;
				case TaskType.Message:
					if (!ShowMessages) {
						return;
					}
					break;
				default:
					return;
			}
			
			taskView.AddTask(task);
		}
		
		
		void TaskServiceCleared(object sender, EventArgs e)
		{
			if (TaskService.InUpdate)
				return;
			taskView.ClearTasks();
			UpdateToolstripStatus();
		}
		
		void TaskServiceAdded(object sender, TaskEventArgs e)
		{
			if (TaskService.InUpdate)
				return;
			AddTask(e.Task);
			UpdateToolstripStatus();
		}
		
		void TaskServiceRemoved(object sender, TaskEventArgs e)
		{
			if (TaskService.InUpdate)
				return;
			taskView.RemoveTask(e.Task);
			UpdateToolstripStatus();
		}
		
		void UpdateToolstripStatus()
		{
			ToolbarService.UpdateToolbar(toolStrip);
			ToolbarService.UpdateToolbarText(toolStrip);
		}
		
		void InternalShowResults()
		{
			// listView.CreateControl is called in the constructor now.
			if (!taskView.IsHandleCreated) {
				return;
			}
			
			taskView.BeginUpdate();
			taskView.ClearTasks();
			
			foreach (Task task in TaskService.Tasks) {
				AddTask(task);
			}
			
			taskView.EndUpdate();
			UpdateToolstripStatus();
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
