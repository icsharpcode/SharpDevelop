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
	/// Tests that the WixLibraryFolderNode can be expanded when the project
	/// contains WixLibrary items.
	/// </summary>
	[TestFixture]
	public class WixProjectWithUnexpandedLibraryItemsTestFixture
	{
		WixProject wixProject;
		WixLibraryFolderNode wixLibraryFolderNode;
		WixLibraryProjectItem firstWixLibraryItem;
		WixLibraryProjectItem secondWixLibraryItem;
		WixExtensionFolderNode wixExtensionFolderNode;
		WixExtensionProjectItem firstWixExtensionItem;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			SD.InitializeForUnitTests();
			wixProject = WixBindingTestsHelper.CreateEmptyWixProject();
			
			// Add wix library item.
			firstWixLibraryItem = new WixLibraryProjectItem(wixProject);
			firstWixLibraryItem.Include = "first.wixlib";
			ProjectService.AddProjectItem(wixProject, firstWixLibraryItem);
			
			// Add another wix library item.
			secondWixLibraryItem = new WixLibraryProjectItem(wixProject);
			secondWixLibraryItem.Include = "second.wixlib";
			ProjectService.AddProjectItem(wixProject, secondWixLibraryItem);
			
			// Add a wix extension item.
			firstWixExtensionItem = new WixExtensionProjectItem(wixProject);
			firstWixExtensionItem.Include = "first-ext.dll";
			ProjectService.AddProjectItem(wixProject, firstWixExtensionItem);
			
			wixLibraryFolderNode = new WixLibraryFolderNode(wixProject);
			wixExtensionFolderNode = new WixExtensionFolderNode(wixProject);
		}
		
		/// <summary>
		/// WixLibraryNode must have a dummy child node in order to be expandable
		/// by the user in the tree view.
		/// </summary>
		[Test]
		public void SingleCustomNodeChildForLibraryFolderNode()
		{
			CustomNode childNode = wixLibraryFolderNode.Nodes[0] as CustomNode;
			Assert.IsNotNull(childNode);
		}
		
		[Test]
		public void SingleCustomNodeChildForExtensionFolderNode()
		{
			CustomNode childNode = wixExtensionFolderNode.Nodes[0] as CustomNode;
			Assert.IsNotNull(childNode);
		}		
	}
}
