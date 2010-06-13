// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
	}
}
