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
using System.Collections.Generic;
using System.IO;
using System.Text;
using ICSharpCode.UnitTesting;
using NUnit.Framework;

namespace UnitTesting.Tests.TestRunner
{
	[TestFixture]
	public class TestResultsReaderTests
	{
		List<TestResult> ReadTestResults(string resultsText)
		{
			var reader = new TestResultsReader(new StringReader(resultsText));
			var results = new List<TestResult>();
			reader.TestFinished += (sender, e) => results.Add(e.Result);
			reader.Run();
			return results;
		}
		
		[Test]
		public void OneTestPass()
		{
			string resultsText =
				"Name: MyTest\r\n" +
				"Result: Success\r\n";
			
			List<TestResult> results = ReadTestResults(resultsText);
			
			Assert.AreEqual(1, results.Count);
			
			TestResult result = results[0];
			Assert.AreEqual("MyTest", result.Name);
			Assert.IsTrue(result.IsSuccess);
			Assert.AreEqual(TestResultType.Success, result.ResultType);
		}
		
		[Test]
		public void OneTestIgnored()
		{
			string resultsText =
				"Name: MyTest\r\n" +
				"Result: Ignored\r\n";
			
			List<TestResult> results = ReadTestResults(resultsText);
			
			Assert.AreEqual(1, results.Count);
			
			TestResult result = results[0];
			Assert.AreEqual("MyTest", result.Name);
			Assert.IsTrue(result.IsIgnored);
			Assert.IsFalse(result.IsSuccess);
			Assert.AreEqual(TestResultType.Ignored, result.ResultType);
		}
		
		[Test]
		public void OneTestPassInParts()
		{
			string resultsText =
				"Name: MyTest\r\n" +
				"Result: Success\r\n";
			
			var stream = new MemoryStream();
			var streamReader = new StreamReader(stream);
			var reader = new TestResultsReader(streamReader);
			var results = new List<TestResult>();
			reader.TestFinished += (sender, e) => results.Add(e.Result);
			
			foreach (char ch in resultsText) {
				byte[] bytes = Encoding.UTF8.GetBytes(new char[] { ch });
				stream.Write(bytes, 0, bytes.Length);
				stream.Position--;
				reader.Run();
			}
			
			Assert.AreEqual(1, results.Count);
			
			TestResult result = results[0];
			Assert.AreEqual("MyTest", result.Name);
			Assert.IsTrue(result.IsSuccess);
		}
		
		[Test]
		public void OneTestFailure()
		{
			string resultsText =
				"Name: MyTest\r\n" +
				"Result: Failure\r\n";
			
			List<TestResult> results = ReadTestResults(resultsText);
			
			Assert.AreEqual(1, results.Count);
			
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
			string resultsText =
				"Name: Test\r\n" +
				"Message: Should not be 0.\r\n" +
				"Result: Failure\r\n";
			
			List<TestResult> results = ReadTestResults(resultsText);
			
			Assert.AreEqual(1, results.Count);
			
			TestResult result = results[0];
			Assert.AreEqual("Test", result.Name);
			Assert.AreEqual("Should not be 0.", result.Message);
			Assert.IsTrue(result.IsFailure);
		}
		
		[Test]
		public void TestStackTrace()
		{
			string resultsText =
				"Name: Test\r\n" +
				"StackTrace: stack trace\r\n" +
				"Result: Failure\r\n";
			
			List<TestResult> results = ReadTestResults(resultsText);
			
			Assert.AreEqual(1, results.Count);
			
			TestResult result = results[0];
			Assert.AreEqual("Test", result.Name);
			Assert.AreEqual("stack trace", result.StackTrace);
			Assert.IsTrue(result.IsFailure);
		}
		
		[Test]
		public void ResultWithNoTestName()
		{
			string resultsText = "Result: Failure\r\n";
			
			List<TestResult> results = ReadTestResults(resultsText);
			
			Assert.AreEqual(0, results.Count);
		}
		
		[Test]
		public void MissingNameValuePairOnFirstLine()
		{
			string resultsText =
				"MissingNameValuePair\r\n" +
				"Name: Test\r\n" +
				"Result: Failure\r\n";
			
			List<TestResult> results = ReadTestResults(resultsText);
			
			Assert.AreEqual(1, results.Count);
			
			TestResult result = results[0];
			Assert.AreEqual("Test", result.Name);
			Assert.IsTrue(result.IsFailure);
		}
		
		[Test]
		public void TwoLineTestMessage()
		{
			string resultsText =
				"Name: Test\r\n" +
				"Message: Should not be 0.\r\n" +
				" Should be 1.\r\n" +
				"Result: Failure\r\n";
			
			List<TestResult> results = ReadTestResults(resultsText);
			
			Assert.AreEqual(1, results.Count);
			
			TestResult result = results[0];
			Assert.AreEqual("Test", result.Name);
			Assert.AreEqual("Should not be 0.\r\nShould be 1.", result.Message);
			Assert.IsTrue(result.IsFailure);
		}
		
		[Test]
		public void ThreeLineTestMessage()
		{
			string resultsText =
				"Name: Test\r\n" +
				"Message: Should not be 0.\r\n" +
				" Should be 1.\r\n" +
				" End of message.\r\n" +
				"Result: Failure\r\n";
			
			List<TestResult> results = ReadTestResults(resultsText);
			
			Assert.AreEqual(1, results.Count);
			
			TestResult result = results[0];
			Assert.AreEqual("Test", result.Name);
			Assert.AreEqual("Should not be 0.\r\nShould be 1.\r\nEnd of message.", result.Message);
			Assert.IsTrue(result.IsFailure);
		}
		
		[Test]
		public void TwoTestFailures()
		{
			string resultsText =
				"Name: MyTest1\r\n" +
				"Result: Failure\r\n" +
				"Name: MyTest2\r\n" +
				"Result: Failure\r\n";
			
			List<TestResult> results = ReadTestResults(resultsText);
			
			Assert.AreEqual(2, results.Count);
			
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
			string resultsText =
				"Name: MyTest1\r\n" +
				"Message: FirstLine\r\n" +
				" SecondLine\r\n" +
				"Result: Failure\r\n" +
				"Name: MyTest2\r\n" +
				"Message: FirstLine\r\n" +
				" SecondLine\r\n" +
				" ThirdLine\r\n" +
				"Result: Failure\r\n";
			
			List<TestResult> results = ReadTestResults(resultsText);
			
			Assert.AreEqual(2, results.Count);
			
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
