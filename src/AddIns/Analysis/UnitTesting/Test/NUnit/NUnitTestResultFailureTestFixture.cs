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
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.UnitTesting;
using NUnit.Framework;

namespace UnitTesting.Tests.NUnit
{
	[TestFixture]
	public class NUnitTestResultFailureTestFixture
	{
		NUnitTestResult nunitTestResult;

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
			nunitTestResult = new NUnitTestResult(testResult);
		}
		
		[Test] 
		public void TestResultTypeIsFailure()
		{
			Assert.AreEqual(TestResultType.Failure, nunitTestResult.ResultType);
		}
		
		[Test]
		public void TestResultMessageIsTestFailed()
		{
			Assert.AreEqual("Test failed", nunitTestResult.Message);
		}
		
		[Test]
		public void StackTraceFilePositionFileNameMatchesLastFileNameInStackTrace()
		{
			string expectedFileName = @"c:\myprojects\test\mytest.cs";
			Assert.AreEqual(expectedFileName, nunitTestResult.StackTraceFilePosition.FileName);
		}
		
		[Test]
		public void StackTraceFilePositionLineNumberIs28WhichIsEqualToReportedNUnitErrorLine()
		{
			Assert.AreEqual(28, nunitTestResult.StackTraceFilePosition.BeginLine);
		}
		
		[Test]
		public void StackTraceFilePositionColumnNumberIsOne()
		{
			Assert.AreEqual(1, nunitTestResult.StackTraceFilePosition.BeginColumn);
		}
		
		[Test]
		public void ChangingStackTraceToEmptyStringSetsStackTraceFilePositionToEmpty()
		{
			nunitTestResult.StackTraceFilePosition = new DomRegion("test.cs", 10, 2);
			nunitTestResult.StackTrace = String.Empty;
			Assert.IsTrue(nunitTestResult.StackTraceFilePosition.IsEmpty);
		}
	}
}
