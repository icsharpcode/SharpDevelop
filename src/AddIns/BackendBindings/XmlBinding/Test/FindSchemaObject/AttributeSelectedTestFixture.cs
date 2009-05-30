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
	public class AttributeSelectedTestFixture : SchemaTestFixtureBase
	{
		XmlSchemaAttribute schemaAttribute;
		
		public override void FixtureInit()
		{
			XmlSchemaCompletionDataCollection schemas = new XmlSchemaCompletionDataCollection();
			schemas.Add(SchemaCompletionData);
			XmlCompletionDataProvider provider = new XmlCompletionDataProvider(schemas, SchemaCompletionData, String.Empty);
			string xml = "<note xmlns='http://www.w3schools.com' name=''></note>";
			schemaAttribute = (XmlSchemaAttribute)XmlView.GetSchemaObjectSelected(xml, xml.IndexOf("name"), provider);
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
				"\t<xs:attribute name=\"name\"  type=\"xs:string\"/>\r\n" +
				"        </xs:complexType>\r\n" +
				"    </xs:element>\r\n" +
				"</xs:schema>";
		}
	}
}
