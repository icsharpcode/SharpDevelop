// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
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
		public static void ClearTasks()
		{
			TaskService.BuildMessageViewCategory.ClearText();
			TaskService.Clear();
		}
			
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
			if (ProjectService.OpenSolution != null) {
				Build.ClearTasks();
				new ICSharpCode.SharpDevelop.Commands.SaveAllFiles().Run();
				Build.ShowResults(ProjectService.OpenSolution.Build());
			}
		}
	}
	
	public class Rebuild : AbstractMenuCommand
	{
		public override void Run()
		{
			if (ProjectService.OpenSolution != null) {
				Build.ClearTasks();
				new ICSharpCode.SharpDevelop.Commands.SaveAllFiles().Run();
				Build.ShowResults(ProjectService.OpenSolution.Rebuild());
			}
		}
	}
	
	public class Clean : AbstractMenuCommand
	{
		public override void Run()
		{
			if (ProjectService.OpenSolution != null) {
				Build.ClearTasks();
				Build.ShowResults(ProjectService.OpenSolution.Clean());
			}
		}
	}
	
	public class Publish : AbstractMenuCommand
	{
		public override void Run()
		{
			if (ProjectService.OpenSolution != null) {
				Build.ClearTasks();
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
			if (ProjectService.CurrentProject != null) {
				Build.ClearTasks();
				new ICSharpCode.SharpDevelop.Commands.SaveAllFiles().Run();
				BuildProject.ShowResults(ProjectService.CurrentProject.Build());
			}
		}
	}
	
	public class RebuildProject : AbstractMenuCommand
	{
		public override void Run()
		{
			if (ProjectService.CurrentProject != null) {
				Build.ClearTasks();
				new ICSharpCode.SharpDevelop.Commands.SaveAllFiles().Run();
				BuildProject.ShowResults(ProjectService.CurrentProject.Rebuild());
			}
		}
	}
	
	public class CleanProject : AbstractMenuCommand
	{
		public override void Run()
		{
			if (ProjectService.CurrentProject != null) {
				Build.ClearTasks();
				BuildProject.ShowResults(ProjectService.CurrentProject.Clean());
			}
		}
	}
	
	public class PublishProject : AbstractMenuCommand
	{
		public override void Run()
		{
			if (ProjectService.CurrentProject != null) {
				Build.ClearTasks();
				BuildProject.ShowResults(ProjectService.CurrentProject.Publish());
			}
		}
	}
}
