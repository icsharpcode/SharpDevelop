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
				
				// AssemblyTreeNodes (no derived node classes!) appear at the bottom of list
				if ((x.GetType() == typeof(AssemblyTreeNode)) && (y.GetType() != typeof(AssemblyTreeNode)))
					return 1;
				if ((x.GetType() != typeof(AssemblyTreeNode)) && (y.GetType() == typeof(AssemblyTreeNode)))
					return -1;
				
				// All other nodes are compared by their Text property
				return stringComparer.Compare(x.Text.ToString(), y.Text.ToString());
			}
		}

		protected static readonly IComparer<SharpTreeNode> ChildNodeComparer = new WorkspaceChildComparer();
		
		public WorkspaceTreeNode()
		{
			SD.ClassBrowser.CurrentWorkspace.AssemblyLists.CollectionChanged += AssemblyListsCollectionChanged;
		}
		
		protected override object GetModel()
		{
			return SD.ClassBrowser.CurrentWorkspace;
		}
		
		protected override IModelCollection<object> ModelChildren {
			get { return SD.ClassBrowser.MainAssemblyList.Assemblies.Concat(SD.ClassBrowser.UnpinnedAssemblies.Assemblies); }
		}
		
		protected override IComparer<SharpTreeNode> NodeComparer {
			get { return ChildNodeComparer; }
		}
		
		public override object Text {
			get {
				return String.Format(SD.ResourceService.GetString("MainWindow.Windows.ClassBrowser.Workspace"), SD.ClassBrowser.MainAssemblyList.Name);
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
			foreach (var assemblyList in SD.ClassBrowser.AssemblyLists) {
				var treeNode = SD.TreeNodeFactory.CreateTreeNode(assemblyList);
				if (treeNode != null)
					Children.OrderedInsert(treeNode, ChildNodeComparer);
			}
		}
		
		void AssemblyListsCollectionChanged(IReadOnlyCollection<IAssemblyList> removedItems, IReadOnlyCollection<IAssemblyList> addedItems)
		{
			SynchronizeModelChildren();
		}
	}
}
