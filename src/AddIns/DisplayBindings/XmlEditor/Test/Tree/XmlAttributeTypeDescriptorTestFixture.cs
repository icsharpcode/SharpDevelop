// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.XmlEditor;
using NUnit.Framework;
using System;
using System.ComponentModel;
using System.Xml;

namespace XmlEditor.Tests.Tree
{
	[TestFixture]
	public class XmlAttributeTypeDescriptorTestFixture
	{
		XmlAttributeTypeDescriptor typeDescriptor;
		
		[SetUp]
		public void Init()
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml("<root first='a'/>");
			typeDescriptor = new XmlAttributeTypeDescriptor(doc.DocumentElement.Attributes);
		}
		
		[Test]
		public void OneProperty()
		{
			PropertyDescriptorCollection properties = typeDescriptor.GetProperties();
			Assert.AreEqual(1, properties.Count);
		}
		
		[Test]
		public void PropertyName()
		{
			PropertyDescriptorCollection properties = typeDescriptor.GetProperties();
			PropertyDescriptor descriptor = properties[0];
			Assert.AreEqual("first", descriptor.Name);
		}
		
		[Test]
		public void PropertyOwner()
		{
			Assert.IsTrue(Object.ReferenceEquals(typeDescriptor, typeDescriptor.GetPropertyOwner(null)));
		}
	}
}
