// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.XmlEditor;
using NUnit.Framework;

namespace XmlEditor.Tests.Schema
{
	/// <summary>
	/// Element that has a single attribute.
	/// </summary>
	[TestFixture]
	public class ElementWithAttributeSchemaTestFixture : SchemaTestFixtureBase
	{
		XmlCompletionItemCollection attributeCompletionItems;
		string attributeName;
		
		public override void FixtureInit()
		{
			XmlElementPath path = new XmlElementPath();
			path.AddElement(new QualifiedName("note", "http://www.w3schools.com"));
			
			attributeCompletionItems = SchemaCompletion.GetAttributeCompletion(path);
			attributeName = attributeCompletionItems[0].Text;
		}

		[Test]
		public void AttributeCount()
		{
			Assert.AreEqual(1, attributeCompletionItems.Count, "Should be one attribute.");
		}
		
		[Test]
		public void AttributeName()
		{
			Assert.AreEqual("name", attributeName, "Attribute name is incorrect.");
		}		
		
		[Test]
		public void NoAttributesForUnknownElement()
		{
			XmlElementPath path = new XmlElementPath();
			path.AddElement(new QualifiedName("foobar", "http://www.w3schools.com"));
			XmlCompletionItemCollection attributes = SchemaCompletion.GetAttributeCompletion(path);
			
			Assert.AreEqual(0, attributes.Count, "Should not find attributes for unknown element.");
		}
		
		protected override string GetSchema()
		{
			return "<xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" targetNamespace=\"http://www.w3schools.com\" xmlns=\"http://www.w3schools.com\" elementFormDefault=\"qualified\">\r\n" +
				"    <xs:element name=\"note\">\r\n" +
				"        <xs:complexType>\r\n" +
				"\t<xs:attribute name=\"name\"  type=\"xs:string\"/>\r\n" +
				"        </xs:complexType>\r\n" +
				"    </xs:element>\r\n" +
				"</xs:schema>";
		}
	}
}
