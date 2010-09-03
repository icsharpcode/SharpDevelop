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
	/// Tests that an expanded WixLibraryFolderNode contains WixLibraryNodes.
	/// </summary>
	[TestFixture]
	public class WixProjectWithLibraryItemsTestFixture
	{
		WixProject wixProject;
		WixLibraryFolderNode wixLibraryFolderNode;
		WixLibraryProjectItem firstWixLibraryItem;
		WixLibraryProjectItem secondWixLibraryItem;
		WixLibraryNode firstWixLibraryNode;
		WixLibraryNode secondWixLibraryNode;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			wixProject = WixBindingTestsHelper.CreateEmptyWixProject();
			
			// Add wix library item.
			firstWixLibraryItem = new WixLibraryProjectItem(wixProject);
			firstWixLibraryItem.Include = @"..\..\first.wixlib";
			ProjectService.AddProjectItem(wixProject, firstWixLibraryItem);
			
			// Add another wix library item.
			secondWixLibraryItem = new WixLibraryProjectItem(wixProject);
			secondWixLibraryItem.Include = @"..\..\second.wixlib";
			ProjectService.AddProjectItem(wixProject, secondWixLibraryItem);

			// Run Initialize on the WixLibraryFolderNode, which is 
			// equivalent to expanding the node, so it adds it children. Cannot
			// call ExtTreeNode.Expanding since this relies on the tree node
			// being visible.
			WixLibraryFolderNodeTester nodeTester = new WixLibraryFolderNodeTester(wixProject);
			nodeTester.RunInitialize();
			
			wixLibraryFolderNode = (WixLibraryFolderNode)nodeTester;
			firstWixLibraryNode = (WixLibraryNode)wixLibraryFolderNode.Nodes[0];
			secondWixLibraryNode = (WixLibraryNode)wixLibraryFolderNode.Nodes[1];
		}
		
		[Test]
		public void WixLibraryHasTwoChildren()
		{
			Assert.AreEqual(2, wixLibraryFolderNode.Nodes.Count);
		}
		
		[Test]
		public void FirstWixLibraryNodeText()
		{
			Assert.AreEqual("first.wixlib", firstWixLibraryNode.Text);
		}
		
		[Test]
		public void FirstWixLibraryContextMenuPath()
		{
			Assert.AreEqual("/SharpDevelop/Pads/ProjectBrowser/ContextMenu/WixLibraryNode", firstWixLibraryNode.ContextmenuAddinTreePath);
		}
	
		[Test]
		public void SecondWixLibraryNodeText()
		{
			Assert.AreEqual("second.wixlib", secondWixLibraryNode.Text);
		}
		
		[Test]
		public void NewWixLibraryProjectItem()
		{
			IProject p = WixBindingTestsHelper.CreateEmptyWixProject();
			WixLibraryProjectItem item = new WixLibraryProjectItem(p);
			Assert.AreEqual(WixItemType.Library, item.ItemType);
		}
	}
}
