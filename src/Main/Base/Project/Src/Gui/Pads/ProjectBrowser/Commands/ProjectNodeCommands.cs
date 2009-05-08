// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;

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
			AbstractProjectBrowserTreeNode node  = ProjectBrowserPad.Instance.SelectedNode;
			if (node == null) {
				return;
			}
			if (node.Project.IsStartable) {
				BuildProjectBeforeExecute build = new BuildProjectBeforeExecute(node.Project);
				build.BuildComplete += delegate {
					if (build.LastBuildResults.ErrorCount == 0) {
						node.Project.Start(true);
					}
				};
				build.Run();
			} else {
				MessageService.ShowError("${res:BackendBindings.ExecutionManager.CantExecuteDLLError}");
			}
		}
	}
}
