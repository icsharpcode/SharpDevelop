// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using ICSharpCode.SharpDevelop.Util;

namespace ICSharpCode.UnitTesting
{
	public class NUnitTestRunner : TestProcessRunnerBase
	{
		UnitTestingOptions options;
		
		public NUnitTestRunner()
			: this(new TestProcessRunnerBaseContext(), 
				new UnitTestingOptions())
		{
		}
		
		public NUnitTestRunner(TestProcessRunnerBaseContext context, UnitTestingOptions options)
			: base(context)
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
