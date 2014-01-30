// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
		WixXmlAttribute componentKeyPathAttribute;
		WixXmlAttribute fileSourceAttribute;
		WixXmlAttribute fileSrcAttribute;
		WixSchemaCompletion wixSchema = new WixSchemaCompletion();
		WixDocument doc;
		
		[SetUp]
		public void Init()
		{
			doc = new WixDocument();
			doc.FileName = @"C:\Projects\Setup\Files.wxs";
			doc.LoadXml(GetWixXml());
			XmlElement productElement = doc.GetProduct();
			WixXmlAttributeCollection attributes = wixSchema.GetAttributes(productElement);
			productIdAttribute = attributes["Id"];
			productUpgradeCodeAttribute = attributes["UpgradeCode"];
			
			XmlElement componentElement = (XmlElement)doc.SelectSingleNode("//w:Component", new WixNamespaceManager(doc.NameTable));
			attributes = wixSchema.GetAttributes(componentElement);
			componentGuidAttribute = attributes["Guid"];
			componentKeyPathAttribute = attributes["KeyPath"];
			
			XmlElement fileElement = (XmlElement)doc.SelectSingleNode("//w:File", new WixNamespaceManager(doc.NameTable));
			attributes = wixSchema.GetAttributes(fileElement);
			fileSourceAttribute = attributes["Source"];
			fileSrcAttribute = attributes["src"];
		}
		
		[Test]
		public void ProductIdAttributeIsAutogenUuid()
		{
			Assert.AreEqual(WixXmlAttributeType.AutogenGuid, productIdAttribute.AttributeType);
		}

		[Test]
		public void ProductIdAttributeHasDocument()
		{
			Assert.IsTrue(Object.ReferenceEquals(doc, productIdAttribute.Document));
		}
		
		[Test]
		public void UpgradeCodeAttributeIsUuid()
		{
			Assert.AreEqual(WixXmlAttributeType.Guid, productUpgradeCodeAttribute.AttributeType);
		}
		
		[Test]
		public void ComponentGuidAttributeIsUuid()
		{
			Assert.AreEqual(WixXmlAttributeType.ComponentGuid, componentGuidAttribute.AttributeType);
		}

		[Test]
		public void FileSourceAttributeIsFileName()
		{
			Assert.AreEqual(WixXmlAttributeType.FileName, fileSourceAttribute.AttributeType);
		}
		
		[Test]
		public void FileSrcAttributeIsFileName()
		{
			Assert.AreEqual(WixXmlAttributeType.FileName, fileSrcAttribute.AttributeType);
		}
		
		[Test]
		public void KeyPathHasYesNoValues()
		{
			Assert.Contains("yes", componentKeyPathAttribute.Values);
			Assert.Contains("no", componentKeyPathAttribute.Values);
			Assert.AreEqual(WixXmlAttributeType.Text, componentKeyPathAttribute.AttributeType);
		}

		string GetWixXml()
		{
			return "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Product Name='DialogTest' \r\n" +
				"\t         Version='1.0' \r\n" +
				"\t         Language='1013' \r\n" +
				"\t         Manufacturer='#develop' \r\n" +
				"\t         Id='????????-????-????-????-????????????'>\r\n" +
				"\t\t<Fragment>\r\n" +
				"\t\t\t<Directory Id=\"TARGETDIR\" SourceName=\"SourceDir\">\r\n" +
				"\t\t\t\t<Component Guid=''>\r\n" +
				"\t\t\t\t\t<File src='bin\\test.exe'/>\r\n" +
				"\t\t\t\t</Component>\r\n" +
				"\t\t\t</Directory>\r\n" +
				"\t\t</Fragment>\r\n" +
				"\t</Product>\r\n" +
				"</Wix>";
		}
	}
}
