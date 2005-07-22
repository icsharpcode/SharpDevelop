// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

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
