// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Debugging;

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
				if (DebuggerService.IsDebuggerLoaded && DebuggerService.CurrentDebugger.IsDebugging) {
					if (MessageService.AskQuestion("${res:XML.MainMenu.RunMenu.Compile.StopDebuggingQuestion}",
					                               "${res:XML.MainMenu.RunMenu.Compile.StopDebuggingTitle}"))
					{
						DebuggerService.CurrentDebugger.Stop();
					} else {
						return;
					}
				}
				BeforeBuild();
				StartBuild();
			} else {
				MSBuildEngine.AddNoSingleFileCompilationError();
			}
		}
		
		protected void CallbackMethod(BuildResults results)
		{
			MSBuildEngine.ShowResults(results);
			AfterBuild();
			if (BuildComplete != null)
				BuildComplete(this, EventArgs.Empty);
		}
		
		public abstract void StartBuild();
		
		public event EventHandler BuildComplete;
	}
	
	public sealed class Build : AbstractBuildMenuCommand
	{
		public override void StartBuild()
		{
			ProjectService.RaiseEventStartBuild();
			ProjectService.OpenSolution.Build(CallbackMethod);
		}
		
		public override void AfterBuild()
		{
			ProjectService.RaiseEventEndBuild();
		}
	}
	
	public sealed class Rebuild : AbstractBuildMenuCommand
	{
		public override void StartBuild()
		{
			ProjectService.RaiseEventStartBuild();
			ProjectService.OpenSolution.Rebuild(CallbackMethod);
		}
		
		public override void AfterBuild()
		{
			ProjectService.RaiseEventEndBuild();
		}
	}
	
	public sealed class Clean : AbstractBuildMenuCommand
	{
		public override void StartBuild()
		{
			ProjectService.OpenSolution.Clean(CallbackMethod);
		}
	}
	
	public sealed class Publish : AbstractBuildMenuCommand
	{
		public override void StartBuild()
		{
			ProjectService.OpenSolution.Publish(CallbackMethod);
		}
	}
	
	public abstract class AbstractProjectBuildMenuCommand : AbstractBuildMenuCommand
	{
		protected IProject targetProject;
		protected IProject ProjectToBuild {
			get {
				return targetProject ?? ProjectService.CurrentProject;
			}
		}
		
		public override bool CanRunBuild {
			get {
				return base.CanRunBuild && this.ProjectToBuild != null;
			}
		}
	}
	public class BuildProject : AbstractProjectBuildMenuCommand
	{
		IDictionary<string, string> additionalProperties = new SortedList<string, string>();
		
		public IDictionary<string, string> AdditionalProperties {
			get {
				return additionalProperties;
			}
		}
		
		public BuildProject()
		{
		}
		public BuildProject(IProject targetProject)
		{
			this.targetProject = targetProject;
		}
		
		public override void StartBuild()
		{
			ProjectService.RaiseEventStartBuild();
			this.ProjectToBuild.Build(CallbackMethod, AdditionalProperties);
		}
		
		public override void AfterBuild()
		{
			ProjectService.RaiseEventEndBuild();
		}
	}
	
	public sealed class RebuildProject : BuildProject
	{
		public RebuildProject() {}
		public RebuildProject(IProject targetProject) : base(targetProject) {}
		
		public override void StartBuild()
		{
			ProjectService.RaiseEventStartBuild();
			this.ProjectToBuild.Rebuild(CallbackMethod, AdditionalProperties);
		}
	}
	
	public class CleanProject : AbstractProjectBuildMenuCommand
	{
		public override void StartBuild()
		{
			this.ProjectToBuild.Clean(CallbackMethod, null);
		}
	}
	
	public class PublishProject : AbstractProjectBuildMenuCommand
	{
		public override void StartBuild()
		{
			this.ProjectToBuild.Publish(CallbackMethod, null);
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
			ProjectService.OpenSolution.ApplySolutionConfigurationAndPlatformToProjects();
		}
	}
	
	public class EditConfigurationsCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			using (SolutionConfigurationEditor sce = new SolutionConfigurationEditor()) {
				sce.ShowDialog();
				ProjectService.OpenSolution.Save();
				ProjectService.OpenSolution.ApplySolutionConfigurationAndPlatformToProjects();
			}
		}
	}
}
