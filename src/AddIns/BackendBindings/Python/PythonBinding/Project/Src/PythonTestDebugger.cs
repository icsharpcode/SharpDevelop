// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.PythonBinding
{
	public class PythonTestDebugger : TestDebuggerBase
	{
		AddInOptions options;
		IPythonFileService fileService;
		PythonTestRunnerApplication testRunnerApplication;
		PythonStandardLibraryPath pythonStandardLibraryPath;
		
		public PythonTestDebugger()
			: this(new UnitTestDebuggerService(),
				new UnitTestMessageService(),
				new TestResultsMonitor(),
				new AddInOptions(),
				new PythonStandardLibraryPath(),
				new PythonFileService())
		{
		}
		
		public PythonTestDebugger(IUnitTestDebuggerService debuggerService,
			IUnitTestMessageService messageService,
			ITestResultsMonitor testResultsMonitor,
			AddInOptions options,
			PythonStandardLibraryPath pythonStandardLibraryPath,
			IPythonFileService fileService)
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
