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
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Tree
{
	[TestFixture]
	public class TestResultIgnoreTaskTestFixture
	{
		SDTask task;
		
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
