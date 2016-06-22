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
