// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;

namespace WixBinding.Tests.DirectoryImport
{
	[TestFixture]
	public class AddDirectoryWithInvalidIdCharsTestFixture
	{
		WixDocument document;
		
		[SetUp]
		public void Init()
		{
			document = new WixDocument();
			document.FileName = @"C:\Projects\Test\Setup.wxs";
			document.LoadXml(GetWixXml());
		}
		
		[Test]
		public void AddDirectoryWithHyphen()
		{
			string directoryName = "Test-directory";
			WixDirectoryElement element = document.GetRootDirectory().AddDirectory(directoryName);
			Assert.AreEqual("Test_directory", element.Id);
		}
		
		string GetWixXml()
		{
			return "<Wix xmlns=\"http://schemas.microsoft.com/wix/2006/wi\">\r\n" +
				"\t<Product Name=\"MySetup\" \r\n" +
				"\t         Manufacturer=\"\" \r\n" +
				"\t         Id=\"F4A71A3A-C271-4BE8-B72C-F47CC956B3AA\" \r\n" +
				"\t         Language=\"1033\" \r\n" +
				"\t         Version=\"1.0.0.0\">\r\n" +
				"\t\t<Package Id=\"6B8BE64F-3768-49CA-8BC2-86A76424DFE9\"/>\r\n" +
				"\t\t<Directory Id=\"TARGETDIR\" SourceName=\"SourceDir\">\r\n" +
				"\t\t</Directory>\r\n" +
				"\t</Product>\r\n" +
				"</Wix>";
		}
	}
}
