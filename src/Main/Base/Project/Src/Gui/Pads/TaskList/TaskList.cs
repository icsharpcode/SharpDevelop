// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
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
	public class OpenTaskView : AbstractPadContent
	{
		TaskView taskView = new TaskView();
		
		public override Control Control {
			get {
				return taskView;
			}
		}
		
		public OpenTaskView()
		{		
			RedrawContent();

			TaskService.Cleared += new EventHandler(TaskServiceCleared);
			TaskService.Added   += new TaskEventHandler(TaskServiceAdded);
			TaskService.Removed += new TaskEventHandler(TaskServiceRemoved);
			
			ProjectService.SolutionLoaded += OnCombineOpen;
			ProjectService.SolutionClosed += OnCombineClosed;
									
			ShowResults2(null, null);
		}
		
		public override void RedrawContent()
		{
			taskView.RefreshColumnNames();
		}
		
		void OnCombineOpen(object sender, SolutionEventArgs e)
		{
			taskView.Items.Clear();
		}
		
		void OnCombineClosed(object sender, EventArgs e)
		{
			taskView.Items.Clear();
		}
		
		public CompilerResults CompilerResults = null;
		
		void TaskServiceCleared(object sender, EventArgs e)
		{
			taskView.Items.Clear();
		}
		
		void TaskServiceAdded(object sender, TaskEventArgs e)
		{
			if (e.Task.TaskType == TaskType.Comment) {
				taskView.AddTask(e.Task);
			}
		}
		
		void TaskServiceRemoved(object sender, TaskEventArgs e)
		{
			Task task = e.Task;
			if (task.TaskType == TaskType.Comment) {
				for (int i = 0; i < taskView.Items.Count; ++i) {
					if ((Task)taskView.Items[i].Tag == task) {
						taskView.Items.RemoveAt(i);
						break;
					}
				}
			}
		}
		
		void ShowResults2(object sender, EventArgs e)
		{
			taskView.BeginUpdate();
			taskView.Items.Clear();
			
			foreach (Task task in TaskService.CommentTasks) {
				taskView.AddTask(task);
			}
			
			taskView.EndUpdate();
		}
		
		public void ShowResults(object sender, EventArgs e)
		{
			taskView.Invoke(new EventHandler(ShowResults2));
//			SelectTaskView(null, null);
		}
	}

}
