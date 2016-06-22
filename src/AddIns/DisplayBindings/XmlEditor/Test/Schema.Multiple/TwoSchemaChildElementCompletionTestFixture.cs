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

using System;
using System.IO;
using ICSharpCode.XmlEditor;
using NUnit.Framework;

namespace XmlEditor.Tests.Schema.Multiple
{
	[TestFixture]
	public class TwoSchemaChildElementCompletionTestFixture
	{
		XmlSchemaCompletionCollection schemas;
		
		[SetUp]
		public void Init()
		{
			schemas = new XmlSchemaCompletionCollection();
			foreach (string schema in GetSchemas()) {
				StringReader reader = new StringReader(schema);
				schemas.Add(new XmlSchemaCompletion(reader));
			}
		}
		
		string[] GetSchemas()
		{
			return new string[] { GetFooSchema(), GetBarSchema() };
		}
		
		string GetFooSchema()
		{
			return 
				"<xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" targetNamespace=\"foo\" xmlns=\"foo\" elementFormDefault=\"qualified\">\r\n" +
				"    <xs:element name=\"foo-note\">\r\n" +
				"        <xs:complexType> \r\n" +
				"            <xs:sequence>\r\n" +
				"                <xs:element name=\"foo-text\" type=\"text-type\"/>\r\n" +
				"            </xs:sequence>\r\n" +
				"        </xs:complexType>\r\n" +
				"    </xs:element>\r\n" +
				"    <xs:complexType name=\"text-type\">\r\n" +
				"        <xs:attribute name=\"foo-text-attribute\">\r\n" +
				"            <xs:simpleType>\r\n" +
				"                <xs:restriction base=\"xs:string\">\r\n" +
				"                    <xs:enumeration value=\"first\"/>\r\n" +
				"                    <xs:enumeration value=\"second\"/>\r\n" +
				"                    <xs:enumeration value=\"third\"/>\r\n" +
				"                    <xs:enumeration value=\"fourth\"/>\r\n" +
				"                </xs:restriction>\r\n" +
				"            </xs:simpleType>\r\n" +
				"        </xs:attribute>\r\n" +
				"    </xs:complexType>\r\n" +
				"</xs:schema>";
		}
		
		string GetBarSchema()
		{
			return 
				"<xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" targetNamespace=\"bar\" xmlns=\"bar\" elementFormDefault=\"qualified\">\r\n" +
				"    <xs:element name=\"bar-note\">\r\n" +
				"        <xs:complexType> \r\n" +
				"            <xs:sequence>\r\n" +
				"                <xs:element name=\"bar-text\" type=\"text-type\"/>\r\n" +
				"            </xs:sequence>\r\n" +
				"        </xs:complexType>\r\n" +
				"    </xs:element>\r\n" +
				"    <xs:complexType name=\"text-type\">\r\n" +
				"        <xs:attribute name=\"bar-text-attribute\">\r\n" +
				"            <xs:simpleType>\r\n" +
				"                <xs:restriction base=\"xs:string\">\r\n" +
				"                    <xs:enumeration value=\"bar-first\"/>\r\n" +
				"                    <xs:enumeration value=\"bar-second\"/>\r\n" +
				"                    <xs:enumeration value=\"bar-third\"/>\r\n" +
				"                    <xs:enumeration value=\"bar-fourth\"/>\r\n" +
				"                </xs:restriction>\r\n" +
				"            </xs:simpleType>\r\n" +
				"        </xs:attribute>\r\n" +
				"    </xs:complexType>\r\n" +
				"</xs:schema>";
		}
		
		[Test]
		public void FooSchemaRootElementAndBarRootElementInPath()
		{
			XmlElementPath path = new XmlElementPath();
			path.AddElement(new QualifiedName("foo-note", "foo"));
			path.AddElement(new QualifiedName("bar-note", "bar", "b"));
			XmlCompletionItemCollection items = schemas.GetElementCompletionForAllNamespaces(path, null);
			items.Sort();
			
			XmlCompletionItemCollection expectedItems = new XmlCompletionItemCollection();
			expectedItems.Add(new XmlCompletionItem("b:bar-text", XmlCompletionItemType.XmlElement));
			expectedItems.Add(new XmlCompletionItem("foo-text", XmlCompletionItemType.XmlElement));
			
			Assert.AreEqual(expectedItems, items);
		}
		
		[Test]
		public void FooSchemaElementAndBarElementInXml()
		{
			string xml = 
				"<foo-note xmlns='foo' xmlns:b='bar'>\r\n" +
				"    <b:bar-note>\r\n" +
				"        <";
			
			XmlCompletionItemCollection expectedItems = new XmlCompletionItemCollection();
			expectedItems.Add(new XmlCompletionItem("b:bar-text", XmlCompletionItemType.XmlElement));
			expectedItems.Add(new XmlCompletionItem("foo-text", XmlCompletionItemType.XmlElement));
			
			XmlCompletionItemCollection items = schemas.GetElementCompletion(xml, null);
			items.Sort();
			
			Assert.AreEqual(expectedItems, items);
		}
		
