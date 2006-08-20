// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
		XmlDocument doc;
		WixSchemaCompletionData schema = new WixSchemaCompletionData();
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			doc = new XmlDocument();
			doc.LoadXml("<Wix xmlns='" + WixNamespaceManager.Namespace + "'/>");
		}
		
		[Test]
		public void ElementHasNoAttributes()
		{
			XmlElement element = doc.CreateElement("File", WixNamespaceManager.Namespace);
			string[] attributeNames = new string[] {"Foo", "Bar"};
			WixXmlAttributeCollection attributes = WixXmlAttribute.GetAttributes(element, attributeNames);
			Assert.IsNotNull(attributes[0]);
			Assert.IsNotNull(attributes[1]);
			Assert.AreEqual(2, attributes.Count);
		}
		
		[Test]
		public void ElementHasOneAttribute()
		{
			XmlElement element = doc.CreateElement("File", WixNamespaceManager.Namespace);
			element.SetAttribute("Id", "Test");
			string[] attributeNames = new string[] {"Id"};
			WixXmlAttributeCollection attributes = WixXmlAttribute.GetAttributes(element, attributeNames);
			Assert.AreEqual(1, attributes.Count);
			Assert.IsNotNull(attributes[0]);
		}
		
		[Test]
		public void UnknownElementAttribute()
		{
			XmlElement element = doc.CreateElement("File", WixNamespaceManager.Namespace);
			element.SetAttribute("Id", "Test");
			string[] attributeNames = new string[0];
			WixXmlAttributeCollection attributes = WixXmlAttribute.GetAttributes(element, attributeNames);
			Assert.AreEqual(1, attributes.Count);
			Assert.IsNotNull(attributes[0]);
		}
	}
}
