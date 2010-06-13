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
			Assert.AreEqual(28, nunitTestResult.StackTraceFilePosition.Line);
		}
		
		[Test]
		public void StackTraceFilePositionColumnNumberIsOne()
		{
			Assert.AreEqual(1, nunitTestResult.StackTraceFilePosition.Column);
		}
		
		[Test]
		public void ChangingStackTraceToEmptyStringSetsStackTraceFilePositionToEmpty()
		{
			nunitTestResult.StackTraceFilePosition = new FilePosition("test.cs", 10, 2);
			nunitTestResult.StackTrace = String.Empty;
			Assert.IsTrue(nunitTestResult.StackTraceFilePosition.IsEmpty);
		}
	}
}
