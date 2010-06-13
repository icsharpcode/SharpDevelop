// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Text;
using Gallio.Extension;
using NUnit.Framework;

namespace Gallio.SharpDevelop.Tests
{
	[TestFixture]
	public class TestResultsWriterTestFixture
	{
		TestResultsWriter writer;
		StringBuilder results;
		
		[SetUp]
		public void Init()
		{
			results = new StringBuilder();
			StringWriter stringWriter = new StringWriter(results);
			writer = new TestResultsWriter(stringWriter);
		}
		
		[TearDown]
		public void TearDown()
		{
			writer.Dispose();
		}
		
		[Test]
		public void WriteSuccessTestResult()
		{
			TestResult testResult = new TestResult("MyTests.MyTestClass.MyTestMethod");
			testResult.ResultType = TestResultType.Success;
			writer.Write(testResult);
			
			string expectedText =
				"Name: MyTests.MyTestClass.MyTestMethod\r\n" +
				"Result: Success\r\n";
			
			Assert.AreEqual(expectedText, results.ToString());
		}
		
		[Test]
		public void WriteIgnoredTestResult()
		{
			TestResult testResult = new TestResult("MyTests.MyTestClass.MyTestMethod");
			testResult.ResultType = TestResultType.Ignored;
			writer.Write(testResult);
			
			string expectedText =
				"Name: MyTests.MyTestClass.MyTestMethod\r\n" +
				"Result: Ignored\r\n";
			
			Assert.AreEqual(expectedText, results.ToString());
		}
		
		[Test]
		public void WriteFailedTestResult()
		{
			TestResult testResult = new TestResult("MyTests.MyTestClass.MyTestMethod");
			testResult.ResultType = TestResultType.Failure;
			testResult.Message = "message";
			testResult.StackTrace = "stacktrace";
			writer.Write(testResult);
			
			string expectedText =
				"Name: MyTests.MyTestClass.MyTestMethod\r\n" +
				"Result: Failure\r\n" +
				"Message: message\r\n" +
				"StackTrace: stacktrace\r\n";
			
			Assert.AreEqual(expectedText, results.ToString());
		}
		
		[Test]
		public void WriteFailedTestResultWithTwoLineMessage()
		{
			TestResult testResult = new TestResult("MyTests.MyTestClass.MyTestMethod");
			testResult.ResultType = TestResultType.Failure;
			testResult.Message = "first\r\n" +
				"second";
			testResult.StackTrace = "stacktrace";
			writer.Write(testResult);
			
			string expectedText =
				"Name: MyTests.MyTestClass.MyTestMethod\r\n" +
				"Result: Failure\r\n" +
				"Message: first\r\n" +
				" second\r\n" +
				"StackTrace: stacktrace\r\n";
			
			Assert.AreEqual(expectedText, results.ToString());
		}
		
		[Test]
		public void WriteFailedTestResultWithThreeLineStackTrace()
		{
			TestResult testResult = new TestResult("MyTests.MyTestClass.MyTestMethod");
			testResult.ResultType = TestResultType.Failure;
			testResult.Message = "first\r\n" +
				"second\r\n";
			testResult.StackTrace = "first\r\n" +
				"second\r\n" +
				"third\r\n";
			writer.Write(testResult);
			
			string expectedText =
				"Name: MyTests.MyTestClass.MyTestMethod\r\n" +
				"Result: Failure\r\n" +
				"Message: first\r\n" +
				" second\r\n" +
				"StackTrace: first\r\n" +
				" second\r\n" +
				" third\r\n";
			
			Assert.AreEqual(expectedText, results.ToString());
		}
	}
}
