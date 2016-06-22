// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
			TaskService.ClearExceptCommentTasks();
			ICSharpCode.SharpDevelop.Commands.SaveAllFiles.SaveAll();
		}
		
		public virtual void AfterBuild() {}
		
		public override void Run()
		{
			if (CanRunBuild) {
				if (SD.Debugger.IsDebuggerLoaded && SD.Debugger.IsDebugging) {
					if (MessageService.AskQuestion("${res:XML.MainMenu.RunMenu.Compile.StopDebuggingQuestion}",
					                               "${res:XML.MainMenu.RunMenu.Compile.StopDebuggingTitle}"))
					{
						SD.Debugger.Stop();
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
			TaskService.Add(new SDTask(null, StringParser.Parse("${res:BackendBindings.ExecutionManager.NoSingleFileCompilation}"), 0, 0, TaskType.Error));
			SD.Workbench.GetPad(typeof(ErrorListPad)).BringPadToFront();
		}
	}
	
	public class Build : AbstractBuildMenuCommand
	{
		public override async void StartBuild()
		{
			CallbackMethod(await SD.BuildService.BuildAsync(ProjectService.OpenSolution, new BuildOptions(BuildTarget.Build)));
		}
	}
	
	public class BuildBeforeExecute : Build
	{
		public override void Run()
		{
			if (BuildOptions.BuildOnExecute == BuildDetection.DoNotBuild) {
				LastBuildResults = new BuildResults { Result = BuildResultCode.Success };
				OnBuildComplete(EventArgs.Empty);
			} else {
				base.Run();
			}
		}
		
		public override async void StartBuild()
		{
			var options = new BuildOptions(BuildTarget.Build) { BuildDetection = BuildOptions.BuildOnExecute };
			CallbackMethod(await SD.BuildService.BuildAsync(ProjectService.OpenSolution, options));
		}
	}
	
	public class BuildProjectBeforeExecute : BuildProject
	{
		public BuildProjectBeforeExecute(IProject project) : base(project)
		{
		}
		
		public override void Run()
		{
			if (BuildOptions.BuildOnExecute == BuildDetection.DoNotBuild) {
				LastBuildResults = new BuildResults { Result = BuildResultCode.Success };
				OnBuildComplete(EventArgs.Empty);
			} else {
				base.Run();
			}
		}
		
		public override async void StartBuild()
		{
			var options = new BuildOptions(BuildTarget.Build) { BuildDetection = BuildOptions.BuildOnExecute };
			CallbackMethod(await SD.BuildService.BuildAsync(this.ProjectToBuild, options));
		}
	}
	
	public class Rebuild : Build
	{
		public override async void StartBuild()
		{
			CallbackMethod(await SD.BuildService.BuildAsync(ProjectService.OpenSolution, new BuildOptions(BuildTarget.Rebuild)));
		}
	}
	
	public class Clean : AbstractBuildMenuCommand
	{
		public override async void StartBuild()
		{
			CallbackMethod(await SD.BuildService.BuildAsync(ProjectService.OpenSolution, new BuildOptions(BuildTarget.Clean)));
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
		
		public override async void StartBuild()
		{
			CallbackMethod(await SD.BuildService.BuildAsync(this.ProjectToBuild, new BuildOptions(BuildTarget.Build)));
		}
	}
	
	public class RebuildProject : BuildProject
	{
		public RebuildProject() {}
		public RebuildProject(IProject targetProject) : base(targetProject) {}
		
		public override async void StartBuild()
		{
			CallbackMethod(await SD.BuildService.BuildAsync(this.ProjectToBuild, new BuildOptions(BuildTarget.Rebuild)));
		}
	}
	
	public class CleanProject : AbstractProjectBuildMenuCommand
	{
		public override async void StartBuild()
		{
			CallbackMethod(await SD.BuildService.BuildAsync(this.ProjectToBuild, new BuildOptions(BuildTarget.Clean)));
		}
	}
	
	public class AbortBuild : AbstractMenuCommand
	{
		public override void Run()
		{
			SD.BuildService.CancelBuild();
		}
		
		public override bool IsEnabled {
			get { return SD.BuildService.IsBuilding; }
			set { }
		}
	}
	
	public class SetConfigurationMenuBuilder : IMenuItemBuilder
	{
		public IEnumerable<object> BuildItems(Codon codon, object owner)
		{
			if (ProjectService.OpenSolution == null)
				return new MenuItem[0];
			var configurationNames = ProjectService.OpenSolution.ConfigurationNames.ToList();
			string activeConfiguration = ProjectService.OpenSolution.ActiveConfiguration.Configuration;
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
		
		static void SetConfigurationItemClick(object sender, EventArgs e)
		{
			MenuItem item = (MenuItem)sender;
			ISolution solution = ProjectService.OpenSolution;
			solution.ActiveConfiguration = new ConfigurationAndPlatform((string)item.Header, solution.ActiveConfiguration.Platform);
		}
	}
	
	public class SetPlatformMenuBuilder : IMenuItemBuilder
	{
		public IEnumerable<object> BuildItems(Codon codon, object owner)
		{
			if (ProjectService.OpenSolution == null)
				return new MenuItem[0];
			IList<string> platformNames = ProjectService.OpenSolution.PlatformNames.ToList();
			string activePlatform = ProjectService.OpenSolution.ActiveConfiguration.Platform;
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
		
		static void SetPlatformItemClick(object sender, EventArgs e)
		{
			MenuItem item = (MenuItem)sender;
			ISolution solution = ProjectService.OpenSolution;
			solution.ActiveConfiguration = new ConfigurationAndPlatform(solution.ActiveConfiguration.Configuration, (string)item.Header);
		}
	}
	
	public class EditConfigurationsCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			if (SD.ProjectService.CurrentSolution != null)
				SD.UIService.ShowSolutionConfigurationEditorDialog(SD.ProjectService.CurrentSolution);
		}
	}
}
