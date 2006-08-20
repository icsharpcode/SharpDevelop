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
	/// Tests that the WixProject.WixSourceFiles property only returns
	/// FileProjectItems that have a .wxs file extension.
	/// </summary>
	[TestFixture]
	public class GetWixSourceFileProjectItemsTestFixture
	{
		FileProjectItem wixSetupFile;
		FileProjectItem wixDialogsFile;
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
			
			item = new FileProjectItem(p, ItemType.Compile);
			item.Include = "test.wxi";
			p.Items.Add(item);
			
			item = new FileProjectItem(p, ItemType.Compile);
			item.Include = "dialogs.wxs";
			p.Items.Add(item);
			
			wixFileProjectItemCount = 0;
			
			foreach (FileProjectItem fileItem in p.WixSourceFiles) {
				wixFileProjectItemCount++;
			}
			
			wixSetupFile = p.WixSourceFiles[0];
			wixDialogsFile = p.WixSourceFiles[1];
		}
		
		[Test]
		public void TwoWixSourceFiles()
		{
			Assert.AreEqual(2, wixFileProjectItemCount);
		}
		
		[Test]
		public void WixSetupFileProjectItemInclude()
		{
			Assert.AreEqual("setup.wxs", wixSetupFile.Include);
		}
		
		[Test]
		public void WixDialogsFileProjectItemInclude()
		{
			Assert.AreEqual("dialogs.wxs", wixDialogsFile.Include);
		}
	}
}
