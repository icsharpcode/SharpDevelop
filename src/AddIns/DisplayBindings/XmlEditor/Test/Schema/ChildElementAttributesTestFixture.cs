//
// ${App.Name}
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
	/// Child element attributes.
	/// </summary>
	[TestFixture]
	public class ChildElementAttributesTestFixture : SchemaTestFixtureBase
	{
		XmlSchemaCompletionData schemaCompletionData;
		ICompletionData[] attributes;
		
		[TestFixtureSetUp]
		public void FixtureInit()
		{
			StringReader reader = new StringReader(GetSchema());
			schemaCompletionData = new XmlSchemaCompletionData(reader);
						
			XmlElementPath path = new XmlElementPath();
			path.Elements.Add(new QualifiedName("project", "http://nant.sf.net//nant-0.84.xsd"));
			path.Elements.Add(new QualifiedName("attrib", "http://nant.sf.net//nant-0.84.xsd"));
			
			attributes = schemaCompletionData.GetAttributeCompletionData(path);
		}

		[Test]
		public void AttributeCount()
		{
			Assert.AreEqual(10, attributes.Length, "Should be one attribute.");
		}
		
		[Test]
		public void FileAttribute()
		{
			Assert.IsTrue(base.Contains(attributes, "file"),
			              "Attribute file does not exist.");
		}		
		
		string GetSchema()
		{
			return "<xs:schema xmlns:vs=\"urn:schemas-microsoft-com:HTML-Intellisense\" xmlns:nant=\"http://nant.sf.net//nant-0.84.xsd\" targetNamespace=\"http://nant.sf.net//nant-0.84.xsd\" xmlns:xs=\"http://www.w3.org/2001/XMLSchema\">\r\n" +
					"  <xs:element name=\"project\">\r\n" +
					"    <xs:complexType>\r\n" +
					"      <xs:sequence minOccurs=\"0\" maxOccurs=\"unbounded\">\r\n" +
					"        <xs:sequence minOccurs=\"0\" maxOccurs=\"unbounded\">\r\n" +
					"          <xs:sequence minOccurs=\"0\" maxOccurs=\"unbounded\">\r\n" +
					"            <xs:element name=\"attrib\" type=\"nant:attrib\" />\r\n" +
					"          </xs:sequence>\r\n" +
					"        </xs:sequence>\r\n" +
					"      </xs:sequence>\r\n" +
					"      <xs:attribute name=\"name\" use=\"required\" />\r\n" +
					"      <xs:attribute name=\"default\" use=\"optional\" />\r\n" +
					"      <xs:attribute name=\"basedir\" use=\"optional\" />\r\n" +
					"    </xs:complexType>\r\n" +
					"  </xs:element>\r\n" +
					"\r\n" +
					"  <xs:complexType id=\"NAnt.Core.Tasks.AttribTask\" name=\"attrib\">\r\n" +
					"    <xs:attribute name=\"file\" use=\"optional\" />\r\n" +
					"    <xs:attribute name=\"archive\" use=\"optional\" />\r\n" +
					"    <xs:attribute name=\"hidden\" use=\"optional\" />\r\n" +
					"    <xs:attribute name=\"normal\" use=\"optional\" />\r\n" +
					"    <xs:attribute name=\"readonly\" use=\"optional\" />\r\n" +
					"    <xs:attribute name=\"system\" use=\"optional\" />\r\n" +
					"    <xs:attribute name=\"failonerror\" use=\"optional\" />\r\n" +
					"    <xs:attribute name=\"verbose\" use=\"optional\" />\r\n" +
					"    <xs:attribute name=\"if\" use=\"optional\" />\r\n" +
					"    <xs:attribute name=\"unless\" use=\"optional\" />\r\n" +
					"  </xs:complexType>\r\n" +
					"</xs:schema>";
		}
	}
}
