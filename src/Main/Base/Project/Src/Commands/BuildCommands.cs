// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.CodeDom.Compiler;
using System.Windows.Forms;
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
			TaskService.InUpdate = true;
			TaskService.ClearExceptCommentTasks();
			TaskService.InUpdate = false;
			ICSharpCode.SharpDevelop.Commands.SaveAllFiles.SaveAll();
		}
		
		public static int LastErrorCount;
		public static int LastWarningCount;
		
		public static void ShowResults(CompilerResults results)
		{
			if (results != null) {
				LastErrorCount = 0;
				LastWarningCount = 0;
				TaskService.InUpdate = true;
				foreach (CompilerError error in results.Errors) {
					TaskService.Add(new Task(error));
					if (error.IsWarning)
						LastWarningCount++;
					else
						LastErrorCount++;
				}
				TaskService.InUpdate = false;
				if (results.Errors.Count > 0) {
					WorkbenchSingleton.Workbench.GetPad(typeof(ErrorListPad)).BringPadToFront();
				}
			}
		}
		
		public static void AddNoSingleFileCompilationError()
		{
			LastErrorCount = 1;
			LastWarningCount = 0;
			TaskService.Add(new Task(null, StringParser.Parse("${res:BackendBindings.ExecutionManager.NoSingleFileCompilation}"), 0, 0, TaskType.Error));
			WorkbenchSingleton.Workbench.GetPad(typeof(ErrorListPad)).BringPadToFront();
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
		public override void Run()
		{
			Build.BeforeBuild();
			if (ProjectService.OpenSolution == null) {
				Build.AddNoSingleFileCompilationError();
			} else {
				Build.ShowResults(ProjectService.CurrentProject.Build());
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
				Build.ShowResults(ProjectService.CurrentProject.Rebuild());
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
				Build.ShowResults(ProjectService.CurrentProject.Clean());
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
				Build.ShowResults(ProjectService.CurrentProject.Publish());
			}
		}
	}
	
	public class SetConfigurationMenuBuilder : ISubmenuBuilder
	{
		public ToolStripItem[] BuildSubmenu(Codon codon, object owner)
		{
			if (ProjectService.OpenSolution == null)
				return new ToolStripItem[0];
			IList<string> configurationNames = ProjectService.OpenSolution.GetConfigurationNames();
			string activeConfiguration = ProjectService.OpenSolution.Preferences.ActiveConfiguration;
			ToolStripMenuItem[] items = new ToolStripMenuItem[configurationNames.Count];
			for (int i = 0; i < items.Length; i++) {
				items[i] = new ToolStripMenuItem(configurationNames[i]);
				items[i].Click += SetConfigurationItemClick;
				items[i].Checked = activeConfiguration == configurationNames[i];
			}
			return items;
		}
		
		void SetConfigurationItemClick(object sender, EventArgs e)
		{
			ToolStripMenuItem item = (ToolStripMenuItem)sender;
			ProjectService.OpenSolution.Preferences.ActiveConfiguration = item.Text;
		}
	}
}
