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
using System.Diagnostics;
using ICSharpCode.Scripting;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.PythonBinding
{
	public class PythonTestDebugger : TestDebuggerBase
	{
		PythonAddInOptions options;
		IScriptingFileService fileService;
		PythonTestRunnerApplication testRunnerApplication;
		PythonStandardLibraryPath pythonStandardLibraryPath;
		
		public PythonTestDebugger()
			: this(new UnitTestDebuggerService(),
				new UnitTestMessageService(),
				new TestResultsMonitor(),
				new PythonAddInOptions(),
				new PythonStandardLibraryPath(),
				new ScriptingFileService())
		{
		}
		
		public PythonTestDebugger(IUnitTestDebuggerService debuggerService,
			IUnitTestMessageService messageService,
			ITestResultsMonitor testResultsMonitor,
			PythonAddInOptions options,
			PythonStandardLibraryPath pythonStandardLibraryPath,
			IScriptingFileService fileService)
			: base(debuggerService, messageService, testResultsMonitor)
		{
			this.options = options;
			this.pythonStandardLibraryPath = pythonStandardLibraryPath;
			this.fileService = fileService;
		}
		
		public override void Start(SelectedTests selectedTests)
		{
			CreateTestRunnerApplication();
			testRunnerApplication.CreateResponseFile(selectedTests);
			base.Start(selectedTests);
		}
		
		void CreateTestRunnerApplication()
		{
			testRunnerApplication = new PythonTestRunnerApplication(base.TestResultsMonitor.FileName, options, pythonStandardLibraryPath, fileService);
		}
		
		protected override ProcessStartInfo GetProcessStartInfo(SelectedTests selectedTests)
		{
			testRunnerApplication.Debug = true;
			return testRunnerApplication.CreateProcessStartInfo(selectedTests);
		}
		
		public override void Dispose()
		{
			testRunnerApplication.Dispose();
			base.Dispose();
		}
		
		protected override TestResult CreateTestResultForTestFramework(TestResult testResult)
		{
			return new PythonTestResult(testResult);
		}
	}
}
