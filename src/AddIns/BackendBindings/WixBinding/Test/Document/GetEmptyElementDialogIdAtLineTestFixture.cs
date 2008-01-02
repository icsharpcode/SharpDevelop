// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
		StringReader reader;
		string expectedDialogId = "WelcomeDialog";
		
		[SetUp]
		public void SetUpFixture()
		{
			reader = new StringReader(GetWixXml());
		}
		
		[Test]
		public void OnDialogStartTagLine()
		{
			Assert.AreEqual(expectedDialogId, WixDocument.GetDialogId(reader, 8));
		}
		
		[Test]
		public void StartOfDocument()
		{
			Assert.IsNull(WixDocument.GetDialogId(reader, 0));
		}
		
		[Test]
		public void LineBeforeDialogStartTag()
		{
			Assert.IsNull(WixDocument.GetDialogId(reader, 7));
		}
		
		[Test]
		public void LineAfterDialogStartTag()
		{
			Assert.IsNull(WixDocument.GetDialogId(reader, 9));			
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
