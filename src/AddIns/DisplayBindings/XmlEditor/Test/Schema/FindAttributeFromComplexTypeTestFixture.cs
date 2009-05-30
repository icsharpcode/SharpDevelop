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

namespace XmlEditor.Tests.Schema
{
	/// <summary>
	/// Element that has a single attribute.
	/// </summary>
	[TestFixture]
	public class FindAttributeFromComplexTypeFixture : SchemaTestFixtureBase
	{
		XmlSchemaAttribute attribute;
		XmlSchemaAttribute missingAttribute;
		
		public override void FixtureInit()
		{
			XmlElementPath path = new XmlElementPath();
			path.Elements.Add(new QualifiedName("note", "http://www.w3schools.com"));
			
			XmlSchemaElement element = SchemaCompletionData.FindElement(path);
			attribute = SchemaCompletionData.FindAttribute(element, "name");
			missingAttribute = SchemaCompletionData.FindAttribute(element, "missing");
		}
		
		[Test]
		public void AttributeFound()
		{
			Assert.IsNotNull(attribute);
		}		
		
		[Test]
		public void CannotFindUnknownAttribute()
		{
			Assert.IsNull(missingAttribute);
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
