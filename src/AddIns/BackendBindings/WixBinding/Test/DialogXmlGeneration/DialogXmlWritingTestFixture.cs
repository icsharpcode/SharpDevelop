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

namespace WixBinding.Tests.DialogXmlGeneration
{
	[TestFixture]
	public class DialogXmlWritingTestFixture
	{
		XmlElement dialogElement;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(GetWixXml());
			dialogElement = (XmlElement)doc.SelectSingleNode("//w:Dialog", new WixNamespaceManager(doc.NameTable));
			dialogElement.SetAttribute("Id", "id");
			dialogElement.SetAttribute("Title", "title");
			XmlElement controlElement = doc.CreateElement("Control", WixNamespaceManager.Namespace);
			dialogElement.AppendChild(controlElement);
		}
		
		[Test]
		public void Tabs()
		{
			string outputXml = WixDocument.GetXml(dialogElement, "\r\n", false, 4);
			string expectedXml = "<Dialog Id=\"id\" Height=\"270\" Width=\"370\" Title=\"title\">\r\n" +
				"\t<Control />\r\n" +
				"</Dialog>";
			Assert.AreEqual(expectedXml, outputXml);
		}
		
		[Test]
		public void Spaces()
		{
			string outputXml = WixDocument.GetXml(dialogElement, "\n", true, 4);
			string expectedXml = "<Dialog Id=\"id\" Height=\"270\" Width=\"370\" Title=\"title\">\n" +
				"    <Control />\n" +
				"</Dialog>";
			Assert.AreEqual(expectedXml, outputXml);
		}
		
		string GetWixXml()
		{
			return "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<UI>\r\n" +
				"\t\t\t<Dialog Id='WelcomeDialog' Height='270' Width='370' Title='Welcome Dialog Title'/>\r\n" +
				"\t\t</UI>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
		}
	}
}
