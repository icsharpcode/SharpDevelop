// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

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
				node.Project.Start(true);
			} else {
				MessageService.ShowError("${res:BackendBindings.ExecutionManager.CantExecuteDLLError}");
			}
		}
	}
}
