// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Commands;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Project.Commands;
using ICSharpCode.SharpDevelop.Util;

namespace ICSharpCode.UnitTesting
{
	public abstract class AbstractRunTestCommand : AbstractMenuCommand
	{
		static MessageViewCategory testRunnerCategory;
		static AbstractRunTestCommand runningTestCommand;
		List<IProject> projects;
		IProject currentProject;
		TestResultsMonitor testResultsMonitor;
		
		public AbstractRunTestCommand()
		{
			testResultsMonitor = new TestResultsMonitor();
			testResultsMonitor.TestFinished += TestFinished;
		}
		
		/// <summary>
		/// Gets the running test command.
		/// </summary>
		public static AbstractRunTestCommand RunningTestCommand {
			get {
				return runningTestCommand;
			}
		}
		
		/// <summary>
		/// Gets whether a test is currently running.
		/// </summary>
		public static bool IsRunningTest {
			get {
				return runningTestCommand != null;
			}
		}
		
		public override void Run()
		{
			projects = new List<IProject>();
			
			IMember m = TestableCondition.GetMember(Owner);
			IClass c = (m != null) ? m.DeclaringType : TestableCondition.GetClass(Owner);
			IProject project = TestableCondition.GetProject(Owner);
			string namespaceFilter = TestableCondition.GetNamespace(Owner);
			
			if (project != null) {
				projects.Add(project);
			} else if (UnitTestsPad.Instance != null) {
				projects.AddRange(UnitTestsPad.Instance.TestTreeView.GetProjects());
			}
			
			if (projects.Count > 0) {
				runningTestCommand = this;
				try {
					BeforeRun();
					if (IsRunningTest) {
						currentProject = projects[0];
						Run(currentProject, namespaceFilter, c, m);
					}
				} catch {
					runningTestCommand = null;
					throw;
				}
			}
		}
		
		public static MessageViewCategory TestRunnerCategory {
			get {
				if (testRunnerCategory == null) {
					MessageViewCategory.Create(ref testRunnerCategory, "UnitTesting", "${res:ICSharpCode.NUnitPad.NUnitPadContent.PadName}");
				}
				return testRunnerCategory;
			}
		}
		
		/// <summary>
		/// Stops running the tests.
		/// </summary>
		public void Stop()
		{
			runningTestCommand = null;
			UpdateUnitTestsPadToolbar();
			
			projects.Clear();
			
			testResultsMonitor.Stop();
			StopMonitoring();

			OnStop();
		}
		
		/// <summary>
		/// Called before all tests are run. If multiple projects are
		/// to be tested this is called only once.
		/// </summary>
		protected virtual void OnBeforeRunTests()
		{
		}
		
		/// <summary>
		/// Called after all tests have been run even if there have
		/// been errors. If multiple projects are to be tested this is called only once.
		/// </summary>
		protected virtual void OnAfterRunTests()
		{
		}

		protected abstract void RunTests(UnitTestApplicationStartHelper helper);
		
		/// <summary>
		/// Called by derived classes when a single test run
		/// is finished.
		/// </summary>
		protected void TestsFinished()
		{
			WorkbenchSingleton.AssertMainThread();
			
			// Read the rest of the file just in case.
			testResultsMonitor.Stop();
			testResultsMonitor.Read();
			StopMonitoring();
			
			projects.Remove(currentProject);
			if (projects.Count > 0) {
				currentProject = projects[0];
				Run(currentProject, null, null, null);
			} else {
				runningTestCommand = null;
				UpdateUnitTestsPadToolbar();
				if (TaskService.SomethingWentWrong && ErrorListPad.ShowAfterBuild) {
					ShowErrorList();
				}
				OnAfterRunTests();
			}
		}
		
		/// <summary>
		/// Called by derived classes to show a single test result.
		/// </summary>
		protected void ShowResult(TestResult result)
		{
			if (result.IsFailure || result.IsIgnored) {
				TaskService.Add(CreateTask(result));
			}
			UpdateTestResult(result);
		}
		
		/// <summary>
		/// Called when the test run should be stopped.
		/// </summary>
		protected virtual void OnStop()
		{
		}
		
		/// <summary>
		/// Brings the specified pad to the front.
		/// </summary>
		protected void ShowPad(PadDescriptor padDescriptor)
		{
			if (padDescriptor != null) {
				WorkbenchSingleton.SafeThreadAsyncCall(padDescriptor.BringPadToFront);
			}
		}
		
		/// <summary>
		/// Runs the tests after building the project under test.
		/// </summary>
		void Run(IProject project, string namespaceFilter, IClass fixture, IMember test)
		{
			BuildProjectBeforeTestRun build = new BuildProjectBeforeTestRun(project);
			build.BuildComplete += delegate {
				OnBuildComplete(build.LastBuildResults, project, namespaceFilter, fixture, test);
			};
			build.Run();
		}
		
		void ShowUnitTestsPad()
		{
			ShowPad(WorkbenchSingleton.Workbench.GetPad(typeof(UnitTestsPad)));
		}
		
		void UpdateUnitTestsPadToolbar()
		{
			if (UnitTestsPad.Instance != null) {
				UnitTestsPad.Instance.UpdateToolbar();
			}
		}
		
