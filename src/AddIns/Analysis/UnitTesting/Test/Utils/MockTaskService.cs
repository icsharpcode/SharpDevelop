// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.UnitTesting;

namespace UnitTesting.Tests.Utils
{
	public class MockTaskService : IUnitTestTaskService
	{
		MessageViewCategory buildMessageViewCategory = new MessageViewCategory("Build");
		bool clearExceptCommentTasksMethodCalled;
		bool isInUpdateWhilstClearExceptCommentTasksMethodCalled;
		bool inUpdate;
		bool somethingWentWrong;
		List<Task> tasks = new List<Task>();
		
		public bool IsClearExceptCommentTasksMethodCalled {
			get { return clearExceptCommentTasksMethodCalled; }
		}
		
		public void ClearExceptCommentTasks()
		{
			clearExceptCommentTasksMethodCalled = true;
			isInUpdateWhilstClearExceptCommentTasksMethodCalled = inUpdate;
		}
		
		public bool IsInUpdateWhilstClearExceptCommentTasksMethodCalled {
			get { return isInUpdateWhilstClearExceptCommentTasksMethodCalled; }
		}
		
		public bool InUpdate {
			get { return inUpdate; }
			set { inUpdate = value; }
		}
		
		public MessageViewCategory BuildMessageViewCategory {
			get { return buildMessageViewCategory; }
		}
		
		public bool SomethingWentWrong {
			get { return somethingWentWrong; }
		}
		
		public List<Task> Tasks {
			get { return tasks; }
		}
		
		public void Add(Task task)
		{
			if ((task.TaskType == TaskType.Error) || (task.TaskType == TaskType.Warning))  {
				somethingWentWrong = true; 
			}
			tasks.Add(task);
		}
	}
}
