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
using ICSharpCode.Core.Services;
using ICSharpCode.Scripting;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.RubyBinding
{
	public class RubyTestDebugger : TestDebuggerBase
	{
		RubyAddInOptions options;
		IScriptingFileService fileService;
		RubyTestRunnerApplication testRunnerApplication;
		
		public RubyTestDebugger()
			: this(new UnitTestDebuggerService(),
				new UnitTestMessageService(),
				new TestResultsMonitor(),
				new RubyAddInOptions(),
				new ScriptingFileService())
		{
		}
		
		public RubyTestDebugger(IUnitTestDebuggerService debuggerService,
			IUnitTestMessageService messageService,
			ITestResultsMonitor testResultsMonitor,
			RubyAddInOptions options,
			IScriptingFileService fileService)
			: base(debuggerService, messageService, testResultsMonitor)
		{
			this.options = options;
			this.fileService = fileService;
			testResultsMonitor.InitialFilePosition = 0;
		}
		
		public override void Start(SelectedTests selectedTests)
		{
			CreateTestRunnerApplication();
			testRunnerApplication.CreateResponseFile(selectedTests);
			base.Start(selectedTests);
		}
		
		void CreateTestRunnerApplication()
		{
			testRunnerApplication = new RubyTestRunnerApplication(base.TestResultsMonitor.FileName, options, fileService);
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
			return new RubyTestResult(testResult);
		}
	}
}
