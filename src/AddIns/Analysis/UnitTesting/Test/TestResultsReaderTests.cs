// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.UnitTesting;
using NUnit.Framework;

namespace UnitTesting.Tests
{
	[TestFixture]
	public class TestResultsReaderTests
	{
		[Test]
		public void OneTestPass()
		{
			string resultsText = "Name: MyTest\r\n" +
				"Result: Success\r\n";

			TestResultsReader reader = new TestResultsReader();
			TestResult[] results = reader.Read(resultsText);
			
			Assert.AreEqual(1, results.Length);
			
			TestResult result = results[0];
			Assert.AreEqual("MyTest", result.Name);
			Assert.IsTrue(result.IsSuccess);
			Assert.AreEqual(TestResultType.Success, result.ResultType);
		}
		
		[Test]
		public void OneTestIgnored()
		{
			string resultsText = "Name: MyTest\r\n" +
				"Result: Ignored\r\n";

			TestResultsReader reader = new TestResultsReader();
			TestResult[] results = reader.Read(resultsText);
			
			Assert.AreEqual(1, results.Length);
			
			TestResult result = results[0];
			Assert.AreEqual("MyTest", result.Name);
			Assert.IsTrue(result.IsIgnored);
			Assert.IsFalse(result.IsSuccess);
			Assert.AreEqual(TestResultType.Ignored, result.ResultType);
		}
		
		[Test]
		public void OneTestPassInParts()
		{
			string resultsText = "Name: MyTest\r\n" +
				"Result: Success\r\n";

			TestResultsReader reader = new TestResultsReader();
			
			List<TestResult> results = new List<TestResult>();
			foreach (char ch in resultsText) {
				TestResult[] readResults = reader.Read(ch.ToString());
				if (readResults.Length > 0) {
					foreach (TestResult readResult in readResults) {
						results.Add(readResult);
					}
				}
			}
			
			Assert.AreEqual(1, results.Count);
			
			TestResult result = results[0];
			Assert.AreEqual("MyTest", result.Name);
			Assert.IsTrue(result.IsSuccess);
		}
		
		[Test]
		public void OneTestFailure()
		{
			string resultsText = "Name: MyTest\r\n" +
				"Result: Failure\r\n";

			TestResultsReader reader = new TestResultsReader();
			TestResult[] results = reader.Read(resultsText);
			
			Assert.AreEqual(1, results.Length);
			
			TestResult result = results[0];
			Assert.AreEqual("MyTest", result.Name);
			Assert.IsTrue(result.IsFailure);
			Assert.IsFalse(result.IsSuccess);
			Assert.IsFalse(result.IsIgnored);
			Assert.AreEqual(TestResultType.Failure, result.ResultType);
		}
		
		[Test]
		public void TestMessage()
		{
			string resultsText = "Name: Test\r\n" +
				"Message: Should not be 0.\r\n" +
				"Result: Failure\r\n";

			TestResultsReader reader = new TestResultsReader();
			TestResult[] results = reader.Read(resultsText);
			
			Assert.AreEqual(1, results.Length);
			
			TestResult result = results[0];
			Assert.AreEqual("Test", result.Name);
			Assert.AreEqual("Should not be 0.", result.Message);
			Assert.IsTrue(result.IsFailure);
		}
		
		[Test]
		public void TestStackTrace()
		{
			string resultsText = "Name: Test\r\n" +
				"StackTrace: stack trace\r\n" +
				"Result: Failure\r\n";

			TestResultsReader reader = new TestResultsReader();
			TestResult[] results = reader.Read(resultsText);
			
			Assert.AreEqual(1, results.Length);
			
			TestResult result = results[0];
			Assert.AreEqual("Test", result.Name);
			Assert.AreEqual("stack trace", result.StackTrace);
			Assert.IsTrue(result.IsFailure);
		}
		
