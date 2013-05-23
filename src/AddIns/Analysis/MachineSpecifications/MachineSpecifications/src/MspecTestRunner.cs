// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;

using ICSharpCode.UnitTesting;

namespace ICSharpCode.MachineSpecifications
{
	public class MSpecTestRunner : TestProcessRunnerBase
	{
		public MSpecTestRunner(TestExecutionOptions options)
			: base(new MSpecTestProcessRunnerContext(options))
		{
		}
		
		protected override ProcessStartInfo GetProcessStartInfo(IEnumerable<ITest> selectedTests)
		{
			var app = new MSpecApplication(selectedTests);
			var monitor = TestResultsReader as MSpecUnitTestMonitor;
			app.Results = monitor.FileName;
			return app.GetProcessStartInfo();
		}
		
		protected override TestResult CreateTestResultForTestFramework(TestResult testResult)
		{
			return testResult;
		}
		
		public override int GetExpectedNumberOfTestResults(IEnumerable<ITest> selectedTests)
		{
			return 0;
		}
	}
}
