// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.RubyBinding;
using ICSharpCode.UnitTesting;
using NUnit.Framework;

namespace RubyBinding.Tests.Testing
{
	[TestFixture]
	public class RubyTestResultFailureTestFixture
	{
		RubyTestResult RubyTestResult;
		string stackTraceText;
		
		[SetUp]
		public void Init()
		{
			TestResult testResult = new TestResult("MyTest");
			testResult.ResultType = TestResultType.Failure;
			testResult.Message = "test failed";
			
			stackTraceText = 
				"Failure:\r\n" +
				"test_fail(SecondTests)\r\n" +
				"    [d:\\projects\\rubytests\\SecondTests.rb:6:in `test_fail'\r\n" +
				"     d:\\test\\rubytests/sdtestrunner.rb:73:in `start_mediator'\r\n" +
				"     d:\\test\\rubytests/sdtestrunner.rb:47:in `start']:\r\n" +
				"Assertion was false.\r\n" +
				"<false> is not true.\r\n" +
				"";
			
			testResult.StackTrace = stackTraceText;
			RubyTestResult = new RubyTestResult(testResult);
		}
		
		[Test]
		public void TestResultNameIsMyTest()
		{
			Assert.AreEqual("MyTest", RubyTestResult.Name);
		}
		
		[Test]
		public void TestResultTypeIsFailure()
		{
			Assert.AreEqual(TestResultType.Failure, RubyTestResult.ResultType);
		}
		
		[Test]
		public void TestResultMessageIsTestFailed()
		{
			Assert.AreEqual("test failed", RubyTestResult.Message);
		}
		
		[Test]
		public void RubyTestResultHasStackTraceFromOriginalTestResult()
		{
			Assert.AreEqual(stackTraceText, RubyTestResult.StackTrace);
		}
		
		[Test]
		public void StackTraceFilePositionHasExpectedFileName()
		{
			string expectedFileName = @"d:\projects\rubytests\SecondTests.rb";
			Assert.AreEqual(expectedFileName, RubyTestResult.StackTraceFilePosition.FileName);
		}
		
		[Test]
		public void StackTraceFilePositionLineIs6()
		{
			Assert.AreEqual(6, RubyTestResult.StackTraceFilePosition.Line);
		}
		
		[Test]
		public void StackTraceFilePositionColumnIsOne()
		{
			Assert.AreEqual(1, RubyTestResult.StackTraceFilePosition.Column);
		}
		
		[Test]
		public void ChangingStackTraceToEmptyStringSetsStackTraceFilePositionToEmpty()
		{
			RubyTestResult.StackTraceFilePosition = new FilePosition("test.rb", 10, 2);
			RubyTestResult.StackTrace = String.Empty;
			Assert.IsTrue(RubyTestResult.StackTraceFilePosition.IsEmpty);
		}
	}
}
