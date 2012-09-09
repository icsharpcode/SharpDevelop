// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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

namespace ICSharpCode.UnitTesting.Frameworks
{
	/// <summary>
	/// Manages the execution of tests across multiple projects.
	/// Takes care of building the projects (if necessary) and showing progress in the UI.
	/// </summary>
	public class TestExecutionManager
	{
		readonly IBuildProjectFactory buildProjectFactory;
		readonly IUnitTestTaskService taskService;
		readonly IUnitTestSaveAllFilesCommand saveAllFilesCommand;
		readonly ITestService testService;
		readonly IWorkbench workbench;
		readonly IStatusBarService statusBarService;
		readonly IBuildOptions buildOptions;
		
		public TestExecutionManager()
		{
			this.buildProjectFactory = new UnitTestBuildProjectFactory();
			this.taskService = new UnitTestTaskService();
			this.saveAllFilesCommand = new UnitTestSaveAllFilesCommand();
			this.testService = SD.GetRequiredService<ITestService>();
			this.workbench = SD.Workbench;
			this.statusBarService = SD.StatusBar;
			this.buildOptions = new UnitTestBuildOptions();
		}
		
		readonly MultiDictionary<ITestProject, ITest> testsByProject = new MultiDictionary<ITestProject, ITest>();
		CancellationToken cancellationToken;
		
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
			var projectsToBuild = testsByProject.Keys.Select(p => p.GetBuildableForTesting()).Where(b => b != null).ToList();
			if (projectsToBuild.Count > 0) {
				var buildCommand = buildProjectFactory.CreateBuildProjectBeforeTestRun(projectsToBuild);
				var buildResults = await buildCommand.BuildAsync(cancellationToken);
				if (buildResults.Result != BuildResultCode.Success)
					return;
			}
			
			cancellationToken.ThrowIfCancellationRequested();
			using (IProgressMonitor progressMonitor = statusBarService.CreateProgressMonitor(cancellationToken)) {
				int projectsLeftToRun = testsByProject.Count;
				foreach (IGrouping<ITestProject, ITest> g in testsByProject) {
					ITestProject project = g.Key;
					progressMonitor.TaskName = GetProgressMonitorLabel(project);
					progressMonitor.Progress = GetProgress(projectsLeftToRun);
					using (IProgressMonitor nested = progressMonitor.CreateSubTask(1.0 / testsByProject.Count)) {
						await project.RunTestsAsync(g, options, nested);
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
			taskService.BuildMessageViewCategory.ClearText();
			taskService.ClearExceptCommentTasks();
			testService.UnitTestMessageView.ClearText();
		}
		
		void ShowUnitTestsPad()
		{
			workbench.GetPad(typeof(UnitTestsPad)).BringPadToFront();
		}
		
		void ShowOutputPad()
		{
			workbench.GetPad(typeof(CompilerMessageView)).BringPadToFront();
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
		
		void ShowErrorList()
		{
			if (taskService.SomethingWentWrong && buildOptions.ShowErrorListAfterBuild) {
				workbench.GetPad(typeof(ErrorListPad)).BringPadToFront();
			}
		}
	}
}
