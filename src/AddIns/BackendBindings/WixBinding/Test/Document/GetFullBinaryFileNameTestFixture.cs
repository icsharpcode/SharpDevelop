// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using System.IO;

namespace WixBinding.Tests.Document
{
	/// <summary>
	/// Tests the WixDialog.GetBinaryFileName method which returns the binary
	/// full filename to the bitmap using the Binary element and its own filename.
	/// </summary>
	[TestFixture]
	public class GetFullBinaryFileNameTestFixture
	{
		WixDocument document;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			document = new WixDocument();
			document.FileName = @"C:\Projects\Setup\Setup.wxs";
			document.LoadXml(GetWixXml());
		}
		
		[Test]
		public void GetBannerBitmapFileName()
		{
			Assert.AreEqual(@"C:\Projects\Setup\Bitmaps/Banner.bmp", document.GetBinaryFileName("Banner"));
		}
		
		/// <summary>
		/// The icon Binary element has a full path so check that the WixDocument 
		/// uses this filename instead.
		/// </summary>
		[Test]
		public void GetInfoIconFileName()
		{
			Assert.AreEqual(@"C:/Program Files/SharpDevelop/Bitmaps/Info.ico", document.GetBinaryFileName("Info"));
		}
		
		string GetWixXml()
		{
			return "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<Binary Id='Banner' SourceFile='Bitmaps/Banner.bmp'/>\r\n" +
				"\t\t<Binary Id='Info' src='C:/Program Files/SharpDevelop/Bitmaps/Info.ico'/>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
		}
	}
}
