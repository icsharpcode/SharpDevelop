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
	/// Tests complex content extension elements.
	/// </summary>
	[TestFixture]
	public class ComplexContentExtensionTestFixture : SchemaTestFixtureBase
	{
		XmlSchemaCompletionData schemaCompletionData;
		ICompletionData[] bodyChildElements;
		ICompletionData[] bodyAttributes;
		
		[TestFixtureSetUp]
		public void FixtureInit()
		{
			StringReader reader = new StringReader(GetSchema());
			schemaCompletionData = new XmlSchemaCompletionData(reader);
			
			XmlElementPath path = new XmlElementPath();
			path.Elements.Add(new QualifiedName("body", "http://www.w3schools.com")); 
			
			bodyChildElements = schemaCompletionData.GetChildElementCompletionData(path);
			bodyAttributes = schemaCompletionData.GetAttributeCompletionData(path);
		}	
		
		[Test]
		public void TitleHasNoChildElements()
		{
			XmlElementPath path = new XmlElementPath();
			path.Elements.Add(new QualifiedName("body", "http://www.w3schools.com")); 
			path.Elements.Add(new QualifiedName("title", "http://www.w3schools.com")); 

			Assert.AreEqual(0, schemaCompletionData.GetChildElementCompletionData(path).Length,
			                "Should be no child elements.");
		}
		
		[Test]
		public void TextHasNoChildElements()
		{
			XmlElementPath path = new XmlElementPath();
			path.Elements.Add(new QualifiedName("body", "http://www.w3schools.com")); 
			path.Elements.Add(new QualifiedName("text", "http://www.w3schools.com")); 

			Assert.AreEqual(0, schemaCompletionData.GetChildElementCompletionData(path).Length,
			                "Should be no child elements.");
		}		
		
		[Test]
		public void BodyHasTwoChildElements()
		{
			Assert.AreEqual(2, bodyChildElements.Length, 
			                "Should be two child elements.");
		}
		
		[Test]
		public void BodyChildElementIsText()
		{
			Assert.IsTrue(base.Contains(bodyChildElements, "text"), 
			              "Should have a child element called text.");
		}
		
		[Test]
		public void BodyChildElementIsTitle()
		{
			Assert.IsTrue(base.Contains(bodyChildElements, "title"), 
			              "Should have a child element called title.");
		}		
		
		[Test]
		public void BodyAttributeCount()
		{
			Assert.AreEqual(1, bodyAttributes.Length, 
			                "Should be one attribute.");
		}
		
		[Test]
		public void BodyAttributeName()
		{
			Assert.IsTrue(base.Contains(bodyAttributes, "id"), "Attribute id not found.");
		}
		
		string GetSchema()
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
