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

namespace XmlEditor.Tests.Schema
{
	/// <summary>
	/// Tests attribute refs
	/// </summary>
	[TestFixture]
	public class AttributeRefTestFixture : SchemaTestFixtureBase
	{
		XmlSchemaCompletionData schemaCompletionData;
		ICompletionData[] attributes;
		
		[TestFixtureSetUp]
		public void FixtureInit()
		{
			StringReader reader = new StringReader(GetSchema());
			schemaCompletionData = new XmlSchemaCompletionData(reader);
			XmlElementPath path = new XmlElementPath();
			path.Elements.Add(new QualifiedName("html", "http://foo/xhtml"));
			attributes = schemaCompletionData.GetAttributeCompletionData(path);
		}
		
		[Test]
		public void HtmlAttributeCount()
		{
			Assert.AreEqual(4, attributes.Length, 
			                "Should be 4 attributes.");
		}
		
		[Test]
		public void HtmlLangAttribute()
		{
			Assert.IsTrue(base.Contains(attributes, "lang"), "Attribute lang not found.");
		}
		
		[Test]
		public void HtmlIdAttribute()
		{
			Assert.IsTrue(base.Contains(attributes, "id"), "Attribute id not found.");
		}		
		
		[Test]
		public void HtmlDirAttribute()
		{
			Assert.IsTrue(base.Contains(attributes, "dir"), "Attribute dir not found.");
		}			
		
		[Test]
		public void HtmlXmlLangAttribute()
		{
			Assert.IsTrue(base.Contains(attributes, "xml:lang"), "Attribute xml:lang not found.");
		}				
		
		string GetSchema()
		{
			return "<xs:schema version=\"1.0\" xml:lang=\"en\"\r\n" +
					"    xmlns:xs=\"http://www.w3.org/2001/XMLSchema\"\r\n" +
					"    targetNamespace=\"http://foo/xhtml\"\r\n" +
					"    xmlns=\"http://foo/xhtml\"\r\n" +
					"    elementFormDefault=\"qualified\">\r\n" +
					"  <xs:element name=\"html\">\r\n" +
					"    <xs:complexType>\r\n" +
					"      <xs:sequence>\r\n" +
					"        <xs:element ref=\"head\"/>\r\n" +
					"        <xs:element ref=\"body\"/>\r\n" +
					"      </xs:sequence>\r\n" +
					"      <xs:attributeGroup ref=\"i18n\"/>\r\n" +
					"      <xs:attribute name=\"id\" type=\"xs:ID\"/>\r\n" +
					"    </xs:complexType>\r\n" +
					"  </xs:element>\r\n" +
					"\r\n" +
					"  <xs:element name=\"head\" type=\"xs:string\"/>\r\n" +
					"  <xs:element name=\"body\" type=\"xs:string\"/>\r\n" +
					"\r\n" +
					"  <xs:attributeGroup name=\"i18n\">\r\n" +
					"    <xs:annotation>\r\n" +
					"      <xs:documentation>\r\n" +
					"      internationalization attributes\r\n" +
					"      lang        language code (backwards compatible)\r\n" +
					"      xml:lang    language code (as per XML 1.0 spec)\r\n" +
					"      dir         direction for weak/neutral text\r\n" +
					"      </xs:documentation>\r\n" +
					"    </xs:annotation>\r\n" +
					"    <xs:attribute name=\"lang\" type=\"LanguageCode\"/>\r\n" +
					"    <xs:attribute ref=\"xml:lang\"/>\r\n" +
					"\r\n" +
					"    <xs:attribute name=\"dir\">\r\n" +
					"      <xs:simpleType>\r\n" +
					"        <xs:restriction base=\"xs:token\">\r\n" +
					"          <xs:enumeration value=\"ltr\"/>\r\n" +
					"          <xs:enumeration value=\"rtl\"/>\r\n" +
					"        </xs:restriction>\r\n" +
					"      </xs:simpleType>\r\n" +
					"    </xs:attribute>\r\n" +
					"  </xs:attributeGroup>\r\n" +
					"</xs:schema>";
		}
	}
}
