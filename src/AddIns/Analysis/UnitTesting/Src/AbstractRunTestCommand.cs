// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Project.Commands;

namespace ICSharpCode.UnitTesting
{
	public abstract class AbstractRunTestCommand : AbstractMenuCommand
	{
		static AbstractRunTestCommand runningTestCommand;
		IUnitTestsPad unitTestsPad;
		SelectedTests selectedTests;
		IRunTestCommandContext context;
		ITestRunner testRunner;
		IProgressMonitor testProgressMonitor;
		int totalProjectCount;
		
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
			GetUnitTestsPad();
			
			selectedTests = GetSelectedTests();
			if (selectedTests.HasProjects) {
				runningTestCommand = this;
				BeforeRun();
				BuildAndRunTests();
			}
		}
		
		SelectedTests GetSelectedTests()
		{
			return new SelectedTests(Owner, unitTestsPad.GetProjects());
		}
		
		void GetUnitTestsPad()
		{
			unitTestsPad = context.OpenUnitTestsPad;
			if (unitTestsPad == null) {
				unitTestsPad = CreateEmptyUnitTestsPad();
			}
		}
		
		EmptyUnitTestsPad CreateEmptyUnitTestsPad()
		{
			return new EmptyUnitTestsPad(context.OpenSolution);
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
			
			OnBeforeBuild();
		}
		
		void BuildAndRunTests()
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
			return selectedTests.Projects.Any(p => context.RegisteredTestFrameworks.IsBuildNeededBeforeTestRunForProject(p));
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
		
		void StopProgressMonitor()
		{
			if (testProgressMonitor != null) {
				testProgressMonitor.Dispose();
				testProgressMonitor = null;
			}
		}
		
		void UpdateUnitTestsPadToolbar()
		{
			unitTestsPad.UpdateToolbar();
		}
		
		/// <summary>
		/// Called before the build is started (even if no build needs to be performed).
		/// If multiple projects are to be tested this is called only once.
		/// </summary>
		protected virtual void OnBeforeBuild()
		{
		}
		
		/// <summary>
		/// Called before all tests are run (after the build has finished successfully).
		/// If multiple projects are to be tested this is called only once.
		/// </summary>
		protected virtual void OnBeforeRunTests()
		{
		}
		
		/// <summary>
		/// Runs the tests after building the project(s) under test.
		/// </summary>
		void BuildProjectBeforeRunningTests(SelectedTests selectedTests)
		{
			BuildProject build = CreateBuildProjectBeforeTestRun(selectedTests);
			build.BuildComplete += delegate {
				OnBuildComplete(build.LastBuildResults, selectedTests);
			};
			build.Run();
		}
		
		BuildProject CreateBuildProjectBeforeTestRun(SelectedTests selectedTests)
		{
			IEnumerable<IProject> projects = GetProjectsRequiringBuildBeforeTestRun(selectedTests);
			return context.BuildProjectFactory.CreateBuildProjectBeforeTestRun(projects);
		}
		
		IEnumerable<IProject> GetProjectsRequiringBuildBeforeTestRun(SelectedTests selectedTests)
		{
			return selectedTests
				.Projects
				.Where(p => context.RegisteredTestFrameworks.IsBuildNeededBeforeTestRunForProject(p));
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
		
		/// <summary>
		/// Called by derived classes when a single test run
		/// is finished.
		/// </summary>
		protected void TestRunCompleted()
		{
			StopActiveTestRunner();
			selectedTests.RemoveFirstProject();
			if (selectedTests.HasProjects) {
				RunTests(selectedTests);
			} else {
				StopProgressMonitor();
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
				UpdateProgressMonitorStatus(result);
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
			if (testProgressMonitor == null) {
				OnBeforeRunTests();
				testProgressMonitor = context.StatusBarService.CreateProgressMonitor();
				totalProjectCount = selectedTests.ProjectsCount;
			}
			testProgressMonitor.TaskName = GetProgressMonitorLabel(selectedTests.Project);
			testProgressMonitor.Progress = GetProgress(selectedTests.ProjectsCount);
			
			testRunner = CreateTestRunner(selectedTests.Project);
			if (testRunner != null) {
				StartTestRunner();
			}
		}
		
		void StartTestRunner()
		{
			testRunner.MessageReceived += TestRunnerMessageReceived;
			testRunner.AllTestsFinished += AllTestsFinished;
			testRunner.TestFinished += TestFinished;
			testRunner.Start(selectedTests);
		}
		
		string GetProgressMonitorLabel(IProject project)
		{
			StringTagPair tagPair = new StringTagPair("Name", project.Name);
			return StringParser.Parse("${res:ICSharpCode.UnitTesting.StatusBarProgressLabel}", tagPair);
		}
		
		double GetProgress(int projectsLeftToRunCount)
		{
			return (double)(totalProjectCount - projectsLeftToRunCount) / totalProjectCount;
		}
		
		protected virtual ITestRunner CreateTestRunner(IProject project)
		{
			return null;
		}
		
		protected virtual void TestRunnerMessageReceived(object source, MessageReceivedEventArgs e)
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
		
		void UpdateProgressMonitorStatus(TestResult result)
		{
			if (testProgressMonitor != null) {
				if (result.IsFailure) {
					testProgressMonitor.Status = OperationStatus.Error;
				} else if (result.IsIgnored && testProgressMonitor.Status == OperationStatus.Normal) {
					testProgressMonitor.Status = OperationStatus.Warning;
				}
			}
		}
	}
}