		/// <summary>
		/// Sets the initial workbench state before starting
		/// a test run.
		/// </summary>
		void BeforeRun()
		{
			TaskService.BuildMessageViewCategory.ClearText();
			TaskService.InUpdate = true;
			TaskService.ClearExceptCommentTasks();
			TaskService.InUpdate = false;
			
			TestRunnerCategory.ClearText();
			
			ShowUnitTestsPad();
			ShowOutputPad();
			
			UpdateUnitTestsPadToolbar();
			ResetAllTestResults();
			
			OnBeforeRunTests();
		}
		
		/// <summary>
		/// Brings output pad to the front.
		/// </summary>
		void ShowOutputPad()
		{
			ShowPad(WorkbenchSingleton.Workbench.GetPad(typeof(CompilerMessageView)));
		}
		
		Task CreateTask(TestResult result)
		{
			TaskType taskType = TaskType.Warning;
			FileLineReference lineRef = null;
			string message = String.Empty;
			
			if (result.IsFailure) {
				taskType = TaskType.Error;
				lineRef = OutputTextLineParser.GetNUnitOutputFileLineReference(result.StackTrace, true);
				message = GetTestResultMessage(result, "${res:NUnitPad.NUnitPadContent.TestTreeView.TestFailedMessage}");
			} else if (result.IsIgnored) {
				message = GetTestResultMessage(result, "${res:NUnitPad.NUnitPadContent.TestTreeView.TestNotExecutedMessage}");
			}
			if (lineRef == null) {
				lineRef = FindTest(result.Name);
			}
			if (lineRef != null) {
				return new Task(FileName.Create(lineRef.FileName),
				                message, lineRef.Column, lineRef.Line, taskType);
			}
			return new Task(null, message, 0, 0, taskType);
		}
		
		/// <summary>
		/// Returns the test result message if there is on otherwise
		/// uses the string resource to create a message.
		/// </summary>
		string GetTestResultMessage(TestResult result, string stringResource)
		{
			if (result.Message.Length > 0) {
				return result.Message;
			}
			return StringParser.Parse(stringResource, new string[,] {{"TestCase", result.Name}});
		}
		
		/// <summary>
		/// Returns the location of the specified test method in the
		/// project being tested.
		/// </summary>
		FileLineReference FindTest(string methodName)
		{
			TestProject testProject = GetTestProject(currentProject);
			if (testProject != null) {
				TestMethod method = testProject.TestClasses.GetTestMethod(methodName);
				if (method != null) {
					MemberResolveResult resolveResult = new MemberResolveResult(null, null, method.Method);
					FilePosition filePos = resolveResult.GetDefinitionPosition();
					return new FileLineReference(filePos.FileName, filePos.Line, filePos.Column);
				}
			}
			return null;
		}
		
		void ShowErrorList()
		{
			ShowPad(WorkbenchSingleton.Workbench.GetPad(typeof(ErrorListPad)));
		}
		
		/// <summary>
		/// Runs the test for the project after a successful build.
		/// </summary>
		void OnBuildComplete(BuildResults results, IProject project, string namespaceFilter, IClass fixture, IMember test)
		{
			if (results.ErrorCount == 0 && IsRunningTest) {
				UnitTestApplicationStartHelper helper = new UnitTestApplicationStartHelper();
				
				UnitTestingOptions options = new UnitTestingOptions();
				helper.NoThread = options.NoThread;
				helper.NoLogo = options.NoLogo;
				helper.NoDots = options.NoDots;
				helper.Labels = options.Labels;
				helper.ShadowCopy = !options.NoShadow;
				
				if (options.CreateXmlOutputFile) {
					helper.XmlOutputFile = Path.Combine(Path.GetDirectoryName(project.OutputAssemblyFullPath), project.AssemblyName + "-TestResult.xml");
				}
				
				helper.Initialize(project, namespaceFilter, fixture, test);
				helper.Results = Path.GetTempFileName();
				
				ResetTestResults(project);

				testResultsMonitor.FileName = helper.Results;
				testResultsMonitor.Start();
				
				try {
					RunTests(helper);
				} catch {
					StopMonitoring();
					throw;
				}
			} else {
				if (IsRunningTest) {
					Stop();
				}
				if (TaskService.SomethingWentWrong && ErrorListPad.ShowAfterBuild) {
					ShowErrorList();
				}
			}
		}
		
		/// <summary>
		/// Clears the test results in the test tree view for the
		/// project currently being tested.
		/// </summary>
		void ResetTestResults(IProject project)
		{
			TestProject testProject = GetTestProject(project);
			if (testProject != null) {
				testProject.ResetTestResults();
			}
		}
		
		/// <summary>
		/// Clears the test results in the test tree view for all the
		/// displayed projects.
		/// </summary>
		void ResetAllTestResults()
		{
			if (UnitTestsPad.Instance != null) {
				UnitTestsPad.Instance.TestTreeView.ResetTestResults();
			}
		}
		
