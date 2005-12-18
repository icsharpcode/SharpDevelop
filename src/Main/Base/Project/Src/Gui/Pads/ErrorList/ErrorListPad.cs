// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using System.Drawing;
using System.CodeDom.Compiler;
using System.Collections;
using System.IO;
using System.Diagnostics;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class ErrorListPad : AbstractPadContent
	{
		static ErrorListPad instance;
		public static ErrorListPad Instance {
			get {
				return instance;
			}
		}
		
		ToolStrip toolStrip;
		Panel contentPanel = new Panel();
		
		TaskView taskView = new TaskView();

		bool showWarnings = true;
		bool showErrors = true;
		bool showMessages = true;
		
		public bool ShowErrors {
			get {
				return showErrors;
			}
			set {
				showErrors = value;
				InternalShowResults();
			}
		}
		
		public bool ShowMessages {
			get {
				return showMessages;
			}
			set {
				showMessages = value;
				InternalShowResults();
			}
		}
		
		public bool ShowWarnings {
			get {
				return showWarnings;
			}
			set {
				showWarnings = value;
				InternalShowResults();
			}
		}
		
		public override Control Control {
			get {
				return contentPanel;
			}
		}
		
		public ErrorListPad()
		{
			instance = this;
			
			RedrawContent();
			
			TaskService.Cleared += new EventHandler(TaskServiceCleared);
			TaskService.Added   += new TaskEventHandler(TaskServiceAdded);
			TaskService.Removed += new TaskEventHandler(TaskServiceRemoved);
			TaskService.InUpdateChanged += delegate {
				if (!TaskService.InUpdate)
					InternalShowResults();
			};
			
			ProjectService.EndBuild       += ProjectServiceEndBuild;
			ProjectService.SolutionLoaded += OnCombineOpen;
			ProjectService.SolutionClosed += OnCombineClosed;
			
			taskView.CreateControl();
			contentPanel.Controls.Add(taskView);
			
			toolStrip = ToolbarService.CreateToolStrip(this, "/SharpDevelop/Pads/ErrorList/Toolbar");
			toolStrip.Stretch   = true;
			toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			
			contentPanel.Controls.Add(toolStrip);
			
			InternalShowResults();
		}
		
		public override void RedrawContent()
		{
			taskView.RefreshColumnNames();
		}
		
		void OnCombineOpen(object sender, SolutionEventArgs e)
		{
			taskView.ClearTasks();
			UpdateToolstripStatus();
		}
		
		void OnCombineClosed(object sender, EventArgs e)
		{
			try {
				taskView.ClearTasks();
				UpdateToolstripStatus();
			} catch (Exception ex) {
				MessageService.ShowError(ex);
			}
		}
		
		void ProjectServiceEndBuild(object sender, EventArgs e)
		{
			if (TaskService.TaskCount > 0) {
				WorkbenchSingleton.Workbench.WorkbenchLayout.ActivatePad(this.GetType().FullName);
			}
			UpdateToolstripStatus();
		}
		
		public CompilerResults CompilerResults = null;
		
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
	}
}
