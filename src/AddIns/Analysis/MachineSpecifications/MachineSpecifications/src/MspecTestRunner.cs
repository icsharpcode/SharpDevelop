// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.UnitTesting;
using System.Diagnostics;

namespace ICSharpCode.MachineSpecifications
{
	/// <summary>
	/// Description of MspecTestRunner.
	/// </summary>
	public class MSpecTestRunner : TestProcessRunnerBase
	{
        public MSpecTestRunner()
            : base(new UnitTestProcessRunner(), new MSpecUnitTestMonitor(), new UnitTestFileService(), new UnitTestMessageService())
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
