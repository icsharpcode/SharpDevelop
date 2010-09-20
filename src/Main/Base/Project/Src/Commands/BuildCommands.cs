// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Gui;
using System.Windows.Input;

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
				AddNoSingleFileCompilationError();
			}
		}
		
		BuildResults lastBuildResults;
		
		public BuildResults LastBuildResults {
			get { return lastBuildResults; }
			protected set { lastBuildResults = value; }
		}
		
		protected void CallbackMethod(BuildResults results)
		{
			lastBuildResults = results;
			AfterBuild();
			OnBuildComplete(EventArgs.Empty);
		}
		
		public abstract void StartBuild();
		
		public event EventHandler BuildComplete;
		
		protected virtual void OnBuildComplete(EventArgs e)
		{
			if (BuildComplete != null) {
				BuildComplete(this, e);
			}
		}
		
		/// <summary>
		/// Notifies the user that #develp's internal MSBuildEngine
		/// implementation only supports compiling solutions and projects;
		/// it does not allow compiling individual files.
		/// </summary>
		/// <remarks>Adds a message to the <see cref="TaskService"/> and
		/// shows the <see cref="ErrorListPad"/>.</remarks>
		public static void AddNoSingleFileCompilationError()
		{
			TaskService.Add(new Task(null, StringParser.Parse("${res:BackendBindings.ExecutionManager.NoSingleFileCompilation}"), 0, 0, TaskType.Error));
			WorkbenchSingleton.Workbench.GetPad(typeof(ErrorListPad)).BringPadToFront();
		}
	}
	
	public class Build : AbstractBuildMenuCommand
	{
		public override void StartBuild()
		{
			BuildEngine.BuildInGui(ProjectService.OpenSolution, new BuildOptions(BuildTarget.Build, CallbackMethod));
		}
	}
	
	public class BuildBeforeExecute : Build
	{
		public override void Run()
		{
			if (BuildModifiedProjectsOnlyService.Setting == BuildOnExecuteSetting.DoNotBuild) {
				LastBuildResults = new BuildResults { Result = BuildResultCode.Success };
				OnBuildComplete(EventArgs.Empty);
			} else {
				base.Run();
			}
		}
		
		public override void StartBuild()
		{
			BuildEngine.BuildInGui(BuildModifiedProjectsOnlyService.WrapBuildable(ProjectService.OpenSolution),
			                       new BuildOptions(BuildTarget.Build, CallbackMethod));
		}
	}
	
	public class BuildProjectBeforeExecute : BuildProject
	{
		public BuildProjectBeforeExecute(IBuildable project) : base(project)
		{
		}
		
		public override void Run()
		{
			if (BuildModifiedProjectsOnlyService.Setting == BuildOnExecuteSetting.DoNotBuild) {
				LastBuildResults = new BuildResults { Result = BuildResultCode.Success };
				OnBuildComplete(EventArgs.Empty);
			} else {
				base.Run();
			}
		}
		
		public override void StartBuild()
		{
			BuildEngine.BuildInGui(BuildModifiedProjectsOnlyService.WrapBuildable(this.ProjectToBuild),
			                       new BuildOptions(BuildTarget.Build, CallbackMethod));
		}
	}
	
	public class Rebuild : Build
	{
		public override void StartBuild()
		{
			BuildEngine.BuildInGui(ProjectService.OpenSolution, new BuildOptions(BuildTarget.Rebuild, CallbackMethod));
		}
	}
	
	public class Clean : AbstractBuildMenuCommand
	{
		public override void StartBuild()
		{
			BuildEngine.BuildInGui(ProjectService.OpenSolution, new BuildOptions(BuildTarget.Clean, CallbackMethod));
		}
	}
	
	public abstract class AbstractProjectBuildMenuCommand : AbstractBuildMenuCommand
	{
		protected IBuildable targetProject;
		protected IBuildable ProjectToBuild {
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
		public BuildProject()
		{
		}
		public BuildProject(IBuildable targetProject)
		{
			this.targetProject = targetProject;
		}
		
		public override void StartBuild()
		{
			BuildEngine.BuildInGui(this.ProjectToBuild, new BuildOptions(BuildTarget.Build, CallbackMethod));
		}
	}
	
	public class RebuildProject : BuildProject
	{
		public RebuildProject() {}
		public RebuildProject(IBuildable targetProject) : base(targetProject) {}
		
		public override void StartBuild()
		{
			BuildEngine.BuildInGui(this.ProjectToBuild, new BuildOptions(BuildTarget.Rebuild, CallbackMethod));
		}
	}
	
	public class CleanProject : AbstractProjectBuildMenuCommand
	{
		public override void StartBuild()
		{
			BuildEngine.BuildInGui(this.ProjectToBuild, new BuildOptions(BuildTarget.Clean, CallbackMethod));
		}
	}
	
	public class AbortBuild : AbstractMenuCommand
	{
		public override void Run()
		{
			BuildEngine.CancelGuiBuild();
		}
		
		public override bool IsEnabled {
			get { return BuildEngine.IsGuiBuildRunning; }
			set { }
		}
	}
	
	public class SetConfigurationMenuBuilder : IMenuItemBuilder
	{
		public System.Collections.ICollection BuildItems(Codon codon, object owner)
		{
			if (ProjectService.OpenSolution == null)
				return new MenuItem[0];
			IList<string> configurationNames = ProjectService.OpenSolution.GetConfigurationNames();
			string activeConfiguration = ProjectService.OpenSolution.Preferences.ActiveConfiguration;
			MenuItem[] items = new MenuItem[configurationNames.Count];
			for (int i = 0; i < items.Length; i++) {
				items[i] = new MenuItem {
					Header = configurationNames[i],
					IsChecked = activeConfiguration == configurationNames[i]
				};
				items[i].Click += SetConfigurationItemClick;
			}
			return items;
		}
		
		void SetConfigurationItemClick(object sender, EventArgs e)
		{
			MenuItem item = (MenuItem)sender;
			ProjectService.OpenSolution.Preferences.ActiveConfiguration = (string)item.Header;
			ProjectService.OpenSolution.ApplySolutionConfigurationAndPlatformToProjects();
			ProjectBrowserPad.Instance.ProjectBrowserControl.RefreshView();
		}
	}
	
	public class SetPlatformMenuBuilder : IMenuItemBuilder
	{
		public System.Collections.ICollection BuildItems(Codon codon, object owner)
		{
			if (ProjectService.OpenSolution == null)
				return new MenuItem[0];
			IList<string> platformNames = ProjectService.OpenSolution.GetPlatformNames();
			string activePlatform = ProjectService.OpenSolution.Preferences.ActivePlatform;
			MenuItem[] items = new MenuItem[platformNames.Count];
			for (int i = 0; i < items.Length; i++) {
				items[i] = new MenuItem {
					Header = platformNames[i],
					IsChecked = activePlatform == platformNames[i]
				};
				items[i].Click += SetPlatformItemClick;
			}
			return items;
		}
		
		void SetPlatformItemClick(object sender, EventArgs e)
		{
			MenuItem item = (MenuItem)sender;
			ProjectService.OpenSolution.Preferences.ActivePlatform = (string)item.Header;
			ProjectService.OpenSolution.ApplySolutionConfigurationAndPlatformToProjects();
			ProjectBrowserPad.Instance.ProjectBrowserControl.RefreshView();
		}
	}
	
	public class EditConfigurationsCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			using (SolutionConfigurationEditor sce = new SolutionConfigurationEditor()) {
				sce.ShowDialog(WorkbenchSingleton.MainWin32Window);
				ProjectService.SaveSolution();
				ProjectService.OpenSolution.ApplySolutionConfigurationAndPlatformToProjects();
				ProjectBrowserPad.Instance.ProjectBrowserControl.RefreshView();
			}
		}
	}
}
