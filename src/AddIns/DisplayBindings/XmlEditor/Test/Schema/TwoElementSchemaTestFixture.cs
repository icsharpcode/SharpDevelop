// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.TextEditor.Gui.CompletionWindow;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using System;
using System.IO;

namespace XmlEditor.Tests.Schema
{
	/// <summary>
	/// Two elements defined in a schema, one uses the 'type' attribute to
	/// link to the complex type definition.
	/// </summary>
	[TestFixture]
	public class TwoElementSchemaTestFixture : SchemaTestFixtureBase
	{
		XmlSchemaCompletionData schemaCompletionData;
		XmlElementPath noteElementPath;
		XmlElementPath textElementPath;
		
		[TestFixtureSetUp]
		public void FixtureInit()
		{
			StringReader reader = new StringReader(GetSchema());
			schemaCompletionData = new XmlSchemaCompletionData(reader);
			
			// Note element path.
			noteElementPath = new XmlElementPath();
			QualifiedName noteQualifiedName = new QualifiedName("note", "http://www.w3schools.com");
			noteElementPath.Elements.Add(noteQualifiedName);
		
			// Text element path.
			textElementPath = new XmlElementPath();
			textElementPath.Elements.Add(noteQualifiedName);
			textElementPath.Elements.Add(new QualifiedName("text", "http://www.w3schools.com"));
		}	
		
		[Test]
		public void TextElementHasOneAttribute()
		{
			ICompletionData[] attributesCompletionData = schemaCompletionData.GetAttributeCompletionData(textElementPath);
			
			Assert.AreEqual(1, attributesCompletionData.Length, 
			                "Should have 1 text attribute.");
		}
		
		[Test]
		public void TextElementAttributeName()
		{
			ICompletionData[] attributesCompletionData = schemaCompletionData.GetAttributeCompletionData(textElementPath);
			Assert.IsTrue(base.Contains(attributesCompletionData, "foo"),
			              "Unexpected text attribute name.");
		}

		[Test]
		public void NoteElementHasChildElement()
		{
			ICompletionData[] childElementCompletionData
				= schemaCompletionData.GetChildElementCompletionData(noteElementPath);
			
			Assert.AreEqual(1, childElementCompletionData.Length,
			                "Should be one child.");
		}
		
		[Test]
		public void NoteElementHasNoAttributes()
		{	
			ICompletionData[] attributeCompletionData
				= schemaCompletionData.GetAttributeCompletionData(noteElementPath);
			
			Assert.AreEqual(0, attributeCompletionData.Length,
			                "Should no attributes.");
		}

		[Test]
		public void OneRootElement()
		{
			ICompletionData[] elementCompletionData
				= schemaCompletionData.GetElementCompletionData();
			
			Assert.AreEqual(1, elementCompletionData.Length, "Should be 1 root element.");
		}
		
		[Test]
		public void RootElementIsNote()
		{
			ICompletionData[] elementCompletionData
				= schemaCompletionData.GetElementCompletionData();
			
			Assert.IsTrue(Contains(elementCompletionData, "note"), 
			              "Should be called note.");
		}		
		
		string GetSchema()
		{
			return "<xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" targetNamespace=\"http://www.w3schools.com\" xmlns=\"http://www.w3schools.com\" elementFormDefault=\"qualified\">\r\n" +
				"\t<xs:element name=\"note\">\r\n" +
				"\t\t<xs:complexType> \r\n" +
				"\t\t\t<xs:sequence>\r\n" +
				"\t\t\t\t<xs:element name=\"text\" type=\"text-type\"/>\r\n" +
				"\t\t\t</xs:sequence>\r\n" +
				"\t\t</xs:complexType>\r\n" +
				"\t</xs:element>\r\n" +
				"\t<xs:complexType name=\"text-type\">\r\n" +
				"\t\t<xs:attribute name=\"foo\"/>\r\n" +
				"\t</xs:complexType>\r\n" +
				"</xs:schema>";
		}
	}
}
