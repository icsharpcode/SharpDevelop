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
				StartBuild();
			} else {
				MSBuildEngine.AddNoSingleFileCompilationError();
			}
		}
		
		protected void CallbackMethod(CompilerResults results)
		{
			MSBuildEngine.ShowResults(results);
			AfterBuild();
			if (BuildComplete != null)
				BuildComplete(this, EventArgs.Empty);
		}
		
		public abstract void StartBuild();
		
		public event EventHandler BuildComplete;
	}
	
	public class Build : AbstractBuildMenuCommand
	{
		public override void StartBuild()
		{
			ProjectService.OpenSolution.Build(CallbackMethod);
		}
		
		public override void AfterBuild()
		{
			ProjectService.OnEndBuild();
		}
	}
	
	public class Rebuild : Build
	{
		public override void StartBuild()
		{
			ProjectService.OpenSolution.Rebuild(CallbackMethod);
		}
	}
	
	public class Clean : AbstractBuildMenuCommand
	{
		public override void StartBuild()
		{
			ProjectService.OpenSolution.Clean(CallbackMethod);
		}
	}
	
	public class Publish : AbstractBuildMenuCommand
	{
		public override void StartBuild()
		{
			ProjectService.OpenSolution.Publish(CallbackMethod);
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
		public override void StartBuild()
		{
			ProjectService.CurrentProject.Build(CallbackMethod);
		}
		
		public override void AfterBuild()
		{
			ProjectService.OnEndBuild();
		}
	}
	
	public class RebuildProject : BuildProject
	{
		public override void StartBuild()
		{
			ProjectService.CurrentProject.Rebuild(CallbackMethod);
		}
	}
	
	public class CleanProject : AbstractProjectBuildMenuCommand
	{
		public override void StartBuild()
		{
			ProjectService.CurrentProject.Clean(CallbackMethod);
		}
	}
	
	public class PublishProject : AbstractProjectBuildMenuCommand
	{
		public override void StartBuild()
		{
			ProjectService.CurrentProject.Publish(CallbackMethod);
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
