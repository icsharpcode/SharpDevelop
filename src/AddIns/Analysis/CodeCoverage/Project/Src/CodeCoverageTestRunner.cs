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

//using System;
//using System.Diagnostics;
//using System.IO;
//using ICSharpCode.Core;
//using ICSharpCode.SharpDevelop.Project;
//using ICSharpCode.UnitTesting;
//
//namespace ICSharpCode.CodeCoverage
//{
//	public class CodeCoverageTestRunner : TestProcessRunnerBase
//	{
//		UnitTestingOptions options;
//		IFileSystem fileSystem;
//		OpenCoverApplication partCoverApplication;
//		OpenCoverSettingsFactory settingsFactory;
//		
//		public CodeCoverageTestRunner()
//			: this(new CodeCoverageTestRunnerContext())
//		{
//		}
//		
//		public CodeCoverageTestRunner(CodeCoverageTestRunnerContext context)
//			: base(context)
//		{
//			this.options = context.Options;
//			this.fileSystem = context.CodeCoverageFileSystem;
//			settingsFactory = new OpenCoverSettingsFactory(fileSystem);
//		}
//		
//		public bool HasCodeCoverageResults()
//		{
//			return fileSystem.FileExists(CodeCoverageResultsFileName);
//		}
//		
//		public CodeCoverageResults ReadCodeCoverageResults()
//		{
//			TextReader reader = fileSystem.CreateTextReader(CodeCoverageResultsFileName);
//			return new CodeCoverageResults(reader);
//		}
//		
//		public string CodeCoverageResultsFileName {
//			get { return partCoverApplication.CodeCoverageResultsFileName; }
//		}
//		
//		public override void Start(SelectedTests selectedTests)
//		{
//			AddProfilerEnvironmentVariableToProcessRunner();
//			CreatePartCoverApplication(selectedTests);
//			RemoveExistingCodeCoverageResultsFile();
//			CreateDirectoryForCodeCoverageResultsFile();
//			AppendRunningCodeCoverageMessage();
//			
//			base.Start(selectedTests);
//		}
//		
//		void AddProfilerEnvironmentVariableToProcessRunner()
//		{
//			ProcessRunner.EnvironmentVariables.Add("COMPLUS_ProfAPI_ProfilerCompatibilitySetting", "EnableV2Profiler");
//		}
//		
//		void CreatePartCoverApplication(SelectedTests selectedTests)
//		{
//			NUnitConsoleApplication nunitConsoleApp = new NUnitConsoleApplication(selectedTests, options);
//			nunitConsoleApp.Results = base.TestResultsMonitor.FileName;
//			
//			OpenCoverSettings settings = settingsFactory.CreateOpenCoverSettings(selectedTests.Project);
//			partCoverApplication = new OpenCoverApplication(nunitConsoleApp, settings);
//		}
//		
//		void RemoveExistingCodeCoverageResultsFile()
//		{
//			string fileName = CodeCoverageResultsFileName;
//			if (fileSystem.FileExists(fileName)) {
//				fileSystem.DeleteFile(fileName);
//			}
//		}
//		
//		void CreateDirectoryForCodeCoverageResultsFile()
//		{
//			string directory = Path.GetDirectoryName(CodeCoverageResultsFileName);
//			if (!fileSystem.DirectoryExists(directory)) {
//				fileSystem.CreateDirectory(directory);
//			}
//		}
//		
//		void AppendRunningCodeCoverageMessage()
//		{
//			string message = ParseString("${res:ICSharpCode.CodeCoverage.RunningCodeCoverage}");
//			OnMessageReceived(message);
//		}
//		
//		protected virtual string ParseString(string text)
//		{
//			return StringParser.Parse(text);
//		}
//		
//		protected override ProcessStartInfo GetProcessStartInfo(SelectedTests selectedTests)
//		{
//			return partCoverApplication.GetProcessStartInfo();
//		}
//		
//		protected override TestResult CreateTestResultForTestFramework(TestResult testResult)
//		{
//			return new NUnitTestResult(testResult);
//		}
//	}
//}
