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
	public class TaskListPad : AbstractPadContent
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
			
			ProjectService.SolutionLoaded += OnCombineOpen;
			ProjectService.SolutionClosed += OnCombineClosed;
			
			InternalShowResults(null, null);
		}
		
		public override void RedrawContent()
		{
			taskView.RefreshColumnNames();
		}
		
		void OnCombineOpen(object sender, SolutionEventArgs e)
		{
			taskView.ClearTasks();
		}
		
		void OnCombineClosed(object sender, EventArgs e)
		{
			taskView.ClearTasks();
		}
		
		public CompilerResults CompilerResults = null;
		
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
	}

}
