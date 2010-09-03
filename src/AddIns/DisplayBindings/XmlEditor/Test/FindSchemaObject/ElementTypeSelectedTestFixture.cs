// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.XmlEditor;
using NUnit.Framework;
using System;
using System.IO;
using System.Xml.Schema;
using XmlEditor.Tests.Schema;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.FindSchemaObject
{
	/// <summary>
	/// Tests that an xs:element/@type is located in the schema.
	/// </summary>
	[TestFixture]
	public class ElementTypeSelectedTestFixture : SchemaTestFixtureBase
	{
		XmlSchemaComplexType schemaComplexType;
		
		public override void FixtureInit()
		{
			XmlSchemaCompletionCollection schemas = new XmlSchemaCompletionCollection();
			schemas.Add(SchemaCompletion);
			XmlSchemaCompletion xsdSchemaCompletion = new XmlSchemaCompletion(ResourceManager.ReadXsdSchema());
			schemas.Add(xsdSchemaCompletion);
			
			string xml = GetSchema();
			int index = xml.IndexOf("type=\"text-type\"");
			index = xml.IndexOf("text-type", index);
			XmlSchemaDefinition schemaDefinition = new XmlSchemaDefinition(schemas, SchemaCompletion);
			schemaComplexType = (XmlSchemaComplexType)schemaDefinition.GetSelectedSchemaObject(xml, index);
		}
		
		[Test]
		public void ComplexTypeName()
		{
			Assert.AreEqual("text-type", schemaComplexType.QualifiedName.Name);
		}
		
		protected override string GetSchema()
		{
			return "<xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" targetNamespace=\"http://www.w3schools.com\" xmlns=\"http://www.w3schools.com\" elementFormDefault=\"qualified\">\r\n" +
				"\t<xs:element name=\"note\">\r\n" +
				"\t\t<xs:complexType> \r\n" +
				"\t\t\t<xs:sequence>\r\n" +
				"\t\t\t\t<xs:element name=\"text\" type=\"text-type\"/>\r\n" +
				"\t\t\t</xs:sequence>\r\n" +
				"\t\t</xs:complexType>\r\n" +
				"\t</xs:element>\r\n" +
				"\t<xs:complexType name=\"text-type\">\r\n" +
				"\t\t<xs:attribute name=\"foo\"/>\r\n" +
				"\t</xs:complexType>\r\n" +
				"</xs:schema>";
		}
	}
}
