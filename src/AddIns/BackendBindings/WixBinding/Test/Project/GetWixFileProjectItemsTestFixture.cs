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
	/// Tests that the WixProject.GetWixFileProjectItems method only returns
	/// FileProjectItems that have a Wix document file extension.
	/// </summary>
	[TestFixture]
	public class GetWixFileProjectItemsTestFixture
	{
		FileProjectItem wixFileProjectItem;
		int wixFileProjectItemCount;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			WixProject p = WixBindingTestsHelper.CreateEmptyWixProject();
			
			FileProjectItem item = new FileProjectItem(p, ItemType.None);
			item.Include = "readme.txt";
			p.Items.Add(item);
			
			ReferenceProjectItem referenceItem = new ReferenceProjectItem(p);
			referenceItem.Include = "System.Windows.Forms";
			p.Items.Add(referenceItem);
			
			item = new FileProjectItem(p, ItemType.Compile);
			item.Include = "setup.wxs";
			p.Items.Add(item);
			
			wixFileProjectItemCount = 0;
			
			foreach (FileProjectItem fileItem in p.WixFileProjectItems) {
				wixFileProjectItemCount++;
				wixFileProjectItem = fileItem;
			}
		}
		
		[Test]
		public void OneWixFileProjectItem()
		{
			Assert.AreEqual(1, wixFileProjectItemCount);
		}
		
		[Test]
		public void WixFileProjectItemInclude()
		{
			Assert.AreEqual("setup.wxs", wixFileProjectItem.Include);
		}
	}
}
