// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project
{
	public class ReferenceFolder : CustomFolderNode
	{
		IProject project;
		
		public ReferenceFolder(IProject project)
		{
			sortOrder = 0;
			
			this.project = project;

			ContextmenuAddinTreePath = "/SharpDevelop/Pads/ProjectBrowser/ContextMenu/ReferenceFolderNode";
			Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Commands.ProjectBrowser.ReferencesNodeText}");
			OpenedImage = "ProjectBrowser.ReferenceFolder.Open";
			ClosedImage = "ProjectBrowser.ReferenceFolder.Closed";
			
			foreach (ProjectItem item in project.Items) {
				if (item is ReferenceProjectItem) {
					new CustomNode().AddTo(this);
					break;
				}
			}
		}
		
		public void ShowReferences()
		{
			Nodes.Clear();
			
			foreach (ProjectItem item in project.Items) {
				if (item is ReferenceProjectItem) {
					ReferenceNode referenceNode = new ReferenceNode((ReferenceProjectItem)item);
					referenceNode.InsertSorted(this);
				}
			}
			UpdateIcon();
		}
		
		protected override void Initialize()
		{
			ShowReferences();
			base.Initialize();
		}
		
		public override object AcceptVisitor(ProjectBrowserTreeNodeVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
	}
}
