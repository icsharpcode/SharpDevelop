// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using System.IO;
using System.Xml;

namespace WixBinding.Tests.Document
{
	/// <summary>
	/// Tests that we can determine the target directory element's region in the Wix 
	/// document.
	/// </summary>
	[TestFixture]
	public class GetDirectoryElementRegionTests
	{
		[Test]
		public void SingleDirectoryElement()
		{
			string xml = "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<Directory Id='TARGETDIR' Name='SourceDir'>\r\n" +
				"\t\t</Directory>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
			WixDocumentReader wixReader = new WixDocumentReader(xml);
			DomRegion region = wixReader.GetElementRegion("Directory", "TARGETDIR");
			DomRegion expectedRegion = new DomRegion(3, 3, 4, 14);
			Assert.AreEqual(expectedRegion, region);
		}
		
		[Test]
		public void OneNestedDirectoryElement()
		{
			string xml = "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<Directory Id='TARGETDIR' Name='SourceDir'>\r\n" +
				"\t\t\t<Directory Id='ProgramFiles' Name='PFiles'>\r\n" +
				"\t\t\t</Directory>\r\n" +
				"\t\t</Directory>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
			WixDocumentReader wixReader = new WixDocumentReader(xml);
			DomRegion region = wixReader.GetElementRegion("Directory", "TARGETDIR");
			DomRegion expectedRegion = new DomRegion(3, 3, 6, 14);
			Assert.AreEqual(expectedRegion, region);
		}
		
		[Test]
		public void OneNestedEmptyDirectoryElement()
		{
			string xml = "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<Directory Id='TARGETDIR' Name='SourceDir'>\r\n" +
				"\t\t\t<Directory Id='ProgramFiles' Name='PFiles'/>\r\n" +
				"\t\t</Directory>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
			WixDocumentReader wixReader = new WixDocumentReader(xml);
			DomRegion region = wixReader.GetElementRegion("Directory", "TARGETDIR");
			DomRegion expectedRegion = new DomRegion(3, 3, 5, 14);
			Assert.AreEqual(expectedRegion, region);
		}
	}
}
