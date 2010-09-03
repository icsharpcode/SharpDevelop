// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
