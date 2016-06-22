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
	/// Element that uses an attribute group ref.
	/// </summary>
	[TestFixture]
	public class AttributeGroupRefTestFixture : SchemaTestFixtureBase
	{
		XmlCompletionItemCollection attributeCompletionItems;
		
		public override void FixtureInit()
		{			
			XmlElementPath path = new XmlElementPath();
			path.AddElement(new QualifiedName("note", "http://www.w3schools.com"));
			attributeCompletionItems = SchemaCompletion.GetAttributeCompletion(path);
		}
		
		[Test]
		public void AttributeCount()
		{
			Assert.AreEqual(4, attributeCompletionItems.Count, "Should be 4 attributes.");
		}
		
		[Test]
		public void NameAttribute()
		{
			Assert.IsTrue(attributeCompletionItems.Contains("name"), 
			              "Attribute name does not exist.");
		}		
		
		[Test]
		public void IdAttribute()
		{
			Assert.IsTrue(attributeCompletionItems.Contains("id"), 
			              "Attribute id does not exist.");
		}		
		
		[Test]
		public void StyleAttribute()
		{
			Assert.IsTrue(attributeCompletionItems.Contains("style"), 
			              "Attribute style does not exist.");
		}	
		
		[Test]
		public void TitleAttribute()
		{
			Assert.IsTrue(attributeCompletionItems.Contains("title"), 
			              "Attribute title does not exist.");
		}		
		
		protected override string GetSchema()
		{
			return "<xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" targetNamespace=\"http://www.w3schools.com\" xmlns=\"http://www.w3schools.com\" elementFormDefault=\"qualified\">\r\n" +
				"<xs:attributeGroup name=\"coreattrs\">" +
				"\t<xs:attribute name=\"id\" type=\"xs:string\"/>" +
				"\t<xs:attribute name=\"style\" type=\"xs:string\"/>" +
				"\t<xs:attribute name=\"title\" type=\"xs:string\"/>" +
				"</xs:attributeGroup>" +
				"\t<xs:element name=\"note\">\r\n" +
				"\t\t<xs:complexType>\r\n" +
				"\t\t\t<xs:attributeGroup ref=\"coreattrs\"/>" +
				"\t\t\t<xs:attribute name=\"name\" type=\"xs:string\"/>\r\n" +
				"\t\t</xs:complexType>\r\n" +
				"\t</xs:element>\r\n" +
				"</xs:schema>";
		}
	}
}
