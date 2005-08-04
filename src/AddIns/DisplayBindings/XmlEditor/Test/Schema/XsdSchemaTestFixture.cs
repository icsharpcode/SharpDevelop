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
using System.Xml;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Schema
{
	/// <summary>
	/// Tests the xsd schema.
	/// </summary>
	[TestFixture]
	public class XsdSchemaTestFixture : SchemaTestFixtureBase
	{
		XmlSchemaCompletionData schemaCompletionData;
		XmlElementPath choicePath;
		XmlElementPath elementPath;
		XmlElementPath simpleEnumPath;
		XmlElementPath enumPath;
		ICompletionData[] choiceAttributes;
		ICompletionData[] elementAttributes;
		ICompletionData[] simpleEnumElements;
		ICompletionData[] enumAttributes;
		ICompletionData[] elementFormDefaultAttributeValues;
		ICompletionData[] blockDefaultAttributeValues;
		ICompletionData[] finalDefaultAttributeValues;
		ICompletionData[] mixedAttributeValues;
		ICompletionData[] maxOccursAttributeValues;
		
		string namespaceURI = "http://www.w3.org/2001/XMLSchema";
		string prefix = "xs";
		
		[TestFixtureSetUp]
		public void FixtureInit()
		{
			XmlTextReader reader = ResourceManager.GetXsdSchema();
			schemaCompletionData = new XmlSchemaCompletionData(reader);
			
			// Set up choice element's path.
			choicePath = new XmlElementPath();
			choicePath.Elements.Add(new QualifiedName("schema", namespaceURI, prefix));
			choicePath.Elements.Add(new QualifiedName("element", namespaceURI, prefix));
			choicePath.Elements.Add(new QualifiedName("complexType", namespaceURI, prefix));
			
			mixedAttributeValues = schemaCompletionData.GetAttributeValueCompletionData(choicePath, "mixed");

			choicePath.Elements.Add(new QualifiedName("choice", namespaceURI, prefix));
			
			// Get choice element info.
			choiceAttributes = schemaCompletionData.GetAttributeCompletionData(choicePath);
			maxOccursAttributeValues = schemaCompletionData.GetAttributeValueCompletionData(choicePath, "maxOccurs");
			
			// Set up element path.
			elementPath = new XmlElementPath();
			elementPath.Elements.Add(new QualifiedName("schema", namespaceURI, prefix));
			
			elementFormDefaultAttributeValues = schemaCompletionData.GetAttributeValueCompletionData(elementPath, "elementFormDefault");
			blockDefaultAttributeValues = schemaCompletionData.GetAttributeValueCompletionData(elementPath, "blockDefault");
			finalDefaultAttributeValues = schemaCompletionData.GetAttributeValueCompletionData(elementPath, "finalDefault");
			
			elementPath.Elements.Add(new QualifiedName("element", namespaceURI, prefix));
				
			// Get element attribute info.
			elementAttributes = schemaCompletionData.GetAttributeCompletionData(elementPath);

			// Set up simple enum type path.
			simpleEnumPath = new XmlElementPath();
			simpleEnumPath.Elements.Add(new QualifiedName("schema", namespaceURI, prefix));
			simpleEnumPath.Elements.Add(new QualifiedName("simpleType", namespaceURI, prefix));
			simpleEnumPath.Elements.Add(new QualifiedName("restriction", namespaceURI, prefix));
			
			// Get child elements.
			simpleEnumElements = schemaCompletionData.GetChildElementCompletionData(simpleEnumPath);

			// Set up enum path.
			enumPath = new XmlElementPath();
			enumPath.Elements.Add(new QualifiedName("schema", namespaceURI, prefix));
			enumPath.Elements.Add(new QualifiedName("simpleType", namespaceURI, prefix));
			enumPath.Elements.Add(new QualifiedName("restriction", namespaceURI, prefix));
			enumPath.Elements.Add(new QualifiedName("enumeration", namespaceURI, prefix));
			
			// Get attributes.
			enumAttributes = schemaCompletionData.GetAttributeCompletionData(enumPath);
		}
		
		[Test]
		public void ChoiceHasAttributes()
		{
			Assert.IsTrue(choiceAttributes.Length > 0, "Should have at least one attribute.");
		}
		
		[Test]
		public void ChoiceHasMinOccursAttribute()
		{
			Assert.IsTrue(base.Contains(choiceAttributes, "minOccurs"),
			              "Attribute minOccurs missing.");
		}
		
		[Test]
		public void ChoiceHasMaxOccursAttribute()
		{
			Assert.IsTrue(base.Contains(choiceAttributes, "maxOccurs"),
			              "Attribute maxOccurs missing.");
		}
		
		/// <summary>
		/// Tests that prohibited attributes are not added to the completion data.
		/// </summary>
		[Test]
		public void ChoiceDoesNotHaveNameAttribute()
		{
			Assert.IsFalse(base.Contains(choiceAttributes, "name"),
			               "Attribute name should not exist.");
		}
		
		/// <summary>
		/// Tests that prohibited attributes are not added to the completion data.
		/// </summary>
		[Test]
		public void ChoiceDoesNotHaveRefAttribute()
		{
			Assert.IsFalse(base.Contains(choiceAttributes, "ref"),
			               "Attribute ref should not exist.");
		}	
		
		/// <summary>
		/// Duplicate attribute test.
		/// </summary>
		[Test]
		public void ElementNameAttributeAppearsOnce()
		{
			int nameAttributeCount = base.GetItemCount(elementAttributes, "name");
			Assert.AreEqual(1, nameAttributeCount, "Should be only one name attribute.");
		}
		
		[Test]
		public void ElementHasIdAttribute()
		{
			Assert.IsTrue(base.Contains(elementAttributes, "id"), 
			              "id attribute missing.");
		}		
		
		[Test]
		public void SimpleRestrictionTypeHasEnumChildElement()
		{
			Assert.IsTrue(base.Contains(simpleEnumElements, "xs:enumeration"),
			              "enumeration element missing.");			
		}
		
		[Test]
		public void EnumHasValueAttribute()
		{
			Assert.IsTrue(base.Contains(enumAttributes, "value"),
			              "Attribute value missing.");			
		}
		
		[Test]
		public void ElementFormDefaultAttributeHasValueQualified()
		{
			Assert.IsTrue(base.Contains(elementFormDefaultAttributeValues, "qualified"),
			              "Attribute value 'qualified' missing.");
		}
		
		[Test]
		public void BlockDefaultAttributeHasValueAll()
		{
			Assert.IsTrue(base.Contains(blockDefaultAttributeValues, "#all"),
			              "Attribute value '#all' missing.");
		}		
		
		[Test]
		public void BlockDefaultAttributeHasValueExtension()
		{
			Assert.IsTrue(base.Contains(blockDefaultAttributeValues, "extension"),
			              "Attribute value 'extension' missing.");
		}		
		
		[Test]
		public void FinalDefaultAttributeHasValueList()
		{
			Assert.IsTrue(base.Contains(finalDefaultAttributeValues, "list"),
			              "Attribute value 'list' missing.");
		}
		
		/// <summary>
		/// xs:boolean tests.
		/// </summary>
		[Test]
		public void MixedAttributeHasValueTrue()
		{
			Assert.IsTrue(base.Contains(mixedAttributeValues, "true"),
			              "Attribute value 'true' missing.");
		}
		
		[Test]
		public void MaxOccursAttributeHasValueUnbounded()
		{
			Assert.IsTrue(base.Contains(maxOccursAttributeValues, "unbounded"),
			              "Attribute value 'unbounded' missing.");
		}				
	}
}
