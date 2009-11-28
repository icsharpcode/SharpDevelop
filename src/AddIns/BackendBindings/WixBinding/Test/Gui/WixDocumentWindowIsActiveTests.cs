// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.Gui
{
	[TestFixture]
	public class WixDocumentWindowIsActiveTests
	{
		WixDocumentWindow window;
		MockWorkbench workbench;
		WixDocument document;
		
		[SetUp]
		public void Init()
		{
			workbench = new MockWorkbench();
			window = new WixDocumentWindow(workbench);
			
			document = new WixDocument();
			document.FileName = @"d:\Projects\Test\Files.wxs";
			
			MockViewContent view = new MockViewContent();
			view.SetFileName(@"d:\projects\test\files.wxs");
			workbench.ActiveViewContent = view;
		}
		
		[Test]
		public void WixDocumentWindowIsActiveReturnsTrue()
		{
			Assert.IsTrue(window.IsActive(document));
		}
		
		[Test]
		public void WixDocumentWindowIsActiveReturnsFalseWhenWorkbenchViewContentIsNull()
		{
			workbench.ActiveViewContent = null;
			Assert.IsFalse(window.IsActive(document));
		}
		
		[Test]
		public void WixDocumentWindowIsActiveReturnsFalseWhenWixDocumentParameterUsedIsNull()
		{
			Assert.IsFalse(window.IsActive(null));
		}		
	}
}
