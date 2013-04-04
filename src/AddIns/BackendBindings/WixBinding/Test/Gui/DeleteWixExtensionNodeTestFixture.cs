// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.WixBinding;
using Microsoft.Build.Evaluation;
using NUnit.Framework;
using System;
using System.Windows.Forms;
using Rhino.Mocks;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.Gui
{
	/// <summary>
	/// Deletes a WixExtensionNode from the WixExtensionFolderNode parent.
	/// </summary>
	[TestFixture]
	public class DeleteWixExtensionNodeTestFixture
	{
		WixProject wixProject;
		WixExtensionFolderNode wixExtensionFolderNode;
		WixExtensionProjectItem wixExtensionItem;
		WixExtensionNode wixExtensionNode;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			SD.InitializeForUnitTests();
			WixBindingTestsHelper.InitMSBuildEngine();
			
			// create the project.
			ProjectCreateInformation info = new ProjectCreateInformation(MockSolution.Create(), new FileName(@"C:\Projects\Test\Test.wixproj"));

			wixProject = new WixProjectWithOverriddenSave(info);
			
			// Add wix extensionitem.
			wixExtensionItem = new WixExtensionProjectItem(wixProject);
			wixExtensionItem.Include = @"..\..\first.dll";
			ProjectService.AddProjectItem(wixProject, wixExtensionItem);
			
			// Run Initialize on the WixExtensionFolderNode, which is 
			// equivalent to expanding the node, so it adds it children. Cannot
			// call ExtTreeNode.Expanding since this relies on the tree node
			// being visible.
			WixExtensionFolderNodeTester nodeTester = new WixExtensionFolderNodeTester(wixProject);
			nodeTester.RunInitialize();
			
			wixExtensionFolderNode = (WixExtensionFolderNode)nodeTester;
			wixExtensionNode = (WixExtensionNode)wixExtensionFolderNode.Nodes[0];
		}
		
		[Test]
		public void CanDeleteNode()
		{
			Assert.IsTrue(wixExtensionNode.EnableDelete);
		}
		
		[Test]
		public void DeleteNode()
		{
			wixExtensionNode.Delete();
	
			Assert.AreEqual(0, wixExtensionFolderNode.Nodes.Count);
		}
		
		[Test]
		public void ExtensionNodeRemovedFromProject()
		{
			Assert.AreEqual(0, wixProject.WixExtensions.Count);
		}
	}
}
