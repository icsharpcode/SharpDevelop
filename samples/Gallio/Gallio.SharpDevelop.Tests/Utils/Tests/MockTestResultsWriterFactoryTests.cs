// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using Gallio.Extension;
using Gallio.SharpDevelop.Tests.Utils;
using NUnit.Framework;

namespace Gallio.SharpDevelop.Tests.Utils.Tests
{
	[TestFixture]
	public class MockTestResultsWriterFactoryTests
	{
		MockTestResultsWriterFactory factory;
		
		[SetUp]	
		public void Init()
		{
			factory = new MockTestResultsWriterFactory();
		}
		
		[Test]
		public void TestResultsWritersSavedByFactory()
		{
			ITestResultsWriter writer = factory.Create("testresults.txt");
			Assert.AreEqual(writer, factory.TestResultsWriter);
		}
		
		[Test]
		public void TestResultsWriterCreatedIsMockTestResultsWriter()
		{
			MockTestResultsWriter writer = factory.Create("abc.txt") as MockTestResultsWriter;
			Assert.IsNotNull(writer);
		}
		
		[Test]
		public void TestResultsWriterCreatedWithFileNamePassedToFactoryCreateMethod()
		{
			MockTestResultsWriter writer = factory.Create("testresults.txt") as MockTestResultsWriter;
			Assert.AreEqual("testresults.txt", writer.FileName);
		}
		
		[Test]
		public void TestResultsWriterSavesTestResultsWritten()
		{
			TestResult firstResult = new TestResult("test1");
			TestResult secondResult = new TestResult("test2");
			MockTestResultsWriter writer = factory.Create("testresults.txt") as MockTestResultsWriter;
			writer.Write(firstResult);
			writer.Write(secondResult);
			
			TestResult[] expectedTestResults = new TestResult[] { firstResult, secondResult };
			
			Assert.AreEqual(expectedTestResults, writer.TestResults.ToArray());
		}
		
		[Test]
		public void FirstTestResultsWriterReturnsFirstTestResultsWriter()
		{
			TestResult firstResult = new TestResult("test1");
			TestResult secondResult = new TestResult("test2");
			MockTestResultsWriter writer = factory.Create("testresults.txt") as MockTestResultsWriter;
			writer.Write(firstResult);
			writer.Write(secondResult);
			
			Assert.AreEqual(firstResult, writer.FirstTestResult);
		}
		
		[Test]
		public void IsDisposedCalledReturnsTrueAfterDisposeMethodCalled()
		{
			MockTestResultsWriter writer = factory.Create("testresults.txt") as MockTestResultsWriter;
			writer.IsDisposed = false;
			writer.Dispose();
			Assert.IsTrue(writer.IsDisposed);
		}
	}
}
