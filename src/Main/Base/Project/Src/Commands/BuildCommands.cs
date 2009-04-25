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
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Gui;

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
			ShowResults(results);
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
		
		public static void ShowResults(BuildResults results)
		{
			if (results != null) {
				TaskService.InUpdate = true;
				foreach (BuildError error in results.Errors) {
					TaskService.Add(new Task(error));
				}
				TaskService.InUpdate = false;
				if (results.Errors.Count > 0 && ErrorListPad.ShowAfterBuild) {
					WorkbenchSingleton.Workbench.GetPad(typeof(ErrorListPad)).BringPadToFront();
				}
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
		public override void BeforeBuild()
		{
			base.BeforeBuild();
			ProjectService.RaiseEventStartBuild();
		}
		
		public override void StartBuild()
		{
			BuildEngine.BuildInGui(ProjectService.OpenSolution, new BuildOptions(BuildTarget.Build, CallbackMethod));
		}
		
		public override void AfterBuild()
		{
			ProjectService.RaiseEventEndBuild(new BuildEventArgs(LastBuildResults));
			base.AfterBuild();
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
		public BuildProjectBeforeExecute(IProject project) : base(project)
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
		public BuildProject()
		{
		}
		public BuildProject(IProject targetProject)
		{
			this.targetProject = targetProject;
		}
		
		public override void BeforeBuild()
		{
			base.BeforeBuild();
			ProjectService.RaiseEventStartBuild();
		}
		
		public override void StartBuild()
		{
			BuildEngine.BuildInGui(this.ProjectToBuild, new BuildOptions(BuildTarget.Build, CallbackMethod));
		}
		
		public override void AfterBuild()
		{
			ProjectService.RaiseEventEndBuild(new BuildEventArgs(LastBuildResults));
			base.AfterBuild();
		}
	}
	
	public class RebuildProject : BuildProject
	{
		public RebuildProject() {}
		public RebuildProject(IProject targetProject) : base(targetProject) {}
		
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
	
	public class AbortBuild : ISubmenuBuilder
	{
		public ToolStripItem[] BuildSubmenu(Codon codon, object owner)
		{
			return new[] { new MenuItem() };
		}
		
		sealed class MenuItem : ToolStripMenuItem
		{
			public MenuItem()
			{
				WorkbenchSingleton.Workbench.ProcessCommandKey += OnProcessCommandKey;
				ResourceService.LanguageChanged += OnLanguageChanged;
				OnLanguageChanged(null, null);
			}
			
			protected override void Dispose(bool disposing)
			{
				base.Dispose(disposing);
				if (disposing) {
					WorkbenchSingleton.Workbench.ProcessCommandKey -= OnProcessCommandKey;
					ResourceService.LanguageChanged -= OnLanguageChanged;
				}
			}
			
			public override bool Enabled {
				get { return BuildEngine.IsGuiBuildRunning; }
			}
			
			void OnLanguageChanged(object sender, EventArgs e)
			{
				Text = StringParser.Parse("${res:XML.MainMenu.BuildMenu.AbortBuild}");
				ShortcutKeyDisplayString = StringParser.Parse("${res:XML.MainMenu.BuildMenu.BreakKey}");
			}
			
			void OnProcessCommandKey(object sender, KeyEventArgs e)
			{
				// ToolStripMenuItem does not support Pause/Break as shortcut key, so we handle it manually
				if (e.KeyData == Keys.Pause) {
					if (Enabled) {
						LoggingService.Debug("BREAK was pressed, aborting build.");
						PerformClick();
						e.Handled = true;
					}
				}
			}
			
			protected override void OnClick(EventArgs e)
			{
				base.OnClick(e);
				BuildEngine.CancelGuiBuild();
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
			ProjectService.OpenSolution.ApplySolutionConfigurationAndPlatformToProjects();
			ProjectBrowserPad.Instance.ProjectBrowserControl.RefreshView();
		}
	}
	
	public class SetPlatformMenuBuilder : ISubmenuBuilder
	{
		public ToolStripItem[] BuildSubmenu(Codon codon, object owner)
		{
			if (ProjectService.OpenSolution == null)
				return new ToolStripItem[0];
			IList<string> platformNames = ProjectService.OpenSolution.GetPlatformNames();
			string activePlatform = ProjectService.OpenSolution.Preferences.ActivePlatform;
			ToolStripMenuItem[] items = new ToolStripMenuItem[platformNames.Count];
			for (int i = 0; i < items.Length; i++) {
				items[i] = new ToolStripMenuItem(platformNames[i]);
				items[i].Click += SetPlatformItemClick;
				items[i].Checked = activePlatform == platformNames[i];
			}
			return items;
		}
		
		void SetPlatformItemClick(object sender, EventArgs e)
		{
			ToolStripMenuItem item = (ToolStripMenuItem)sender;
			ProjectService.OpenSolution.Preferences.ActivePlatform = item.Text;
			ProjectService.OpenSolution.ApplySolutionConfigurationAndPlatformToProjects();
			ProjectBrowserPad.Instance.ProjectBrowserControl.RefreshView();
		}
	}
	
	public class EditConfigurationsCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			using (SolutionConfigurationEditor sce = new SolutionConfigurationEditor()) {
				sce.ShowDialog();
				ProjectService.SaveSolution();
				ProjectService.OpenSolution.ApplySolutionConfigurationAndPlatformToProjects();
				ProjectBrowserPad.Instance.ProjectBrowserControl.RefreshView();
			}
		}
	}
}
