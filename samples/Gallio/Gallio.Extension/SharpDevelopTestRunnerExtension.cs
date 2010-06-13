// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using Gallio.Runner.Events;
using Gallio.Runner.Extensions;

namespace Gallio.Extension
{
	public class SharpDevelopTestRunnerExtension : TestRunnerExtension
	{
		ITestResultsWriterFactory factory;
		ITestResultsWriter writer;
		
		public SharpDevelopTestRunnerExtension(ITestResultsWriterFactory factory)
		{
			this.factory = factory;
		}
		
		public SharpDevelopTestRunnerExtension()
			: this(new TestResultsWriterFactory())
		{
		}
		
		protected override void Initialize()
		{
			writer = factory.Create(Parameters);
			Events.TestStepFinished += TestStepFinished;
			Events.DisposeStarted += DisposeStarted;
		}

		void TestStepFinished(object source, TestStepFinishedEventArgs e)
		{
			if (e.Test.IsTestCase) {
				GallioTestStepConverter testStep = new GallioTestStepConverter(e);
				TestResult testResult = testStep.GetTestResult();
				writer.Write(testResult);
			}
		}
		
		void DisposeStarted(object source, DisposeStartedEventArgs e)
		{
			writer.Dispose();
		}
	}
}