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

namespace UnitTesting.Tests.Tree
{
	[TestFixture]
	public class TestResultFailureTaskTestFixture
	{
		Task task;
		
		[SetUp]
		public void Init()
		{
			TestResult testResult = new TestResult("MyNamespace.MyTests");
			testResult.ResultType = TestResultType.Failure;
			testResult.Message = "Test failed";
			testResult.StackTrace = 
				"Test Error : MyTest.Test\r\n" +
				"at TestResultTask.Create() in c:\\projects\\SharpDevelop\\TestResultTask.cs:line 45\r\n" +
				"at MyTest.Test() in c:\\myprojects\\test\\..\\test\\mytest.cs:line 28\r\n" +
				"";
			testResult.StackTraceFilePosition = 
				new FilePosition("c:\\myprojects\\test\\..\\test\\mytest.cs", 28, 1);
			task = TestResultTask.Create(testResult, null);
		}
		
		[Test] 
		public void TaskTypeIsError()
		{
			Assert.AreEqual(TaskType.Error, task.TaskType);
		}
		
		[Test]
		public void TaskMessageEqualsTestResultMessage()
		{
			Assert.AreEqual("Test failed", task.Description);
		}
		
		[Test]
		public void TaskFileNameMatchesFileNameInStackTrace()
		{
			string expectedFileName = @"c:\myprojects\test\mytest.cs";
			Assert.AreEqual(expectedFileName, task.FileName.ToString());
		}
		
		[Test]
		public void TaskLineNumberIs28WhichIsEqualToNUnitErrorLine()
		{
			Assert.AreEqual(28, task.Line);
		}
		
		[Test]
		public void TaskColumnNumberIsOne()
		{
			Assert.AreEqual(1, task.Column);
		}
	}
}
