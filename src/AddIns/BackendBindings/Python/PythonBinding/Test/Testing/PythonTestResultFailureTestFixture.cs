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
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.PythonBinding;
using ICSharpCode.UnitTesting;
using NUnit.Framework;

namespace PythonBinding.Tests.Testing
{
	[TestFixture]
	public class PythonTestResultFailureTestFixture
	{
		PythonTestResult pythonTestResult;
		string stackTraceText;
		
		[SetUp]
		public void Init()
		{
			TestResult testResult = new TestResult("MyTest");
			testResult.ResultType = TestResultType.Failure;
			testResult.Message = "test failed";
			
			stackTraceText = 
				"Traceback (most recent call last):\r\n" +
				"  File \"d:\\temp\\test\\PyTests\\Tests\\MyClassTest.py\", line 16, in testAssertEquals\r\n" +
				"    self.assertEqual(10, 15, 'wrong size after resize')\r\n" +
				"AssertionError: wrong size after resize";
			
			testResult.StackTrace = stackTraceText;
			pythonTestResult = new PythonTestResult(testResult);
		}
		
		[Test]
		public void TestResultNameIsMyTest()
		{
			Assert.AreEqual("MyTest", pythonTestResult.Name);
		}
		
		[Test]
		public void TestResultTypeIsFailure()
		{
			Assert.AreEqual(TestResultType.Failure, pythonTestResult.ResultType);
		}
		
		[Test]
		public void TestResultMessageIsTestFailed()
		{
			Assert.AreEqual("test failed", pythonTestResult.Message);
		}
		
		[Test]
		public void PythonTestResultHasStackTraceFromOriginalTestResult()
		{
			Assert.AreEqual(stackTraceText, pythonTestResult.StackTrace);
		}
		
		[Test]
		public void StackTraceFilePositionHasExpectedFileName()
		{
			string expectedFileName = "d:\\temp\\test\\PyTests\\Tests\\MyClassTest.py";
			Assert.AreEqual(expectedFileName, pythonTestResult.StackTraceFilePosition.FileName);
		}
		
		[Test]
		public void StackTraceFilePositionLineIs16()
		{
			Assert.AreEqual(16, pythonTestResult.StackTraceFilePosition.Line);
		}
		
		[Test]
		public void StackTraceFilePositionColumnIsOne()
		{
			Assert.AreEqual(1, pythonTestResult.StackTraceFilePosition.Column);
		}
		
		[Test]
		public void ChangingStackTraceToEmptyStringSetsStackTraceFilePositionToEmpty()
		{
			pythonTestResult.StackTraceFilePosition = new FilePosition("test.cs", 10, 2);
			pythonTestResult.StackTrace = String.Empty;
			Assert.IsTrue(pythonTestResult.StackTraceFilePosition.IsEmpty);
		}	
	}
}
