// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.IO;
using System.Text;

using ICSharpCode.Core;
using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting.Tests.Utils;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using PythonBinding.Tests.Utils;
using UnitTesting.Tests.Utils;

namespace PythonBinding.Tests.Testing
{
	[TestFixture]
	public class PythonTestRunnerRunsSelectedTestMethodTestFixture
	{
		MockCSharpProject project;
		PythonTestRunner testRunner;
		MockProcessRunner processRunner;
		MockTestResultsMonitor testResultsMonitor;
		SelectedTests selectedTests;
		MockMethod methodToTest;
		PythonAddInOptions options;
		MockScriptingFileService fileService;
		StringBuilder responseFileText;
		StringWriter responseFileStringWriter;
		PythonStandardLibraryPath standardLibraryPath;
		MockMessageService messageService;
		
		[SetUp]
		public void Init()
		{
			CreateTestRunner();
			CreateTestMethod();
		}
		
		void CreateTestRunner()
		{
			processRunner = new MockProcessRunner();
			testResultsMonitor = new MockTestResultsMonitor();
			options = new PythonAddInOptions(new Properties());
			options.PythonFileName = @"c:\ironpython\ipy.exe";
			fileService = new MockScriptingFileService();
			messageService = new MockMessageService();
			standardLibraryPath = new PythonStandardLibraryPath(@"c:\python\lib");
			PythonTestRunnerContext context = new PythonTestRunnerContext(processRunner, 
				testResultsMonitor, 
				messageService,
				options,
				standardLibraryPath,
				fileService);
			
			testRunner = new PythonTestRunner(context);
		}
		
		void CreateTestMethod()
		{
			project = new MockCSharpProject();
			MockClass c = new MockClass("MyNamespace.MyTestClass");
			methodToTest = new MockMethod(c, "MyTestMethod");
		}
		
		void RunTestsOnSelectedTestMethod()
		{
			fileService.SetTempFileName(@"d:\temp\tmp66.tmp");
			CreateTemporaryResponseFileWriter();
			
			selectedTests = new SelectedTests(project, null, null, methodToTest);
			testRunner.Start(selectedTests);
		}
		
		void CreateTemporaryResponseFileWriter()
		{
			responseFileText = new StringBuilder();
			responseFileStringWriter = new StringWriter(responseFileText);
			fileService.SetTextWriter(responseFileStringWriter);
		}
		
		[Test]
		public void TestRunnerProcessFileNameIsIronPythonConsoleExeTakenFromAddInOptions()
		{
			RunTestsOnSelectedTestMethod();
			
			string expectedFileName = @"c:\ironpython\ipy.exe";
			Assert.AreEqual(expectedFileName, processRunner.CommandPassedToStartMethod);
		}
		
		[Test]
		public void CommandLineArgumentHasSharpDevelopTestPythonScriptAndResponseFileName()
		{
			AddInPathHelper helper = new AddInPathHelper("PythonBinding");
			AddIn addin = helper.CreateDummyAddInInsideAddInTree();
			addin.FileName = @"c:\sharpdevelop\addins\pythonbinding\pythonbinding.addin";
			
			RunTestsOnSelectedTestMethod();
			
			string expectedCommandLine =
				"\"c:\\sharpdevelop\\addins\\pythonbinding\\TestRunner\\sdtest.py\" " +
				"\"@d:\\temp\\tmp66.tmp\"";
			
			Assert.AreEqual(expectedCommandLine, processRunner.CommandArgumentsPassedToStartMethod);
		}
		
		[Test]
		public void ResponseFileCreatedUsingTempFileName()
		{
			RunTestsOnSelectedTestMethod();
			
			Assert.AreEqual(@"d:\temp\tmp66.tmp", fileService.CreateTextWriterInfoPassedToCreateTextWriter.FileName);
		}
		
		[Test]
		public void ResponseFileCreatedWithUtf8Encoding()
		{
			RunTestsOnSelectedTestMethod();
			
			Assert.AreEqual(Encoding.UTF8, fileService.CreateTextWriterInfoPassedToCreateTextWriter.Encoding);
		}
		
