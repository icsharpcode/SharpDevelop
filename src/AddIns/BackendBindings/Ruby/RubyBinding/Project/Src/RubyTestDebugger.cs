// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using ICSharpCode.Core.Services;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.RubyBinding
{
	public class RubyTestDebugger : TestDebuggerBase
	{
		RubyAddInOptions options;
		IRubyFileService fileService;
		RubyTestRunnerApplication testRunnerApplication;
		
		public RubyTestDebugger()
			: this(new UnitTestDebuggerService(),
				ServiceManager.Instance.MessageService,
				new TestResultsMonitor(),
				new RubyAddInOptions(),
				new RubyFileService())
		{
		}
		
		public RubyTestDebugger(IUnitTestDebuggerService debuggerService,
			IMessageService messageService,
			ITestResultsMonitor testResultsMonitor,
			RubyAddInOptions options,
			IRubyFileService fileService)
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
