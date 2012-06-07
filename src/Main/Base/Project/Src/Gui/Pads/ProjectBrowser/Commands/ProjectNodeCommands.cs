// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;

namespace ICSharpCode.SharpDevelop.Project.Commands
{
	public class SetAsStartupProject : AbstractMenuCommand
	{
		public override void Run()
		{
			Solution solution = ProjectService.OpenSolution;
			IProject project = ProjectService.CurrentProject;
			if (solution == null || project == null)
				return;
			if (project.IsStartable) {
				solution.Preferences.StartupProject = project;
			} else {
				MessageService.ShowError("${res:BackendBindings.ExecutionManager.CantExecuteDLLError}");
			}
		}
	}
	
	public class RunProject : AbstractMenuCommand
	{
		public override void Run()
		{
			DoRunProject();
		}

		internal static void DoRunProject(bool withDebugging = true)
		{
			IProject project = ProjectService.CurrentProject;
			if (project == null)
				return;
			if (project.IsStartable) {
				BuildProjectBeforeExecute build = new BuildProjectBeforeExecute(project);
				build.BuildComplete += delegate {
					if (build.LastBuildResults.ErrorCount == 0) {
						project.Start(withDebugging);
					}
				};
				build.Run();
			} else {
				MessageService.ShowError("${res:BackendBindings.ExecutionManager.CantExecuteDLLError}");
			}
		}
	}
	
	public class RunProjectWithoutDebugger : AbstractMenuCommand
	{
		public override void Run()
		{
			RunProject.DoRunProject(false);
		}
	}
}
