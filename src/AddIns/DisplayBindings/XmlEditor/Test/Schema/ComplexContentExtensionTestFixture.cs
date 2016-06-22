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
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.XmlEditor;
using NUnit.Framework;

namespace XmlEditor.Tests.Schema
{
	/// <summary>
	/// Tests complex content extension elements.
	/// </summary>
	[TestFixture]
	public class ComplexContentExtensionTestFixture : SchemaTestFixtureBase
	{
		XmlCompletionItemCollection bodyChildElements;
		XmlCompletionItemCollection bodyAttributes;
		
		public override void FixtureInit()
		{
			XmlElementPath path = new XmlElementPath();
			path.AddElement(new QualifiedName("body", "http://www.w3schools.com")); 
			
			bodyChildElements = SchemaCompletion.GetChildElementCompletion(path);
			bodyAttributes = SchemaCompletion.GetAttributeCompletion(path);
		}	
		
		[Test]
		public void TitleHasNoChildElements()
		{
			XmlElementPath path = new XmlElementPath();
			path.AddElement(new QualifiedName("body", "http://www.w3schools.com")); 
			path.AddElement(new QualifiedName("title", "http://www.w3schools.com")); 

			Assert.AreEqual(0, SchemaCompletion.GetChildElementCompletion(path).Count,
			                "Should be no child elements.");
		}
		
		[Test]
		public void TextHasNoChildElements()
		{
			XmlElementPath path = new XmlElementPath();
			path.AddElement(new QualifiedName("body", "http://www.w3schools.com")); 
			path.AddElement(new QualifiedName("text", "http://www.w3schools.com")); 

			Assert.AreEqual(0, SchemaCompletion.GetChildElementCompletion(path).Count,
			                "Should be no child elements.");
		}		
		
		[Test]
		public void BodyHasTwoChildElements()
		{
			Assert.AreEqual(2, bodyChildElements.Count, 
			                "Should be two child elements.");
		}
		
		[Test]
		public void BodyChildElementIsText()
		{
			Assert.IsTrue(bodyChildElements.Contains("text"), 
			              "Should have a child element called text.");
		}
		
		[Test]
		public void BodyChildElementIsTitle()
		{
			Assert.IsTrue(bodyChildElements.Contains("title"), 
			              "Should have a child element called title.");
		}		
		
		[Test]
		public void BodyAttributeCount()
		{
			Assert.AreEqual(1, bodyAttributes.Count, 
			                "Should be one attribute.");
		}
		
		[Test]
		public void BodyAttributeName()
		{
			Assert.IsTrue(bodyAttributes.Contains("id"), "Attribute id not found.");
		}
		
		protected override string GetSchema()
		{
			return "<xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" targetNamespace=\"http://www.w3schools.com\"  xmlns=\"http://www.w3schools.com\" elementFormDefault=\"qualified\">\r\n" +
				"\t<xs:complexType name=\"Block\">\r\n" +
				"\t\t<xs:choice minOccurs=\"0\" maxOccurs=\"unbounded\">\r\n" +
				"\t\t\t<xs:element name=\"title\" type=\"xs:string\"/>\r\n" +
				"\t\t\t<xs:element name=\"text\" type=\"xs:string\"/>\r\n" +
				"\t\t</xs:choice>\r\n" +
				"\t</xs:complexType>\r\n" +
				"\r\n" +
				"\t<xs:element name=\"body\">\r\n" +
				"\t\t<xs:complexType>\r\n" +
				"\t\t\t<xs:complexContent>\r\n" +
				"\t\t\t\t<xs:extension base=\"Block\">\r\n" +
				"\t\t\t\t\t<xs:attribute name=\"id\" type=\"xs:string\"/>\r\n" +
				"\t\t\t\t</xs:extension>\r\n" +
				"\t\t\t</xs:complexContent>\r\n" +
				"\t\t</xs:complexType>\r\n" +
				"\t</xs:element>\r\n" +
				"</xs:schema>";
		}
	}
}
