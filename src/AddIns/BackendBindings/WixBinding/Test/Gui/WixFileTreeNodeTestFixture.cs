// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
