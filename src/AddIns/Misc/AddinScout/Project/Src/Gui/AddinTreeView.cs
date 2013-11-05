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
	public class AddinTreeView : Panel
	{
		public TreeView treeView = new TreeView();
		
		public AddinTreeView()
		{
//			treeView.BorderStyle = BorderStyle.;
//			treeView.AfterSelect += new TreeViewEventHandler(this.tvSelectHandler);
			
			PopulateTreeView();
			
			
			treeView.ImageList = new ImageList();
			treeView.ImageList.ColorDepth = ColorDepth.Depth32Bit;
			treeView.ImageList.Images.Add(IconService.GetBitmap("Icons.16x16.Class"));
			treeView.ImageList.Images.Add(IconService.GetBitmap("Icons.16x16.Collection"));
			treeView.ImageList.Images.Add(IconService.GetBitmap("Icons.16x16.OpenCollection"));
			treeView.ImageList.Images.Add(IconService.GetBitmap("Icons.16x16.ClosedFolderBitmap"));
			treeView.ImageList.Images.Add(IconService.GetBitmap("Icons.16x16.OpenFolderBitmap"));
			
			treeView.Dock = DockStyle.Fill;
			Controls.Add(treeView);
		}
		
		void PopulateTreeView()
		{
			TreeNode rootNode = new TreeNode("Addins");
			rootNode.ImageIndex = rootNode.SelectedImageIndex = 0;
			rootNode.Expand();
			
			treeView.Nodes.Add(rootNode);
			
			foreach (var addin in SD.AddInTree.AddIns) {
				TreeNode newNode = new TreeNode(addin.Properties["name"]);
				newNode.ImageIndex = 1;
				newNode.SelectedImageIndex = 2;
				newNode.Tag = addin;
				GetExtensions(addin, newNode);
				rootNode.Nodes.Add(newNode);
			}
		}
		
		void GetExtensions(AddIn ai, TreeNode treeNode)
		{
			if (!ai.Enabled)
				return;
			foreach (ExtensionPath ext in ai.Paths.Values) {
				TreeNode newNode = new TreeNode(ext.Name);
				newNode.ImageIndex = 3;
				newNode.SelectedImageIndex = 4;
				newNode.Tag = ext;
				treeNode.Nodes.Add(newNode);
			}
		}
	}
}
