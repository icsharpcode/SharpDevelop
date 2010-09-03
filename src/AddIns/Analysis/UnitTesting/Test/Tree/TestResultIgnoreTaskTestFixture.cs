// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Tree
{
	[TestFixture]
	public class TestResultIgnoreTaskTestFixture
	{
		Task task;
		
		[SetUp]
		public void Init()
		{
			string className = "MyNamespace.MyTests";
			TestProject testProject = 
				TestProjectHelper.CreateTestProjectWithTestClassAndSingleTestMethod(className, "MyTestMethod");
			
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
		public void TaskFileNameMatchesCompilationUnitFileName()
		{
			string expectedFileName = @"c:\projects\tests\MyTests.cs";
			Assert.AreEqual(expectedFileName, task.FileName.ToString());
		}
		
		[Test]
		public void TaskLineNumberIs4SameAsMethod()
		{
			Assert.AreEqual(4, task.Line);
		}
		
		[Test]
		public void TaskColumnNumberIs19SameAsMethod()
		{
			Assert.AreEqual(19, task.Column); 
		}
	}
}
