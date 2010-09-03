// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using System.Windows.Forms;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.Gui
{
	/// <summary>
	/// Tests that an expanded WixExtensionFolderNode contains WixExtensionNodes.
	/// </summary>
	[TestFixture]
	public class WixProjectWithWixExtensionItemsTestFixture
	{
		WixProject wixProject;
		WixExtensionFolderNode wixExtensionFolderNode;
		WixExtensionProjectItem firstWixExtensionItem;
		WixExtensionProjectItem secondWixExtensionItem;
		WixExtensionNode firstWixExtensionNode;
		WixExtensionNode secondWixExtensionNode;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			wixProject = WixBindingTestsHelper.CreateEmptyWixProject();
			
			// Add wix Extension item.
			firstWixExtensionItem = new WixExtensionProjectItem(wixProject);
			firstWixExtensionItem.Include = @"..\..\first-ext.dll";
			ProjectService.AddProjectItem(wixProject, firstWixExtensionItem);
			
			// Add another wix Extension item.
			secondWixExtensionItem = new WixExtensionProjectItem(wixProject);
			secondWixExtensionItem.Include = @"..\..\second-ext.dll";
			ProjectService.AddProjectItem(wixProject, secondWixExtensionItem);

			// Run Initialize on the WixExtensionFolderNode, which is 
			// equivalent to expanding the node, so it adds it children. Cannot
			// call ExtTreeNode.Expanding since this relies on the tree node
			// being visible.
			WixExtensionFolderNodeTester nodeTester = new WixExtensionFolderNodeTester(wixProject);
			nodeTester.RunInitialize();
			
			wixExtensionFolderNode = (WixExtensionFolderNode)nodeTester;
			if (wixExtensionFolderNode.Nodes.Count > 1) {
				firstWixExtensionNode = (WixExtensionNode)wixExtensionFolderNode.Nodes[0];
				secondWixExtensionNode = (WixExtensionNode)wixExtensionFolderNode.Nodes[1];
			}
		}
		
		[Test]
		public void WixExtensionHasTwoChildren()
		{
			Assert.AreEqual(2, wixExtensionFolderNode.Nodes.Count);
		}
		
		[Test]
		public void FirstWixExtensionNodeText()
		{
			Assert.AreEqual("first-ext.dll", firstWixExtensionNode.Text);
		}
		
		[Test]
		public void FirstWixExtensionContextMenuPath()
		{
			Assert.AreEqual("/SharpDevelop/Pads/ProjectBrowser/ContextMenu/WixExtensionNode", firstWixExtensionNode.ContextmenuAddinTreePath);
		}
	
		[Test]
		public void SecondWixExtensionNodeText()
		{
			Assert.AreEqual("second-ext.dll", secondWixExtensionNode.Text);
		}
		
		[Test]
		public void NewWixExtensionProjectItem()
		{
			IProject p = WixBindingTestsHelper.CreateEmptyWixProject();
			WixExtensionProjectItem item = new WixExtensionProjectItem(p);
			Assert.AreEqual(WixItemType.Extension, item.ItemType);
		}
		
		[Test]
		public void EnableDelete()
		{
			Assert.IsTrue(firstWixExtensionNode.EnableDelete);
		}
	}
}
