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
using System.IO;

using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.MSTest
{
	public class MSTestDebugger : TestProcessRunnerBase
	{
		string resultsFileName;
		
		public MSTestDebugger(TestExecutionOptions options)
			: base(new MSTestProcessRunnerContext(options))
		{
		}
		
		protected override ProcessStartInfo GetProcessStartInfo(IEnumerable<ITest> selectedTests)
		{
			resultsFileName = new MSTestResultsFileName(selectedTests).FileName;
			MSTestRunner.CreateDirectoryForResultsFile(resultsFileName);
			
			var monitor = (MSTestMonitor)TestResultsReader;
			monitor.ResultsFileName = resultsFileName;
			
			var app = new MSTestApplication(selectedTests, resultsFileName);
			return app.ProcessStartInfo;
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
