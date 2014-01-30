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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ICSharpCode.Core;
using ICSharpCode.NRefactory.Utils;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.UnitTesting.Frameworks
{
	/// <summary>
	/// Manages the execution of tests across multiple projects.
	/// Takes care of building the projects (if necessary) and showing progress in the UI.
	/// </summary>
	public class TestExecutionManager
	{
		readonly IBuildService buildService;
		readonly IUnitTestTaskService taskService;
		readonly IUnitTestSaveAllFilesCommand saveAllFilesCommand;
		readonly ITestService testService;
		readonly IWorkbench workbench;
		readonly IMessageLoop mainThread;
		readonly IStatusBarService statusBarService;
		readonly IBuildOptions buildOptions;
		
		public TestExecutionManager()
		{
			this.buildService = SD.BuildService;
			this.taskService = new UnitTestTaskService();
			this.saveAllFilesCommand = new UnitTestSaveAllFilesCommand();
			this.testService = SD.GetRequiredService<ITestService>();
			this.workbench = SD.Workbench;
			this.statusBarService = SD.StatusBar;
			this.mainThread = SD.MainThread;
			this.buildOptions = new UnitTestBuildOptions();
		}
		
		readonly MultiDictionary<ITestProject, ITest> testsByProject = new MultiDictionary<ITestProject, ITest>();
		CancellationToken cancellationToken;
		ITestProject currentProjectBeingTested;
		IProgressMonitor testProgressMonitor;
		
		public async Task RunTestsAsync(IEnumerable<ITest> selectedTests, TestExecutionOptions options, CancellationToken cancellationToken)
		{
			this.cancellationToken = cancellationToken;
			GroupTestsByProject(selectedTests);
			
			ClearTasks();
			ShowUnitTestsPad();
			ShowOutputPad();
			
			ResetTestResults();
			saveAllFilesCommand.SaveAllFiles();
			
			// Run the build, if necessary:
			var projectsToBuild = testsByProject.Keys.Where(p => p.IsBuildNeededBeforeTestRun).Select(p => p.Project).ToList();
			if (projectsToBuild.Count > 0) {
				using (cancellationToken.Register(buildService.CancelBuild)) {
					var buildOptions = new BuildOptions(BuildTarget.Build);
					buildOptions.BuildDetection = BuildOptions.BuildOnExecute;
					var buildResults = await buildService.BuildAsync(projectsToBuild, buildOptions);
					if (buildResults.Result != BuildResultCode.Success)
						return;
				}
			}
			
			cancellationToken.ThrowIfCancellationRequested();
			using (IProgressMonitor progressMonitor = statusBarService.CreateProgressMonitor(cancellationToken)) {
				int projectsLeftToRun = testsByProject.Count;
				foreach (IGrouping<ITestProject, ITest> g in testsByProject.OrderBy(g => g.Key.DisplayName)) {
					currentProjectBeingTested = g.Key;
					progressMonitor.TaskName = GetProgressMonitorLabel(currentProjectBeingTested);
					progressMonitor.Progress = GetProgress(projectsLeftToRun);
					using (testProgressMonitor = progressMonitor.CreateSubTask(1.0 / testsByProject.Count)) {
						using (ITestRunner testRunner = currentProjectBeingTested.CreateTestRunner(options)) {
							testRunner.TestFinished += testRunner_TestFinished;
							var writer = new MessageViewCategoryTextWriter(testService.UnitTestMessageView);
							await testRunner.RunAsync(g, testProgressMonitor, writer, testProgressMonitor.CancellationToken);
						}
					}
					projectsLeftToRun--;
					progressMonitor.CancellationToken.ThrowIfCancellationRequested();
				}
			}
			
			ShowErrorList();
		}

		void GroupTestsByProject(IEnumerable<ITest> selectedTests)
		{
			foreach (ITest test in selectedTests) {
				if (test == null)
					continue;
				if (test.ParentProject == null) {
					// When a solution is selected, select all its projects individually
					foreach (ITest project in test.NestedTests) {
						Debug.Assert(project == project.ParentProject);
						testsByProject.Add(project.ParentProject, project);
					}
				} else {
					testsByProject.Add(test.ParentProject, test);
				}
				cancellationToken.ThrowIfCancellationRequested();
			}
		}
		
		void ClearTasks()
		{
			taskService.BuildMessageViewCategory.Clear();
			taskService.ClearExceptCommentTasks();
			testService.UnitTestMessageView.Clear();
		}
		
		void ShowUnitTestsPad()
		{
			var descriptor = workbench.GetPad(typeof(UnitTestsPad));
			descriptor.BringPadToFront();
			var pad = descriptor.PadContent as UnitTestsPad;
			if (pad != null) {
				pad.TreeView.SelectedTests = testsByProject.Values;
			}
		}
		
		void ShowOutputPad()
		{
			testService.UnitTestMessageView.Activate(true);
		}
		
		void ResetTestResults()
		{
			cancellationToken.ThrowIfCancellationRequested();
			foreach (ITest test in testsByProject.Values) {
				test.ResetTestResults();
			}
			cancellationToken.ThrowIfCancellationRequested();
		}
		
		string GetProgressMonitorLabel(ITestProject project)
		{
			StringTagPair tagPair = new StringTagPair("Name", project.DisplayName);
			return StringParser.Parse("${res:ICSharpCode.UnitTesting.StatusBarProgressLabel}", tagPair);
		}
		
		double GetProgress(int projectsLeftToRunCount)
		{
			int totalProjectCount = testsByProject.Count;
			return (double)(totalProjectCount - projectsLeftToRunCount) / totalProjectCount;
		}
		
		void testRunner_TestFinished(object sender, TestFinishedEventArgs e)
		{
			mainThread.InvokeAsyncAndForget(delegate {
				ShowResult(e.Result);
			});
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
			SDTask task = TestResultTask.Create(testResult, currentProjectBeingTested);
			taskService.Add(task);
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
		
		void UpdateTestResult(TestResult result)
		{
			if (currentProjectBeingTested != null) {
				currentProjectBeingTested.UpdateTestResult(result);
			}
		}
		
		void ShowErrorList()
		{
			if (taskService.SomethingWentWrong && buildOptions.ShowErrorListAfterBuild) {
				workbench.GetPad(typeof(ErrorListPad)).BringPadToFront();
			}
		}
	}
}
