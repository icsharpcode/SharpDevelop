using System;
using System.Drawing;
using System.IO;
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
			Text = "References";
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
					referenceNode.AddTo(this);
				}
			}
		}
		protected override void Initialize()
		{
			ShowReferences();
		}
		
		public override object AcceptVisitor(ProjectBrowserTreeNodeVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
	}
}
