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
	/// Element that uses an attribute group ref.
	/// </summary>
	[TestFixture]
	public class NestedAttributeGroupRefTestFixture : SchemaTestFixtureBase
	{
		XmlSchemaCompletionData schemaCompletionData;
		ICompletionData[] attributeCompletionData;
		
		[TestFixtureSetUp]
		public void FixtureInit()
		{
			StringReader reader = new StringReader(GetSchema());
			schemaCompletionData = new XmlSchemaCompletionData(reader);
			
			XmlElementPath path = new XmlElementPath();
			path.Elements.Add(new QualifiedName("note", "http://www.w3schools.com"));
			attributeCompletionData = schemaCompletionData.GetAttributeCompletionData(path);
		}
		
		[Test]
		public void AttributeCount()
		{
			Assert.AreEqual(7, attributeCompletionData.Length, "Should be 7 attributes.");
		}
		
		[Test]
		public void NameAttribute()
		{
			Assert.IsTrue(base.Contains(attributeCompletionData, "name"), 
			              "Attribute name does not exist.");
		}		
		
		[Test]
		public void IdAttribute()
		{
			Assert.IsTrue(base.Contains(attributeCompletionData, "id"), 
			              "Attribute id does not exist.");
		}		
		
		[Test]
		public void StyleAttribute()
		{
			Assert.IsTrue(base.Contains(attributeCompletionData, "style"), 
			              "Attribute style does not exist.");
		}	
		
		[Test]
		public void TitleAttribute()
		{
			Assert.IsTrue(base.Contains(attributeCompletionData, "title"), 
			              "Attribute title does not exist.");
		}		
		
		[Test]
		public void BaseIdAttribute()
		{
			Assert.IsTrue(base.Contains(attributeCompletionData, "baseid"), 
			              "Attribute baseid does not exist.");
		}		
		
		[Test]
		public void BaseStyleAttribute()
		{
			Assert.IsTrue(base.Contains(attributeCompletionData, "basestyle"), 
			              "Attribute basestyle does not exist.");
		}	
		
		[Test]
		public void BaseTitleAttribute()
		{
			Assert.IsTrue(base.Contains(attributeCompletionData, "basetitle"), 
			              "Attribute basetitle does not exist.");
		}			
		
		string GetSchema()
		{
			return "<xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" targetNamespace=\"http://www.w3schools.com\" xmlns=\"http://www.w3schools.com\" elementFormDefault=\"qualified\">\r\n" +
				"<xs:attributeGroup name=\"coreattrs\">" +
				"\t<xs:attribute name=\"id\" type=\"xs:string\"/>" +
				"\t<xs:attribute name=\"style\" type=\"xs:string\"/>" +
				"\t<xs:attribute name=\"title\" type=\"xs:string\"/>" +
				"\t<xs:attributeGroup ref=\"baseattrs\"/>" +
				"</xs:attributeGroup>" +
				"<xs:attributeGroup name=\"baseattrs\">" +
				"\t<xs:attribute name=\"baseid\" type=\"xs:string\"/>" +
				"\t<xs:attribute name=\"basestyle\" type=\"xs:string\"/>" +
				"\t<xs:attribute name=\"basetitle\" type=\"xs:string\"/>" +
				"</xs:attributeGroup>" +				
				"\t<xs:element name=\"note\">\r\n" +
				"\t\t<xs:complexType>\r\n" +
				"\t\t\t<xs:attributeGroup ref=\"coreattrs\"/>" +
				"\t\t\t<xs:attribute name=\"name\" type=\"xs:string\"/>\r\n" +
				"\t\t</xs:complexType>\r\n" +
				"\t</xs:element>\r\n" +
				"</xs:schema>";
		}
	}
}
