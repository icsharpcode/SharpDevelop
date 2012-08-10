// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using ICSharpCode.CodeCoverage.Tests.Utils;
using UnitTesting.Tests.Utils;

namespace ICSharpCode.CodeCoverage.Tests.Testing
{
	[TestFixture]
	public class RunTestWithCodeCoverageCommandTests
	{
		DerivedRunTestWithCodeCoverageCommand command;
		MockRunTestCommandContext context;
		MockCodeCoverageTestRunnerFactory mockCodeCoverageTestRunnerFactory;
		
		[SetUp]
		public void Init()
		{
			context = new MockRunTestCommandContext();
			mockCodeCoverageTestRunnerFactory = new MockCodeCoverageTestRunnerFactory();
			command = new DerivedRunTestWithCodeCoverageCommand(context, mockCodeCoverageTestRunnerFactory);
		}
		
		[Test]
		public void OnBeforeRunTestsWhenNoCodeCoverageMessageViewCreatedCreatesNewMessageViewCategory()
		{
			command.CodeCoverageMessageViewCategory = null;
			command.CallOnBeforeRunTests();
			
			Assert.AreEqual("CodeCoverage", command.CodeCoverageMessageViewCategory.Category);
		}
		
		[Test]
		public void OnBeforeRunTestsWhenNoCodeCoverageMessageViewCreatedCreatesNewMessageViewCategoryWithCodeCoverageDisplayCategoryName()
		{
			command.CodeCoverageMessageViewCategory = null;
			command.ParsedStringToReturn = "Code Coverage";
			command.CallOnBeforeRunTests();
			
			string expectedDisplayCategoryName = "Code Coverage";
			Assert.AreEqual(expectedDisplayCategoryName, command.CodeCoverageMessageViewCategory.DisplayCategory);
		}
		
		[Test]
		public void OnBeforeRunTestsWhenNoCodeCoverageMessageViewCreatedPassedStringResourceToStringParser()
		{
			command.CodeCoverageMessageViewCategory = null;
			command.ParsedString = null;
			command.CallOnBeforeRunTests();
			
			string expectedStringResourceName = "${res:ICSharpCode.UnitTesting.CodeCoverage}";
			Assert.AreEqual(expectedStringResourceName, command.ParsedString);
		}
		
		[Test]
		public void OnBeforeRunTestsWhenCodeCoverageMessageViewCreatedPreviouslyDoesNotCreateAnotherMessageView()
		{
			MessageViewCategory view = new MessageViewCategory("Test");
			command.CodeCoverageMessageViewCategory = view;
			command.CallOnBeforeRunTests();
			Assert.AreEqual(view, command.CodeCoverageMessageViewCategory);
		}
		
		[Test]
		public void OnBeforeRunTestsClearsCodeCoverageMessageViewTextWithSafeAsyncCall()
		{
			MessageViewCategory view = new MessageViewCategory("Test");
			view.AppendText("abc");
			command.CodeCoverageMessageViewCategory = view;
			command.CallOnBeforeRunTests();
			
			Assert.AreEqual(String.Empty, view.Text);
		}
		
		[Test]
		public void OnBeforeRunTestsClearsCodeCoverageResults()
		{
			command.CallOnBeforeRunTests();
			
			Action expectedAction = CodeCoverageService.ClearResults;
			Assert.AreEqual(expectedAction, context.MockUnitTestWorkbench.SafeThreadAsyncMethodCalls[0]);
		}
		
		[Test]
		public void OnAfterRunTestsWhenNoCriticalTestErrorsCodeCoveragePadIsShown()
		{
			context.MockTaskService.HasCriticalErrorsReturnValue = false;
			PadDescriptor padDescriptor = AddCodeCoveragePadToMockWorkbench();
			command.CallOnAfterRunTests();
			
			Action expectedAction = padDescriptor.BringPadToFront;
			Assert.AreEqual(expectedAction, context.MockUnitTestWorkbench.SafeThreadAsyncMethodCalls[0]);
		}
		
		PadDescriptor AddCodeCoveragePadToMockWorkbench()
		{
			PadDescriptor padDescriptor = new PadDescriptor(typeof(CodeCoveragePad), "Code Coverage", String.Empty);
			context.MockUnitTestWorkbench.AddPadDescriptor(padDescriptor);
			return padDescriptor;
		}
		
