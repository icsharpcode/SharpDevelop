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
			AbstractProjectBrowserTreeNode node  = ProjectBrowserPad.Instance.SelectedNode;
			Solution solution = ProjectService.OpenSolution;
			if (node == null || solution == null) {
				return;
			}
			if (node.Project.IsStartable) {
				solution.Preferences.StartupProject = node.Project;
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
			AbstractProjectBrowserTreeNode node = ProjectBrowserPad.Instance.SelectedNode;
			if (node == null) {
				return;
			}
			if (node.Project.IsStartable) {
				BuildProjectBeforeExecute build = new BuildProjectBeforeExecute(node.Project);
				build.BuildComplete += delegate {
					if (build.LastBuildResults.ErrorCount == 0) {
						node.Project.Start(withDebugging);
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
