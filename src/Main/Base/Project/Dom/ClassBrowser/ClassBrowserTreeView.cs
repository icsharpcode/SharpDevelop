// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ICSharpCode.Core.Presentation;
using ICSharpCode.TreeView;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.SharpDevelop.Dom.ClassBrowser
{
	public class ClassBrowserTreeView : SharpTreeView, IClassBrowserTreeView
	{
		ClassBrowserWorkspace currentWorkspace;
		
		public ClassBrowserTreeView()
		{
			Workspace = ClassBrowserSettings.LoadDefaultWorkspace();
		}
		
		public ClassBrowserWorkspace Workspace {
			get { return currentWorkspace; }
			set {
				if (currentWorkspace == value)
					return;
				currentWorkspace = value;
				if (currentWorkspace != null) {
					this.Root = new WorkspaceTreeNode(currentWorkspace);
					this.ShowRoot = currentWorkspace.LoadedAssemblies.Count > 0 || !currentWorkspace.IsAssigned;
				} else {
					this.Root = null;
				}
			}
		}
		
		protected override void OnContextMenuOpening(ContextMenuEventArgs e)
		{
			var treeNode = this.SelectedItem as ModelCollectionTreeNode;
			if (treeNode != null) {
				treeNode.ShowContextMenu();
			}
		}
	}
	
	public interface IClassBrowserTreeView
	{
		
	}
}