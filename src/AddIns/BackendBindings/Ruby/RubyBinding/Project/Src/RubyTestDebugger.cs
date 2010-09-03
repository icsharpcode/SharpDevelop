// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
