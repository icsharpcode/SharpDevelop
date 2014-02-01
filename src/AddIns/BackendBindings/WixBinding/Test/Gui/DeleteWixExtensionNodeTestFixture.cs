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
