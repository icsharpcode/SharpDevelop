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
	/// Tests that the WixProject.WixFiles property only returns
	/// FileProjectItems that have a Wix document file extension.
	/// </summary>
	[TestFixture]
	public class GetWixFileProjectItemsTestFixture
	{
		FileProjectItem wixSourceFileProjectItem;
		FileProjectItem wixIncludeFileProjectItem;
		int wixFileProjectItemCount;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			WixProject p = WixBindingTestsHelper.CreateEmptyWixProject();
			
			FileProjectItem item = new FileProjectItem(p, ItemType.None);
			item.Include = "readme.txt";
			ProjectService.AddProjectItem(p, item);
			
			ReferenceProjectItem referenceItem = new ReferenceProjectItem(p);
			referenceItem.Include = "System.Windows.Forms";
			ProjectService.AddProjectItem(p, referenceItem);
			
			item = new FileProjectItem(p, ItemType.Compile);
			item.Include = "setup.wxs";
			ProjectService.AddProjectItem(p, item);
			
			item = new FileProjectItem(p, ItemType.Compile);
			item.Include = "components.wxi";
			ProjectService.AddProjectItem(p, item);
			
			wixFileProjectItemCount = 0;
			
			foreach (FileProjectItem fileItem in p.WixFiles) {
				wixFileProjectItemCount++;
			}
			wixSourceFileProjectItem = p.WixFiles[0];
			wixIncludeFileProjectItem = p.WixFiles[1];
		}
		
		[Test]
		public void TwoWixFileProjectItems()
		{
			Assert.AreEqual(2, wixFileProjectItemCount);
		}
		
		[Test]
		public void WixSourceFileProjectItem()
		{
			Assert.AreEqual("setup.wxs", wixSourceFileProjectItem.Include);
		}
		
		[Test]
		public void WixIncludeFileProjectItem()
		{
			Assert.AreEqual("components.wxi", wixIncludeFileProjectItem.Include);
		}
	}
}
