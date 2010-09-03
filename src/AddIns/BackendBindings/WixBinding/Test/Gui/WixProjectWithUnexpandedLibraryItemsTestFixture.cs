// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
