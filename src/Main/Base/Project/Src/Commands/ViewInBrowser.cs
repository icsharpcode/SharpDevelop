// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Project.Commands;

namespace ICSharpCode.SharpDevelop.Project.Commands
{
	public class ViewInBrowser : AbstractMenuCommand
	{
		public override void Run()
		{
			var node  = ProjectBrowserPad.Instance.SelectedNode as FileNode;
			if (node == null) {
				return;
			}
			
			try {
				Process.Start(node.FileName);
			} catch (Exception ex) {
				MessageService.ShowMessage(ex.Message);
			}
		}
	}
}
