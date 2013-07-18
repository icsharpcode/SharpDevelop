// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ICSharpCode.TreeView;

namespace ICSharpCode.SharpDevelop.Dom.ClassBrowser
{
	/// <summary>
	/// Description of WorkspaceTreeNode.
	/// </summary>
	public class WorkspaceTreeNode : ModelCollectionTreeNode
	{
		WorkspaceModel workspace;
		
		public IMutableModelCollection<SharpTreeNode> SpecialNodes {
			get { return workspace.SpecialNodes; }
		}

		public AssemblyList AssemblyList {
			get { return workspace.AssemblyList; }
			set { workspace.AssemblyList = value; }
		}
		
		public WorkspaceTreeNode()
		{
			this.workspace = new WorkspaceModel();
			this.workspace.SpecialNodes.CollectionChanged += SpecialNodesModelCollectionChanged;
		}
		
		protected override object GetModel()
		{
			return workspace;
		}
		
		protected override IModelCollection<object> ModelChildren {
			get { return workspace.AssemblyList.Assemblies; }
		}
		
		protected override IComparer<SharpTreeNode> NodeComparer {
			get { return NodeTextComparer; }
		}
		
		public override object Text {
			get {
				return "Workspace " + AssemblyList.Name;
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
			Children.AddRange(workspace.SpecialNodes);
		}
		
		void SpecialNodesModelCollectionChanged(IReadOnlyCollection<SharpTreeNode> removedItems, IReadOnlyCollection<SharpTreeNode> addedItems)
		{
			SynchronizeModelChildren();
		}
	}
}
