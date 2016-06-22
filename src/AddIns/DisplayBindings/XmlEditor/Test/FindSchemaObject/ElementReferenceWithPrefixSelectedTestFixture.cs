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
	/// Tests that an xs:element/@ref='prefix:name' is located in the schema.
	/// </summary>
	[TestFixture]
	public class ElementReferenceWithPrefixSelectedTestFixture : SchemaTestFixtureBase
	{
		XmlSchemaElement referencedSchemaElement;
		
		public override void FixtureInit()
		{
			XmlSchemaCompletionCollection schemas = new XmlSchemaCompletionCollection();
			schemas.Add(SchemaCompletion);
			XmlSchemaCompletion xsdSchemaCompletionData = new XmlSchemaCompletion(ResourceManager.ReadXsdSchema());
			schemas.Add(xsdSchemaCompletionData);
			
			string xml = GetSchema();
			
			int index = xml.IndexOf("ref=\"xs:list");
			index = xml.IndexOf("xs", index);
			XmlSchemaDefinition schemaDefinition = new XmlSchemaDefinition(schemas, SchemaCompletion);
			referencedSchemaElement = (XmlSchemaElement)schemaDefinition.GetSelectedSchemaObject(xml, index);
		}
		
		[Test]
		public void ReferencedElementName()
		{
			Assert.AreEqual("list", referencedSchemaElement.QualifiedName.Name);
		}
		
		[Test]
		public void ReferencedElementNamespace()
		{
			Assert.AreEqual("http://www.w3.org/2001/XMLSchema", referencedSchemaElement.QualifiedName.Namespace);
		}

		protected override string GetSchema()
		{
			return "<xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" targetNamespace=\"http://www.w3schools.com\"  xmlns=\"http://www.w3schools.com\">\r\n" +
				"\r\n" +
				"<!-- definition of simple elements -->\r\n" +
				"<xs:element name=\"name\" type=\"xs:string\"/>\r\n" +
				"<xs:element name=\"address\" type=\"xs:string\"/>\r\n" +
				"\r\n" +
				"<!-- definition of complex elements -->\r\n" +
				"<xs:element name=\"shipto\">\r\n" +
				" <xs:complexType>\r\n" +
				"  <xs:sequence>\r\n" +
				"   <xs:element ref=\"name\"/>\r\n" +
				"   <xs:element ref=\"xs:list\"/>\r\n" +
				"  </xs:sequence>\r\n" +
				"  <xs:attribute name=\"address\"/>\r\n" +
				" </xs:complexType>\r\n" +
				"</xs:element>\r\n" +
				"\r\n" +
				"<xs:element name=\"shiporder\">\r\n" +
				" <xs:complexType>\r\n" +
				"  <xs:sequence>\r\n" +
				"   <xs:element ref=\"shipto\"/>\r\n" +
				"  </xs:sequence>\r\n" +
				"  <xs:attribute name=\"id\"/>\r\n" +
				" </xs:complexType>\r\n" +
				"</xs:element>\r\n" +
				"\r\n" +
				"</xs:schema>";
		}
	}
}
