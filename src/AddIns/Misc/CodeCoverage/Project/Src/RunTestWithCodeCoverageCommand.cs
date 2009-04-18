// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Specialized;
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
		PartCoverRunner runner;
		
		public RunTestWithCodeCoverageCommand()
		{
			runner = new PartCoverRunner();
			runner.Exited += PartCoverExited;
			runner.OutputLineReceived += OutputLineReceived;
		}
		
		protected override void RunTests(UnitTestApplicationStartHelper helper)
		{
			SetPartCoverRunnerProperties(helper);
			RunPartCover();
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
			WorkbenchSingleton.SafeThreadAsyncCall(Category.ClearText);
			WorkbenchSingleton.SafeThreadAsyncCall(CodeCoverageService.ClearResults);
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
					MessageViewCategory.Create(ref category, "CodeCoverage", StringParser.Parse("${res:ICSharpCode.UnitTesting.CodeCoverage}"));
				}
				return category;
			}
		}
		
		void SetPartCoverRunnerProperties(UnitTestApplicationStartHelper helper)
		{
			string partCoverOutputDirectory = GetPartCoverOutputDirectory(helper.Project);
			PartCoverSettings settings = GetPartCoverSettings(helper.Project);
			
			// By default get the code coverage for everything if
			// no include or exclude regular expressions have been
			// set for this project. Note that the CodeCoverageResults
			// will ignore any type that has no source code available
			// for it even though the type may be in the Part Cover
			// results file.
			if (settings.Include.Count == 0) {
				settings.Include.Add("[*]*");
			}
			
			runner.PartCoverFileName = GetPartCoverFileName();
			runner.Target = helper.UnitTestApplication;
			runner.TargetArguments = helper.GetArguments();
			runner.TargetWorkingDirectory = Path.GetDirectoryName(helper.Assemblies[0]);
			runner.Output = Path.Combine(partCoverOutputDirectory, "Coverage.Xml");
			AddStringsToCollection(settings.Include, runner.Include);
			AddStringsToCollection(settings.Exclude, runner.Exclude);
		}
		
		void RunPartCover()
		{
			// Remove existing coverage results file.
			if (File.Exists(runner.Output)) {
				File.Delete(runner.Output);
			}
			
			// Create PartCover output directory.
			if (!Directory.Exists(Path.GetDirectoryName(runner.Output))) {
				Directory.CreateDirectory(Path.GetDirectoryName(runner.Output));
			}
			
			Category.AppendLine(StringParser.Parse("${res:ICSharpCode.CodeCoverage.RunningCodeCoverage}"));
			Category.AppendLine(runner.CommandLine);
			
			runner.Start();
		}
		
		/// <summary>
		/// Displays the output from PartCover after it has exited.
		/// </summary>
		/// <param name="sender">The event source.</param>
		/// <param name="e">The PartCover exit event arguments.</param>
		void PartCoverExited(object sender, PartCoverExitEventArgs e)
		{
			DisplayCoverageResults(runner.Output);
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
		/// Returns the full path to the PartCover console application if it
		/// exists.
		/// </summary>
		/// <remarks>
		/// Use Path.GetFullPath otherwise we end up with a filename path like:
		/// C:\Program Files\SharpDevelop\bin\..\bin\Tools\PartCover\PartCover.exe
		/// </remarks>
		string GetPartCoverFileName()
		{
			return Path.GetFullPath(Path.Combine(FileUtility.ApplicationRootPath, @"bin\Tools\PartCover\PartCover.exe"));
		}
		
		string GetPartCoverOutputDirectory(IProject project)
		{
			return Path.Combine(project.Directory, "PartCover");
		}
		
		PartCoverSettings GetPartCoverSettings(IProject project)
		{
			string fileName = PartCoverSettings.GetFileName(project);
			if (File.Exists(fileName)) {
				return new PartCoverSettings(fileName);
			}
			return new PartCoverSettings();
		}
		
		void AddStringsToCollection(StringCollection source, StringCollection target)
		{
			target.Clear();
			foreach (string item in source) {
				target.Add(item);
			}
		}
	}
}
