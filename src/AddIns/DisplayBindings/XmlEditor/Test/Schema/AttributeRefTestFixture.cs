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
	/// Tests attribute refs
	/// </summary>
	[TestFixture]
	public class AttributeRefTestFixture : SchemaTestFixtureBase
	{
		XmlCompletionItemCollection attributes;
		
		public override void FixtureInit()
		{
			XmlElementPath path = new XmlElementPath();
			path.AddElement(new QualifiedName("html", "http://foo/xhtml"));
			attributes = SchemaCompletion.GetAttributeCompletion(path);
		}
		
		[Test]
		public void HtmlAttributeCount()
		{
			Assert.AreEqual(4, attributes.Count, 
			                "Should be 4 attributes.");
		}
		
		[Test]
		public void HtmlLangAttribute()
		{
			Assert.IsTrue(attributes.Contains("lang"), "Attribute lang not found.");
		}
		
		[Test]
		public void HtmlIdAttribute()
		{
			Assert.IsTrue(attributes.Contains("id"), "Attribute id not found.");
		}		
		
		[Test]
		public void HtmlDirAttribute()
		{
			Assert.IsTrue(attributes.Contains("dir"), "Attribute dir not found.");
		}			
		
		[Test]
		public void HtmlXmlLangAttribute()
		{
			Assert.IsTrue(attributes.Contains("xml:lang"), "Attribute xml:lang not found.");
		}				
		
		protected override string GetSchema()
		{
			return "<xs:schema version=\"1.0\" xml:lang=\"en\"\r\n" +
					"    xmlns:xs=\"http://www.w3.org/2001/XMLSchema\"\r\n" +
					"    targetNamespace=\"http://foo/xhtml\"\r\n" +
					"    xmlns=\"http://foo/xhtml\"\r\n" +
					"    elementFormDefault=\"qualified\">\r\n" +
					"  <xs:element name=\"html\">\r\n" +
					"    <xs:complexType>\r\n" +
					"      <xs:sequence>\r\n" +
					"        <xs:element ref=\"head\"/>\r\n" +
					"        <xs:element ref=\"body\"/>\r\n" +
					"      </xs:sequence>\r\n" +
					"      <xs:attributeGroup ref=\"i18n\"/>\r\n" +
					"      <xs:attribute name=\"id\" type=\"xs:ID\"/>\r\n" +
					"    </xs:complexType>\r\n" +
					"  </xs:element>\r\n" +
					"\r\n" +
					"  <xs:element name=\"head\" type=\"xs:string\"/>\r\n" +
					"  <xs:element name=\"body\" type=\"xs:string\"/>\r\n" +
					"\r\n" +
					"  <xs:attributeGroup name=\"i18n\">\r\n" +
					"    <xs:annotation>\r\n" +
					"      <xs:documentation>\r\n" +
					"      internationalization attributes\r\n" +
					"      lang        language code (backwards compatible)\r\n" +
					"      xml:lang    language code (as per XML 1.0 spec)\r\n" +
					"      dir         direction for weak/neutral text\r\n" +
					"      </xs:documentation>\r\n" +
					"    </xs:annotation>\r\n" +
					"    <xs:attribute name=\"lang\" type=\"LanguageCode\"/>\r\n" +
					"    <xs:attribute ref=\"xml:lang\"/>\r\n" +
					"\r\n" +
					"    <xs:attribute name=\"dir\">\r\n" +
					"      <xs:simpleType>\r\n" +
					"        <xs:restriction base=\"xs:token\">\r\n" +
					"          <xs:enumeration value=\"ltr\"/>\r\n" +
					"          <xs:enumeration value=\"rtl\"/>\r\n" +
					"        </xs:restriction>\r\n" +
					"      </xs:simpleType>\r\n" +
					"    </xs:attribute>\r\n" +
					"  </xs:attributeGroup>\r\n" +
					"</xs:schema>";
		}
	}
}
