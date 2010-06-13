// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Tree
{
	[TestFixture]
	public class TestResultIgnoreTaskWithoutMatchingTestMethodTestFixture
	{
		Task task;
		
		[SetUp]
		public void Init()
		{
			MockRegisteredTestFrameworks testFrameworks = new MockRegisteredTestFrameworks();
			MockClass c = MockClass.CreateMockClassWithoutAnyAttributes();
			TestClass testClass = new TestClass(c, testFrameworks);
			
			TestProject testProject = new TestProject(c.Project, c.ProjectContent, testFrameworks);
			testProject.TestClasses.Add(testClass);
			
			TestResult testResult = new TestResult("MyNamespace.MyTests.MyTestMethod");
			testResult.ResultType = TestResultType.Ignored;
			testResult.Message = "Test ignored";
			
			task = TestResultTask.Create(testResult, testProject);
		}
		
		[Test] 
		public void TaskTypeIsWarning()
		{
			Assert.AreEqual(TaskType.Warning, task.TaskType);
		}
		
		[Test]
		public void TaskMessageEqualsTestResultMessage()
		{
			Assert.AreEqual("Test ignored", task.Description);
		}
		
		[Test]
		public void TaskFileNameIsNull()
		{
			Assert.IsNull(task.FileName);
		}
		
		[Test]
		public void TaskLineNumberIsZero()
		{
			Assert.AreEqual(0, task.Line);
		}
		
		[Test]
		public void TaskColumnNumberIsZero()
		{
			Assert.AreEqual(0, task.Column); 
		}
	}
}
