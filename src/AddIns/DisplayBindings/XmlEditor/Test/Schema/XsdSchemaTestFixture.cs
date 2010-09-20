// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Xml;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Schema
{
	/// <summary>
	/// Tests the xsd schema.
	/// </summary>
	[TestFixture]
	public class XsdSchemaTestFixture
	{
		XmlSchemaCompletion schemaCompletion;
		XmlElementPath choicePath;
		XmlElementPath elementPath;
		XmlElementPath simpleEnumPath;
		XmlElementPath enumPath;
		XmlElementPath allElementPath;
		XmlElementPath allElementAnnotationPath;
		XmlCompletionItemCollection choiceAttributes;
		XmlCompletionItemCollection elementAttributes;
		XmlCompletionItemCollection simpleEnumElements;
		XmlCompletionItemCollection enumAttributes;
		XmlCompletionItemCollection elementFormDefaultAttributeValues;
		XmlCompletionItemCollection blockDefaultAttributeValues;
		XmlCompletionItemCollection finalDefaultAttributeValues;
		XmlCompletionItemCollection mixedAttributeValues;
		XmlCompletionItemCollection maxOccursAttributeValues;
		XmlCompletionItemCollection allElementChildElements;
		XmlCompletionItemCollection allElementAnnotationChildElements;
		
		string namespaceURI = "http://www.w3.org/2001/XMLSchema";
		string prefix = "xs";
		
		[TestFixtureSetUp]
		public void FixtureInit()
		{
			schemaCompletion = new XmlSchemaCompletion(ResourceManager.ReadXsdSchema());
			
			// Set up choice element's path.
			choicePath = new XmlElementPath();
			choicePath.AddElement(new QualifiedName("schema", namespaceURI, prefix));
			choicePath.AddElement(new QualifiedName("element", namespaceURI, prefix));
			choicePath.AddElement(new QualifiedName("complexType", namespaceURI, prefix));
			
			mixedAttributeValues = schemaCompletion.GetAttributeValueCompletion(choicePath, "mixed");

			choicePath.AddElement(new QualifiedName("choice", namespaceURI, prefix));
			
			// Get choice element info.
			choiceAttributes = schemaCompletion.GetAttributeCompletion(choicePath);
			maxOccursAttributeValues = schemaCompletion.GetAttributeValueCompletion(choicePath, "maxOccurs");
			
			// Set up element path.
			elementPath = new XmlElementPath();
			elementPath.AddElement(new QualifiedName("schema", namespaceURI, prefix));
			
			elementFormDefaultAttributeValues = schemaCompletion.GetAttributeValueCompletion(elementPath, "elementFormDefault");
			blockDefaultAttributeValues = schemaCompletion.GetAttributeValueCompletion(elementPath, "blockDefault");
			finalDefaultAttributeValues = schemaCompletion.GetAttributeValueCompletion(elementPath, "finalDefault");
			
			elementPath.AddElement(new QualifiedName("element", namespaceURI, prefix));
				
			// Get element attribute info.
			elementAttributes = schemaCompletion.GetAttributeCompletion(elementPath);

			// Set up simple enum type path.
			simpleEnumPath = new XmlElementPath();
			simpleEnumPath.AddElement(new QualifiedName("schema", namespaceURI, prefix));
			simpleEnumPath.AddElement(new QualifiedName("simpleType", namespaceURI, prefix));
			simpleEnumPath.AddElement(new QualifiedName("restriction", namespaceURI, prefix));
			
			// Get child elements.
			simpleEnumElements = schemaCompletion.GetChildElementCompletion(simpleEnumPath);

			// Set up enum path.
			enumPath = new XmlElementPath();
			enumPath.AddElement(new QualifiedName("schema", namespaceURI, prefix));
			enumPath.AddElement(new QualifiedName("simpleType", namespaceURI, prefix));
			enumPath.AddElement(new QualifiedName("restriction", namespaceURI, prefix));
			enumPath.AddElement(new QualifiedName("enumeration", namespaceURI, prefix));
			
			// Get attributes.
			enumAttributes = schemaCompletion.GetAttributeCompletion(enumPath);
			
			// Set up xs:all path.
			allElementPath = new XmlElementPath();
			allElementPath.AddElement(new QualifiedName("schema", namespaceURI, prefix));
			allElementPath.AddElement(new QualifiedName("element", namespaceURI, prefix));
			allElementPath.AddElement(new QualifiedName("complexType", namespaceURI, prefix));
			allElementPath.AddElement(new QualifiedName("all", namespaceURI, prefix));
		
			// Get child elements of the xs:all element.
			allElementChildElements = schemaCompletion.GetChildElementCompletion(allElementPath);
			
			// Set up the path to the annotation element that is a child of xs:all.
			allElementAnnotationPath = new XmlElementPath();
			allElementAnnotationPath.AddElement(new QualifiedName("schema", namespaceURI, prefix));
			allElementAnnotationPath.AddElement(new QualifiedName("element", namespaceURI, prefix));
			allElementAnnotationPath.AddElement(new QualifiedName("complexType", namespaceURI, prefix));
			allElementAnnotationPath.AddElement(new QualifiedName("all", namespaceURI, prefix));
			allElementAnnotationPath.AddElement(new QualifiedName("annotation", namespaceURI, prefix));
			
			// Get the xs:all annotation child element.
			allElementAnnotationChildElements = schemaCompletion.GetChildElementCompletion(allElementAnnotationPath);
		}
		
		[Test]
		public void ChoiceHasAttributes()
		{
			Assert.IsTrue(choiceAttributes.Count > 0, "Should have at least one attribute.");
		}
		
		[Test]
		public void ChoiceHasMinOccursAttribute()
		{
			Assert.IsTrue(choiceAttributes.Contains("minOccurs"),
			              "Attribute minOccurs missing.");
		}
		
		[Test]
		public void ChoiceHasMaxOccursAttribute()
		{
			Assert.IsTrue(choiceAttributes.Contains("maxOccurs"),
			              "Attribute maxOccurs missing.");
		}
		
		/// <summary>
		/// Tests that prohibited attributes are not added to the completion data.
		/// </summary>
		[Test]
		public void ChoiceDoesNotHaveNameAttribute()
		{
			Assert.IsFalse(choiceAttributes.Contains("name"),
			               "Attribute name should not exist.");
		}
		
		/// <summary>
		/// Tests that prohibited attributes are not added to the completion data.
		/// </summary>
		[Test]
		public void ChoiceDoesNotHaveRefAttribute()
		{
			Assert.IsFalse(choiceAttributes.Contains("ref"),
			               "Attribute ref should not exist.");
		}	
		
		/// <summary>
		/// Duplicate attribute test.
		/// </summary>
		[Test]
		public void ElementNameAttributeAppearsOnce()
		{
			int nameAttributeCount = elementAttributes.GetOccurrences("name");
			Assert.AreEqual(1, nameAttributeCount, "Should be only one name attribute.");
		}
		
		[Test]
		public void ElementHasIdAttribute()
		{
			Assert.IsTrue(elementAttributes.Contains("id"), 
			              "id attribute missing.");
		}		
		
		[Test]
		public void SimpleRestrictionTypeHasEnumChildElement()
		{
			Assert.IsTrue(simpleEnumElements.Contains("xs:enumeration"),
			              "enumeration element missing.");			
		}
		
		[Test]
		public void EnumHasValueAttribute()
		{
			Assert.IsTrue(enumAttributes.Contains("value"),
			              "Attribute value missing.");			
		}
		
		[Test]
		public void ElementFormDefaultAttributeHasValueQualified()
		{
			Assert.IsTrue(elementFormDefaultAttributeValues.Contains("qualified"),
			              "Attribute value 'qualified' missing.");
		}
		
		[Test]
		public void BlockDefaultAttributeHasValueAll()
		{
			Assert.IsTrue(blockDefaultAttributeValues.Contains("#all"),
			              "Attribute value '#all' missing.");
		}		
		
		[Test]
		public void BlockDefaultAttributeHasValueExtension()
		{
			Assert.IsTrue(blockDefaultAttributeValues.Contains("extension"),
			              "Attribute value 'extension' missing.");
		}		
		
		[Test]
		public void FinalDefaultAttributeHasValueList()
		{
			Assert.IsTrue(finalDefaultAttributeValues.Contains("list"),
			              "Attribute value 'list' missing.");
		}
		
		/// <summary>
		/// xs:boolean tests.
		/// </summary>
		[Test]
		public void MixedAttributeHasValueTrue()
		{
			Assert.IsTrue(mixedAttributeValues.Contains("true"),
			              "Attribute value 'true' missing.");
		}
		
		[Test]
		public void MaxOccursAttributeHasValueUnbounded()
		{
			Assert.IsTrue(maxOccursAttributeValues.Contains("unbounded"),
			              "Attribute value 'unbounded' missing.");
		}	
				
		[Test]
		public void AllElementHasAnnotationChildElement()
		{
			Assert.IsTrue(allElementChildElements.Contains("xs:annotation"),
			              "Should have an annotation child element.");
		}
		
		[Test]
		public void AllElementHasElementChildElement()
		{
			Assert.IsTrue(allElementChildElements.Contains("xs:element"),
			              "Should have an child element called 'element'.");
		}
		
		[Test]
		public void AllElementAnnotationHasDocumentationChildElement()
		{
			Assert.IsTrue(allElementAnnotationChildElements.Contains("xs:documentation"),
			              "Should have documentation child element.");
		}
	}
}
