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
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using RubyBinding.Tests.Utils;
using UnitTesting.Tests.Utils;

namespace RubyBinding.Tests.Testing
{
	[TestFixture]
	public class RubyTestDebuggerRunsSelectedTestMethodTestFixture
	{
		MockDebuggerService debuggerService;
		UnitTesting.Tests.Utils.MockDebugger debugger;
		MockMessageService messageService;
		MockCSharpProject project;
		RubyTestDebugger testDebugger;
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
			CreateTestDebugger();
			CreateTestMethod();
		}
		
		void CreateTestDebugger()
		{
			debuggerService = new MockDebuggerService();
			debugger = debuggerService.MockDebugger;
			messageService = new MockMessageService();
			testResultsMonitor = new MockTestResultsMonitor();
			testResultsMonitor.InitialFilePosition = 3;
			options = new RubyAddInOptions(new Properties());
			options.RubyFileName = @"c:\ironruby\ir.exe";
			fileService = new MockScriptingFileService();
			testDebugger = new RubyTestDebugger(debuggerService, messageService, testResultsMonitor, options, fileService);
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
			testDebugger.Start(selectedTests);
		}
		
		void CreateTemporaryResponseFileWriter()
		{
			responseFileText = new StringBuilder();
			responseFileStringWriter = new StringWriter(responseFileText);
			fileService.SetTextWriter(responseFileStringWriter);
		}
		
		[Test]
		public void TestDebuggerProcessFileNameIsIronRubyConsoleExeTakenFromAddInOptions()
		{
			RunTestsOnSelectedTestMethod();
			
			string expectedFileName = @"c:\ironruby\ir.exe";
			Assert.AreEqual(expectedFileName, debugger.ProcessStartInfo.FileName);
		}
		
		[Test]
		public void DisposingTestRunnerDeletesTemporaryResponseFile()
		{
			fileService.FileNameDeleted = null;
			RunTestsOnSelectedTestMethod();
			testDebugger.Dispose();
			
			string expectedFileName = @"d:\temp\tmp66.tmp";
			Assert.AreEqual(expectedFileName, fileService.FileNameDeleted);
		}
		
		[Test]
		public void DisposingTestRunnerDisposesTestResultsMonitor()
		{
			RunTestsOnSelectedTestMethod();
			testDebugger.Dispose();
			Assert.IsTrue(testResultsMonitor.IsDisposeMethodCalled);
		}
		
		[Test]
		public void CommandLineArgumentHasSharpDevelopTestRubyScriptFileNameAndTestResultsFileNameResponseFileName()
		{
			AddInPathHelper helper = new AddInPathHelper("RubyBinding");
			AddIn addin = helper.CreateDummyAddInInsideAddInTree();
			addin.FileName = @"c:\sharpdevelop\addins\rubybinding\rubybinding.addin";
				
			testResultsMonitor.FileName = @"d:\testresults.txt";
			RunTestsOnSelectedTestMethod();
			
			string expectedCommandLine =
				"--disable-gems " +
				"-D " +
				"\"-Ic:\\sharpdevelop\\addins\\rubybinding\\TestRunner\" " +
				"\"c:\\sharpdevelop\\addins\\rubybinding\\TestRunner\\sdtest.rb\" " +
				"--name=MyTestMethod " +
				"-- " +
				"\"d:\\testresults.txt\" " +
				"\"d:\\temp\\tmp66.tmp\"";
			
			Assert.AreEqual(expectedCommandLine, debugger.ProcessStartInfo.Arguments);
		}
		
		[Test]
		public void RubyTestResultReturnedFromTestFinishedEvent()
		{
			TestResult testResult = null;
			testDebugger.TestFinished += delegate(object source, TestFinishedEventArgs e) { 
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
