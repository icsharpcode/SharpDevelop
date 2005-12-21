// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
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
	public abstract class AbstractBuildMenuCommand : AbstractMenuCommand
	{
		public virtual bool CanRunBuild {
			get {
				return ProjectService.OpenSolution!=null;
			}
		}
		public virtual void BeforeBuild()
		{
			TaskService.BuildMessageViewCategory.ClearText();
			TaskService.InUpdate = true;
			TaskService.ClearExceptCommentTasks();
			TaskService.InUpdate = false;
			ICSharpCode.SharpDevelop.Commands.SaveAllFiles.SaveAll();
		}
		
		public virtual void AfterBuild() {}
		
		public override void Run()
		{
			if (CanRunBuild) {
				BeforeBuild();
				RunBuild();
				AfterBuild();
			} else {
				MSBuildEngine.AddNoSingleFileCompilationError();
			}
		}

		public abstract void RunBuild();
	}
	
	public class Build : AbstractBuildMenuCommand
	{
		public override void RunBuild()
		{
			MSBuildEngine.ShowResults(ProjectService.OpenSolution.Build());
		}
		
		public override void AfterBuild()
		{
			ProjectService.OnEndBuild();
		}
	}
	
	public class Rebuild : Build
	{
		public override void RunBuild()
		{
			MSBuildEngine.ShowResults(ProjectService.OpenSolution.Rebuild());
		}
	}
	
	public class Clean : AbstractBuildMenuCommand
	{
		public override void RunBuild()
		{
			MSBuildEngine.ShowResults(ProjectService.OpenSolution.Clean());
		}
	}
	
	public class Publish : AbstractBuildMenuCommand
	{
		public override void RunBuild()
		{
			MSBuildEngine.ShowResults(ProjectService.OpenSolution.Publish());
		}
	}
	
	public abstract class AbstractProjectBuildMenuCommand : AbstractBuildMenuCommand
	{
		public override bool CanRunBuild {
			get {
				return base.CanRunBuild 
					&& ProjectService.CurrentProject!=null;
			}
		}
	}
	public class BuildProject : AbstractProjectBuildMenuCommand
	{
		public override void RunBuild()
		{
			MSBuildEngine.ShowResults(ProjectService.CurrentProject.Build());
		}
		
		public override void AfterBuild()
		{
			ProjectService.OnEndBuild();
		}
	}
	
	public class RebuildProject : BuildProject
	{
		public override void RunBuild()
		{
			MSBuildEngine.ShowResults(ProjectService.CurrentProject.Rebuild());
		}
	}
	
	public class CleanProject : AbstractProjectBuildMenuCommand
	{
		public override void RunBuild()
		{
			MSBuildEngine.ShowResults(ProjectService.CurrentProject.Clean());
		}
	}
	
	public class PublishProject : AbstractProjectBuildMenuCommand
	{
		public override void RunBuild()
		{
			MSBuildEngine.ShowResults(ProjectService.CurrentProject.Publish());
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
			ProjectService.OpenSolution.ApplySolutionConfigurationToProjects();
		}
	}
}
