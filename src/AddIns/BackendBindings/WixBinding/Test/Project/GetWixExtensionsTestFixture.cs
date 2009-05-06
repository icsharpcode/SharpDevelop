// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.Project
{
	/// <summary>
	/// Tests that the WixProject's WixExtensions property return ProjectItems.
	/// </summary>
	[TestFixture]
	public class GetWixExtensionsTestFixture
	{
		WixExtensionProjectItem extensionItem;
		
		[SetUp]
		public void SetUpFixture()
		{
			WixProject p = WixBindingTestsHelper.CreateEmptyWixProject();
			
			WixExtensionProjectItem compilerItem = new WixExtensionProjectItem(p);
			compilerItem.Include = @"$(WixToolPath)\WixNetFxExtension.dll";
			ProjectService.AddProjectItem(p, compilerItem);
			
			extensionItem = p.WixExtensions[0];
		}
		
		[Test]
		public void WixExtensionItemName()
		{
			Assert.AreEqual("WixExtension", extensionItem.ItemType.ItemName);
		}
		
		
		[Test]
		public void WixExtensionItemInclude()
		{
			Assert.AreEqual(@"$(WixToolPath)\WixNetFxExtension.dll", extensionItem.Include);
		}
		
		[Test]
		public void IsExtensionType()
		{
			Assert.IsInstanceOf(typeof(WixExtensionProjectItem), extensionItem);
		}
	}
}
