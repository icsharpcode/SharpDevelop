// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.MachineSpecifications
{
	/// <summary>
	/// Description of MSpecTestDebugger.
	/// </summary>
	public class MSpecTestDebugger : TestDebuggerBase
	{
        public MSpecTestDebugger()
            : base(new UnitTestDebuggerService(), new UnitTestMessageService(), new MSpecUnitTestMonitor())
        { }

		protected override ProcessStartInfo GetProcessStartInfo(SelectedTests selectedTests)
		{
			var app = new MSpecApplication(selectedTests);
			app.Results = TestResultsMonitor.FileName;
			return app.GetProcessStartInfo();
		}
		
		protected override TestResult CreateTestResultForTestFramework(TestResult testResult)
		{
            return testResult;
		}
	}
}
