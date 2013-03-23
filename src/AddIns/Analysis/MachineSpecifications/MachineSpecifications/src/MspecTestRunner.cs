/*
 * Created by SharpDevelop.
 * User: trecio
 * Date: 2011-06-18
 * Time: 14:01
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;

using ICSharpCode.UnitTesting;

namespace ICSharpCode.MachineSpecifications
{
	/// <summary>
	/// Description of MspecTestRunner.
	/// </summary>
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
