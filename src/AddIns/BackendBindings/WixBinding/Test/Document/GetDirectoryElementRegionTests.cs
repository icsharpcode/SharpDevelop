// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
			DomRegion region = WixDocument.GetElementRegion(new StringReader(xml), "Directory", "TARGETDIR");
			DomRegion expectedRegion = new DomRegion(2, 2, 3, 13);
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
			DomRegion region = WixDocument.GetElementRegion(new StringReader(xml), "Directory", "TARGETDIR");
			DomRegion expectedRegion = new DomRegion(2, 2, 5, 13);
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
			DomRegion region = WixDocument.GetElementRegion(new StringReader(xml), "Directory", "TARGETDIR");
			DomRegion expectedRegion = new DomRegion(2, 2, 4, 13);
			Assert.AreEqual(expectedRegion, region);
		}

	}
}
