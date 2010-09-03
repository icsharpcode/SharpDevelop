// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.UnitTesting
{
	public class UnitTestTaskService : IUnitTestTaskService
	{
		public void ClearExceptCommentTasks()
		{
			TaskService.ClearExceptCommentTasks();
		}
		
		public bool InUpdate {
			get { return TaskService.InUpdate; }
			set { TaskService.InUpdate = value; }
		}
		
		public MessageViewCategory BuildMessageViewCategory {
			get { return TaskService.BuildMessageViewCategory; }
		}
		
		public void Add(Task task)
		{
			TaskService.Add(task);
		}
		
		public bool SomethingWentWrong {
			get { return TaskService.SomethingWentWrong; }
		}
		
		public bool HasCriticalErrors(bool treatWarningsAsErrors)
		{
			return TaskService.HasCriticalErrors(treatWarningsAsErrors);
		}
	}
}
