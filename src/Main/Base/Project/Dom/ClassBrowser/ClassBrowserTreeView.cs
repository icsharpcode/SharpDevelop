// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows.Controls;
using ICSharpCode.TreeView;

namespace ICSharpCode.SharpDevelop.Dom.ClassBrowser
{
	public class ClassBrowserTreeView : SharpTreeView, IClassBrowserTreeView
	{
		#region IClassBrowser implementation

		public ICollection<IAssemblyList> AssemblyLists {
			get { return ((WorkspaceTreeNode)Root).AssemblyLists; }
		}

		public IAssemblyList MainAssemblyList {
			get { return ((WorkspaceTreeNode)Root).AssemblyList; }
			set { ((WorkspaceTreeNode)Root).AssemblyList = value; }
		}

		#endregion
		
		public ClassBrowserTreeView()
		{
			WorkspaceTreeNode root = new WorkspaceTreeNode();
			ClassBrowserTreeView instance = this;
			root.AssemblyLists.CollectionChanged += delegate {
				instance.ShowRoot = root.AssemblyLists.Count > 0;
			};
			root.PropertyChanged += delegate {
				instance.ShowRoot = root.AssemblyLists.Count > 0;
			};
			this.Root = root;
		}
		
		protected override void OnContextMenuOpening(ContextMenuEventArgs e)
		{
			var treeNode = this.SelectedItem as ModelCollectionTreeNode;
			if (treeNode != null) {
				treeNode.ShowContextMenu();
			}
		}
	}
	
	public interface IClassBrowserTreeView : IClassBrowser
	{
		
	}
}