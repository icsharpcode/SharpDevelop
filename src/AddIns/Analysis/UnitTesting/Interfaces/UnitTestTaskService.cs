// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.UnitTesting
{
	public class UnitTestTaskService : IUnitTestTaskService
	{
		public void ClearExceptCommentTasks()
		{
			TaskService.ClearExceptCommentTasks();
		}
		
		public IOutputCategory BuildMessageViewCategory {
			get { return TaskService.BuildMessageViewCategory; }
		}
		
		public void Add(SDTask task)
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
