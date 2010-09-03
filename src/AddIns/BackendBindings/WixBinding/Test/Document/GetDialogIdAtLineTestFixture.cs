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
	/// Tests that the dialog element can be located using the 
	/// WixDocument.GetDialogId method based on the current line in the
	/// Wix document xml.
	/// </summary>
	[TestFixture]
	public class GetDialogIdAtLineTestFixture
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
		public void FirstLineAfterDialogStartTag()
		{
			Assert.AreEqual(expectedDialogId, wixReader.GetDialogId(10));			
		}
		
		[Test]
		public void DialogEndTagLine()
		{
			Assert.AreEqual(expectedDialogId, wixReader.GetDialogId(16));
		}
		
		[Test]
		public void FirstLineAfterDialogEndTag()
		{
			Assert.IsNull(wixReader.GetDialogId(17));
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
				"\t\t\t<Dialog Id='WelcomeDialog' Width='370' Height='270' Title='[ProductName] [Setup]'>\r\n" +
				"\t\t\t\t<Control Id='Next' Type='PushButton' X='236' Y='243' Width='56' Height='17' Default='yes' Text='[Button_Next]'>\r\n" +
				"\t\t\t\t\t<Publish Event='NewDialog' Value='ViewLicenseAgreement'>1</Publish>\r\n" +
				"\t\t\t\t</Control>\r\n" +
				"\t\t\t\t<Control Id='Cancel' Type='PushButton' X='304' Y='243' Width='56' Height='17' Cancel='yes' Text='[Button_Cancel]'>\r\n" +
				"\t\t\t\t\t<Publish Event='SpawnDialog' Value='CancelDialog'>1</Publish>\r\n" +
				"\t\t\t\t</Control>\r\n" +
				"\t\t\t</Dialog>\r\n" +
				"\t\t</UI>\r\n" +
				"\t</Product>\r\n" +
				"</Wix>";
		}
	}
}
