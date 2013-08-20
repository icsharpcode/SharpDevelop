// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows.Controls;
using ICSharpCode.Core;
using ICSharpCode.TreeView;

namespace ICSharpCode.SharpDevelop.Dom.ClassBrowser
{
	public class ClassBrowserTreeView : SharpTreeView, IClassBrowserTreeView
	{
		#region IClassBrowser implementation
		
		WorkspaceModel workspace;

		public ICollection<IAssemblyList> AssemblyLists {
			get { return workspace.AssemblyLists; }
		}

		public IAssemblyList MainAssemblyList {
			get { return workspace.MainAssemblyList; }
			set { workspace.MainAssemblyList = value; }
		}
		
		public IAssemblyList UnpinnedAssemblies {
			get { return workspace.UnpinnedAssemblies; }
			set { workspace.UnpinnedAssemblies = value; }
		}
		
		public IAssemblyModel FindAssemblyModel(FileName fileName)
		{
			return workspace.FindAssemblyModel(fileName);
		}

		#endregion
		
		public ClassBrowserTreeView()
		{
			WorkspaceTreeNode root = new WorkspaceTreeNode();
			this.workspace = root.Workspace;
			ClassBrowserTreeView instance = this;
			root.Workspace.AssemblyLists.CollectionChanged += delegate {
				instance.ShowRoot = root.Workspace.AssemblyLists.Count > 0;
			};
			root.PropertyChanged += delegate {
				instance.ShowRoot = root.Workspace.AssemblyLists.Count > 0;
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