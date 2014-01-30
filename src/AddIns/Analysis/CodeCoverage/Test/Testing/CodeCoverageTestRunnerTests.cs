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
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.Text;
//using ICSharpCode.Core;
//using ICSharpCode.CodeCoverage;
//using ICSharpCode.CodeCoverage.Tests.Utils;
//using ICSharpCode.SharpDevelop;
//using ICSharpCode.UnitTesting;
//using NUnit.Framework;
//using UnitTesting.Tests.Utils;
//
//namespace ICSharpCode.CodeCoverage.Tests.Testing
//{
//	[TestFixture]
//	public class CodeCoverageTestRunnerTests
//	{
//		MockProcessRunner processRunner;
//		MockTestResultsMonitor testResultsMonitor;
//		UnitTestingOptions options;
//		DerivedCodeCoverageTestRunner testRunner;
//		MockFileSystem fileSystem;
//		MockMessageService messageService;
//		
//		[SetUp]
//		public void Init()
//		{
//			processRunner = new MockProcessRunner();
//			testResultsMonitor = new MockTestResultsMonitor();
//			options = new UnitTestingOptions(new Properties());
//			fileSystem = new MockFileSystem();
//			messageService = new MockMessageService();
//			testRunner = new DerivedCodeCoverageTestRunner(processRunner, testResultsMonitor, options, fileSystem, messageService);
//		}
//		
//		[Test]
//		public void CreateTestResultForTestFrameworkReturnsNUnitTestResult()
//		{
//			TestResult testResult = new TestResult("abc");
//			Assert.IsInstanceOf(typeof(NUnitTestResult), testRunner.CallCreateTestResultForTestFramework(testResult));
//		}
//		
//		[Test]
//		public void HasCodeCoverageResultsWhenCoverageFileExistsReturnsTrue()
//		{
//			StartTestRunner();
//			
//			fileSystem.FileExistsReturnValue = true;
//			
//			Assert.IsTrue(testRunner.HasCodeCoverageResults());
//		}
//		
//		void StartTestRunner()
//		{
//			FileUtility.ApplicationRootPath = @"d:\sharpdevelop";
//			MockCSharpProject project = new MockCSharpProject();
//			SelectedTests tests = new SelectedTests(project);
//			testRunner.Start(tests);
//		}
//		
//		[Test]
//		public void HasCodeCoverageResultsWhenCoverageFileDoesNotExistsReturnsFalse()
//		{
//			fileSystem.FileExistsReturnValue = false;
//			StartTestRunner();
//			Assert.IsFalse(testRunner.HasCodeCoverageResults());
//		}
//		
//		[Test]
//		public void HasCodeCoverageResultsAfterTestRunChecksPassesCodeCoverageFileToFileExistsMethod()
//		{
//			fileSystem.FileExistsReturnValue = false;
//			fileSystem.FileExistsPathParameter = null;
//			StartTestRunner();
//			testRunner.HasCodeCoverageResults();
//			
//			string expectedFileName = 
//				@"c:\projects\MyTests\OpenCover\coverage.xml";
//			
//			Assert.AreEqual(expectedFileName, fileSystem.FileExistsPathParameter);
//		}
//		
//		[Test]
//		public void ReadCodeCoverageResultsAfterTestRunChecksPassesCodeCoverageFileToCreateTextReaderMethod()
//		{
//			StartTestRunner();
//			
//			fileSystem.FileExistsReturnValue = true;
//			fileSystem.CreateTextReaderPathParameter = null;
//			fileSystem.CreateTextReaderReturnValue = new StringReader("<abc/>");
//			
//			testRunner.ReadCodeCoverageResults();
//			
//			string expectedFileName = 
//				@"c:\projects\MyTests\OpenCover\coverage.xml";
//			
//			Assert.AreEqual(expectedFileName, fileSystem.CreateTextReaderPathParameter);
//		}
//		
//		[Test]
//		public void GetProcessStartInfoWhenTestResultsFileNameSetReturnsCommandLineWithTestResultsFileName()
//		{
//			FileUtility.ApplicationRootPath = @"d:\sharpdevelop";
//			testResultsMonitor.FileName = @"d:\temp\results.txt";
//			
//			fileSystem.CreateTextReaderReturnValue = CreatePartCoverSettingsTextReader();
//			fileSystem.FileExistsReturnValue = true;
//			
//			MockCSharpProject project = new MockCSharpProject();
//			SelectedTests tests = new SelectedTests(project);
//			testRunner.Start(tests);
//			ProcessStartInfo processStartInfo = testRunner.CallGetProcessStartInfo(tests);
//			
//			string expectedCommandLine =
//				"-register:user -target:\"d:\\sharpdevelop\\bin\\Tools\\NUnit\\nunit-console-x86.exe\" " +
//				"-targetdir:\"c:\\projects\\MyTests\\bin\\Debug\" " +
//				"-targetargs:\"\\\"c:\\projects\\MyTests\\bin\\Debug\\MyTests.dll\\\" /noxml /results=\\\"d:\\temp\\results.txt\\\"\" " +
//				"-output:\"c:\\projects\\MyTests\\OpenCover\\coverage.xml\" " +
//				"-filter:\"+[MyTests]* \"";
//			
//			Assert.AreEqual(expectedCommandLine, processStartInfo.Arguments);
//		}
//		
//		TextReader CreatePartCoverSettingsTextReader()
//		{
//			OpenCoverSettings settings = new OpenCoverSettings();
//			settings.Include.Add("[MyTests]*");
//			StringBuilder text = new StringBuilder();
//			StringWriter writer = new StringWriter(text);
//			settings.Save(writer);
//			
//			return new StringReader(text.ToString());
//		}
//		
//		[Test]
//		public void StartSetsProfilerEnvironmentVariableInProcessRunner()
//		{
//			StartTestRunner();
//			string environmentVariableValue = processRunner.EnvironmentVariables["COMPLUS_ProfAPI_ProfilerCompatibilitySetting"];
//			Assert.AreEqual("EnableV2Profiler", environmentVariableValue);
//		}
//		
//		[Test]
//		public void StartWhenCodeCoverageResultsFileExistsDeletesExistingCodeCoverageResultsFile()
//		{
//			fileSystem.FileExistsReturnValue = true;
//			fileSystem.CreateTextReaderReturnValue = new StringReader("<abc/>");
//			StartTestRunner();
//			
//			string expectedFileName = @"c:\projects\MyTests\OpenCover\coverage.xml";
//			Assert.AreEqual(expectedFileName, fileSystem.DeleteFilePathParameter);
//		}
//		
//		[Test]
//		public void StartWhenCodeCoverageResultsFileDoesNotExistsCodeCoverageResultsFileIsNotDeleted()
//		{
//			fileSystem.FileExistsReturnValue = false;
//			StartTestRunner();
//			
//			Assert.IsNull(fileSystem.DeleteFilePathParameter);
//		}
//		
//		[Test]
//		public void StartCreatesDirectoryCodeCoverageResultsFileIfDoesNotExist()
//		{
//			fileSystem.DirectoryExistsReturnValue = false;
//			StartTestRunner();
//			
//			string expectedDirectory = @"c:\projects\MyTests\OpenCover";
//			Assert.AreEqual(expectedDirectory, fileSystem.CreateDirectoryPathParameter);
//		}
//		
//		[Test]
//		public void StartChecksDirectoryForCodeCoverageResultsExists()
//		{
//			fileSystem.DirectoryExistsReturnValue = true;
//			StartTestRunner();
//			
//			string expectedDirectory = @"c:\projects\MyTests\OpenCover";
//			Assert.AreEqual(expectedDirectory, fileSystem.DirectoryExistsPathParameter);
//		}
//		
//		[Test]
//		public void StartDoesNotCreateDirectoryForCodeCoverageResultsFileIfItExists()
//		{
//			fileSystem.DirectoryExistsReturnValue = true;
//			StartTestRunner();
//			
//			Assert.IsNull(fileSystem.CreateDirectoryPathParameter);
//		}
//		
//		[Test]
//		public void StartFiresMessagesReceivedEventTwice()
//		{
//			List<string> messages = new List<string>();
//			testRunner.MessageReceived += delegate(object o, MessageReceivedEventArgs e) {
//				messages.Add(e.Message);
//			};
//			
//			testRunner.ParseStringReturnValue = "Running code coverage";
//			StartTestRunner();
//			
//			string[] expectedMessages = new string[] {
//				"Running code coverage",
//				GetCodeCoverageCommandLine()
//			};
//			
//			Assert.AreEqual(expectedMessages, messages.ToArray());
//		}
//		
//		string GetCodeCoverageCommandLine()
//		{
//			return 
//				"\"d:\\sharpdevelop\\bin\\Tools\\OpenCover\\OpenCover.Console.exe\" -register:user " +
//				"-target:\"d:\\sharpdevelop\\bin\\Tools\\NUnit\\nunit-console-x86.exe\" " +
//				"-targetdir:\"c:\\projects\\MyTests\\bin\\Debug\" " +
//				"-targetargs:\"\\\"c:\\projects\\MyTests\\bin\\Debug\\MyTests.dll\\\" /noxml\" " + 
//				"-output:\"c:\\projects\\MyTests\\OpenCover\\coverage.xml\" " +
//				"-filter:\"+[*]* \"";
//		}
//		
//		[Test]
//		public void StartParsesTextForRunningCodeCoverageMessages()
//		{
//			testRunner.ParseStringReturnValue = "Running code coverage";
//			StartTestRunner();
//			
//			string expectedStringResource = "${res:ICSharpCode.CodeCoverage.RunningCodeCoverage}";
//			
//			Assert.AreEqual(expectedStringResource, testRunner.ParseStringParameter);
//		}
//	}
//}
