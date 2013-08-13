// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ICSharpCode.NRefactory.Utils;
using ICSharpCode.TreeView;

namespace ICSharpCode.SharpDevelop.Dom.ClassBrowser
{
	/// <summary>
	/// Description of WorkspaceTreeNode.
	/// </summary>
	public class WorkspaceTreeNode : ModelCollectionTreeNode
	{
		class WorkspaceChildComparer : IComparer<SharpTreeNode>
		{
			IComparer<string> stringComparer = StringComparer.OrdinalIgnoreCase;
			
			public int Compare(SharpTreeNode x, SharpTreeNode y)
			{
				// Solution node has precedence over other nodes
				if ((x is SolutionTreeNode) && !(y is SolutionTreeNode))
					return -1;
				if (!(x is SolutionTreeNode) && (y is SolutionTreeNode))
					return 1;
				
				// Both nodes are solutions or not solutions, compare their Text property
				return stringComparer.Compare(x.Text.ToString(), y.Text.ToString());
			}
		}
		
		WorkspaceModel workspace;
		protected static readonly IComparer<SharpTreeNode> ChildNodeComparer = new WorkspaceChildComparer();
		
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
			get { return ChildNodeComparer; }
		}
		
		public override object Text {
			get {
				return String.Format(SD.ResourceService.GetString("MainWindow.Windows.ClassBrowser.Workspace"), AssemblyList.Name);
			}
		}
		
		public override object Icon {
			get {
				return SD.ResourceService.GetImageSource("Icons.16x16.Workspace");
			}
		}
		
		protected override bool IsSpecialNode()
		{
			return true;
		}
		
		protected override void InsertSpecialNodes()
		{
			foreach (var node in workspace.SpecialNodes) {
				Children.OrderedInsert(node, ChildNodeComparer);
			}
		}
		
		void SpecialNodesModelCollectionChanged(IReadOnlyCollection<SharpTreeNode> removedItems, IReadOnlyCollection<SharpTreeNode> addedItems)
		{
			SynchronizeModelChildren();
		}
	}
}
