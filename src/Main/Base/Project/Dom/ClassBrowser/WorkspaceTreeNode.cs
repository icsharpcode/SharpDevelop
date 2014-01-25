// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
		
		IModelCollection<object> combinedModelChildren;

		protected static readonly IComparer<SharpTreeNode> ChildNodeComparer = new WorkspaceChildComparer();
		
		public WorkspaceTreeNode()
		{
			combinedModelChildren = SD.ClassBrowser.MainAssemblyList.Assemblies.Concat(SD.ClassBrowser.UnpinnedAssemblies.Assemblies);
			SD.ClassBrowser.CurrentWorkspace.AssemblyLists.CollectionChanged += AssemblyListsCollectionChanged;
		}
		
		protected override object GetModel()
		{
			return SD.ClassBrowser.CurrentWorkspace;
		}
		
		protected override IModelCollection<object> ModelChildren {
			get { return combinedModelChildren; }
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
