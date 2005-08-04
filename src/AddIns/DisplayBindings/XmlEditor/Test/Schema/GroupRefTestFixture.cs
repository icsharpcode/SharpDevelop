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
	/// Tests element group refs
	/// </summary>
	[TestFixture]
	public class GroupRefTestFixture : SchemaTestFixtureBase
	{
		XmlSchemaCompletionData schemaCompletionData;
		ICompletionData[] childElements;
		ICompletionData[] paraAttributes;
		
		[TestFixtureSetUp]
		public void FixtureInit()
		{
			StringReader reader = new StringReader(GetSchema());
			schemaCompletionData = new XmlSchemaCompletionData(reader);
			XmlElementPath path = new XmlElementPath();
			
			path.Elements.Add(new QualifiedName("html", "http://foo/xhtml"));
			path.Elements.Add(new QualifiedName("body", "http://foo/xhtml"));
			
			childElements = schemaCompletionData.GetChildElementCompletionData(path);
			
			path.Elements.Add(new QualifiedName("p", "http://foo/xhtml"));
			paraAttributes = schemaCompletionData.GetAttributeCompletionData(path);
		}
		
		[Test]
		public void BodyHasFourChildElements()
		{
			Assert.AreEqual(4, childElements.Length, 
			                "Should be 4 child elements.");
		}
		
		[Test]
		public void BodyChildElementForm()
		{
			Assert.IsTrue(base.Contains(childElements, "form"), 
			              "Should have a child element called form.");
		}
		
		[Test]
		public void BodyChildElementPara()
		{
			Assert.IsTrue(base.Contains(childElements, "p"), 
			              "Should have a child element called p.");
		}		
		
		[Test]
		public void BodyChildElementTest()
		{
			Assert.IsTrue(base.Contains(childElements, "test"), 
			              "Should have a child element called test.");
		}		
		
		[Test]
		public void BodyChildElementId()
		{
			Assert.IsTrue(base.Contains(childElements, "id"), 
			              "Should have a child element called id.");
		}		
		
		[Test]
		public void ParaElementHasIdAttribute()
		{
			Assert.IsTrue(base.Contains(paraAttributes, "id"), 
			              "Should have an attribute called id.");			
		}
		
		string GetSchema()
		{
			return "<xs:schema version=\"1.0\" xml:lang=\"en\"\r\n" +
				"    xmlns:xs=\"http://www.w3.org/2001/XMLSchema\"\r\n" +
				"    targetNamespace=\"http://foo/xhtml\"\r\n" +
				"    xmlns=\"http://foo/xhtml\"\r\n" +
				"    elementFormDefault=\"qualified\">\r\n" +
				"\r\n" +
				"  <xs:element name=\"html\">\r\n" +
				"    <xs:complexType>\r\n" +
				"      <xs:sequence>\r\n" +
				"        <xs:element ref=\"head\"/>\r\n" +
				"        <xs:element ref=\"body\"/>\r\n" +
				"      </xs:sequence>\r\n" +
				"    </xs:complexType>\r\n" +
				"  </xs:element>\r\n" +
				"\r\n" +
				"  <xs:element name=\"head\" type=\"xs:string\"/>\r\n" +
				"  <xs:element name=\"body\">\r\n" +
				"    <xs:complexType>\r\n" +
				"      <xs:sequence>\r\n" +
				"        <xs:group ref=\"block\"/>\r\n" +
				"        <xs:element name=\"form\"/>\r\n" +
				"      </xs:sequence>\r\n" +
				"    </xs:complexType>\r\n" +
				"  </xs:element>\r\n" +
				"\r\n" +
				"\r\n" +
				"  <xs:group name=\"block\">\r\n" +
				"    <xs:choice>\r\n" +
				"      <xs:element ref=\"p\"/>\r\n" +
				"      <xs:group ref=\"heading\"/>\r\n" +
				"    </xs:choice>\r\n" +
				"  </xs:group>\r\n" +
				"\r\n" +
				"  <xs:element name=\"p\">\r\n" +
				"    <xs:complexType>\r\n" +
				"      <xs:attribute name=\"id\"/>" +
				"    </xs:complexType>\r\n" +
				"  </xs:element>\r\n" +				
				"\r\n" +
				"  <xs:group name=\"heading\">\r\n" +
				"    <xs:choice>\r\n" +
				"      <xs:element name=\"test\"/>\r\n" +
				"      <xs:element name=\"id\"/>\r\n" +
				"    </xs:choice>\r\n" +
				"  </xs:group>\r\n" +
				"\r\n" +
				"</xs:schema>";
		}
	}
}