		[Test]
		public void FooSchemaRootElementOnlyInPath()
		{
			XmlElementPath path = new XmlElementPath();
			path.NamespacesInScope.Add(new XmlNamespace("b", "bar"));
			path.AddElement(new QualifiedName("foo-note", "foo"));
			
			XmlCompletionItemCollection items = schemas.GetElementCompletionForAllNamespaces(path, null);
			items.Sort();
			
			XmlCompletionItemCollection expectedItems = new XmlCompletionItemCollection();
			expectedItems.Add(new XmlCompletionItem("b:bar-note", XmlCompletionItemType.XmlElement));
			expectedItems.Add(new XmlCompletionItem("foo-text", XmlCompletionItemType.XmlElement));
			
			Assert.AreEqual(expectedItems, items);
		}
		
		[Test]
		public void UnusedNamespaceInScopeCannotBeFoundInSchemaCollection()
		{
			XmlElementPath path = new XmlElementPath();
			path.NamespacesInScope.Add(new XmlNamespace("b", "namespace-which-does-not-exist-in-schema-collection"));
			path.AddElement(new QualifiedName("foo-note", "foo"));
			
			XmlCompletionItemCollection items = schemas.GetElementCompletionForAllNamespaces(path, null);
			items.Sort();
			
			XmlCompletionItemCollection expectedItems = new XmlCompletionItemCollection();
			expectedItems.Add(new XmlCompletionItem("foo-text", XmlCompletionItemType.XmlElement));
			
			Assert.AreEqual(expectedItems, items);
		}
		
		[Test]
		public void DefaultSchemaRootElementsReturnedWhenNoNamespaceExplicitlyDefinedInXmlAndXmlIsEmpty()
		{
			XmlSchemaCompletion defaultSchema = schemas["foo"];
			string xml = "<";
			
			XmlCompletionItemCollection items = schemas.GetElementCompletion(xml, defaultSchema);
			items.Sort();
			
			XmlCompletionItemCollection expectedItems = new XmlCompletionItemCollection();
			expectedItems.Add(new XmlCompletionItem("foo-note", XmlCompletionItemType.XmlElement));
			
			Assert.AreEqual(expectedItems, items);
		}
		
		[Test]
		public void DefaultSchemaRootElementsReturnedOnceWhenNamespaceDefinedButNoElementsExistForthatNamespace()
		{
			XmlSchemaCompletion defaultSchema = schemas["foo"];
			string xml = "<b:bar-note xmlns='foo' xmlns:b='bar'><";
			
			XmlCompletionItemCollection items = schemas.GetElementCompletion(xml, defaultSchema);
			items.Sort();
			
			XmlCompletionItemCollection expectedItems = new XmlCompletionItemCollection();
			expectedItems.Add(new XmlCompletionItem("b:bar-text", XmlCompletionItemType.XmlElement));
			expectedItems.Add(new XmlCompletionItem("foo-note", XmlCompletionItemType.XmlElement));
			
			Assert.AreEqual(expectedItems, items);
		}
		
		[Test]
		public void CorrectAttributesReturnedForElementWhenTwoNamespacesInXml()
		{
			string xml = 
				"<b:bar-note xmlns='foo' xmlns:b='bar'>\r\n" +
				"    <foo-note>\r\n" +
				"        <b:bar-text/>\r\n" +
				"        <foo-text ";
			
			XmlCompletionItemCollection items = schemas.GetAttributeCompletion(xml, null);
			
			XmlCompletionItemCollection expectedItems = new XmlCompletionItemCollection();
			expectedItems.Add(new XmlCompletionItem("foo-text-attribute", XmlCompletionItemType.XmlAttribute));
			
			Assert.AreEqual(expectedItems, items);
		}
		
		[Test]
		public void CorrectAttributeValuesReturnedForElementWhenTwoNamespacesInXml()
		{
			string xml = 
				"<b:bar-note xmlns='foo' xmlns:b='bar'>\r\n" +
				"    <foo-note>\r\n" +
				"        <b:bar-text/>\r\n" +
				"        <foo-text foo-text-attribute='f'";
			
			string xmlUpToCursor = xml.Substring(0, xml.Length - 1);
			
			XmlCompletionItemCollection items = schemas.GetAttributeValueCompletion('f', xmlUpToCursor, null);
			items.Sort();
			
			XmlCompletionItemCollection expectedItems = new XmlCompletionItemCollection();
			expectedItems.Add(new XmlCompletionItem("first", XmlCompletionItemType.XmlAttributeValue));
			expectedItems.Add(new XmlCompletionItem("fourth", XmlCompletionItemType.XmlAttributeValue));
			expectedItems.Add(new XmlCompletionItem("second", XmlCompletionItemType.XmlAttributeValue));
			expectedItems.Add(new XmlCompletionItem("third", XmlCompletionItemType.XmlAttributeValue));
			
			Assert.AreEqual(expectedItems, items);
		}
	}
}
