// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using ICSharpCode.CodeCoverage;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.CodeCoverage.Tests.Utils
{
	public class DerivedCodeCoverageTestRunner : CodeCoverageTestRunner
	{
		public string ParseStringReturnValue;
		public string ParseStringParameter;
		
		public DerivedCodeCoverageTestRunner(IUnitTestProcessRunner processRunner,
			ITestResultsMonitor testResultsMonitor,
			UnitTestingOptions options,
			IFileSystem fileSystem,
			IUnitTestMessageService messageService)
			: base(new CodeCoverageTestRunnerContext(processRunner, testResultsMonitor, fileSystem, messageService, options))
		{
		}
		
		public ProcessStartInfo CallGetProcessStartInfo(SelectedTests selectedTests)
		{
			return base.GetProcessStartInfo(selectedTests);
		}
		
		public TestResult CallCreateTestResultForTestFramework(TestResult testResult)
		{
			return base.CreateTestResultForTestFramework(testResult);
		}
		
		protected override string ParseString(string text)
		{
			ParseStringParameter = text;
			return ParseStringReturnValue;
		}
	}
}
