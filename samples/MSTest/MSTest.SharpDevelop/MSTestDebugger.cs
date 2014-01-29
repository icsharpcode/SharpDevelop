// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.MSTest
{
	public class MSTestDebugger : TestProcessRunnerBase
	{
		string resultsFileName;
		
		public MSTestDebugger(TestExecutionOptions options)
			: base(new MSTestProcessRunnerContext(options))
		{
		}
		
		protected override ProcessStartInfo GetProcessStartInfo(IEnumerable<ITest> selectedTests)
		{
			resultsFileName = new MSTestResultsFileName(selectedTests).FileName;
			MSTestRunner.CreateDirectoryForResultsFile(resultsFileName);
			
			var monitor = (MSTestMonitor)TestResultsReader;
			monitor.ResultsFileName = resultsFileName;
			
			var app = new MSTestApplication(selectedTests, resultsFileName);
			return app.ProcessStartInfo;
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
