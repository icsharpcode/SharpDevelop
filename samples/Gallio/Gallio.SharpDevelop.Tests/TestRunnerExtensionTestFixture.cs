// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using Gallio.Extension;
using Gallio.Runtime.Logging;
using Gallio.SharpDevelop.Tests.Utils;
using ICSharpCode.UnitTesting;
using NUnit.Framework;

namespace Gallio.SharpDevelop.Tests
{
	[TestFixture]
	public class TestRunnerExtensionTestFixture
	{
		SharpDevelopTestRunnerExtension testRunnerExtension;
		MockTestResultsWriterFactory factory;
		MockTestRunnerEvents testRunnerEvents;
		MockTestResultsWriter writer;
		NullLogger logger;
		
		[SetUp]
		public void Init()
		{
			testRunnerEvents = new MockTestRunnerEvents();
			logger = new NullLogger();
			factory = new MockTestResultsWriterFactory();
			
			testRunnerExtension = new SharpDevelopTestRunnerExtension(factory);
			testRunnerExtension.Parameters = @"c:\temp\tmp77.tmp";
			testRunnerExtension.Install(testRunnerEvents, logger);
			
			writer = factory.TestResultsWriter;
		}
		
		[Test]
		public void TestResultWriterCreatedWithFileNameTakenFromTestRunnerExtensionParameters()
		{
			string expectedFileName = @"c:\temp\tmp77.tmp";
			Assert.AreEqual(expectedFileName, writer.FileName);
		}
		
		[Test]
		public void FiringTestStepFinishedEventWritesTestResultNameToTestResultsWriter()
		{
			string testName = "MyNamespace.MyTests.MyTestMethod";
			testRunnerEvents.FireTestStepFinishedEvent("MyNamespace.MyTests.MyTestMethod");
			
			Assert.AreEqual(testName, writer.FirstTestResult.Name);
		}
		
		[Test]
		public void TestRunnerEventsDisposeStartedEventCausesTestResultsWriterToBeDisposed()
		{
			testRunnerEvents.FireDisposeStartedEvent();
			Assert.IsTrue(writer.IsDisposed);
		}
		
		[Test]
		public void FiringTestStepFinishedEventWithNonTestCaseDoesNotWriteTestResultToTestResultsWriter()
		{
			testRunnerEvents.FireTestStepFinishedEventForNonTestCase("MyNamespace.MyTests.MyTestMethod");
			
			Assert.AreEqual(0, writer.TestResults.Count);
		}
	}
}
