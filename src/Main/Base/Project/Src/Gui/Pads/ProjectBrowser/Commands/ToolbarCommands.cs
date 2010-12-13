// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project.Commands
{
	public class ShowPropertiesForNode : AbstractMenuCommand
	{
		public override void Run()
		{
			AbstractProjectBrowserTreeNode node  = ProjectBrowserPad.Instance.SelectedNode;
			if (node == null) {
				return;
			}
			node.ShowProperties();
		}
	}
	
	public class ToggleShowAll : AbstractCheckableMenuCommand
	{
		public override bool IsChecked {
			get {
				return AbstractProjectBrowserTreeNode.ShowAll;
			}
			set {
				ProjectBrowserPad.Instance.ProjectBrowserControl.ShowAll = value;
			}
		}
	}
	
	public class RefreshProjectBrowser : AbstractMenuCommand
	{
		public override void Run()
		{
			ProjectBrowserPad.Instance.ProjectBrowserControl.RefreshView();
		}
	}
	
	public class CollapseAllProjectBrowser : AbstractMenuCommand
	{
		public override void Run()
		{
			ProjectBrowserPad.Instance.ProjectBrowserControl.ExpandOrCollapseAll(false);
		}
	}
}
