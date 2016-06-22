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
	/// Tests elements that are abstract and require substitution groups.
	/// </summary>
	[TestFixture]
	public class AbstractElementTestFixture : SchemaTestFixtureBase
	{		
		XmlCompletionItemCollection itemsElementChildren;
		XmlCompletionItemCollection fileElementAttributes;
		XmlCompletionItemCollection fileElementChildren;
		
		public override void FixtureInit()
		{
			XmlElementPath path = new XmlElementPath();
			
			path.AddElement(new QualifiedName("project", "http://foo"));
			path.AddElement(new QualifiedName("items", "http://foo"));
			
			itemsElementChildren = SchemaCompletion.GetChildElementCompletion(path);
			
			path.AddElement(new QualifiedName("file", "http://foo"));
			
			fileElementAttributes = SchemaCompletion.GetAttributeCompletion(path);
			fileElementChildren = SchemaCompletion.GetChildElementCompletion(path);
		}
		
		[Test]
		public void ItemsElementHasTwoChildElements()
		{
			Assert.AreEqual(2, itemsElementChildren.Count, 
			                "Should be 2 child elements.");
		}
		
		[Test]
		public void ReferenceElementIsChildOfItemsElement()
		{
			Assert.IsTrue(itemsElementChildren.Contains("reference"));
		}
		
		[Test]
		public void FileElementIsChildOfItemsElement()
		{
			Assert.IsTrue(itemsElementChildren.Contains("file"));
		}
		
		[Test]
		public void FileElementHasAttributeNamedType()
		{
			Assert.IsTrue(fileElementAttributes.Contains("type"));
		}
		
		[Test]
		public void FileElementHasTwoChildElements()
		{
			Assert.AreEqual(2, fileElementChildren.Count, "Should be 2 child elements.");
		}
		
		protected override string GetSchema()
		{
			return "<xs:schema targetNamespace=\"http://foo\" xmlns:foo=\"http://foo\" xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" elementFormDefault=\"qualified\">\r\n" +
					"    <xs:element name=\"project\">\r\n" +
					"        <xs:complexType>\r\n" +
					"            <xs:sequence>\r\n" +
					"                <xs:group ref=\"foo:projectItems\" minOccurs=\"0\" maxOccurs=\"unbounded\"/>\r\n" +
					"            </xs:sequence>\r\n" +
					"        </xs:complexType>\r\n" +
					"    </xs:element>\r\n" +
					"\r\n" +
					"    <xs:group name=\"projectItems\">\r\n" +
					"        <xs:choice>\r\n" +
					"            <xs:element name=\"items\" type=\"foo:itemGroupType\"/>\r\n" +
					"            <xs:element name=\"message\" type=\"xs:string\"/>\r\n" +
					"        </xs:choice>\r\n" +
					"    </xs:group>\r\n" +
					"\r\n" +
					"    <xs:complexType name=\"itemGroupType\">\r\n" +
					"        <xs:sequence minOccurs=\"0\" maxOccurs=\"unbounded\">\r\n" +
					"            <xs:element ref=\"foo:item\"/>\r\n" +
					"        </xs:sequence>\r\n" +
					"        <xs:attribute name=\"name\" type=\"xs:string\" use=\"optional\"/>\r\n" +
					"    </xs:complexType>\r\n" +
					"\r\n" +
					"    <xs:element name=\"item\" type=\"foo:itemType\" abstract=\"true\"/>\r\n" +
					"\r\n" +
					"<xs:complexType name=\"itemType\">\r\n" +
					"        <xs:attribute name=\"name\" type=\"xs:string\" use=\"optional\"/>\r\n" +
					"    </xs:complexType>\r\n" +
					"\r\n" +
					"    <xs:element name=\"reference\" substitutionGroup=\"foo:item\">\r\n" +
					"        <xs:complexType>\r\n" +
					"            <xs:complexContent>\r\n" +
					"                <xs:extension base=\"foo:itemType\">\r\n" +
					"                    <xs:sequence minOccurs=\"0\" maxOccurs=\"unbounded\">\r\n" +
					"                        <xs:choice>\r\n" +
					"                            <xs:element name=\"name\"/>\r\n" +
					"                            <xs:element name=\"location\"/>\r\n" +
					"                        </xs:choice>\r\n" +
					"                    </xs:sequence>\r\n" +
					"                    <xs:attribute name=\"description\" type=\"xs:string\"/>\r\n" +
					"                 </xs:extension>\r\n" +
					"            </xs:complexContent>\r\n" +
					"        </xs:complexType>\r\n" +
					"    </xs:element>\r\n" +
					"\r\n" +
					"    <xs:element name=\"file\" substitutionGroup=\"foo:item\">\r\n" +
					"        <xs:complexType>\r\n" +
					"            <xs:complexContent>\r\n" +
					"                <xs:extension base=\"foo:itemType\">\r\n" +
					"                    <xs:sequence minOccurs=\"0\" maxOccurs=\"unbounded\">\r\n" +
					"                        <xs:choice>\r\n" +
					"                            <xs:element name=\"name\"/>\r\n" +
					"                            <xs:element name=\"attributes\"/>\r\n" +
					"                         </xs:choice>\r\n" +
					"                    </xs:sequence>\r\n" +
					"                    <xs:attribute name=\"type\" type=\"xs:string\"/>\r\n" +
					"                </xs:extension>\r\n" +
					"            </xs:complexContent>\r\n" +
					"        </xs:complexType>\r\n" +
					"    </xs:element>\r\n" +
					"</xs:schema>";
		}
	}
}
