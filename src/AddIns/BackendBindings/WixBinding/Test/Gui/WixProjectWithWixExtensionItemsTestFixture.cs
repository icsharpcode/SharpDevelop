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
			SD.InitializeForUnitTests();
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
