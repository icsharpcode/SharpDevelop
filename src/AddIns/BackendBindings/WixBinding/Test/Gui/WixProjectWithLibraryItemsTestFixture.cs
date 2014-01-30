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

using ICSharpCode.SharpDevelop;
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
			SD.InitializeForUnitTests();
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
