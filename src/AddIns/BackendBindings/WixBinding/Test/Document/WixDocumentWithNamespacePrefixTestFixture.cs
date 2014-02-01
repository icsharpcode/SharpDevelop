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
