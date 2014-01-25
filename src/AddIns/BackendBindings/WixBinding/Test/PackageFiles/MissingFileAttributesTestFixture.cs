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
using System.Collections.ObjectModel;
using System.Xml;

namespace WixBinding.Tests.PackageFiles
{
	[TestFixture]
	public class MissingAttributesTests
	{
		WixDocument doc;
		WixSchemaCompletion schema = new WixSchemaCompletion();
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			doc = new WixDocument();
			doc.LoadXml("<Wix xmlns='" + WixNamespaceManager.Namespace + "'/>");
		}
		
		[Test]
		public void ElementHasNoAttributes()
		{
			XmlElement element = doc.CreateElement("File", WixNamespaceManager.Namespace);
			WixXmlAttributeCollection attributes = schema.GetAttributes(element);
			Assert.IsTrue(attributes.Count > 0);
			Assert.IsTrue(Object.ReferenceEquals(attributes[0].Document, doc));
		}
		
		[Test]
		public void ElementHasOneAttribute()
		{
			XmlElement element = doc.CreateElement("File", WixNamespaceManager.Namespace);
			element.SetAttribute("Id", "Test");
			WixXmlAttributeCollection attributes = schema.GetAttributes(element);
			
			int idAttributeCount = 0;
			foreach (WixXmlAttribute attribute in attributes) {
				if (attribute.Name 	== "Id") {
					idAttributeCount++;
				}
			}
			Assert.AreEqual(1, idAttributeCount);
		}
		
		[Test]
		public void UnknownElementAttribute()
		{
			XmlElement element = doc.CreateElement("File", WixNamespaceManager.Namespace);
			element.SetAttribute("Test", "TestValue");
			WixXmlAttributeCollection attributes = schema.GetAttributes(element);
			Assert.IsTrue(attributes.Count > 1);
			Assert.IsNotNull(attributes["Test"]);
		}
	}
}
