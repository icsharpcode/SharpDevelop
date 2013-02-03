// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.UnitTesting
{
	public class NUnitTestRunner : TestProcessRunnerBase
	{
		UnitTestingOptions options;
		
		public NUnitTestRunner(TestExecutionOptions executionOptions)
			: this(new TestProcessRunnerBaseContext(executionOptions),
			       UnitTestingOptions.Instance.Clone())
		{
		}
		
		public NUnitTestRunner(TestProcessRunnerBaseContext context,
			UnitTestingOptions options)
			: base(context)
		{
			this.options = options;
		}
		
		protected override ProcessStartInfo GetProcessStartInfo(IEnumerable<ITest> selectedTests)
		{
			NUnitConsoleApplication app = new NUnitConsoleApplication(selectedTests, options);
			app.ResultsPipe = base.TestResultsReader.PipeName;
			return app.GetProcessStartInfo();
		}
		
		protected override TestResult CreateTestResultForTestFramework(TestResult testResult)
		{
			return new NUnitTestResult(testResult);
		}
		
		public override int GetExpectedNumberOfTestResults(IEnumerable<ITest> selectedTests)
		{
			return GetNumberOfTestMethods(selectedTests);
		}
		
		public static int GetNumberOfTestMethods(IEnumerable<ITest> selectedTests)
		{
			int count = 0;
			foreach (ITest test in selectedTests) {
				if (test is NUnitTestMethod)
					count++;
				else
					count += GetNumberOfTestMethods(test.NestedTests);
			}
			return count;
		}
	}
}
