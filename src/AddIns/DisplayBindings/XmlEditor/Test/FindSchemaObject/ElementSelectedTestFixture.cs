// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 1683 $</version>
// </file>

using ICSharpCode.TextEditor.Gui.CompletionWindow;
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
		
		public override void FixtureInit()
		{
			XmlSchemaCompletionDataCollection schemas = new XmlSchemaCompletionDataCollection();
			schemas.Add(SchemaCompletionData);
			XmlCompletionDataProvider provider = new XmlCompletionDataProvider(schemas, SchemaCompletionData, String.Empty);
			string xml = "<note xmlns='http://www.w3schools.com'></note>";
			schemaElement = (XmlSchemaElement)XmlView.GetSchemaObjectSelected(xml, xml.IndexOf("note xmlns"), provider);
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
	}
}
