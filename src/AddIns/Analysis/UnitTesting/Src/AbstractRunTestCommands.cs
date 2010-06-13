// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.UnitTesting
{
	public abstract class AbstractRunTestCommand : AbstractMenuCommand
	{
		static AbstractRunTestCommand runningTestCommand;
		IUnitTestsPad unitTestsPad;
		SelectedTests selectedTests;
		IRunTestCommandContext context;
		ITestRunner testRunner;
		
		public AbstractRunTestCommand()
			: this(new RunTestCommandContext())
		{
		}
		
		public AbstractRunTestCommand(IRunTestCommandContext context)
		{
			this.context = context;
		}
		
		protected IRunTestCommandContext Context {
			get { return context; }
		}
		
		/// <summary>
		/// Gets the running test command.
		/// </summary>
		public static AbstractRunTestCommand RunningTestCommand {
			get { return runningTestCommand; }
			set { runningTestCommand = value; }
		}
		
		/// <summary>
		/// Gets whether a test is currently running.
		/// </summary>
		public static bool IsRunningTest {
			get { return runningTestCommand != null; }
		}
		
		public override void Run()
		{
			GetUnitTestsPad(context.OpenUnitTestsPad);
			
			selectedTests = new SelectedTests(Owner, this.unitTestsPad.GetProjects());
			if (selectedTests.HasProjects) {
				runningTestCommand = this;
				BeforeRun();
				RunTests();
			}
		}
		
		void GetUnitTestsPad(IUnitTestsPad unitTestsPad)
		{
			if (unitTestsPad != null) {
				this.unitTestsPad = unitTestsPad;
			} else {
				this.unitTestsPad = new EmptyUnitTestsPad();
			}
		}
		
		/// <summary>
		/// Sets the initial workbench state before starting
		/// a test run.
		/// </summary>
		void BeforeRun()
		{
			ClearTasks();
			ClearUnitTestCategoryText();
			
			ShowUnitTestsPad();
			ShowOutputPad();
			
			UpdateUnitTestsPadToolbar();
			ResetAllTestResultsInUnitTestsPad();
			
			OnBeforeRunTests();
		}
		
		void RunTests()
		{
			if (IsBuildNeededBeforeTestRun()) {
				BuildProjectBeforeRunningTests(selectedTests);
			} else {
				context.SaveAllFilesCommand.SaveAllFiles();
				RunTests(selectedTests);
			}
		}
		
		bool IsBuildNeededBeforeTestRun()
		{
			return context.RegisteredTestFrameworks.IsBuildNeededBeforeTestRunForProject(selectedTests.Project);
		}
		
		void ClearTasks()
		{
			context.TaskService.BuildMessageViewCategory.ClearText();
			context.TaskService.InUpdate = true;
			context.TaskService.ClearExceptCommentTasks();
			context.TaskService.InUpdate = false;
		}
		
		void ClearUnitTestCategoryText()
		{
			context.UnitTestCategory.ClearText();
		}
		
		void ShowUnitTestsPad()
		{
			unitTestsPad.BringToFront();
		}
		
		void UpdateUnitTestsPadToolbar()
		{
			unitTestsPad.UpdateToolbar();
		}
		
		/// <summary>
		/// Called before all tests are run. If multiple projects are
		/// to be tested this is called only once.
		/// </summary>
		protected virtual void OnBeforeRunTests()
		{
		}
		
		/// <summary>
		/// Runs the tests after building the project under test.
		/// </summary>
		void BuildProjectBeforeRunningTests(SelectedTests selectedTests)
		{
			BuildProjectBeforeTestRun build = CreateBuildProjectBeforeTestRun(selectedTests);
			build.BuildComplete += delegate {
				OnBuildComplete(build.LastBuildResults, selectedTests);
			};
			build.Run();
		}
		
		BuildProjectBeforeTestRun CreateBuildProjectBeforeTestRun(SelectedTests selectedTests)
		{
			return context.BuildProjectFactory.CreateBuildProjectBeforeTestRun(selectedTests.Project);
		}
		
		/// <summary>
		/// Stops running the tests.
		/// </summary>
		public void Stop()
		{
			StopActiveTestRunner();
			
			runningTestCommand = null;
			UpdateUnitTestsPadToolbar();
			
			OnStop();
		}
		
		void StopActiveTestRunner()
		{
			if (testRunner != null) {
				testRunner.Stop();
				testRunner.Dispose();
				testRunner = null;
			}
		}
		
		/// <summary>
		/// Called after all tests have been run even if there have
		/// been errors. If multiple projects are to be tested this is called only once.
		/// </summary>
		protected virtual void OnAfterRunTests()
		{
		}
		
		protected virtual void RunTests(NUnitConsoleApplication helper)
		{
		}
		
		/// <summary>
		/// Called by derived classes when a single test run
		/// is finished.
		/// </summary>
		protected void TestRunCompleted()
		{
			StopActiveTestRunner();
			selectedTests.RemoveFirstProject();
			if (selectedTests.HasProjects) {
				RunTests();
			} else {
				runningTestCommand = null;
				UpdateUnitTestsPadToolbar();
				ShowErrorList();
				OnAfterRunTests();
			}
		}
		
		void TestFinished(object source, TestFinishedEventArgs e)
		{
			context.Workbench.SafeThreadAsyncCall(ShowResult, e.Result);
		}
		
		protected void ShowResult(TestResult result)
		{
			if (IsTestResultFailureOrIsIgnored(result)) {
				AddTaskForTestResult(result);
			}
			UpdateTestResult(result);
		}
		
		bool IsTestResultFailureOrIsIgnored(TestResult result)
		{
			return result.IsFailure || result.IsIgnored;
		}
		
		void AddTaskForTestResult(TestResult testResult)
		{
			TestProject project = GetTestProjectForProject(selectedTests.Project);
			Task task = TestResultTask.Create(testResult, project);
			context.TaskService.Add(task);
		}
		
		/// <summary>
		/// Called when the test run should be stopped.
		/// </summary>
		protected virtual void OnStop()
		{
		}
		
		void ShowOutputPad()
		{
			ShowPad(context.Workbench.GetPad(typeof(CompilerMessageView)));
		}
		
		protected void ShowPad(PadDescriptor padDescriptor)
		{
			context.Workbench.SafeThreadAsyncCall(padDescriptor.BringPadToFront);
		}
		
		void ShowErrorList()
		{
			if (HasErrorsThatShouldBeDisplayed()) {
				ShowPad(context.Workbench.GetPad(typeof(ErrorListPad)));
			}
		}
		
		bool HasErrorsThatShouldBeDisplayed()
		{
			return context.TaskService.SomethingWentWrong &&
				context.BuildOptions.ShowErrorListAfterBuild;
		}
		
		/// <summary>
		/// Runs the test for the project after a successful build.
		/// </summary>
		void OnBuildComplete(BuildResults results, SelectedTests selectedTests)
		{
			if (BuildHasNoErrorsAndStillRunningTests(results)) {
				RunTests(selectedTests);
			} else {
				if (IsRunningTest) {
					Stop();
				}
				ShowErrorList();
			}
		}
		
		bool BuildHasNoErrorsAndStillRunningTests(BuildResults results)
		{
			return (results.ErrorCount == 0) && IsRunningTest;
		}
		
		void RunTests(SelectedTests selectedTests)
		{
			testRunner = CreateTestRunner(selectedTests.Project);
			if (testRunner != null) {
				testRunner.MessageReceived += TestRunnerMessageReceived;
				testRunner.AllTestsFinished += AllTestsFinished;
				testRunner.TestFinished += TestFinished;
				testRunner.Start(selectedTests);
			}
		}
		
		protected virtual ITestRunner CreateTestRunner(IProject project)
		{
			return null;
		}
		
		void TestRunnerMessageReceived(object source, MessageReceivedEventArgs e)
		{
			context.UnitTestCategory.AppendLine(e.Message);
		}
		
		void AllTestsFinished(object source, EventArgs e)
		{
			context.Workbench.SafeThreadAsyncCall(TestRunCompleted);
		}
		
		/// <summary>
		/// Clears the test results in the test tree view for all the
		/// displayed projects.
		/// </summary>
		void ResetAllTestResultsInUnitTestsPad()
		{
			unitTestsPad.ResetTestResults();
		}
		
		TestProject GetTestProjectForProject(IProject project)
		{
			return unitTestsPad.GetTestProject(project);
		}
		
		void UpdateTestResult(TestResult result)
		{
			TestProject testProject = GetTestProjectForProject(selectedTests.Project);
			if (testProject != null) {
				testProject.UpdateTestResult(result);
			}
		}
	}
}
