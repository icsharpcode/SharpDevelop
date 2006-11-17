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
			
			wixLibraryFolderNode = new WixLibraryFolderNode(wixProject);
		}
		
		/// <summary>
		/// WixLibraryNode must have a dummy child node in order to be expandable
		/// by the user in the tree view.
		/// </summary>
		[Test]
		public void SingleCustomNodeChild()
		{
			CustomNode childNode = wixLibraryFolderNode.Nodes[0] as CustomNode;
			Assert.IsNotNull(childNode);
		}
	}
}
