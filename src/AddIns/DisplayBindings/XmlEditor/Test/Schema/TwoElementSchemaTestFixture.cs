// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.XmlEditor;
using NUnit.Framework;

namespace XmlEditor.Tests.Schema
{
	/// <summary>
	/// Two elements defined in a schema, one uses the 'type' attribute to
	/// link to the complex type definition.
	/// </summary>
	[TestFixture]
	public class TwoElementSchemaTestFixture : SchemaTestFixtureBase
	{
		XmlElementPath noteElementPath;
		XmlElementPath textElementPath;
		
		public override void FixtureInit()
		{
			// Note element path.
			noteElementPath = new XmlElementPath();
			QualifiedName noteQualifiedName = new QualifiedName("note", "http://www.w3schools.com");
			noteElementPath.AddElement(noteQualifiedName);
		
			// Text element path.
			textElementPath = new XmlElementPath();
			textElementPath.AddElement(noteQualifiedName);
			textElementPath.AddElement(new QualifiedName("text", "http://www.w3schools.com"));
		}	
		
		[Test]
		public void TextElementHasOneAttribute()
		{
			XmlCompletionItemCollection attributesCompletionItems = SchemaCompletion.GetAttributeCompletion(textElementPath);
			
			Assert.AreEqual(1, attributesCompletionItems.Count, 
			                "Should have 1 text attribute.");
		}
		
		[Test]
		public void TextElementAttributeName()
		{
			XmlCompletionItemCollection attributesCompletionItems = SchemaCompletion.GetAttributeCompletion(textElementPath);
			Assert.IsTrue(attributesCompletionItems.Contains("foo"),
			              "Unexpected text attribute name.");
		}

		[Test]
		public void NoteElementHasChildElement()
		{
			XmlCompletionItemCollection childElementCompletionItems
				= SchemaCompletion.GetChildElementCompletion(noteElementPath);
			
			Assert.AreEqual(1, childElementCompletionItems.Count,
			                "Should be one child.");
		}
		
		[Test]
		public void NoteElementHasNoAttributes()
		{	
			XmlCompletionItemCollection attributeCompletionItems
				= SchemaCompletion.GetAttributeCompletion(noteElementPath);
			
			Assert.AreEqual(0, attributeCompletionItems.Count,
			                "Should no attributes.");
		}

		[Test]
		public void OneRootElement()
		{
			Assert.AreEqual(1, SchemaCompletion.GetRootElementCompletion().Count, "Should be 1 root element.");
		}
		
		[Test]
		public void RootElementIsNote()
		{
			Assert.IsTrue(SchemaCompletion.GetRootElementCompletion().Contains("note"),
			              "Should be called note.");
		}
		
		protected override string GetSchema()
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
