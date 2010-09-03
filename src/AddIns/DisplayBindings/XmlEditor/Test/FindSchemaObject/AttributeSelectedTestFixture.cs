// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.XmlEditor;
using NUnit.Framework;
using System;
using System.IO;
using System.Xml.Schema;
using XmlEditor.Tests.Schema;

namespace XmlEditor.Tests.FindSchemaObject
{
	[TestFixture]
	public class AttributeSelectedTestFixture : SchemaTestFixtureBase
	{
		XmlSchemaAttribute schemaAttribute;
		
		public override void FixtureInit()
		{
			XmlSchemaCompletionCollection schemas = new XmlSchemaCompletionCollection();
			schemas.Add(SchemaCompletion);
			string xml = "<note xmlns='http://www.w3schools.com' name=''></note>";
			XmlSchemaDefinition schemaDefinition = new XmlSchemaDefinition(schemas, null);
			schemaAttribute = (XmlSchemaAttribute)schemaDefinition.GetSelectedSchemaObject(xml, xml.IndexOf("name"));
		}
		
		[Test]
		public void AttributeName()
		{
			Assert.AreEqual("name", schemaAttribute.QualifiedName.Name, "Attribute name is incorrect.");
		}		
		
		protected override string GetSchema()
		{
			return "<xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" targetNamespace=\"http://www.w3schools.com\" xmlns=\"http://www.w3schools.com\" elementFormDefault=\"qualified\">\r\n" +
				"    <xs:element name=\"note\">\r\n" +
				"        <xs:complexType>\r\n" +
				"            <xs:attribute name=\"name\"  type=\"xs:string\"/>\r\n" +
				"        </xs:complexType>\r\n" +
				"    </xs:element>\r\n" +
				"</xs:schema>";
		}
	}
}
