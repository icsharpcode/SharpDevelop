// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
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
			throw new System.NotImplementedException();
//			ProjectBrowserView browser = Owner as ProjectBrowserView;
//			AbstractBrowserNode node   = null;
//			
//			if (browser != null) {
//				node = browser.SelectedNode as FolderNode;
//			} else {
//				ICSharpCode.SharpDevelop.Gui.ProjectBrowser.ProjectBrowserView pbv = (ICSharpCode.SharpDevelop.Gui.ProjectBrowser.ProjectBrowserView)WorkbenchSingleton.Workbench.GetPad(typeof(ICSharpCode.SharpDevelop.Gui.ProjectBrowser.ProjectBrowserView));
//				
//				node = pbv.GetNodeFromProject(ProjectService.CurrentProject);
//			}
//			
//			if (node != null) {
//				Combine combine                = node.Combine;
//				combine.SingleStartProjectName = node.Project.Name;
//				combine.SingleStartupProject   = true;
//				
//				ProjectService.SaveCombine();
//			}
		}
	}
}
