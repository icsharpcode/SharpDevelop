// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using Gallio.SharpDevelop;

namespace Gallio.SharpDevelop.Tests
{
	[TestFixture]
	public class GallioTestFailureTestFixture
	{
		GallioTestResult gallioTestResult;

		[SetUp]
		public void Init()
		{
			TestResult testResult = new TestResult("MyNamespace.MyTests");
			testResult.ResultType = TestResultType.Failure;
			testResult.Message = "Test failed";
			testResult.StackTrace = 
				@"   at GallioTest.MyClass.AssertWithFailureMessage() in d:\temp\test\..\GallioTest\MyClass.cs:line 46";
			gallioTestResult = new GallioTestResult(testResult);
		}
		
		[Test] 
		public void TestResultTypeIsFailure()
		{
			Assert.AreEqual(TestResultType.Failure, gallioTestResult.ResultType);
		}
		
		[Test]
		public void TestResultMessageIsTestFailed()
		{
			Assert.AreEqual("Test failed", gallioTestResult.Message);
		}
		
		[Test]
		public void TestResultHasStackTrace()
		{
			string expectedStackTrace =
				@"   at GallioTest.MyClass.AssertWithFailureMessage() in d:\temp\test\..\GallioTest\MyClass.cs:line 46";
			Assert.AreEqual(expectedStackTrace, gallioTestResult.StackTrace);
		}
		
		[Test]
		public void TestResultHasExpectedName()
		{
			Assert.AreEqual("MyNamespace.MyTests", gallioTestResult.Name);
		}
		
		[Test]
		public void StackTraceFilePositionFileNameMatchesLastFileNameInStackTrace()
		{
			string expectedFileName = @"d:\temp\GallioTest\MyClass.cs";
			Assert.AreEqual(expectedFileName, gallioTestResult.StackTraceFilePosition.FileName);
		}
		
		[Test]
		public void StackTraceFilePositionLineNumberIs46WhichIsEqualToReportedErrorLine()
		{
			Assert.AreEqual(46, gallioTestResult.StackTraceFilePosition.Line);
		}
		
		[Test]
		public void StackTraceFilePositionColumnNumberIsOne()
		{
			Assert.AreEqual(1, gallioTestResult.StackTraceFilePosition.Column);
		}
		
		[Test]
		public void ChangingStackTraceToEmptyStringSetsStackTraceFilePositionToEmpty()
		{
			gallioTestResult.StackTraceFilePosition = new FilePosition("test.cs", 10, 2);
			gallioTestResult.StackTrace = String.Empty;
			Assert.IsTrue(gallioTestResult.StackTraceFilePosition.IsEmpty);
		}		
	}
}
