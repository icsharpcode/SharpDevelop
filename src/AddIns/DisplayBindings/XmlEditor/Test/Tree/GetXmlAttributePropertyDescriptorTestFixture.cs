// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 2111 $</version>
// </file>

using ICSharpCode.XmlEditor;
using NUnit.Framework;
using System;
using System.ComponentModel;
using System.Xml;

namespace XmlEditor.Tests.Tree
{
	[TestFixture]
	public class GetXmlAttributePropertyDescriptorTestFixture
	{
		PropertyDescriptorCollection properties;
		XmlAttributePropertyDescriptor firstAttributePropertyDescriptor;
		XmlAttributePropertyDescriptor secondAttributePropertyDescriptor;
		XmlAttribute firstAttribute;
		
		[SetUp]
		public void Init()
		{
			XmlDocument document = new XmlDocument();
			document.LoadXml("<root first='a' second='b'/>");
			firstAttribute = document.DocumentElement.GetAttributeNode("first");
			properties = XmlAttributePropertyDescriptor.GetProperties(document.DocumentElement.Attributes);
			firstAttributePropertyDescriptor = (XmlAttributePropertyDescriptor)properties["first"];
			secondAttributePropertyDescriptor = (XmlAttributePropertyDescriptor)properties["second"];
		}
		
		[Test]
		public void TwoPropertyDescriptors()
		{
			Assert.AreEqual(2, properties.Count);
		}
		
		[Test]
		public void FirstPropertyName()
		{
			Assert.AreEqual("first", firstAttributePropertyDescriptor.Name);
		}
		
		[Test]
		public void FirstPropertyComponentType()
		{
			Assert.AreEqual(typeof(String), firstAttributePropertyDescriptor.ComponentType);
		}
		
		[Test]
		public void FirstPropertyReadOnly()
		{
			Assert.IsFalse(firstAttributePropertyDescriptor.IsReadOnly);
		}
		
		[Test]
		public void FirstPropertyType()
		{
			Assert.AreEqual(typeof(String), firstAttributePropertyDescriptor.PropertyType);
		}
		
		[Test]
		public void FirstPropertyCanResetValue()
		{
			Assert.IsFalse(firstAttributePropertyDescriptor.CanResetValue(null));
		}
		
		[Test]
		public void FirstPropertyShouldSerializeValue()
		{
			Assert.IsTrue(firstAttributePropertyDescriptor.ShouldSerializeValue(null));
		}
		
		[Test]
		public void FirstPropertyValue()
		{
			Assert.AreEqual("a", (String)firstAttributePropertyDescriptor.GetValue(null));
		}
		
		[Test]
		public void SecondPropertyValue()
		{
			Assert.AreEqual("b", (String)secondAttributePropertyDescriptor.GetValue(null));
		}

		[Test]
		public void SetFirstPropertyValue()
		{
			firstAttributePropertyDescriptor.SetValue(null, "new value");
			Assert.AreEqual("new value", (String)firstAttributePropertyDescriptor.GetValue(null));
			Assert.AreEqual("new value", firstAttribute.Value);
		}
		
		[Test]
		public void ResetValueDoesNothing()
		{
			firstAttributePropertyDescriptor.SetValue(null, "new value");
			firstAttributePropertyDescriptor.ResetValue(null);
			Assert.AreEqual("new value", firstAttribute.Value);
		}
	}
}
