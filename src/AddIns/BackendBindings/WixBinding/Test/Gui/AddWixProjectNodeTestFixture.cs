// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Internal.Templates;
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
