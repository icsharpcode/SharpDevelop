//
// SharpDevelop Xml Editor
//
// Copyright (C) 2005 Matthew Ward
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// Matthew Ward (mrward@users.sourceforge.net)

using ICSharpCode.TextEditor.Gui.CompletionWindow;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using System;
using System.IO;
using System.Xml;

namespace XmlEditor.Tests.Schema
{
	/// <summary>
	/// Tests that the completion data retrieves the annotation documentation
	/// that an attribute value may have.
	/// </summary>
	[TestFixture]
	public class AttributeValueAnnotationTestFixture : SchemaTestFixtureBase
	{
		XmlSchemaCompletionData schemaCompletionData;
		ICompletionData[] barAttributeValuesCompletionData;
		
		[TestFixtureSetUp]
		public void FixtureInit()
		{
			StringReader reader = new StringReader(GetSchema());
			schemaCompletionData = new XmlSchemaCompletionData(reader);
			
			XmlElementPath path = new XmlElementPath();
			path.Elements.Add(new QualifiedName("foo", "http://foo.com"));
			barAttributeValuesCompletionData = schemaCompletionData.GetAttributeValueCompletionData(path, "bar");
		}
				
		[Test]
		public void BarAttributeValueDefaultDocumentation()
		{
			Assert.IsTrue(base.ContainsDescription(barAttributeValuesCompletionData, "default", "Default attribute value info."),
			                "Description for attribute value 'default' is incorrect.");
		}
		
		string GetSchema()
		{
			return "<xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\"\r\n" +
				"\ttargetNamespace=\"http://foo.com\"\r\n" +
				"\txmlns=\"http://foo.com\">\r\n" +
				"\t<xs:element name=\"foo\">\r\n" +
				"\t\t<xs:complexType>\r\n" +
				"\t\t\t<xs:attribute name=\"bar\">\r\n" +
				"\t\t\t\t<xs:simpleType>\r\n" +
				"\t\t\t\t\t<xs:restriction base=\"xs:NMTOKEN\">\r\n" +
				"\t\t\t\t\t\t<xs:enumeration value=\"default\">\r\n" +
				"\t\t\t\t\t\t\t<xs:annotation><xs:documentation>Default attribute value info.</xs:documentation></xs:annotation>\r\n" +
				"\t\t\t\t\t\t</xs:enumeration>\r\n" +
				"\t\t\t\t\t\t<xs:enumeration value=\"enable\">\r\n" +
				"\t\t\t\t\t\t\t<xs:annotation><xs:documentation>Enable attribute value info.</xs:documentation></xs:annotation>\r\n" +
				"\t\t\t\t\t\t</xs:enumeration>\r\n" +
				"\t\t\t\t\t\t<xs:enumeration value=\"disable\">\r\n" +
				"\t\t\t\t\t\t\t<xs:annotation><xs:documentation>Disable attribute value info.</xs:documentation></xs:annotation>\r\n" +
				"\t\t\t\t\t\t</xs:enumeration>\r\n" +
				"\t\t\t\t\t\t<xs:enumeration value=\"hide\">\r\n" +
				"\t\t\t\t\t\t\t<xs:annotation><xs:documentation>Hide attribute value info.</xs:documentation></xs:annotation>\r\n" +
				"\t\t\t\t\t\t</xs:enumeration>\r\n" +
				"\t\t\t\t\t\t<xs:enumeration value=\"show\">\r\n" +
				"\t\t\t\t\t\t\t<xs:annotation><xs:documentation>Show attribute value info.</xs:documentation></xs:annotation>\r\n" +
				"\t\t\t\t\t\t</xs:enumeration>\r\n" +
				"\t\t\t\t\t</xs:restriction>\r\n" +
				"\t\t\t\t</xs:simpleType>\r\n" +
				"\t\t\t</xs:attribute>\r\n" +
				"\t\t</xs:complexType>\r\n" +
				"\t</xs:element>\r\n" +
				"</xs:schema>";
		}		
	}
}
