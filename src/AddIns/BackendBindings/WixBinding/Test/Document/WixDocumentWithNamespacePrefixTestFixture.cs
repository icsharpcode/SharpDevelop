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
	/// Tests that adding a new element to the WixDocument prefixes the namespace
	/// to the element.
	/// </summary>
	[TestFixture]
	public class WixDocumentWithNamespacePrefixTestFixture
	{		
		XmlElement directory;
		WixDocument doc;
		string prefixBeforeLoad;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			doc = new WixDocument();
			prefixBeforeLoad = doc.GetWixNamespacePrefix();
			doc.LoadXml(GetWixXml());
			directory = doc.CreateWixElement("Directory");
			directory.OwnerDocument.DocumentElement.AppendChild(directory);
		}
		
		[Test]
		public void PrefixBeforeLoad()
		{
			Assert.AreEqual(String.Empty, prefixBeforeLoad);
		}
		
		[Test]
		public void LocalName()
		{
			Assert.AreEqual("Directory", directory.LocalName);
		}
		
		[Test]
		public void Name()
		{
			Assert.AreEqual("w:Directory", directory.Name);
		}
		
		[Test]
		public void Namespace()
		{
			Assert.AreEqual(WixNamespaceManager.Namespace, directory.NamespaceURI);
		}
		
		[Test]
		public void WixNamespacePrefix()
		{
			Assert.AreEqual("w", doc.GetWixNamespacePrefix());
		}
		
		[Test]
		public void Prefix()
		{
			Assert.AreEqual("w", directory.Prefix);
		}
		
		string GetWixXml()
		{
			return "<w:Wix xmlns:w='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<w:Product Name='DialogTest' \r\n" +
				"\t         Version='1.0' \r\n" +
				"\t         Language='1013' \r\n" +
				"\t         Manufacturer='#develop' \r\n" +
				"\t         Id='????????-????-????-????-????????????'>\r\n" +
				"\t\t<w:Package/>\r\n" +
				"\t</w:Product>\r\n" +
				"</w:Wix>";
		}
	}
}
