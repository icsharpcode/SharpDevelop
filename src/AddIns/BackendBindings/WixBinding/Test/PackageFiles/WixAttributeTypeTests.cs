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

namespace WixBinding.Tests.PackageFiles
{
	[TestFixture]
	public class WixAttributeTypeTests
	{
		WixXmlAttribute productIdAttribute;
		WixXmlAttribute productUpgradeCodeAttribute;
		WixXmlAttribute componentGuidAttribute;
		WixSchemaCompletionData wixSchema = new WixSchemaCompletionData();
		
		[SetUp]
		public void Init()
		{
			WixDocument doc = new WixDocument();
			doc.LoadXml(GetWixXml());
			XmlElement productElement = doc.Product;
			WixXmlAttributeCollection attributes = wixSchema.GetAttributes(productElement);
			productIdAttribute = attributes["Id"];
			productUpgradeCodeAttribute = attributes["UpgradeCode"];
			
			XmlElement componentElement = (XmlElement)doc.SelectSingleNode("//w:Component", new WixNamespaceManager(doc.NameTable));
			attributes = wixSchema.GetAttributes(componentElement);
			componentGuidAttribute = attributes["Guid"];
		}
		
		[Test]
		public void ProductIdAttributeIsAutogenUuid()
		{
			Assert.AreEqual(WixXmlAttributeType.AutogenUuid, productIdAttribute.AttributeType);
		}
		
		[Test]
		public void UpgradeCodeAttributeIsUuid()
		{
			Assert.AreEqual(WixXmlAttributeType.Uuid, productUpgradeCodeAttribute.AttributeType);
		}
		
		[Test]
		public void ComponentGuidAttributeIsUuid()
		{
			Assert.AreEqual(WixXmlAttributeType.ComponentGuid, componentGuidAttribute.AttributeType);
		}

		string GetWixXml()
		{
			return "<Wix xmlns='http://schemas.microsoft.com/wix/2003/01/wi'>\r\n" +
				"\t<Product Name='DialogTest' \r\n" +
				"\t         Version='1.0' \r\n" +
				"\t         Language='1013' \r\n" +
				"\t         Manufacturer='#develop' \r\n" +
				"\t         Id='????????-????-????-????-????????????'>\r\n" +
				"\t\t<Fragment>\r\n" +
				"\t\t\t<Directory Id=\"TARGETDIR\" SourceName=\"SourceDir\">\r\n" +
				"\t\t\t\t<Component Guid=''/>\r\n" +
				"\t\t\t</Directory>\r\n" +
				"\t\t</Fragment>\r\n" +
				"\t</Product>\r\n" +
				"</Wix>";
		}
	}
}
