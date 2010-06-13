// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using Model = Gallio.Model;
using Gallio.Extension;
using Gallio.Runner.Events;
using Gallio.SharpDevelop;
using Gallio.SharpDevelop.Tests.Utils;
using NUnit.Framework;

namespace Gallio.SharpDevelop.Tests
{
	[TestFixture]
	public class GallioTestToSharpDevelopTestResultConversionTests
	{
		GallioTestStepFinishedEventArgsFactory factory;
		TestStepFinishedEventArgs testStepEventArgs;
		GallioTestStepConverter converter;
		TestResult testResult;
		
		[SetUp]
		public void Init()
		{
			factory = new GallioTestStepFinishedEventArgsFactory();
		}
		
		[Test]
		public void GallioTestNameIsConvertedToSharpDevelopTestName()
		{
			string gallioTestName = "MyNamespace/MyClass/MyTestMethod";
			CreateGallioTestStepEventArgs(gallioTestName);
			ConvertToSharpDevelopTestResult();
			
			string expectedTestName = "MyNamespace.MyClass.MyTestMethod";
			Assert.AreEqual(expectedTestName, testResult.Name);
		}
		
		void CreateGallioTestStepEventArgs(string testName)
		{
			testStepEventArgs = factory.Create(testName);
		}
		
		void ConvertToSharpDevelopTestResult()
		{
			converter = new GallioTestStepConverter(testStepEventArgs);
			testResult = converter.GetTestResult();
		}
		
		[Test]
		public void GallioTestOutcomePassedIsConvertedToSharpDevelopTestSuccess()
		{
			CreateGallioTestStepEventArgs();
			UpdateGallioTestOutcomeToPassed();
			ConvertToSharpDevelopTestResult();
			
			Assert.AreEqual(TestResultType.Success, testResult.ResultType);
		}
		
		void CreateGallioTestStepEventArgs()
		{
			CreateGallioTestStepEventArgs("MyNamespace.MyClass.MyTest");
		}
		
		void UpdateGallioTestOutcomeToPassed()
		{
			UpdateGallioTestOutcome(Model.TestOutcome.Passed);
		}
		
		void UpdateGallioTestOutcome(Model.TestOutcome testOutcome)
		{
			testStepEventArgs.TestStepRun.Result.Outcome = testOutcome;
		}
		
		[Test]
		public void GallioTestOutcomeErrorIsConvertedToSharpDevelopTestFailure()
		{
			CreateGallioTestStepEventArgs();
			UpdateGallioTestOutcomeToError();
			ConvertToSharpDevelopTestResult();
			
			Assert.AreEqual(TestResultType.Failure, testResult.ResultType);
		}
		
		void UpdateGallioTestOutcomeToError()
		{
			UpdateGallioTestOutcome(Model.TestOutcome.Error);
		}
		
		[Test]
		public void GallioTestOutcomeSkippedIsConvertedToSharpDevelopTestIgnored()
		{
			CreateGallioTestStepEventArgs();
			UpdateGallioTestOutcomeToSkipped();
			ConvertToSharpDevelopTestResult();
			
			Assert.AreEqual(TestResultType.Ignored, testResult.ResultType);
		}
		
		void UpdateGallioTestOutcomeToSkipped()
		{
			UpdateGallioTestOutcome(Model.TestOutcome.Skipped);
		}
		
		[Test]
		public void GallioTestOutcomeInconclusiveIsConvertedToSharpDevelopTestIgnored()
		{
			CreateGallioTestStepEventArgs();
			UpdateGallioTestOutcomeToInconclusive();
			ConvertToSharpDevelopTestResult();
			
			Assert.AreEqual(TestResultType.Ignored, testResult.ResultType);
		}
		
		void UpdateGallioTestOutcomeToInconclusive()
		{
			UpdateGallioTestOutcome(Model.TestOutcome.Inconclusive);
		}
		
		[Test]
		public void GallioAssertionFailureMessageIsConvertedToSharpDevelopTestResultMessage()
		{
			CreateGallioTestStepEventArgs();
			UpdateGallioTestOutcomeToError();
			UpdateGallioTestLogWithAssertionFailure();
			ConvertToSharpDevelopTestResult();
			
			string expectedMessage = "Expected value to be true. User assertion message.";
			Assert.AreEqual(expectedMessage, testResult.Message);
		}
		
		void UpdateGallioTestLogWithAssertionFailure()
		{
			GallioBodyTagFactory factory = new GallioBodyTagFactory();
			testStepEventArgs.TestStepRun.TestLog.Streams.Add(factory.CreateAssertionFailureStructuredStream());
		}
		
		[Test]
		public void GallioAssertionFailureStackTraceIsConvertedToSharpDevelopTestResultStackTrace()
		{
			CreateGallioTestStepEventArgs();
			UpdateGallioTestOutcomeToError();
			UpdateGallioTestLogWithAssertionFailure();
			ConvertToSharpDevelopTestResult();
			
			string expectedStackTrace =
				@"   at GallioTest.MyClass.AssertWithFailureMessage() in d:\temp\test\GallioTest\MyClass.cs:line 46 ";
			Assert.AreEqual(expectedStackTrace, testResult.StackTrace);
		}
	}
}
