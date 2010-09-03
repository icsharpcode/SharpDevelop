// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
	public class ExtensionElementTestFixture : SchemaTestFixtureBase
	{
		XmlCompletionItemCollection schemaChildElements;
		XmlCompletionItemCollection annotationChildElements;
		XmlCompletionItemCollection annotationAttributes;
		XmlCompletionItemCollection includeAttributes;
		XmlCompletionItemCollection appInfoAttributes;
		XmlCompletionItemCollection schemaAttributes;
		XmlCompletionItemCollection fooAttributes;
		
		public override void FixtureInit()
		{
			XmlElementPath path = new XmlElementPath();
			path.AddElement(new QualifiedName("schema", "http://www.w3.org/2001/XMLSchema"));
			
			schemaChildElements = SchemaCompletion.GetChildElementCompletion(path);
			schemaAttributes = SchemaCompletion.GetAttributeCompletion(path);
			
			// Get include elements attributes.
			path.AddElement(new QualifiedName("include", "http://www.w3.org/2001/XMLSchema"));
			includeAttributes = SchemaCompletion.GetAttributeCompletion(path);
		
			// Get annotation element info.
			path.Elements.RemoveLast();
			path.AddElement(new QualifiedName("annotation", "http://www.w3.org/2001/XMLSchema"));
			
			annotationChildElements = SchemaCompletion.GetChildElementCompletion(path);
			annotationAttributes = SchemaCompletion.GetAttributeCompletion(path);
		
			// Get app info attributes.
			path.AddElement(new QualifiedName("appinfo", "http://www.w3.org/2001/XMLSchema"));
			appInfoAttributes = SchemaCompletion.GetAttributeCompletion(path);
			
			// Get foo attributes.
			path = new XmlElementPath();
			path.AddElement(new QualifiedName("foo", "http://www.w3.org/2001/XMLSchema"));
			fooAttributes = SchemaCompletion.GetAttributeCompletion(path);
		}
		
		[Test]
		public void SchemaHasSevenChildElements()
		{
			Assert.AreEqual(7, schemaChildElements.Count, 
			                "Should be 7 child elements.");
		}
		
		[Test]
		public void SchemaChildElementIsInclude()
		{
			Assert.IsTrue(schemaChildElements.Contains("include"), 
			              "Should have a child element called include.");
		}
		
		[Test]
		public void SchemaChildElementIsImport()
		{
			Assert.IsTrue(schemaChildElements.Contains("import"), 
			              "Should have a child element called import.");
		}		
		
		[Test]
		public void SchemaChildElementIsNotation()
		{
			Assert.IsTrue(schemaChildElements.Contains("notation"), 
			              "Should have a child element called notation.");
		}		
		
		/// <summary>
		/// Tests that the extended element has the base type's attributes. 
		/// </summary>
		[Test]
		public void FooHasClassAttribute()
		{
			Assert.IsTrue(fooAttributes.Contains("class"), 
			              "Should have an attribute called class.");						
		}
		
		[Test]
		public void AnnotationElementHasOneAttribute()
		{
			Assert.AreEqual(1, annotationAttributes.Count, "Should be one attribute.");
		}
		
		[Test]
		public void AnnotationHasIdAttribute()
		{
			Assert.IsTrue(annotationAttributes.Contains("id"), 
			              "Should have an attribute called id.");			
		}
		
		[Test]
		public void AnnotationHasTwoChildElements()
		{
			Assert.AreEqual(2, annotationChildElements.Count, 
			                "Should be 2 child elements.");
		}
		
		[Test]
		public void AnnotationChildElementIsAppInfo()
		{
			Assert.IsTrue(annotationChildElements.Contains("appinfo"), 
			              "Should have a child element called appinfo.");
		}
		
		[Test]
		public void AnnotationChildElementIsDocumentation()
		{
			Assert.IsTrue(annotationChildElements.Contains("documentation"), 
			              "Should have a child element called documentation.");
		}		
		
		[Test]
		public void IncludeElementHasOneAttribute()
		{
			Assert.AreEqual(1, includeAttributes.Count, "Should be one attribute.");
		}
		
		[Test]
		public void IncludeHasSchemaLocationAttribute()
		{
			Assert.IsTrue(includeAttributes.Contains("schemaLocation"), 
			              "Should have an attribute called schemaLocation.");			
		}	
		
		[Test]
		public void AppInfoElementHasOneAttribute()
		{
			Assert.AreEqual(1, appInfoAttributes.Count, "Should be one attribute.");
		}
		
		[Test]
		public void AppInfoHasIdAttribute()
		{
			Assert.IsTrue(appInfoAttributes.Contains("id"), 
			              "Should have an attribute called id.");			
		}		
		
		protected override string GetSchema()
		{
			return "<xs:schema targetNamespace=\"http://www.w3.org/2001/XMLSchema\" elementFormDefault=\"qualified\" version=\"1.0\" xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" xml:lang=\"EN\">\r\n" +
				"\r\n" +
				" <xs:element name=\"schema\" id=\"schema\">\r\n" +
				"  <xs:complexType>\r\n" +
				"   <xs:complexContent>\r\n" +
				"    <xs:extension base=\"xs:openAttrs\">\r\n" +
				"     <xs:sequence>\r\n" +
				"      <xs:choice minOccurs=\"0\" maxOccurs=\"unbounded\">\r\n" +
				"       <xs:element ref=\"xs:include\"/>\r\n" +
				"       <xs:element name=\"import\"/>\r\n" +
				"       <xs:element name=\"redefine\"/>\r\n" +
				"       <xs:element ref=\"xs:annotation\"/>\r\n" +
				"      </xs:choice>\r\n" +
				"      <xs:sequence minOccurs=\"0\" maxOccurs=\"unbounded\">\r\n" +
				"       <xs:group ref=\"xs:schemaTop\"/>\r\n" +
				"       <xs:element ref=\"xs:annotation\" minOccurs=\"0\" maxOccurs=\"unbounded\"/>\r\n" +
				"      </xs:sequence>\r\n" +
				"     </xs:sequence>\r\n" +
				"     <xs:attribute name=\"targetNamespace\" type=\"xs:anyURI\"/>\r\n" +
				"     <xs:attribute name=\"version\" type=\"xs:token\"/>\r\n" +
				"     <xs:attribute name=\"finalDefault\" type=\"xs:fullDerivationSet\" use=\"optional\" default=\"\"/>\r\n" +
				"     <xs:attribute name=\"blockDefault\" type=\"xs:blockSet\" use=\"optional\" default=\"\"/>\r\n" +
				"     <xs:attribute name=\"attributeFormDefault\" type=\"xs:formChoice\" use=\"optional\" default=\"unqualified\"/>\r\n" +
				"     <xs:attribute name=\"elementFormDefault\" type=\"xs:formChoice\" use=\"optional\" default=\"unqualified\"/>\r\n" +
				"     <xs:attribute name=\"id\" type=\"xs:ID\"/>\r\n" +
				"     <xs:attribute ref=\"xml:lang\"/>\r\n" +
				"    </xs:extension>\r\n" +
				"   </xs:complexContent>\r\n" +
				"  </xs:complexType>\r\n" +
				"\r\n" +
				"\r\n" +
				" </xs:element>\r\n" +
				"\r\n" +
				"<xs:complexType name=\"openAttrs\">\r\n" +
				"   <xs:complexContent>\r\n" +
				"     <xs:restriction base=\"xs:anyType\">\r\n" +
				"       <xs:anyAttribute namespace=\"##other\" processContents=\"lax\"/>\r\n" +
				"     </xs:restriction>\r\n" +
				"   </xs:complexContent>\r\n" +
				" </xs:complexType>\r\n" +
				"\r\n" +
				"<xs:complexType name=\"anyType\" mixed=\"true\">\r\n" +
				"  <xs:annotation>\r\n" +
				"   <xs:documentation>\r\n" +
				"   Not the real urType, but as close an approximation as we can\r\n" +
				"   get in the XML representation</xs:documentation>\r\n" +
				"  </xs:annotation>\r\n" +
				"  <xs:sequence>\r\n" +
				"   <xs:any minOccurs=\"0\" maxOccurs=\"unbounded\" processContents=\"lax\"/>\r\n" +
				"  </xs:sequence>\r\n" +
				"  <xs:anyAttribute processContents=\"lax\"/>\r\n" +
				" </xs:complexType>\r\n" +
				"\r\n" +
				"<xs:element name=\"include\" id=\"include\">\r\n" +
				"  <xs:complexType>\r\n" +
				"     <xs:attribute name=\"schemaLocation\" type=\"xs:anyURI\" use=\"required\"/>\r\n" +
				"  </xs:complexType>\r\n" +
				" </xs:element>\r\n" +
				"\r\n" +
				"<xs:element name=\"annotation\" id=\"annotation\">\r\n" +
				"   <xs:complexType>\r\n" +
				"    <xs:complexContent>\r\n" +
				"     <xs:extension base=\"xs:openAttrs\">\r\n" +
				"      <xs:choice minOccurs=\"0\" maxOccurs=\"unbounded\">\r\n" +
				"       <xs:element ref=\"xs:appinfo\"/>\r\n" +
				"       <xs:element name=\"documentation\"/>\r\n" +
				"      </xs:choice>\r\n" +
				"      <xs:attribute name=\"id\" type=\"xs:ID\"/>\r\n" +
				"     </xs:extension>\r\n" +
				"    </xs:complexContent>\r\n" +
				"   </xs:complexType>\r\n" +
				" </xs:element>\r\n" +
				"\r\n" +
				" <xs:group name=\"schemaTop\">\r\n" +
				"  <xs:choice>\r\n" +
				"   <xs:element name=\"element\"/>\r\n" +
				"   <xs:element name=\"attribute\"/>\r\n" +
				"   <xs:element name=\"notation\"/>\r\n" +
				"  </xs:choice>\r\n" +
				" </xs:group>\r\n" +
				"\r\n" +
				"\r\n" +
				"<xs:element name=\"appinfo\" id=\"appinfo\">\r\n" +
				"  <xs:complexType>\r\n" +
				"     <xs:attribute name=\"id\" type=\"xs:anyURI\" use=\"required\"/>\r\n" +
				"  </xs:complexType>\r\n" +
				" </xs:element>\r\n" +
				"\r\n" +
				"\r\n" +
				" <xs:element name=\"foo\">\r\n" +
				"  <xs:complexType>\r\n" +
				"   <xs:complexContent>\r\n" +
				"    <xs:extension base=\"xs:fooBase\">\r\n" +
				"      <xs:attribute name=\"id\" type=\"xs:ID\"/>\r\n" +
				"     </xs:extension>\r\n" +
				"    </xs:complexContent>\r\n" +
				"   </xs:complexType>\r\n" +
				" </xs:element>\r\n" +
				"\r\n" +
				"<xs:complexType name=\"fooBase\">\r\n" +
				"      <xs:attribute name=\"class\" type=\"xs:string\"/>\r\n" +
				" </xs:complexType>\r\n" +
				"</xs:schema>";
		}
	}
}
