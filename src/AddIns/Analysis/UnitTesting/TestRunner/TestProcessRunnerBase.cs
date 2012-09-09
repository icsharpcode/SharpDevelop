// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Util;

namespace ICSharpCode.UnitTesting
{
	public class TestProcessRunnerBase : TestRunnerBase
	{
		IUnitTestProcessRunner processRunner;
		ITestResultsMonitor testResultsMonitor;
		IFileSystem fileSystem;
		IMessageService messageService;
		
		public TestProcessRunnerBase(TestProcessRunnerBaseContext context)
		{
			this.processRunner = context.TestProcessRunner;
			this.testResultsMonitor = context.TestResultsMonitor;
			this.fileSystem = context.FileSystem;
			this.messageService = context.MessageService;
			
			processRunner.LogStandardOutputAndError = false;
			processRunner.OutputLineReceived += OutputLineReceived;
			processRunner.ErrorLineReceived += OutputLineReceived;
			processRunner.ProcessExited += OnAllTestsFinished;
			testResultsMonitor.TestFinished += OnTestFinished;
		}
		
		protected ITestResultsMonitor TestResultsMonitor {
			get { return testResultsMonitor; }
		}
		
		protected IUnitTestProcessRunner ProcessRunner {
			get { return processRunner; }
		}
		
		void OutputLineReceived(object source, LineReceivedEventArgs e)
		{
			OnMessageReceived(e.Line);
		}
		
		public override void Start(IEnumerable<ITest> selectedTests)
		{
			ProcessStartInfo startInfo = GetProcessStartInfo(selectedTests);
			Start(startInfo);
		}
		
		void Start(ProcessStartInfo processStartInfo)
		{
			LogCommandLine(processStartInfo);
			
			if (ApplicationFileNameExists(processStartInfo.FileName)) {
				testResultsMonitor.Start();
				processRunner.WorkingDirectory = processStartInfo.WorkingDirectory;
				processRunner.Start(processStartInfo.FileName, processStartInfo.Arguments);
			} else {
				ShowApplicationDoesNotExistMessage(processStartInfo.FileName);
			}
		}
		
		bool ApplicationFileNameExists(string fileName)
		{
			return fileSystem.FileExists(fileName);
		}
		
		void ShowApplicationDoesNotExistMessage(string fileName)
		{
			string resourceString = "${res:ICSharpCode.UnitTesting.TestRunnerNotFoundMessageFormat}";
			messageService.ShowErrorFormatted(resourceString, fileName);
		}
		
		public override void Stop()
		{
			processRunner.Kill();
			testResultsMonitor.Stop();
			testResultsMonitor.Read();
		}
		
		public override void Dispose()
		{
			testResultsMonitor.Dispose();
			testResultsMonitor.TestFinished -= OnTestFinished;
			processRunner.ErrorLineReceived -= OutputLineReceived;
			processRunner.OutputLineReceived -= OutputLineReceived;
		}
	}
}
