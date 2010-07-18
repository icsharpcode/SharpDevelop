// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
			IFileSystem fileSystem)
			: base(processRunner, testResultsMonitor, options, fileSystem)
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
