// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