		[Test]
		public void OnAfterRunTestsWhenCriticalErrorsCodeCoveragePadIsNotShown()
		{
			context.MockTaskService.HasCriticalErrorsReturnValue = true;
			PadDescriptor padDescriptor = AddCodeCoveragePadToMockWorkbench();
			command.CallOnAfterRunTests();
			
			Assert.AreEqual(0, context.MockUnitTestWorkbench.SafeThreadAsyncMethodCalls.Count);			
		}
		
		[Test]
		public void OnAfterRunTestsDoesNotTreatWarningsAsErrors()
		{
			context.MockTaskService.TreatWarningsAsErrorsParameterPassedToHasCriticalErrors = true;
			AddCodeCoveragePadToMockWorkbench();
			command.CallOnAfterRunTests();
			
			Assert.IsFalse(context.MockTaskService.TreatWarningsAsErrorsParameterPassedToHasCriticalErrors);			
		}
		
		[Test]
		public void MessageReceivedFromTestRunnerIsAddedToCodeCoverageMessageViewNotUnitTestsMessageView()
		{
			command.CodeCoverageMessageViewCategory = null;
			MessageReceivedEventArgs e = new MessageReceivedEventArgs("test");
			command.CallTestRunnerMessageReceived(this, e);
			string expectedText = "test\r\n";
			Assert.AreEqual(expectedText, command.CodeCoverageMessageViewCategory.Text);
		}
		
		[Test]
		public void CreateTestRunnerCreatesNewCodeCoverageTestRunner()
		{
			CodeCoverageTestRunner expectedTestRunner = mockCodeCoverageTestRunnerFactory.TestRunner;
			Assert.AreEqual(expectedTestRunner, command.CallCreateTestRunner(null));
		}
		
		[Test]
		public void CodeCoverageProcessExitsAndCodeCoverageFileExistsCausesCodeCoverageResultsToBeDisplayed()
		{
			ActionArguments<CodeCoverageResults> actionArgs = 
				CreateTestRunnerAndFireCodeCoverageProcessExitEvent();
			
			Action<CodeCoverageResults> expectedAction = CodeCoverageService.ShowResults;
			Assert.AreEqual(expectedAction, actionArgs.Action);
		}
		
		ActionArguments<CodeCoverageResults> CreateTestRunnerAndFireCodeCoverageProcessExitEvent()
		{
			command.CallCreateTestRunner(null);
			MockCSharpProject project = new MockCSharpProject();
			SelectedTests tests = new SelectedTests(project);
			mockCodeCoverageTestRunnerFactory.FileSystem.FileExistsReturnValue = true;
			mockCodeCoverageTestRunnerFactory.FileSystem.CreateTextReaderReturnValue = new StringReader("<a/>");
			mockCodeCoverageTestRunnerFactory.TestRunner.Start(tests);
			
			mockCodeCoverageTestRunnerFactory.FileSystem.CreateTextReaderReturnValue = CreateCodeCoverageResultsTextReader();
			
			mockCodeCoverageTestRunnerFactory.ProcessRunner.FireProcessExitedEvent();
			
			object actionArgsAsObject = context.MockUnitTestWorkbench.SafeThreadAsyncMethodCallsWithArguments[0];
			return (ActionArguments<CodeCoverageResults>)actionArgsAsObject;
		}
		
		[Test]
		public void CodeCoverageResultsFromXmlHasModuleCalledMyTests()
		{
			CodeCoverageResults results = CreateCodeCoverageResults();
			string expectedName = "MyTests";
			Assert.AreEqual(expectedName, results.Modules[0].Name);
		}
		
		CodeCoverageResults CreateCodeCoverageResults()
		{
			TextReader reader = CreateCodeCoverageResultsTextReader();
			return new CodeCoverageResults(reader);
		}
		
