using System;

namespace ICSharpCode.Core
{
	public delegate void TaskEventHandler(object sender, TaskEventArgs e);
	
	public class TaskEventArgs : EventArgs
	{
		Task task;
		
		public Task Task {
			get {
				return task;
			}
		}
		
		public TaskEventArgs(Task task)
		{
			this.task = task;
		}
	}
}
