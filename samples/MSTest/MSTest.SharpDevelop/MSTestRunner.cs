// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.IO;

using ICSharpCode.SharpDevelop.Util;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.MSTest
{
	public class MSTestRunner : TestRunnerBase
	{
		IUnitTestProcessRunner processRunner;
		IFileSystem fileSystem;
		IUnitTestMessageService messageService;
		string resultsFileName;
		
		public MSTestRunner()
			: this(new UnitTestProcessRunner(),
			       new UnitTestFileService(),
			       new UnitTestMessageService())
		{
		}

		public MSTestRunner(
			IUnitTestProcessRunner processRunner,
			IFileSystem fileSystem,
			IUnitTestMessageService messageService)
		{
			this.processRunner = processRunner;
			this.fileSystem = fileSystem;
			this.messageService = messageService;
			
			processRunner.LogStandardOutputAndError = false;
			processRunner.OutputLineReceived += OutputLineReceived;
			processRunner.ErrorLineReceived += OutputLineReceived;
			processRunner.ProcessExited += ProcessRunnerExited;
		}
		
		void ProcessRunnerExited(object source, EventArgs e)
		{
			// Read all tests.
			if (FileExists(resultsFileName)) {
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
		
		void OutputLineReceived(object source, LineReceivedEventArgs e)
		{
			OnMessageReceived(e.Line);
		}
		
		public override void Start(SelectedTests selectedTests)
		{
			ProcessStartInfo startInfo = GetProcessStartInfo(selectedTests);
			TryDeleteResultsFile();
			Start(startInfo);
		}
		
		protected override ProcessStartInfo GetProcessStartInfo(SelectedTests selectedTests)
		{
			resultsFileName = new MSTestResultsFileName(selectedTests).FileName;
			CreateDirectoryForResultsFile();
			var mstestApplication = new MSTestApplication(selectedTests, resultsFileName);
			return mstestApplication.ProcessStartInfo;
		}
		
		void Start(ProcessStartInfo processStartInfo)
		{
			LogCommandLine(processStartInfo);
			
			if (FileExists(processStartInfo.FileName)) {
				processRunner.WorkingDirectory = processStartInfo.WorkingDirectory;
				processRunner.Start(processStartInfo.FileName, processStartInfo.Arguments);
			} else {
				ShowApplicationDoesNotExistMessage(processStartInfo.FileName);
			}
		}
		
		void CreateDirectoryForResultsFile()
		{
			string path = Path.GetDirectoryName(resultsFileName);
			if (!Directory.Exists(path)) {
				Directory.CreateDirectory(path);
			}
		}
		
		bool FileExists(string fileName)
		{
			return fileSystem.FileExists(fileName);
		}
		
		void ShowApplicationDoesNotExistMessage(string fileName)
		{
			string resourceString = "${res:ICSharpCode.UnitTesting.TestRunnerNotFoundMessageFormat}";
			messageService.ShowFormattedErrorMessage(resourceString, fileName);
		}
		
		public override void Stop()
		{
			processRunner.Kill();
		}
		
		public override void Dispose()
		{
			processRunner.ErrorLineReceived -= OutputLineReceived;
			processRunner.OutputLineReceived -= OutputLineReceived;
			processRunner.ProcessExited -= ProcessRunnerExited;
			
			TryDeleteResultsFile();
		}

		void TryDeleteResultsFile()
		{
			try {
				Console.WriteLine("Deleting results file: " + resultsFileName);
				File.Delete(resultsFileName);
			} catch (Exception ex) {
				Console.WriteLine(ex.Message);
			}
		}
	}
}