		[Test]
		public void ResultWithNoTestName()
		{
			string resultsText = "Result: Failure\r\n";

			TestResultsReader reader = new TestResultsReader();
			TestResult[] results = reader.Read(resultsText);
			
			Assert.AreEqual(0, results.Length);
		}
		
		[Test]
		public void MissingNameValuePairOnFirstLine()
		{
			string resultsText = "MissingNameValuePair\r\n" +
				"Name: Test\r\n" +
				"Result: Failure\r\n";

			TestResultsReader reader = new TestResultsReader();
			TestResult[] results = reader.Read(resultsText);
			
			Assert.AreEqual(1, results.Length);
			
			TestResult result = results[0];
			Assert.AreEqual("Test", result.Name);
			Assert.IsTrue(result.IsFailure);
		}
		
		[Test]
		public void TwoLineTestMessage()
		{
			string resultsText = "Name: Test\r\n" +
				"Message: Should not be 0.\r\n" +
				" Should be 1.\r\n" +
				"Result: Failure\r\n";

			TestResultsReader reader = new TestResultsReader();
			TestResult[] results = reader.Read(resultsText);
			
			Assert.AreEqual(1, results.Length);
			
			TestResult result = results[0];
			Assert.AreEqual("Test", result.Name);
			Assert.AreEqual("Should not be 0.\r\nShould be 1.", result.Message);
			Assert.IsTrue(result.IsFailure);
		}
		
		[Test]
		public void ThreeLineTestMessage()
		{
			string resultsText = "Name: Test\r\n" +
				"Message: Should not be 0.\r\n" +
				" Should be 1.\r\n" +
				" End of message.\r\n" +
				"Result: Failure\r\n";

			TestResultsReader reader = new TestResultsReader();
			TestResult[] results = reader.Read(resultsText);
			
			Assert.AreEqual(1, results.Length);
			
			TestResult result = results[0];
			Assert.AreEqual("Test", result.Name);
			Assert.AreEqual("Should not be 0.\r\nShould be 1.\r\nEnd of message.", result.Message);
			Assert.IsTrue(result.IsFailure);
		}
		
		[Test]
		public void TwoTestFailures()
		{
			string resultsText = "Name: MyTest1\r\n" +
				"Result: Failure\r\n" +
				"Name: MyTest2\r\n" +
				"Result: Failure\r\n";

			TestResultsReader reader = new TestResultsReader();
			TestResult[] results = reader.Read(resultsText);
			
			Assert.AreEqual(2, results.Length);
			
			TestResult result1 = results[0];
			Assert.AreEqual("MyTest1", result1.Name);
			Assert.IsTrue(result1.IsFailure);
			
			TestResult result2 = results[1];
			Assert.AreEqual("MyTest2", result2.Name);
			Assert.IsTrue(result2.IsFailure);
		}
		
		[Test]
		public void TwoTestFailuresWithMultilineMessages()
		{
			string resultsText = "Name: MyTest1\r\n" +
				"Message: FirstLine\r\n" +
				" SecondLine\r\n" +
				"Result: Failure\r\n" +
				"Name: MyTest2\r\n" +
				"Message: FirstLine\r\n" +
				" SecondLine\r\n" +
				" ThirdLine\r\n" +
				"Result: Failure\r\n";

			TestResultsReader reader = new TestResultsReader();
			TestResult[] results = reader.Read(resultsText);
			
			Assert.AreEqual(2, results.Length);
			
			TestResult result1 = results[0];
			Assert.AreEqual("MyTest1", result1.Name);
			Assert.AreEqual("FirstLine\r\nSecondLine", result1.Message);
			Assert.IsTrue(result1.IsFailure);
			
			TestResult result2 = results[1];
			Assert.AreEqual("MyTest2", result2.Name);
			Assert.AreEqual("FirstLine\r\nSecondLine\r\nThirdLine", result2.Message);
			Assert.IsTrue(result2.IsFailure);
		}
	}
}
