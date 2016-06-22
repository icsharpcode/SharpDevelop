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

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using System.Windows.Forms;
using System.Resources;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.Gui
{
	/// <summary>
	/// Tests the initial state of a WixProjectNode that is created by the 
	/// WixProjectNodeBuilder.
	/// </summary>
	[TestFixture]
	public class AddWixProjectNodeTestFixture
	{
		TreeNode parentNode;
		WixProject wixProject;
		ProjectNode wixProjectNode;
		WixLibraryFolderNode wixLibraryFolderNode;
		WixExtensionFolderNode wixExtensionFolderNode;
		ReferenceFolder referenceFolderNode;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			SD.InitializeForUnitTests();
			WixBindingTestsHelper.RegisterResourceStringsWithSharpDevelopResourceManager();

			wixProject = WixBindingTestsHelper.CreateEmptyWixProject();
			parentNode = new TreeNode();
			WixProjectNodeBuilder builder = new WixProjectNodeBuilder();
			wixProjectNode = builder.AddProjectNode(parentNode, wixProject) as ProjectNode;
			
			foreach (TreeNode node in wixProjectNode.Nodes) {
				if (node is WixLibraryFolderNode) {
					wixLibraryFolderNode = node as WixLibraryFolderNode;
				} else if (node is WixExtensionFolderNode) {
					wixExtensionFolderNode = node as WixExtensionFolderNode;
				}
			}
			
			foreach (TreeNode node in wixProjectNode.Nodes) {
				referenceFolderNode = node as ReferenceFolder;
				if (referenceFolderNode != null) {
					break;
				}
			}
		}
		
		[Test]
		public void OneNodeAdded()
		{
			Assert.AreEqual(1, parentNode.Nodes.Count);
		}
		
		[Test]
		public void IsProjectNodeAddedToParent()
		{
			Assert.AreSame(wixProjectNode, parentNode.Nodes[0]);
		}
		
		[Test]
		public void ProjectNodeReturned()
		{
			Assert.IsNotNull(wixProjectNode);
		}
		
		[Test]
		public void HasOneWixLibraryFolderNode()
		{
			int nodeCount = 0;
			foreach (TreeNode node in wixProjectNode.Nodes) {
				WixLibraryFolderNode folderNode = node as WixLibraryFolderNode;
				if (folderNode != null) {
					++nodeCount;
				}
			}
			Assert.AreEqual(1, nodeCount, "Should be one WixLibraryFolderNode.");
		}
		
		[Test]
		public void WixLibraryFolderNodeName()
		{
			Assert.AreEqual("WiX Libraries", wixLibraryFolderNode.Text);
		}
		
		[Test]
		public void WixExtensionFolderNodeName()
		{
			Assert.AreEqual("WiX Extensions", wixExtensionFolderNode.Text);
		}
		
		[Test]
		public void WixLibraryFolderNodeHasNoChildren()
		{
			Assert.AreEqual(0, wixLibraryFolderNode.Nodes.Count);
		}
		
		[Test]
		public void WixExtensionFolderNodeHasNoChildren()
		{
			Assert.AreEqual(0, wixExtensionFolderNode.Nodes.Count);
		}
		
		[Test]
		public void WixLibraryFolderNodeOpenImage()
		{
			Assert.AreEqual("ProjectBrowser.ReferenceFolder.Open", wixLibraryFolderNode.OpenedImage);
		}
		
		[Test]
		public void WixLibraryFolderNodeClosedImage()
		{
			Assert.AreEqual("ProjectBrowser.ReferenceFolder.Closed", wixLibraryFolderNode.ClosedImage);
		}
		
		[Test]
		public void WixExtensionFolderNodeOpenImage()
		{
			Assert.AreEqual("ProjectBrowser.ReferenceFolder.Open", wixExtensionFolderNode.OpenedImage);
		}
		
		[Test]
		public void WixExtensionFolderNodeClosedImage()
		{
			Assert.AreEqual("ProjectBrowser.ReferenceFolder.Closed", wixExtensionFolderNode.ClosedImage);
		}
		
		[Test]
		public void WixLibraryContextMenuPath()
		{
			Assert.AreEqual("/SharpDevelop/Pads/ProjectBrowser/ContextMenu/WixLibraryFolderNode", wixLibraryFolderNode.ContextmenuAddinTreePath);
		}

		[Test]
		public void WixExtensionContextMenuPath()
		{
			Assert.AreEqual("/SharpDevelop/Pads/ProjectBrowser/ContextMenu/WixExtensionFolderNode", wixExtensionFolderNode.ContextmenuAddinTreePath);
		}
		
		[Test]
		public void ReferenceFolderNodeAdded()
		{
			Assert.IsNotNull(referenceFolderNode);
		}
	}
}
