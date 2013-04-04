// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.WixBinding;
using Microsoft.Build.Evaluation;
using NUnit.Framework;
using Rhino.Mocks;
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
			SD.InitializeForUnitTests();
			WixBindingTestsHelper.InitMSBuildEngine();
			
			// create the project.
			ProjectCreateInformation info = new ProjectCreateInformation(MockSolution.Create(), new FileName(@"C:\Projects\Test\Test.wixproj"));

			wixProject = new WixProjectWithOverriddenSave(info);
			
			// Add wix library item.
			wixLibraryItem = new WixLibraryProjectItem(wixProject);
			wixLibraryItem.Include = @"..\..\first.wixlib";
			ProjectService.AddProjectItem(wixProject, wixLibraryItem);
			
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
