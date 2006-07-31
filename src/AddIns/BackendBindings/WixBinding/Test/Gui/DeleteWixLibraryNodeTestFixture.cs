// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
	/// Deletes a WixLibraryNode from the WixLibraryFolderNode parent.
	/// </summary>
	[TestFixture]
	public class DeleteWixLibraryNodeTestFixture
	{
		WixProject wixProject;
		WixLibraryFolderNode wixLibraryFolderNode;
		WixLibraryProjectItem wixLibraryItem;
		WixLibraryNode wixLibraryNode;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			wixProject = WixBindingTestsHelper.CreateEmptyWixProject();
			
			// Add wix library item.
			wixLibraryItem = new WixLibraryProjectItem(wixProject);
			wixLibraryItem.Include = @"..\..\first.wixlib";
			wixProject.Items.Add(wixLibraryItem);
			
			// Run Initialize on the WixLibraryFolderNode, which is 
			// equivalent to expanding the node, so it adds it children. Cannot
			// call ExtTreeNode.Expanding since this relies on the tree node
			// being visible.
			WixLibraryFolderNodeTester nodeTester = new WixLibraryFolderNodeTester(wixProject);
			nodeTester.RunInitialize();
			
			wixLibraryFolderNode = (WixLibraryFolderNode)nodeTester;
			wixLibraryNode = (WixLibraryNode)wixLibraryFolderNode.Nodes[0];
		}
		
		[Test]
		public void CanDeleteNode()
		{
			Assert.IsTrue(wixLibraryNode.EnableDelete);
		}
		
		[Test]
		public void DeleteNode()
		{
			wixLibraryNode.Delete();
	
			Assert.AreEqual(0, wixLibraryFolderNode.Nodes.Count);
		}
	}
}
