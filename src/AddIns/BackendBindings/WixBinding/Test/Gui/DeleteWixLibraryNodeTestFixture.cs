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
