// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
