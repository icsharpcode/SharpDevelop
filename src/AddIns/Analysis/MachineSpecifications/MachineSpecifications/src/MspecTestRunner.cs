/*
 * Created by SharpDevelop.
 * User: trecio
 * Date: 2011-06-18
 * Time: 14:01
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
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
