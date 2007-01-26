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
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Util;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.CodeCoverage
{
	/// <summary>
	/// Menu command selected after right clicking a test method in the text editor
	/// to run tests with code coverage.
	/// </summary>
	public class RunTestWithCodeCoverageCommand : AbstractRunTestCommand
	{
		static MessageViewCategory category;
		NCoverRunner runner;
		string ncoverFileName;
		
		public RunTestWithCodeCoverageCommand()
		{
			runner = new NCoverRunner();
			runner.NCoverExited += new NCoverExitEventHandler(NCoverExited);
			runner.OutputLineReceived += new LineReceivedEventHandler(OutputLineReceived);
		}
		
		protected override void RunTests(UnitTestApplicationStartHelper helper)
		{
			SetNCoverRunnerProperties(ncoverFileName, helper);
			RunNCover();
		}
		
		protected override void OnStop()
		{
			runner.Stop();
		}
		
		/// <summary>
		/// Clears the code coverage results on display before running
		/// a series of tests.
		/// </summary>
		protected override void OnBeforeRunTests()
		{
			ncoverFileName = GetNCoverFileName();
			if (ncoverFileName != null) {
				WorkbenchSingleton.SafeThreadAsyncCall(Category.ClearText);
				WorkbenchSingleton.SafeThreadAsyncCall(CodeCoverageService.ClearResults);
			} else {
				using (CodeCoverageRunnerNotFoundForm form = new CodeCoverageRunnerNotFoundForm()) {
					form.ShowDialog();
				}
				Stop();
			}
		}
		
		/// <summary>
		/// Shows the code coverage results window only if there were no
		/// test failures.
		/// </summary>
		protected override void OnAfterRunTests()
		{
			if (!TaskService.HasCriticalErrors(false)) {
				ShowPad(WorkbenchSingleton.Workbench.GetPad(typeof(CodeCoveragePad)));
			}
		}
		
		/// <summary>
		/// Gets the message view output window.
		/// </summary>
		MessageViewCategory Category {
			get {
				if (category == null) {
					category = new MessageViewCategory(StringParser.Parse("${res:ICSharpCode.UnitTesting.CodeCoverage}"));
					CompilerMessageView cmv = (CompilerMessageView)WorkbenchSingleton.Workbench.GetPad(typeof(CompilerMessageView)).PadContent;
					cmv.AddCategory(category);
				}
				return category;
			}
		}
		
		bool FileNameExists(string fileName)
		{
			return fileName.Length > 0 && File.Exists(fileName);
		}
		
		void SetNCoverRunnerProperties(string ncoverFileName, UnitTestApplicationStartHelper helper)
		{
			string ncoverOutputDirectory = GetNCoverOutputDirectory(helper.Project);
			NCoverSettings settings = GetNCoverSettings(helper.Project);
					
			runner.NCoverFileName = ncoverFileName;
			runner.ProfiledApplicationCommand = UnitTestApplicationStartHelper.UnitTestConsoleApplication;
			runner.ProfiledApplicationCommandLineArguments = helper.GetArguments();
			runner.WorkingDirectory = Path.GetDirectoryName(helper.Assemblies[0]);
			runner.CoverageResultsFileName = Path.Combine(ncoverOutputDirectory, "Coverage.Xml");
			runner.LogFileName = Path.Combine(ncoverOutputDirectory, "Coverage.log");
			runner.AssemblyList = settings.AssemblyList;
			runner.ExcludedAttributesList = settings.ExcludedAttributesList;
		}
		
		void RunNCover()
		{
			// Remove existing coverage results file.
			if (File.Exists(runner.CoverageResultsFileName)) {
				File.Delete(runner.CoverageResultsFileName);
			}
			
			// Create NCover output directory.
			if (!Directory.Exists(Path.GetDirectoryName(runner.CoverageResultsFileName))) {
				Directory.CreateDirectory(Path.GetDirectoryName(runner.CoverageResultsFileName));
			}
			
			Category.AppendLine(StringParser.Parse("${res:ICSharpCode.CodeCoverage.RunningCodeCoverage}"));
			Category.AppendLine(runner.CommandLine);
			
			runner.Start();
		}
		
		/// <summary>
		/// Displays the output from NCover after it has exited.
		/// </summary>
		/// <param name="sender">The event source.</param>
		/// <param name="e">The NCover exit event arguments.</param>
		void NCoverExited(object sender, NCoverExitEventArgs e)
		{
			System.Diagnostics.Debug.Assert(e.Error.Length == 0);
			
			DisplayCoverageResults(runner.CoverageResultsFileName);
			WorkbenchSingleton.SafeThreadAsyncCall(TestsFinished);
		}
		
		void OutputLineReceived(object sender, LineReceivedEventArgs e)
		{
			Category.AppendLine(e.Line);
		}
		
		void DisplayCoverageResults(string fileName)
		{
			if (!File.Exists(fileName)) {
				Task task = new Task(String.Empty, String.Concat(StringParser.Parse("${res:ICSharpCode.CodeCoverage.NoCodeCoverageResultsGenerated}"), " ", fileName), 0, 0, TaskType.Error);
				WorkbenchSingleton.SafeThreadAsyncCall(TaskService.Add, task);
				return;
			}
			
			CodeCoverageResults results = new CodeCoverageResults(fileName);
			WorkbenchSingleton.SafeThreadAsyncCall(CodeCoverageService.ShowResults, results);
		}
		
		/// <summary>
		/// Returns the full path to the NCover console application if it
		/// exists.
		/// </summary>
		string GetNCoverFileName()
		{
			string ncoverFileName = CodeCoverageOptions.NCoverFileName;
			if (FileNameExists(ncoverFileName)) {
				return ncoverFileName;
			} else {
				ncoverFileName = GetDefaultNCoverFileName();
				if (FileNameExists(ncoverFileName)) {
					return ncoverFileName;
				}
			}
			return null;
		}
		
		/// <summary>
		/// Returns the default full path to the NCover console application.
		/// </summary>
		string GetDefaultNCoverFileName()
		{
			string programFilesPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
			return Path.Combine(programFilesPath, @"NCover\NCover.Console.exe");
		}
		
		string GetNCoverOutputDirectory(IProject project)
		{
			return Path.Combine(project.Directory, "NCover");
		}
		
		NCoverSettings GetNCoverSettings(IProject project)
		{
			string fileName = NCoverSettings.GetFileName(project);
			if (File.Exists(fileName)) {
				return new NCoverSettings(fileName);
			}
			return new NCoverSettings();
		}
	}
}
