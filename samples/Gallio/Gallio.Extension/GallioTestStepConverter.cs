// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using Model = Gallio.Model;
using Gallio.Common.Markup;
using Gallio.Runner.Events;

namespace Gallio.Extension
{
	public class GallioTestStepConverter
	{
		TestStepEventArgs eventArgs;
		TestResult testResult;
		SharpDevelopTagFormatter tagFormatter;
		
		public GallioTestStepConverter(TestStepEventArgs eventArgs)
		{
			this.eventArgs = eventArgs;
			Convert();
		}
		
		void Convert()
		{
			CreateTestResult();
			ConvertTestOutcome();
			ConvertTestMessages();
		}
		
		void CreateTestResult()
		{
			string name = GetTestName();
			testResult = new TestResult(name);
		}
		
		string GetTestName()
		{
			return eventArgs.Test.FullName.Replace('/', '.');
		}
		
		void ConvertTestOutcome()
		{
			testResult.ResultType = GetTestResultType(eventArgs.TestStepRun.Result.Outcome.Status);
		}
		
		TestResultType GetTestResultType(Model.TestStatus status)
		{
			switch (status) {
				case Model.TestStatus.Passed:
					return TestResultType.Success;
				case Model.TestStatus.Skipped:
				case Model.TestStatus.Inconclusive:
					return TestResultType.Ignored;
				case Model.TestStatus.Failed:
					return TestResultType.Failure;
			}
			return TestResultType.None;
		}
		
		void ConvertTestMessages()
		{
			tagFormatter = new SharpDevelopTagFormatter();
			tagFormatter.Visit(eventArgs.TestStepRun);
			testResult.Message = tagFormatter.TestResultMessage;
			testResult.StackTrace = tagFormatter.GetStackTrace();
		}
		
		public TestResult GetTestResult()
		{
			return testResult;
		}
	}
}
