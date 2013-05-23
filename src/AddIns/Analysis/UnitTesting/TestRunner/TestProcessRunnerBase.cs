// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;

using System.Threading.Tasks;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.UnitTesting
{
	public abstract class TestProcessRunnerBase : TestRunnerBase
	{
		TestExecutionOptions executionOptions;
		IProcessRunner processRunner;
		ITestResultsReader testResultsReader;
		IFileSystem fileSystem;
		IMessageService messageService;
		
		public TestProcessRunnerBase(TestProcessRunnerBaseContext context)
		{
			this.executionOptions = context.ExecutionOptions;
			this.processRunner = context.TestProcessRunner;
			this.testResultsReader = context.TestResultsReader;
			this.fileSystem = context.FileSystem;
			this.messageService = context.MessageService;
			
			testResultsReader.TestFinished += OnTestFinished;
		}
		
		protected ITestResultsReader TestResultsReader {
			get { return testResultsReader; }
		}
		
		protected IProcessRunner ProcessRunner {
			get { return processRunner; }
		}
		
		public override void Start(IEnumerable<ITest> selectedTests)
		{
			ProcessStartInfo startInfo = GetProcessStartInfo(selectedTests);
			ProcessStartInfo modifiedStartInfo = ModifyProcessStartInfoBeforeRun(startInfo, selectedTests);
			Start(modifiedStartInfo);
		}
		
		ProcessStartInfo ModifyProcessStartInfoBeforeRun(ProcessStartInfo startInfo, IEnumerable<ITest> selectedTests)
		{
			return executionOptions.ModifyProcessStartInfoBeforeTestRun(startInfo, selectedTests);
		}
		
		void Start(ProcessStartInfo processStartInfo)
		{
			LogCommandLine(processStartInfo);
			
			if (ApplicationFileNameExists(processStartInfo.FileName)) {
				testResultsReader.Start();
				processRunner.WorkingDirectory = DirectoryName.Create(processStartInfo.WorkingDirectory);
				processRunner.RedirectStandardOutputAndErrorToSingleStream = true;
				processRunner.StartCommandLine("\"" + processStartInfo.FileName + "\" " + processStartInfo.Arguments);
				Task.WhenAll(
					processRunner.OpenStandardOutputReader().CopyToAsync(output),
					processRunner.WaitForExitAsync()).ContinueWith(_ => OnAllTestsFinished()).FireAndForget();
			} else {
				ShowApplicationDoesNotExistMessage(processStartInfo.FileName);
			}
		}
		
		bool ApplicationFileNameExists(string fileName)
		{
			return fileSystem.FileExists(FileName.Create(fileName));
		}
		
		void ShowApplicationDoesNotExistMessage(string fileName)
		{
			string resourceString = "${res:ICSharpCode.UnitTesting.TestRunnerNotFoundMessageFormat}";
			messageService.ShowErrorFormatted(resourceString, fileName);
		}
		
		protected override void OnAllTestsFinished()
		{
			testResultsReader.Join();
			base.OnAllTestsFinished();
		}
		
		public override void Stop()
		{
			SD.Log.Info("Killing unit test runner");
			processRunner.Kill();
		}
		
		public override void Dispose()
		{
			testResultsReader.Dispose();
			testResultsReader.TestFinished -= OnTestFinished;
		}
	}
}
