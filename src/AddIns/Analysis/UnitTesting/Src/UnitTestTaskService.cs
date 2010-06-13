// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
	}
}
