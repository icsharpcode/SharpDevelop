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
	/// Retrieve completion data for an xml schema that specifies only one 
	/// element.
	/// </summary>
	[TestFixture]
	public class SingleElementSchemaTestFixture : SchemaTestFixtureBase
	{
		XmlSchemaCompletionData schemaCompletionData;
		ICompletionData[] childElementCompletionData;
		ICompletionData[] attributeCompletionData;
		
		[TestFixtureSetUp]
		public void FixtureInit()
		{
			StringReader reader = new StringReader(GetSchema());
			schemaCompletionData = new XmlSchemaCompletionData(reader);

			XmlElementPath path = new XmlElementPath();
			path.Elements.Add(new QualifiedName("note", "http://www.w3schools.com"));

			attributeCompletionData = 
				schemaCompletionData.GetAttributeCompletionData(path);

			childElementCompletionData = 
				schemaCompletionData.GetChildElementCompletionData(path);
		}
		
		[Test]
		public void NamespaceUri()
		{
			Assert.AreEqual("http://www.w3schools.com", 
			                schemaCompletionData.NamespaceUri,
			                "Unexpected namespace.");
		}
		
		[Test]
		public void NoteElementHasNoAttributes()
		{
			Assert.AreEqual(0, attributeCompletionData.Length, 
			                "Not expecting any attributes.");
		}
		
		[Test]
		public void NoteElementHasNoChildElements()
		{
			Assert.AreEqual(0, childElementCompletionData.Length, "" +
			                "Not expecting any child elements.");
		}
		
		string GetSchema()
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
