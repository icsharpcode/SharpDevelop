// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using System.IO;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.PackageFiles
{
	/// <summary>
	/// Tests that the package editor will display an error message indicating that no target directory
	/// could be found the WiX files. Previously the editor would display an error message that 
	/// there were no WiX files - SD2-1349.
	/// </summary>
	[TestFixture]
	public class NoRootDirectoryFoundFixture : PackageFilesTestFixtureBase
	{
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			base.InitFixture();
		}
		
		[Test]
		public void NoSourceFileFoundErrorNotShown()
		{
			Assert.IsFalse(view.IsNoSourceFileFoundMessageDisplayed);
		}
		
		[Test]
		public void NoRootDirectoryFoundMessageIsShown()
		{
			Assert.IsTrue(view.IsNoRootDirectoryFoundMessageDisplayed);
		}
		
		[Test]
		public void ContextMenuIsDisabled()
		{
			Assert.IsFalse(view.ContextMenuEnabled);
		}
		
		/// <summary>
		/// Deliberately use WiX 2.0's namespace so the test fixture will not find any
		/// target directory element.
		/// </summary>
		protected override string GetWixXml()
		{
			return "<Wix xmlns=\"http://schemas.microsoft.com/wix/2003/1/wi\">\r\n" +
				"\t<Product Name=\"MySetup\" \r\n" +
				"\t         Manufacturer=\"\" \r\n" +
				"\t         Id=\"F4A71A3A-C271-4BE8-B72C-F47CC956B3AA\" \r\n" +
				"\t         Language=\"1033\" \r\n" +
				"\t         Version=\"1.0.0.0\">\r\n" +
				"\t\t<Package Id=\"6B8BE64F-3768-49CA-8BC2-86A76424DFE9\"/>\r\n" +
				"\t</Product>\r\n" +
				"</Wix>";
		}
	}
}
