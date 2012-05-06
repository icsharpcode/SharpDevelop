// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.IO;

using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.MSTest
{
	public class MSTestDebugger : TestRunnerBase
	{
		IUnitTestDebuggerService debuggerService;
		IUnitTestMessageService messageService;
		IDebugger debugger;
		string resultsFileName;
		
		public MSTestDebugger()
			: this(
				new UnitTestDebuggerService(),
				new UnitTestMessageService())
		{
		}
		
		public MSTestDebugger(
			IUnitTestDebuggerService debuggerService,
			IUnitTestMessageService messageService)
		{
			this.debuggerService = debuggerService;
			this.messageService = messageService;
			this.debugger = debuggerService.CurrentDebugger;
		}
		
		public override void Start(SelectedTests selectedTests)
		{
			ProcessStartInfo startInfo = GetProcessStartInfo(selectedTests);
			if (IsDebuggerRunning) {
				if (CanStopDebugging()) {
					debugger.Stop();
					Start(startInfo);
				}
			} else {
				Start(startInfo);
			}
		}
		
		protected override ProcessStartInfo GetProcessStartInfo(SelectedTests selectedTests)
		{
			resultsFileName = new MSTestResultsFileName(selectedTests).FileName;
			CreateDirectoryForResultsFile();
			var mstestApplication = new MSTestApplication(selectedTests, resultsFileName);
			return mstestApplication.ProcessStartInfo;
		}
		
		public bool IsDebuggerRunning {
			get { return debuggerService.IsDebuggerLoaded && debugger.IsDebugging; }
		}
		
		bool CanStopDebugging()
		{
			string question = "${res:XML.MainMenu.RunMenu.Compile.StopDebuggingQuestion}";
			string caption = "${res:XML.MainMenu.RunMenu.Compile.StopDebuggingTitle}";
			return messageService.AskQuestion(question, caption);
		}
		
		void CreateDirectoryForResultsFile()
		{
			string path = Path.GetDirectoryName(resultsFileName);
			if (!Directory.Exists(path)) {
				Directory.CreateDirectory(path);
			}
		}
		
		void Start(ProcessStartInfo startInfo)
		{
			StartDebugger(startInfo);
		}
		
		void StartDebugger(ProcessStartInfo startInfo)
		{
			LogCommandLine(startInfo);
			
			bool running = false;
			debugger.DebugStopped += DebugStopped;
			try {
				debugger.Start(startInfo);
				running = true;
			} finally {
				if (!running) {
					debugger.DebugStopped -= DebugStopped;
				}
			}
		}
		
		void DebugStopped(object source, EventArgs e)
		{
			debugger.DebugStopped -= DebugStopped;
			
			if (File.Exists(resultsFileName)) {
				var testResults = new MSTestResults(resultsFileName);
				var workbench = new UnitTestWorkbench();
				workbench.SafeThreadAsyncCall(() => UpdateTestResults(testResults));
			} else {
				messageService.ShowFormattedErrorMessage("Unable to find test results file: '{0}'.", resultsFileName);
				OnAllTestsFinished(source, e);
			}
		}
		
		void UpdateTestResults(MSTestResults testResults)
		{
			foreach (TestResult result in testResults) {
				OnTestFinished(this, new TestFinishedEventArgs(result));
			}
			OnAllTestsFinished(this, new EventArgs());
		}
		
		public override void Stop()
		{
			if (debugger.IsDebugging) {
				debugger.Stop();
			}
		}
		
		public override void Dispose()
		{
			Stop();
			try {
				File.Delete(resultsFileName);
			} catch { }
		}
	}
}
