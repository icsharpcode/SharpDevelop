// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.CodeDom.Compiler;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core;


namespace ICSharpCode.SharpDevelop.Project.Commands
{
	public class Build : AbstractMenuCommand
	{
		public static void BeforeBuild()
		{
			TaskService.BuildMessageViewCategory.ClearText();
			TaskService.ClearExceptCommentTasks();
			ICSharpCode.SharpDevelop.Commands.SaveAllFiles.SaveAll();
		}
		
		public static int LastErrorCount;
		public static int LastWarningCount;
		
		public static void ShowResults(CompilerResults results)
		{
			if (results != null) {
				LastErrorCount = 0;
				LastWarningCount = 0;
				foreach (CompilerError error in results.Errors) {
					TaskService.Add(new Task(error));
					if (error.IsWarning)
						LastWarningCount++;
					else
						LastErrorCount++;
				}
				if (results.Errors.Count > 0) {
					WorkbenchSingleton.Workbench.GetPad(typeof(ErrorList)).BringPadToFront();
				}
			}
		}
		
		public static void AddNoSingleFileCompilationError()
		{
			LastErrorCount = 1;
			LastWarningCount = 0;
			TaskService.Add(new Task(null, StringParser.Parse("${res:BackendBindings.ExecutionManager.NoSingleFileCompilation}"), 0, 0, TaskType.Error));
			WorkbenchSingleton.Workbench.GetPad(typeof(ErrorList)).BringPadToFront();
		}
		
		public override void Run()
		{
			Build.BeforeBuild();
			if (ProjectService.OpenSolution == null) {
				Build.AddNoSingleFileCompilationError();
			} else {
				Build.ShowResults(ProjectService.OpenSolution.Build());
			}
			Build.AfterBuild();
		}
		
		public static void AfterBuild()
		{
			ProjectService.OnEndBuild();
		}
	}
	
	public class Rebuild : AbstractMenuCommand
	{
		public override void Run()
		{
			Build.BeforeBuild();
			if (ProjectService.OpenSolution == null) {
				Build.AddNoSingleFileCompilationError();
			} else {
				Build.ShowResults(ProjectService.OpenSolution.Rebuild());
			}
			Build.AfterBuild();
		}
	}
	
	public class Clean : AbstractMenuCommand
	{
		public override void Run()
		{
			Build.BeforeBuild();
			if (ProjectService.OpenSolution == null) {
				Build.AddNoSingleFileCompilationError();
			} else {
				Build.ShowResults(ProjectService.OpenSolution.Clean());
			}
		}
	}
	
	public class Publish : AbstractMenuCommand
	{
		public override void Run()
		{
			Build.BeforeBuild();
			if (ProjectService.OpenSolution == null) {
				Build.AddNoSingleFileCompilationError();
			} else {
				Build.ShowResults(ProjectService.OpenSolution.Publish());
			}
		}
	}
	
	public class BuildProject : AbstractMenuCommand
	{
		
		public static void ShowResults(CompilerResults results)
		{
			if (results != null) {
				foreach (CompilerError error in results.Errors) {
					TaskService.Add(new Task(error));
				}
				if (results.Errors.Count > 0) {
					WorkbenchSingleton.Workbench.GetPad(typeof(ErrorList)).BringPadToFront();
				}
			}
		}
		
		public override void Run()
		{
			Build.BeforeBuild();
			if (ProjectService.OpenSolution == null) {
				Build.AddNoSingleFileCompilationError();
			} else {
				BuildProject.ShowResults(ProjectService.CurrentProject.Build());
			}
			Build.AfterBuild();
		}
	}
	
	public class RebuildProject : AbstractMenuCommand
	{
		public override void Run()
		{
			Build.BeforeBuild();
			if (ProjectService.OpenSolution == null) {
				Build.AddNoSingleFileCompilationError();
			} else {
				BuildProject.ShowResults(ProjectService.CurrentProject.Rebuild());
			}
			Build.AfterBuild();
		}
	}
	
	public class CleanProject : AbstractMenuCommand
	{
		public override void Run()
		{
			Build.BeforeBuild();
			if (ProjectService.OpenSolution == null) {
				Build.AddNoSingleFileCompilationError();
			} else {
				BuildProject.ShowResults(ProjectService.CurrentProject.Clean());
			}
		}
	}
	
	public class PublishProject : AbstractMenuCommand
	{
		public override void Run()
		{
			Build.BeforeBuild();
			if (ProjectService.OpenSolution == null) {
				Build.AddNoSingleFileCompilationError();
			} else {
				BuildProject.ShowResults(ProjectService.CurrentProject.Publish());
			}
		}
	}
}
