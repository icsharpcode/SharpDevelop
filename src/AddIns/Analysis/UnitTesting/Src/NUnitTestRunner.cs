// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using ICSharpCode.SharpDevelop.Util;

namespace ICSharpCode.UnitTesting
{
	public class NUnitTestRunner : TestProcessRunnerBase
	{
		UnitTestingOptions options;
		
		public NUnitTestRunner()
			: this(new UnitTestProcessRunner(),
				new TestResultsMonitor(),
				new UnitTestingOptions())
		{
		}
		
		public NUnitTestRunner(IUnitTestProcessRunner processRunner,
			ITestResultsMonitor testResultsMonitor,
			UnitTestingOptions options)
			: base(processRunner, testResultsMonitor)
		{
			this.options = options;
		}
		
		protected override ProcessStartInfo GetProcessStartInfo(SelectedTests selectedTests)
		{
			NUnitConsoleApplication app = new NUnitConsoleApplication(selectedTests, options);
			app.Results = base.TestResultsMonitor.FileName;
			return app.GetProcessStartInfo();
		}
		
		protected override TestResult CreateTestResultForTestFramework(TestResult testResult)
		{
			return new NUnitTestResult(testResult);
		}
	}
}
