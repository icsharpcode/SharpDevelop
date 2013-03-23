/*
 * Created by SharpDevelop.
 * User: trecio
 * Date: 2011-06-18
 * Time: 14:02
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;

using ICSharpCode.SharpDevelop;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.MachineSpecifications
{
	/// <summary>
	/// Description of MSpecTestDebugger.
	/// </summary>
	public class MSpecTestDebugger : TestDebuggerBase
	{
		public MSpecTestDebugger()
			: base(new UnitTestDebuggerService(), SD.MessageService, new MSpecUnitTestMonitor())
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
