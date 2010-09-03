// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
