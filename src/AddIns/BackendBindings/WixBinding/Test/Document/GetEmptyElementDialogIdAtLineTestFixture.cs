// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using System.IO;
using System.Xml;

namespace WixBinding.Tests.Document
{
	/// <summary>
	/// Tests that a dialog element which is an empty element (no end tag) can 
	/// be located using the WixDocument.GetDialogId method based on the current 
	/// line in the Wix document xml.
	/// </summary>
	[TestFixture]
	public class GetEmptyElementDialogIdAtLineTestFixture
	{
		string expectedDialogId = "WelcomeDialog";
		WixDocumentReader wixReader;
		
		[SetUp]
		public void SetUpFixture()
		{
			wixReader = new WixDocumentReader(GetWixXml());
		}
		
		[Test]
		public void OnDialogStartTagLine()
		{
			Assert.AreEqual(expectedDialogId, wixReader.GetDialogId(9));
		}
		
		[Test]
		public void StartOfDocument()
		{
			Assert.IsNull(wixReader.GetDialogId(1));
		}
		
		[Test]
		public void LineBeforeDialogStartTag()
		{
			Assert.IsNull(wixReader.GetDialogId(8));
		}
		
		[Test]
		public void LineAfterDialogStartTag()
		{
			Assert.IsNull(wixReader.GetDialogId(10));			
		}
		
		string GetWixXml()
		{
			return "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Product Name='DialogTest' \r\n" +
				"\t         Version='1.0' \r\n" +
				"\t         Language='1013' \r\n" +
				"\t         Manufacturer='#develop' \r\n" +
				"\t         Id='????????-????-????-????-????????????'>\r\n" +
				"\t\t<Package/>\r\n" +
				"\t\t<UI>\r\n" +
				"\t\t\t<Dialog Id='WelcomeDialog' Height='100' Width='200'/>\r\n" +
				"\t\t</UI>\r\n" +
				"\t</Product>\r\n" +
				"</Wix>";
		}
	}
}
