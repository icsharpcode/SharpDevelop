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
using ICSharpCode.SharpDevelop;
using ICSharpCode.UnitTesting;
using NUnit.Framework;

namespace UnitTesting.Tests.Tree
{
	[TestFixture]
	public class TestResultFailureTaskTestFixture
	{
		SDTask task;
		
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
