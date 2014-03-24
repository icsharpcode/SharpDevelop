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

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Debugging;

namespace ICSharpCode.UnitTesting
{
	public abstract class TestDebuggerBase : TestRunnerBase
	{
		IMessageService messageService;
		IDebuggerService debugger;
		ITestResultsReader testResultsReader;
		
		public TestDebuggerBase()
			: this(SD.Debugger,
				SD.MessageService,
				new TestResultsReader())
		{
		}
		
		public TestDebuggerBase(IDebuggerService debuggerService,
			IMessageService messageService,
			ITestResultsReader testResultsReader)
		{
			this.debugger = debuggerService;
			this.messageService = messageService;
			this.testResultsReader = testResultsReader;
			
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
			get { return debugger.IsDebuggerLoaded && debugger.IsDebugging; }
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
