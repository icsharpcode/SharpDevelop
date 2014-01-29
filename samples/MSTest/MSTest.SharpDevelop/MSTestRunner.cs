// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using ICSharpCode.UnitTesting;

namespace ICSharpCode.MSTest
{
	public class MSTestRunner : TestProcessRunnerBase
	{
		string resultsFileName;
		
		public MSTestRunner(TestExecutionOptions options)
			: base(new MSTestProcessRunnerContext(options))
		{
		}
		
		protected override ProcessStartInfo GetProcessStartInfo(IEnumerable<ITest> selectedTests)
		{
			resultsFileName = new MSTestResultsFileName(selectedTests).FileName;
			CreateDirectoryForResultsFile(resultsFileName);
			
			var monitor = (MSTestMonitor)TestResultsReader;
			monitor.ResultsFileName = resultsFileName;
			
			var app = new MSTestApplication(selectedTests, resultsFileName);
			return app.ProcessStartInfo;
		}
		
		public static void CreateDirectoryForResultsFile(string resultsFileName)
		{
			string path = Path.GetDirectoryName(resultsFileName);
			if (!Directory.Exists(path)) {
				Directory.CreateDirectory(path);
			}
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
