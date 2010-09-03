// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// Tree node displayed in project browser that represents a WixExtension project item. 
	/// </summary>
	public class WixExtensionNode : AbstractProjectBrowserTreeNode
	{
		WixExtensionProjectItem projectItem;
		
		public WixExtensionNode(WixExtensionProjectItem projectItem)
		{
			Text = Path.GetFileName(projectItem.Include);
			this.projectItem = projectItem;
			ContextmenuAddinTreePath = "/SharpDevelop/Pads/ProjectBrowser/ContextMenu/WixExtensionNode";
			SetIcon("Icons.16x16.Library");
		}
		
		public override object AcceptVisitor(ProjectBrowserTreeNodeVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override bool EnableDelete {
			get { return true; }
		}
		
		public override void Delete()
		{
			IProject project = projectItem.Project;
			ProjectService.RemoveProjectItem(project, projectItem);
			((ExtTreeNode)Parent).Refresh();
			project.Save();
		}
	}
}