		[Test]
		public void ResponseFileCreatedWithAppendSetToFalse()
		{
			RunTestsOnSelectedTestMethod();
			
			Assert.IsFalse(fileService.CreateTextWriterInfoPassedToCreateTextWriter.Append);
		}
		
		[Test]
		public void DisposingTestRunnerDeletesTemporaryResponseFile()
		{
			fileService.FileNameDeleted = null;
			RunTestsOnSelectedTestMethod();
			testRunner.Dispose();
			
			string expectedFileName = @"d:\temp\tmp66.tmp";
			Assert.AreEqual(expectedFileName, fileService.FileNameDeleted);
		}
		
		[Test]
		public void DisposingTestRunnerDisposesTestResultsMonitor()
		{
			RunTestsOnSelectedTestMethod();
			testRunner.Dispose();
			Assert.IsTrue(testResultsMonitor.IsDisposeMethodCalled);
		}
		
		[Test]
		public void ResponseFileTextContainsPythonLibraryPathAndTestResultsFileNameAndFullyQualifiedTestMethod()
		{
			standardLibraryPath.Path = String.Empty;
			options.PythonLibraryPath = @"c:\python26\lib";
			testResultsMonitor.FileName = @"c:\temp\test-results.txt";
			RunTestsOnSelectedTestMethod();
			
			string expectedText =
				"/p:\"c:\\python26\\lib\"\r\n" +
				"/r:\"c:\\temp\\test-results.txt\"\r\n" +
				"MyNamespace.MyTestClass.MyTestMethod\r\n";
			
			Assert.AreEqual(expectedText, responseFileText.ToString());
		}
		
		[Test]
		public void ResponseFileTextDoesNotContainPythonLibraryPathIfItIsAnEmptyString()
		{
			options.PythonLibraryPath = String.Empty;
			standardLibraryPath.Path = String.Empty;
			testResultsMonitor.FileName = @"c:\temp\test-results.txt";
			RunTestsOnSelectedTestMethod();
			
			string expectedText =
				"/r:\"c:\\temp\\test-results.txt\"\r\n" +
				"MyNamespace.MyTestClass.MyTestMethod\r\n";
			
			Assert.AreEqual(expectedText, responseFileText.ToString());
		}
		
		[Test]
		public void ResponseFileTextContainsPythonLibraryPathFromPythonStandardLibraryPathObjectIfNotDefinedInAddInOptions()
		{
			standardLibraryPath.Path = @"c:\python\lib";
			options.PythonLibraryPath = String.Empty;
			testResultsMonitor.FileName = @"c:\temp\test-results.txt";
			RunTestsOnSelectedTestMethod();
			
			string expectedText =
				"/p:\"c:\\python\\lib\"\r\n" +
				"/r:\"c:\\temp\\test-results.txt\"\r\n" +
				"MyNamespace.MyTestClass.MyTestMethod\r\n";
			
			Assert.AreEqual(expectedText, responseFileText.ToString());
		}
		
		[Test]
		public void ResponseFileTextWriterDisposedAfterTestsRun()
		{
			RunTestsOnSelectedTestMethod();
			Assert.Throws<ObjectDisposedException>(delegate { responseFileStringWriter.Write("test"); });
		}
		
		[Test]
		public void ProcessRunnerWorkingDirectoryIsDirectoryContainingProject()
		{
			RunTestsOnSelectedTestMethod();
			
			string expectedDirectory = @"c:\projects\MyTests";
			string actualDirectory = processRunner.WorkingDirectory;
			
			Assert.AreEqual(expectedDirectory, actualDirectory);
		}
		
		[Test]
		public void PythonTestResultReturnedFromTestFinishedEvent()
		{
			TestResult testResult = null;
			testRunner.TestFinished += delegate(object source, TestFinishedEventArgs e) { 
				testResult = e.Result;
			};
			TestResult testResultToFire = new TestResult("test");
			testResultsMonitor.FireTestFinishedEvent(testResultToFire);
			
			Assert.IsInstanceOf(typeof(PythonTestResult), testResult);
		}
	}
}
