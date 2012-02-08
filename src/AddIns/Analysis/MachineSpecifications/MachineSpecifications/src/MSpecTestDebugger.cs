/*
 * Created by SharpDevelop.
 * User: trecio
 * Date: 2011-06-18
 * Time: 14:02
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
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
