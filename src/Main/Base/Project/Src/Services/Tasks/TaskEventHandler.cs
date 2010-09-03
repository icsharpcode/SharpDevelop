// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.SharpDevelop
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
