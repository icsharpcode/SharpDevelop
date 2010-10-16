// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;

using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Schema.Multiple
{
	[TestFixture]
	public class DuplicateSchemaNamespaceAddedToCollectionTestFixture
	{
		XmlSchemaCompletionCollection schemas;
		XmlSchemaCompletion fooSchema;
		XmlSchemaCompletion barSchema;
		XmlSchemaCompletion duplicateFooSchema;
		
		[SetUp]
		public void Init()
		{
			CreateBarSchema();
			CreateFooSchema();
			CreateDuplicateFooSchema();
			
			schemas = new XmlSchemaCompletionCollection();
			schemas.Add(fooSchema);
			schemas.Add(barSchema);
			schemas.Add(duplicateFooSchema);
		}
		
		void CreateFooSchema()
		{
			string xml =
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

			fooSchema = new XmlSchemaCompletion(new StringReader(xml));
			fooSchema.FileName = "foo.xsd";
		}
		
		void CreateBarSchema()
		{
			string xml = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' targetNamespace='bar' />";
			barSchema = new XmlSchemaCompletion(new StringReader(xml));
			barSchema.FileName = "bar.xsd";
		}
		
		void CreateDuplicateFooSchema()
		{
			string xml =
				"<xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" targetNamespace=\"foo\" xmlns=\"foo\" elementFormDefault=\"qualified\">\r\n" +
				"    <xs:element name=\"duplicate-foo-note\">\r\n" +
				"        <xs:complexType> \r\n" +
				"            <xs:sequence>\r\n" +
				"                <xs:element name=\"duplicate-foo-text\" type=\"duplicate-text-type\"/>\r\n" +
				"            </xs:sequence>\r\n" +
				"        </xs:complexType>\r\n" +
				"    </xs:element>\r\n" +
				"    <xs:complexType name=\"duplicate-text-type\">\r\n" +
				"        <xs:attribute name=\"duplicate-foo-text-attribute\">\r\n" +
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

			duplicateFooSchema = new XmlSchemaCompletion(new StringReader(xml));
			duplicateFooSchema.FileName = "duplicate-foo.xsd";
		}
		
		[Test]
		public void SchemasHaveTheSameNamespace()
		{
			Assert.AreEqual(fooSchema.NamespaceUri, duplicateFooSchema.NamespaceUri);
		}
		
		[Test]
		public void SchemaNamespaceIsNotEmptyString()
		{
			Assert.AreEqual("foo", fooSchema.NamespaceUri);
		}
		
		[Test]
		public void ThreeSchemasInCollection()
		{
			Assert.AreEqual(3, schemas.Count);
		}
		
		[Test]
		public void GetSchemasForFooNamespaceReturnsTwoSchemas()
		{
			Assert.AreEqual(2, schemas.GetSchemas("foo").Count);
		}
		
		[Test]
		public void GetSchemasForFooNamespaceReturnedSchemasContainDuplicateSchemas()
		{
			XmlSchemaCompletionCollection matchedSchemas = schemas.GetSchemas("foo");
			string[] expectedFileNames = new string[] {"foo.xsd", "duplicate-foo.xsd"};

			Assert.AreEqual(expectedFileNames, XmlSchemaCompletionCollectionFileNames.GetFileNames(matchedSchemas));
		}
		
		[Test]
		public void GetSchemasForElementPathReturnsEmptyCollectionWhenNoNamespaceUsedInPathAndNoDefaultSchema()
		{
			XmlElementPath path = new XmlElementPath();
			path.AddElement(new QualifiedName("root", String.Empty));
			
			XmlSchemaCompletionCollection foundSchemas = schemas.GetSchemas(path, null);
			Assert.AreEqual(0, foundSchemas.Count);
		}
		
		[Test]
		public void GetSchemasForElementPathReturnsDefaultSchemaWhenNoNamespaceUsedInPathButHasDefaultSchema()
		{
			XmlElementPath path = new XmlElementPath();
			path.AddElement(new QualifiedName("root", String.Empty));
			
			XmlSchemaCompletion defaultSchema = new XmlSchemaCompletion();
			defaultSchema.FileName = "default.xsd";
			
			XmlSchemaCompletionCollection foundSchemas = schemas.GetSchemas(path, defaultSchema);
			
			string[] expectedFileNames = new string[] {"default.xsd"};
			Assert.AreEqual(expectedFileNames, XmlSchemaCompletionCollectionFileNames.GetFileNames(foundSchemas));
		}
		
		[Test]
		public void GetSchemasForElementPathUpdatesElementNamespacesUsingDefaultSchemaNamespace()
		{
			XmlElementPath path = new XmlElementPath();
			path.AddElement(new QualifiedName("root", String.Empty));
			path.AddElement(new QualifiedName("different-ns-element", "unknown-namespace"));
			path.AddElement(new QualifiedName("child", String.Empty));
			
			string xml = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' targetNamespace='default-ns' />";
			XmlSchemaCompletion defaultSchema = new XmlSchemaCompletion(new StringReader(xml));
			defaultSchema.FileName = "default.xsd";
			
			schemas.GetSchemas(path, defaultSchema);
			
			XmlElementPath expectedPath = new XmlElementPath();
			expectedPath.AddElement(new QualifiedName("root", "default-ns"));
			expectedPath.AddElement(new QualifiedName("different-ns-element", "unknown-namespace"));
			expectedPath.AddElement(new QualifiedName("child", "default-ns"));
			
			Assert.AreEqual(expectedPath, path);
		}
		
		[Test]
		public void GetSchemasForElementPathReturnsDuplicateFooSchemasWhenFooNamespaceUsedInPath()
		{
			XmlElementPath path = new XmlElementPath();
			path.AddElement(new QualifiedName("root", "foo"));
			
			XmlSchemaCompletionCollection foundSchemas = schemas.GetSchemas(path, null);
			string[] expectedFileNames = new string[] {"foo.xsd", "duplicate-foo.xsd"};
			Assert.AreEqual(expectedFileNames, XmlSchemaCompletionCollectionFileNames.GetFileNames(foundSchemas));
		}
		
		[Test]
		public void NamespaceCompletionDoesNotContainDuplicateNamespaces()
		{
			XmlCompletionItemCollection items = schemas.GetNamespaceCompletion();
			items.Sort();
			
			List<XmlCompletionItem> expectedItems = new List<XmlCompletionItem>();
			expectedItems.Add(new XmlCompletionItem("bar", XmlCompletionItemType.NamespaceUri));
			expectedItems.Add(new XmlCompletionItem("foo", XmlCompletionItemType.NamespaceUri));
			
			Assert.AreEqual(expectedItems.ToArray(), items.ToArray());
		}
		
		[Test]
		public void GetRootElementCompletionReturnsRootElementsFromBothFooSchemas()
		{
			XmlNamespaceCollection namespaces = new XmlNamespaceCollection();
			namespaces.Add(new XmlNamespace(String.Empty, "foo"));
			XmlCompletionItemCollection items = schemas.GetRootElementCompletion(namespaces);
			items.Sort();
			
			List<XmlCompletionItem> expectedItems = new List<XmlCompletionItem>();
			expectedItems.Add(new XmlCompletionItem("duplicate-foo-note", XmlCompletionItemType.XmlElement));
			expectedItems.Add(new XmlCompletionItem("foo-note", XmlCompletionItemType.XmlElement));
			
			Assert.AreEqual(expectedItems.ToArray(), items.ToArray());
		}
		
		[Test]
		public void GetChildElementCompletionForDuplicateFooRootElement()
		{
			XmlElementPath path = new XmlElementPath();
			path.AddElement(new QualifiedName("duplicate-foo-note", "foo"));
			XmlCompletionItemCollection items = schemas.GetChildElementCompletion(path, null);
			items.Sort();
			
			List<XmlCompletionItem> expectedItems = new List<XmlCompletionItem>();
			expectedItems.Add(new XmlCompletionItem("duplicate-foo-text", XmlCompletionItemType.XmlElement));
			
			Assert.AreEqual(expectedItems.ToArray(), items.ToArray());
		}
		
		[Test]
		public void GetAttributeCompletionReturnsAttributesFromDuplicateFooSchema()
		{
			string xml = 
				"<duplicate-foo-note xmlns='foo'>\r\n" +
				"        <duplicate-foo-text ";
			
			XmlCompletionItemCollection items = schemas.GetAttributeCompletion(xml, null);
			
			XmlCompletionItemCollection expectedItems = new XmlCompletionItemCollection();
			expectedItems.Add(new XmlCompletionItem("duplicate-foo-text-attribute", XmlCompletionItemType.XmlAttribute));
			
			Assert.AreEqual(expectedItems, items);
		}
		
		[Test]
		public void GetAttributeValueCompletionReturnsValuesForDuplicateFooSchema()
		{
			string xml = 
				"<duplicate-foo-note xmlns='foo'>\r\n" +
				"    <duplicate-foo-text duplicate-foo-text-attribute='f'";
			
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
