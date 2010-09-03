// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

namespace AddInScout
{
	/// <summary>
	/// Description of AddinTreeView.
	/// </summary>
	public class TreeTreeView : Panel
	{
		public TreeView treeView = new TreeView();
		
		public TreeTreeView()
		{
			PopulateTreeView();
			
			
			treeView.ImageList = new ImageList();
			treeView.ImageList.ColorDepth = ColorDepth.Depth32Bit;
			treeView.ImageList.Images.Add(IconService.GetBitmap("Icons.16x16.Class"));
			treeView.ImageList.Images.Add(IconService.GetBitmap("Icons.16x16.Assembly"));
			treeView.ImageList.Images.Add(IconService.GetBitmap("Icons.16x16.OpenAssembly"));
			treeView.ImageList.Images.Add(IconService.GetBitmap("Icons.16x16.ClosedFolderBitmap"));
			treeView.ImageList.Images.Add(IconService.GetBitmap("Icons.16x16.OpenFolderBitmap"));
			
			treeView.Dock = DockStyle.Fill;
			Controls.Add(treeView);
		}
		
		void PopulateTreeView()
		{
			TreeNode rootNode = new TreeNode("AddInTree");
			rootNode.ImageIndex = rootNode.SelectedImageIndex = 0;
			rootNode.Expand();
			
			treeView.Nodes.Add(rootNode);
			
			for (int i = 0; i < AddInTree.AddIns.Count; i++) {
				GetExtensions(AddInTree.AddIns[i], rootNode);
			}
		}
		
		void GetExtensions(AddIn ai, TreeNode treeNode)
		{
			foreach (ExtensionPath ext in ai.Paths.Values) {
				string[] name = ext.Name.Split('/');
				TreeNode currentNode = treeNode;
				if (name.Length < 1) {
					continue;
				}
				for (int i = 1; i < name.Length; ++i) {
					bool found = false;
					foreach (TreeNode n in currentNode.Nodes) {
						if (n.Text == name[i]) {
							currentNode = n;
							found = true;
							break;
						}
					}
					if (found) {
						if (i == name.Length - 1 && currentNode.Tag == null)
							currentNode.Tag = ext;
					} else {
						TreeNode newNode = new TreeNode(name[i]);
						newNode.ImageIndex = 3;
						newNode.SelectedImageIndex = 4;
						if (i == name.Length - 1) {
							newNode.Tag = ext;
						}
						currentNode.Nodes.Add(newNode);
						currentNode = newNode;
					}
				}
			}
		}
	}
}
