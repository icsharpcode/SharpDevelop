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
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using NUnit.Framework;

namespace UnitTesting.Tests.Utils.Tests
{
	[TestFixture]
	public class MockTaskServiceTestFixture
	{
		MockTaskService taskService;
		
		[SetUp]
		public void Init()
		{
			taskService = new MockTaskService();
		}
		
		[Test]
		public void IsClearExceptCommentTasksMethodCalledReturnsFalseByDefault()
		{
			Assert.IsFalse(taskService.IsClearExceptCommentTasksMethodCalled);
		}
		
		[Test]
		public void IsClearExceptCommentTasksMethodCalledReturnsTrueAfterClearExceptCommentTasksMethodCalled()
		{
			taskService.ClearExceptCommentTasks();
			Assert.IsTrue(taskService.IsClearExceptCommentTasksMethodCalled);
		}
		
		[Test]
		public void IsInUpdateWhilstClearExceptCommentTasksMethodCalledReturnsFalseByDefault()
		{
			Assert.IsFalse(taskService.IsInUpdateWhilstClearExceptCommentTasksMethodCalled);
		}
		
		[Test]
		public void IsInUpdateWhilstClearExceptCommentTasksMethodCalledReturnsFalseWhenIsInUpdateIsFalseDuringMethodCall()
		{
			taskService.ClearExceptCommentTasks();
			Assert.IsFalse(taskService.IsInUpdateWhilstClearExceptCommentTasksMethodCalled);
		}
		
		[Test]
		public void IsInUpdateWhilstClearExceptCommentTasksMethodCalledReturnsTrueWhenIsInUpdateIsTrueDuringMethodCall()
		{
			taskService.InUpdate = true;
			taskService.ClearExceptCommentTasks();
			Assert.IsTrue(taskService.IsInUpdateWhilstClearExceptCommentTasksMethodCalled);
		}
		
		[Test]
		public void SomethingWentWrongReturnsFalseByDefault()
		{
			Assert.IsFalse(taskService.SomethingWentWrong);
		}
		
		[Test]
		public void SomethingWentWrongReturnsTrueWhenErrorTaskAddedToTaskService()
		{
			FileName fileName = new FileName("test.cs");
			Task task = new Task(fileName, String.Empty, 1, 2, TaskType.Error);
			taskService.Add(task);
			Assert.IsTrue(taskService.SomethingWentWrong);
		}
		
		[Test]
		public void SomethingWentWrongReturnsTrueWhenWarningTaskAddedToTaskService()
		{
			FileName fileName = new FileName("test.cs");
			Task task = new Task(fileName, String.Empty, 1, 2, TaskType.Warning);
			taskService.Add(task);
			Assert.IsTrue(taskService.SomethingWentWrong);
		}
		
		[Test]
		public void NoTasksAddedToTaskServiceByDefault()
		{
			Assert.AreEqual(0, taskService.Tasks.Count);
		}
		
		[Test]
		public void TasksAddedToTaskServiceAreSaved()
		{
			FileName fileName = new FileName("test.cs");
			Task task = new Task(fileName, "description", 1, 1, TaskType.Error);
			taskService.Add(task);
			
			Task[] tasks = new Task[] { task };
			
			Assert.AreEqual(tasks, taskService.Tasks.ToArray());
		}
		
		[Test]
		public void HasCriticalErrorsWhenReturnValueSetToTrueReturnsTrue()
		{
			taskService.HasCriticalErrorsReturnValue = true;
			Assert.IsTrue(taskService.HasCriticalErrors(false));
		}
		
		[Test]
		public void HasCriticalErrorsWhenReturnValueSetToFalseReturnsFalse()
		{
			taskService.HasCriticalErrorsReturnValue = false;
			Assert.IsFalse(taskService.HasCriticalErrors(false));
		}
		
		[Test]
		public void HasCriticalErrorsWhenTreatWarningsAsErrorsParameterSetToFalseRecordsParameterValue()
		{
			taskService.HasCriticalErrors(false);
			Assert.IsFalse(taskService.TreatWarningsAsErrorsParameterPassedToHasCriticalErrors);
		}
		
		[Test]
		public void HasCriticalErrorsWhenTreatWarningsAsErrorsParameterSetToTrueRecordsParameterValue()
		{
			taskService.HasCriticalErrors(true);
			Assert.IsTrue(taskService.TreatWarningsAsErrorsParameterPassedToHasCriticalErrors);
		}
	}
}