		/// <summary>
		/// Gets the TestProject associated with the specified project
		/// from the test tree view.
		/// </summary>
		TestProject GetTestProject(IProject project)
		{
			if (UnitTestsPad.Instance != null) {
				return UnitTestsPad.Instance.TestTreeView.GetTestProject(project);
			}
			return null;
		}
		
		/// <summary>
		/// Updates the test result in the test tree view.
		/// </summary>
		void UpdateTestResult(TestResult result)
		{
			TestProject testProject = GetTestProject(currentProject);
			if (testProject != null) {
				testProject.UpdateTestResult(result);
			}
		}
		
		void StopMonitoring()
		{
			try {
				File.Delete(testResultsMonitor.FileName);
			} catch { }
			
			testResultsMonitor.Dispose();
		}
		
		void TestFinished(object source, TestFinishedEventArgs e)
		{
			WorkbenchSingleton.SafeThreadAsyncCall(ShowResult, e.Result);
		}
	}
	
	/// <summary>
	/// Custom build command that makes sure errors and warnings
	/// are not cleared from the Errors list before every build since
	/// we may be running multiple tests after each other.
	/// </summary>
	public class BuildProjectBeforeTestRun : BuildProjectBeforeExecute
	{
		public BuildProjectBeforeTestRun(IProject targetProject)
			: base(targetProject)
		{
		}
		
		/// <summary>
		/// Before a build do not clear the tasks, just save any
		/// dirty files.
		/// </summary>
		public override void BeforeBuild()
		{
			SaveAllFiles.SaveAll();
		}
	}
	
	public class RunTestInPadCommand : AbstractRunTestCommand
	{
		ProcessRunner runner;
		
		public RunTestInPadCommand()
		{
			runner = new ProcessRunner();
			runner.LogStandardOutputAndError = false;
			runner.OutputLineReceived += OutputLineReceived;
			runner.ProcessExited += ProcessExited;
		}
		
		protected override void RunTests(UnitTestApplicationStartHelper helper)
		{
			TestRunnerCategory.AppendLine(helper.GetCommandLine());
			runner.Start(helper.UnitTestApplication, helper.GetArguments());
		}
		
		protected override void OnStop()
		{
			runner.Kill();
		}
		
		protected ProcessRunner GetProcessRunner()
		{
			return runner;
		}
		
		void OutputLineReceived(object source, LineReceivedEventArgs e)
		{
			TestRunnerCategory.AppendLine(e.Line);
		}
		
		void ProcessExited(object source, EventArgs e)
		{
			WorkbenchSingleton.SafeThreadAsyncCall(TestsFinished);
		}
		
		void TestFinished(object source, TestFinishedEventArgs e)
		{
			WorkbenchSingleton.SafeThreadAsyncCall(ShowResult, e.Result);
		}
	}
	
	public class RunTestWithDebuggerCommand : AbstractRunTestCommand
	{
		public override void Run()
		{
			if (DebuggerService.IsDebuggerLoaded && DebuggerService.CurrentDebugger.IsDebugging) {
				if (MessageService.AskQuestion("${res:XML.MainMenu.RunMenu.Compile.StopDebuggingQuestion}",
				                               "${res:XML.MainMenu.RunMenu.Compile.StopDebuggingTitle}"))
				{
					DebuggerService.CurrentDebugger.Stop();
					base.Run();
				}
			} else {
				base.Run();
			}
		}
		
		protected override void RunTests(UnitTestApplicationStartHelper helper)
		{
			bool running = false;
			
			try {
				TestRunnerCategory.AppendLine(helper.GetCommandLine());
				ProcessStartInfo startInfo = new ProcessStartInfo(helper.UnitTestApplication);
				startInfo.Arguments = helper.GetArguments();
				startInfo.WorkingDirectory = UnitTestApplicationStartHelper.UnitTestApplicationDirectory;
				DebuggerService.DebugStopped += DebuggerFinished;
				DebuggerService.CurrentDebugger.Start(startInfo);
				running = true;
			} finally {
				if (!running) {
					DebuggerService.DebugStopped -= DebuggerFinished;
				}
			}
		}
		
		protected override void OnStop()
		{
			if (DebuggerService.CurrentDebugger.IsDebugging) {
				DebuggerService.CurrentDebugger.Stop();
			}
		}
		
		void DebuggerFinished(object sender, EventArgs e)
		{
			DebuggerService.DebugStopped -= DebuggerFinished;
			WorkbenchSingleton.SafeThreadAsyncCall(TestsFinished);
		}
	}
		
	public class RunAllTestsInPadCommand : RunTestInPadCommand
	{
		public override void Run()
		{
			// To make sure all tests are run we set the Owner to null.
			Owner = null;
			base.Run();
		}
	}
	
	public class RunProjectTestsInPadCommand : RunTestInPadCommand, ITestTreeView
	{
		public override void Run()
		{
			Owner = this;
			base.Run();
		}
		
		public IMember SelectedMethod {
			get { return null; }
		}
		
		public IClass SelectedClass {
			get { return null; }
		}
		
		public IProject SelectedProject {
			get { return ProjectService.CurrentProject; }
		}
		
		public string SelectedNamespace {
			get { return null; }
		}
	}
}
