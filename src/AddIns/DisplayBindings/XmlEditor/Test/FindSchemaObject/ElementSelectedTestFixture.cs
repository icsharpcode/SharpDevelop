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
	public class ElementSelectedTestFixture : SchemaTestFixtureBase
	{		
		XmlSchemaElement schemaElement;
		XmlSchemaObjectLocation location;
		
		public override void FixtureInit()
		{
			XmlSchemaCompletionCollection schemas = new XmlSchemaCompletionCollection();
			schemas.Add(SchemaCompletion);
			
			string xml = "<note xmlns='http://www.w3schools.com'></note>";
			int index = xml.IndexOf("note xmlns");
			
			XmlSchemaDefinition schemaDefinition = new XmlSchemaDefinition(schemas, null);
			schemaElement = (XmlSchemaElement)schemaDefinition.GetSelectedSchemaObject(xml, index);
			location = schemaDefinition.GetSelectedSchemaObjectLocation(xml, index);
		}
		
		[Test]
		public void SchemaElementNamespace()
		{
			Assert.AreEqual("http://www.w3schools.com", 
			                schemaElement.QualifiedName.Namespace,
			                "Unexpected namespace.");
		}
		
		[Test]
		public void SchemaElementName()
		{
			Assert.AreEqual("note", schemaElement.QualifiedName.Name);
		}
		
		protected override string GetSchema()
		{
			return "<?xml version=\"1.0\"?>\r\n" +
				"<xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\"\r\n" +
				"targetNamespace=\"http://www.w3schools.com\"\r\n" +
				"xmlns=\"http://www.w3schools.com\"\r\n" +
				"elementFormDefault=\"qualified\">\r\n" +
				"<xs:element name=\"note\">\r\n" +
				"</xs:element>\r\n" +
				"</xs:schema>";
		}
		
		[Test]
		public void SchemaObjectLocationLinePositionMatchesSchemaElement()
		{
			Assert.AreEqual(schemaElement.LinePosition, location.LinePosition);
		}
		
		[Test]
		public void SchemaObjectLocationLineNumberMatchesSchemaElement()
		{
			Assert.AreEqual(schemaElement.LineNumber, location.LineNumber);
		}
	}
}
