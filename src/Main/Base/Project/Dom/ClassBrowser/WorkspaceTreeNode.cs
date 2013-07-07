// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.TreeView;

namespace ICSharpCode.SharpDevelop.Dom.ClassBrowser
{
	/// <summary>
	/// Description of WorkspaceTreeNode.
	/// </summary>
	public class WorkspaceTreeNode : ModelCollectionTreeNode
	{
		ClassBrowserWorkspace workspace;
		
		public WorkspaceTreeNode(ClassBrowserWorkspace workspace)
		{
			if (workspace == null)
				throw new ArgumentNullException("workspace");
			this.workspace = workspace;
		}
		
		protected override object GetModel()
		{
			return workspace;
		}
		
		protected override IModelCollection<object> ModelChildren {
			get { return workspace.LoadedAssemblies; }
		}
		
		protected override IComparer<SharpTreeNode> NodeComparer {
			get { return NodeTextComparer; }
		}
		
		public override object Text {
			get {
				return "Workspace " + workspace.Name;
			}
		}
		
		public override object Icon {
			get {
				return SD.ResourceService.GetImageSource("PadIcons.ClassBrowser");
			}
		}
		
		protected override bool IsSpecialNode()
		{
			return true;
		}
		
		protected override void InsertSpecialNodes()
		{
			if (workspace.IsAssigned) {
				InsertChildren(new[] { workspace.AssignedSolution });
			}
		}
	}
}
