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
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using PythonBinding.Tests.Utils;
using UnitTesting.Tests.Utils;

namespace PythonBinding.Tests.Testing
{
	[TestFixture]
	public class PythonTestDebuggerRunsSelectedTestMethodTestFixture
	{
		MockDebuggerService debuggerService;
		UnitTesting.Tests.Utils.MockDebugger debugger;
		MockMessageService messageService;
		MockCSharpProject project;
		PythonTestDebugger testDebugger;
		MockTestResultsMonitor testResultsMonitor;
		SelectedTests selectedTests;
		MockMethod methodToTest;
		PythonAddInOptions options;
		MockScriptingFileService fileService;
		StringBuilder responseFileText;
		StringWriter responseFileStringWriter;
		PythonStandardLibraryPath standardLibraryPath;
		
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
			options = new PythonAddInOptions(new Properties());
			options.PythonFileName = @"c:\ironpython\ipy.exe";
			standardLibraryPath = new PythonStandardLibraryPath(@"c:\python\lib");
			fileService = new MockScriptingFileService();
			testDebugger = new PythonTestDebugger(debuggerService, messageService, testResultsMonitor, options, standardLibraryPath, fileService);
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
		public void TestDebuggerProcessFileNameIsIronPythonConsoleExeTakenFromAddInOptions()
		{
			RunTestsOnSelectedTestMethod();
			
			string expectedFileName = @"c:\ironpython\ipy.exe";
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
		public void CommandLineArgumentHasSharpDevelopTestPythonScriptAndResponseFileName()
		{
			AddInPathHelper helper = new AddInPathHelper("PythonBinding");
			AddIn addin = helper.CreateDummyAddInInsideAddInTree();
			addin.FileName = @"c:\sharpdevelop\addins\pythonbinding\pythonbinding.addin";
			
			RunTestsOnSelectedTestMethod();
			
			string expectedCommandLine =
				"-X:Debug " +
				"\"c:\\sharpdevelop\\addins\\pythonbinding\\TestRunner\\sdtest.py\" " +
				"\"@d:\\temp\\tmp66.tmp\"";
			
			Assert.AreEqual(expectedCommandLine, debugger.ProcessStartInfo.Arguments);
		}
		
		[Test]
		public void PythonTestResultReturnedFromTestFinishedEvent()
		{
			TestResult testResult = null;
			testDebugger.TestFinished += delegate(object source, TestFinishedEventArgs e) { 
				testResult = e.Result;
			};
			TestResult testResultToFire = new TestResult("test");
			testResultsMonitor.FireTestFinishedEvent(testResultToFire);
			
			Assert.IsInstanceOf(typeof(PythonTestResult), testResult);
		}
		
		[Test]
		public void ResponseFileTextContainsPythonLibraryPathFromPythonStandardLibraryPathObjectIfNotDefinedInAddInOptions()
		{
			standardLibraryPath.Path = @"c:\python\lib;c:\python\lib\lib-tk";
			options.PythonLibraryPath = String.Empty;
			testResultsMonitor.FileName = @"c:\temp\test-results.txt";
			RunTestsOnSelectedTestMethod();
			
			string expectedText =
				"/p:\"c:\\python\\lib\"\r\n" +
				"/p:\"c:\\python\\lib\\lib-tk\"\r\n" +
				"/r:\"c:\\temp\\test-results.txt\"\r\n" +
				"MyNamespace.MyTestClass.MyTestMethod\r\n";
			
			Assert.AreEqual(expectedText, responseFileText.ToString());
		}
	}
}
