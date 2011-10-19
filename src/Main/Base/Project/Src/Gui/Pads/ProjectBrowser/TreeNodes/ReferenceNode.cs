// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace ICSharpCode.SharpDevelop.Project
{
	public class ReferenceNode : AbstractProjectBrowserTreeNode
	{
		ReferenceProjectItem referenceProjectItem;
		
		public ReferenceProjectItem ReferenceProjectItem {
			get {
				return referenceProjectItem;
			}
		}
		
		public ReferenceNode(ReferenceProjectItem referenceProjectItem)
		{
			this.referenceProjectItem = referenceProjectItem;
			Tag = referenceProjectItem;
			
			ContextmenuAddinTreePath = "/SharpDevelop/Pads/ProjectBrowser/ContextMenu/ReferenceNode";
			SetIcon("Icons.16x16.Reference");
			Text = referenceProjectItem.ShortName;
		}
		
		#region Cut & Paste
		public override bool EnableDelete {
			get {
				return !Project.ReadOnly;
			}
		}
		
		public override void Delete()
		{
			IProject project = Project;
			ProjectService.RemoveProjectItem(referenceProjectItem.Project, referenceProjectItem);
			Debug.Assert(Parent != null);
			Debug.Assert(Parent is ReferenceFolder);
			((ReferenceFolder)Parent).ShowReferences();
			project.Save();
		}
		#endregion
		
		public override object AcceptVisitor(ProjectBrowserTreeNodeVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
	}
}
