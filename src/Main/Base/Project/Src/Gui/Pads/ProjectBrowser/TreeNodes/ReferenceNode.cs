using System;
using System.IO;

namespace ICSharpCode.SharpDevelop.Project
{
	public class ReferenceNode : AbstractProjectBrowserTreeNode
	{
		ReferenceProjectItem referenceProjectItem;
		
		public ReferenceProjectItem ReferenceProjectItem {
			get {
				return ReferenceProjectItem;
			}
		}
		
		public ReferenceNode(ReferenceProjectItem referenceProjectItem)
		{
			this.referenceProjectItem = referenceProjectItem;
			Tag = referenceProjectItem;

			ContextmenuAddinTreePath = "/SharpDevelop/Pads/ProjectBrowser/ContextMenu/ReferenceNode";
			SetIcon("Icons.16x16.Reference");
			Text = referenceProjectItem.Include;
		}
		
		public override object AcceptVisitor(ProjectBrowserTreeNodeVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
	}
}
