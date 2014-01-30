// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
		public List<SDTask> Tasks = new List<SDTask>();
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
		
		public void Add(SDTask task)
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
