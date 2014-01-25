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