		TextReader CreateCodeCoverageResultsTextReader()
		{
			string xml =
				"<CoverageSession xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">\r\n" +
				"    <Modules>\r\n" +
				"        <Module hash=\"44-54-B6-13-97-49-45-F8-6A-74-9E-49-0C-77-87-C6-9C-54-47-7A\">\r\n" +
				"            <FullName>C:\\Projects\\MyTests\\bin\\MyTests.DLL</FullName>\r\n" +
				"            <ModuleName>MyTests</ModuleName>\r\n" +
				"            <Files>\r\n" +
				"                <File uid=\"1\" fullPath=\"c:\\Projects\\MyTests\\MyTestFixture.cs\" />\r\n" +
				"            </Files>\r\n" +
				"            <Classes>\r\n" +
				"                <Class>\r\n" +
				"                    <FullName>MyTests.Tests.MyTestFixture</FullName>\r\n" +
				"                    <Methods>\r\n" +
				"                        <Method visited=\"true\" cyclomaticComplexity=\"1\" sequenceCoverage=\"100\" branchCoverage=\"100\" isConstructor=\"false\" isStatic=\"false\" isGetter=\"false\" isSetter=\"false\">\r\n" +
				"                            <MetadataToken>100663297</MetadataToken>\r\n" +
				"                            <Name>System.Void MyTests.Tests.MyTestFixture::SimpleTest1()</Name>\r\n" +
				"                            <FileRef uid=\"1\" />\r\n" +
				"                            <SequencePoints>\r\n" +
				"                                <SequencePoint vc='12' sl='20' sc='3' el='20' ec='4'/>\r\n" +
				"                            </SequencePoints>\r\n" +
				"                        </Method>\r\n" +
				"                    </Methods>\r\n" +
				"                </Class>\r\n" +
				"            </Classes>\r\n" +
				"        </Module>\r\n" +
				"    </Modules>\r\n" +
				"</CoverageSession>";
			
			return new StringReader(xml);
		}
		
		[Test]
		public void CodeCoverageProcessExitsAndCodeCoverageFileExistsCausesCodeCoverageResultsToBeReadFromFile()
		{
			ActionArguments<CodeCoverageResults> actionArgs = 
				CreateTestRunnerAndFireCodeCoverageProcessExitEvent();
			
			CodeCoverageResults result = actionArgs.Arg;
			Assert.AreEqual("MyTests", result.Modules[0].Name);
		}
		
		[Test]
		public void CodeCoverageProcessExitsAndCodeCoverageFileDoesNotExistsAddsTaskToTaskList()
		{
			ActionArguments<Task> args = CreateTestRunnerAndFirePartCoverProcessExitEventWhenNoCoverageFileProduced();
			Action<Task> expectedAction = context.MockTaskService.Add;
			Assert.AreEqual(expectedAction, args.Action);
		}
		
		ActionArguments<Task> CreateTestRunnerAndFirePartCoverProcessExitEventWhenNoCoverageFileProduced()
		{
			command.CallCreateTestRunner(null);
			
			MockCSharpProject project = new MockCSharpProject();
			SelectedTests tests = new SelectedTests(project);
			
			mockCodeCoverageTestRunnerFactory.FileSystem.FileExistsReturnValue = true;
			mockCodeCoverageTestRunnerFactory.FileSystem.CreateTextReaderReturnValue = new StringReader("<a/>");
			mockCodeCoverageTestRunnerFactory.TestRunner.Start(tests);
			
			mockCodeCoverageTestRunnerFactory.FileSystem.FileExistsReturnValue = false;
			mockCodeCoverageTestRunnerFactory.ProcessRunner.FireProcessExitedEvent();
			
			object actionArgsAsObject = context.MockUnitTestWorkbench.SafeThreadAsyncMethodCallsWithArguments[0];
			return (ActionArguments<Task>)actionArgsAsObject;
		}
		
		[Test]
		public void CodeCoverageProcessExitsAndCodeCoverageFileDoesNotExistsAddsErrorTaskToTaskList()
		{
			command.ParsedStringToReturn = "No code coverage results file generated.";
			ActionArguments<Task> args = CreateTestRunnerAndFirePartCoverProcessExitEventWhenNoCoverageFileProduced();
			Task task = args.Arg;
			
			string description = @"No code coverage results file generated. c:\projects\MyTests\OpenCover\coverage.xml";
			int column = 1;
			int line = 1;
			Task expectedTask = new Task(null, description, column, line, TaskType.Error);
			
			TaskComparison comparison = new TaskComparison(expectedTask, task);
			
			Assert.IsTrue(comparison.IsMatch, comparison.MismatchReason);
		}
	}
}
