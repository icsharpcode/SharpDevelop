// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.UnitTesting
{
	public abstract class TestProcessRunnerBase : TestRunnerBase
	{
		IUnitTestProcessRunner processRunner;
		ITestResultsReader testResultsReader;
		IFileSystem fileSystem;
		IMessageService messageService;
		
		public TestProcessRunnerBase(TestProcessRunnerBaseContext context)
		{
			this.processRunner = context.TestProcessRunner;
			this.testResultsReader = context.TestResultsReader;
			this.fileSystem = context.FileSystem;
			this.messageService = context.MessageService;
			
			processRunner.LogStandardOutputAndError = false;
			processRunner.OutputLineReceived += OutputLineReceived;
			processRunner.ErrorLineReceived += OutputLineReceived;
			processRunner.ProcessExited += OnAllTestsFinished;
			testResultsReader.TestFinished += OnTestFinished;
		}
		
		protected ITestResultsReader TestResultsReader {
			get { return testResultsReader; }
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
				testResultsReader.Start();
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
		
		protected override void OnAllTestsFinished(object source, EventArgs e)
		{
			testResultsReader.Join();
			base.OnAllTestsFinished(source, e);
		}
		
		public override void Stop()
		{
			SD.Log.Info("Killing unit test runner");
			processRunner.Kill();
		}
		
		public override void Dispose()
		{
			processRunner.Dispose();
			testResultsReader.Dispose();
			testResultsReader.TestFinished -= OnTestFinished;
			processRunner.ErrorLineReceived -= OutputLineReceived;
			processRunner.OutputLineReceived -= OutputLineReceived;
		}
	}
}
