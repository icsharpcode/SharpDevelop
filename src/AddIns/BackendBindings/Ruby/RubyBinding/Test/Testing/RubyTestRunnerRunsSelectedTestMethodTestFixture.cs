// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.IO;
using System.Text;

using ICSharpCode.Core;
using ICSharpCode.RubyBinding;
using ICSharpCode.Scripting.Tests.Utils;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using RubyBinding.Tests.Utils;
using UnitTesting.Tests.Utils;

namespace RubyBinding.Tests.Testing
{
	[TestFixture]
	public class RubyTestRunnerRunsSelectedTestMethodTestFixture
	{
		MockCSharpProject project;
		RubyTestRunner testRunner;
		MockProcessRunner processRunner;
		MockTestResultsMonitor testResultsMonitor;
		SelectedTests selectedTests;
		MockMethod methodToTest;
		RubyAddInOptions options;
		MockScriptingFileService fileService;
		StringBuilder responseFileText;
		StringWriter responseFileStringWriter;
		
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
			testResultsMonitor.InitialFilePosition = 3;
			options = new RubyAddInOptions(new Properties());
			options.RubyFileName = @"c:\ironruby\ir.exe";
			fileService = new MockScriptingFileService();
			MockMessageService messageService = new MockMessageService();
			
			RubyTestRunnerContext context = new RubyTestRunnerContext(processRunner, testResultsMonitor, options, fileService, messageService);
			testRunner = new RubyTestRunner(context);
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
		public void TestRunnerProcessFileNameIsIronRubyConsoleExeTakenFromAddInOptions()
		{
			RunTestsOnSelectedTestMethod();
			
			string expectedFileName = @"c:\ironruby\ir.exe";
			Assert.AreEqual(expectedFileName, processRunner.CommandPassedToStartMethod);
		}
		
		[Test]
		public void CommandLineArgumentHasSharpDevelopTestRubyScriptFileNameAndTestResultsFileNameAndResponseFileName()
		{
			AddInPathHelper helper = new AddInPathHelper("RubyBinding");
			AddIn addin = helper.CreateDummyAddInInsideAddInTree();
			addin.FileName = @"c:\sharpdevelop\addins\rubybinding\rubybinding.addin";
			
			testResultsMonitor.FileName = @"d:\testresults.txt";
			RunTestsOnSelectedTestMethod();
			
			string expectedCommandLine =
				"--disable-gems " +
				"\"-Ic:\\sharpdevelop\\addins\\rubybinding\\TestRunner\" " +
				"\"c:\\sharpdevelop\\addins\\rubybinding\\TestRunner\\sdtest.rb\" " +
				"--name=MyTestMethod " +
				"-- " +
				"\"d:\\testresults.txt\" " +
				"\"d:\\temp\\tmp66.tmp\"";
			
			Assert.AreEqual(expectedCommandLine, processRunner.CommandArgumentsPassedToStartMethod);
		}
		
		[Test]
		public void ResponseFileCreatedUsingTempFileName()
		{
			RunTestsOnSelectedTestMethod();
			
			Assert.AreEqual(@"d:\temp\tmp66.tmp", fileService.CreateTextWriterInfoPassedToCreateTextWriter.FileName);
		}
		
		[Test]
		public void ResponseFileCreatedWithAsciiEncoding()
		{
			RunTestsOnSelectedTestMethod();
			
			Assert.AreEqual(Encoding.ASCII, fileService.CreateTextWriterInfoPassedToCreateTextWriter.Encoding);
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
		public void ResponseFileTextContainsTestMethodFileName()
		{
			methodToTest.CompilationUnit.FileName = @"d:\projects\ruby\test.rb";
			RunTestsOnSelectedTestMethod();
			
			string expectedText =
				"d:\\projects\\ruby\\test.rb\r\n";
			
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
		public void RubyTestResultReturnedFromTestFinishedEvent()
		{
			TestResult testResult = null;
			testRunner.TestFinished += delegate(object source, TestFinishedEventArgs e) { 
				testResult = e.Result;
			};
			TestResult testResultToFire = new TestResult("test");
			testResultsMonitor.FireTestFinishedEvent(testResultToFire);
			
			Assert.IsInstanceOf(typeof(RubyTestResult), testResult);
		}
		
		[Test]
		public void TestResultsMonitorInitialFilePositionIsZero()
		{
			Assert.AreEqual(0, testResultsMonitor.InitialFilePosition);
		}
	}
}
