// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using System.Xml;

namespace WixBinding.Tests.Document
{
	/// <summary>
	/// Tests that the WixDocument.GetDialog method returns null if the
	/// dialog id cannot be found
	/// </summary>
	[TestFixture]
	public class GetMissingDialogFromWixDocumentTestFixture
	{
		WixDocument doc;
		WixDialog dialog;
		
		[SetUp]
		public void SetUpFixture()
		{
			doc = new WixDocument();
			doc.LoadXml(GetWixXml());
			dialog = doc.GetDialog("MissingDialog");
		}
		
		[Test]
		public void MissingDialogId()
		{
			Assert.IsNull(dialog);
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
