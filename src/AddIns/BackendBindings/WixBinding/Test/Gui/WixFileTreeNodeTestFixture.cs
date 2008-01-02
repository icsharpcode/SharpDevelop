// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.WixBinding;
using NUnit.Framework;

namespace WixBinding.Tests.Gui
{
	/// <summary>
	/// Tests the WixFileTreeNode displays the correct text.
	/// </summary>
	[TestFixture]
	public class WixFileTreeNodeTestFixture
	{
		WixFileTreeNode fileNameSpecifiedNode;
	
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			WixDocument doc = new WixDocument();
			
			WixFileElement fileElement = new WixFileElement(doc);
			fileElement.FileName = @"MyApp.exe";
			fileNameSpecifiedNode = new WixFileTreeNode(fileElement);
		}
		
		[Test]
		public void FileName()
		{
			Assert.AreEqual(@"MyApp.exe", fileNameSpecifiedNode.Text);
		}
		
		[Test]
		public void ContextMenuPath()
		{
			Assert.AreEqual("/AddIns/WixBinding/PackageFilesView/ContextMenu/FileTreeNode", fileNameSpecifiedNode.ContextmenuAddinTreePath);
		}
	}
}
