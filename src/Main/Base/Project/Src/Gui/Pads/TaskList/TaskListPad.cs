// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class TaskListPad : AbstractPadContent, IClipboardHandler
	{
		TaskView taskView = new TaskView();
		
		public override Control Control {
			get {
				return taskView;
			}
		}
		
		public TaskListPad()
		{
			RedrawContent();

			TaskService.Cleared += new EventHandler(TaskServiceCleared);
			TaskService.Added   += new TaskEventHandler(TaskServiceAdded);
			TaskService.Removed += new TaskEventHandler(TaskServiceRemoved);
			
			ProjectService.SolutionLoaded += OnSolutionOpen;
			ProjectService.SolutionClosed += OnSolutionClosed;
			
			InternalShowResults(null, null);
		}
		
		public override void RedrawContent()
		{
			taskView.RefreshColumnNames();
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
				taskView.AddTask(e.Task);
			}
		}
		
		void TaskServiceRemoved(object sender, TaskEventArgs e)
		{
			if (e.Task.TaskType == TaskType.Comment) {
				taskView.RemoveTask(e.Task);
			}
		}
		
		void InternalShowResults(object sender, EventArgs e)
		{
			taskView.UpdateResults(TaskService.CommentTasks);
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
