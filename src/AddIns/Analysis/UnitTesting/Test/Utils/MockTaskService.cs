// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.UnitTesting;

namespace UnitTesting.Tests.Utils
{
	public class MockTaskService : IUnitTestTaskService
	{
		public bool IsClearExceptCommentTasksMethodCalled;
		public bool IsInUpdateWhilstClearExceptCommentTasksMethodCalled;
		public List<Task> Tasks = new List<Task>();
		public bool HasCriticalErrorsReturnValue;
		public bool TreatWarningsAsErrorsParameterPassedToHasCriticalErrors;
		
		bool somethingWentWrong;
		bool inUpdate;
		MessageViewCategory buildMessageViewCategory = new MessageViewCategory("Build");
		
		public bool SomethingWentWrong {
			get { return somethingWentWrong; }
			set { somethingWentWrong = value; }
		}
		
		public bool InUpdate {
			get { return inUpdate; }
			set { inUpdate = value; }
		}
		
		public MessageViewCategory BuildMessageViewCategory {
			get { return buildMessageViewCategory; }
		}
		
		public void ClearExceptCommentTasks()
		{
			IsClearExceptCommentTasksMethodCalled = true;
			IsInUpdateWhilstClearExceptCommentTasksMethodCalled = inUpdate;
		}
		
		public void Add(Task task)
		{
			if ((task.TaskType == TaskType.Error) || (task.TaskType == TaskType.Warning))  {
				SomethingWentWrong = true; 
			}
			Tasks.Add(task);
		}
						
		public bool HasCriticalErrors(bool treatWarningsAsErrors)
		{
			TreatWarningsAsErrorsParameterPassedToHasCriticalErrors = treatWarningsAsErrors;
			return HasCriticalErrorsReturnValue;
		}
	}
}
