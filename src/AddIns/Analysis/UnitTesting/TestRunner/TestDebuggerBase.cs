// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Debugging;

namespace ICSharpCode.UnitTesting
{
	public abstract class TestDebuggerBase : TestRunnerBase
	{
		IUnitTestDebuggerService debuggerService;
		IMessageService messageService;
		IDebugger debugger;
		ITestResultsReader testResultsReader;
		
		public TestDebuggerBase()
			: this(new UnitTestDebuggerService(),
				SD.MessageService,
				new TestResultsReader())
		{
		}
		
		public TestDebuggerBase(IUnitTestDebuggerService debuggerService,
			IMessageService messageService,
			ITestResultsReader testResultsReader)
		{
			this.debuggerService = debuggerService;
			this.messageService = messageService;
			this.testResultsReader = testResultsReader;
			this.debugger = debuggerService.CurrentDebugger;
			
			testResultsReader.TestFinished += OnTestFinished;
		}
		
		protected ITestResultsReader TestResultsReader {
			get { return testResultsReader; }
		}
		
		public override void Start(IEnumerable<ITest> selectedTests)
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
		
		public bool IsDebuggerRunning {
			get { return debuggerService.IsDebuggerLoaded && debugger.IsDebugging; }
		}
		
		bool CanStopDebugging()
		{
			string question = "${res:XML.MainMenu.RunMenu.Compile.StopDebuggingQuestion}";
			string caption = "${res:XML.MainMenu.RunMenu.Compile.StopDebuggingTitle}";
			return messageService.AskQuestion(question, caption);
		}
		
		void Start(ProcessStartInfo startInfo)
		{
			testResultsReader.Start();
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
			testResultsReader.Join();
			OnAllTestsFinished();
		}
		
		public override void Stop()
		{
			if (debugger.IsDebugging) {
				debugger.Stop();
			}
		}
		
		public override void Dispose()
		{
			testResultsReader.Dispose();
			testResultsReader.TestFinished -= OnTestFinished;
		}
	}
}
