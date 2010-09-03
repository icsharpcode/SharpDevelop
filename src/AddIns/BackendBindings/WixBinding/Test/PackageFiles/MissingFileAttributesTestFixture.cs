// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
