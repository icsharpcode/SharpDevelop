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
using ICSharpCode.XmlEditor;
using NUnit.Framework;

namespace XmlEditor.Tests.Schema
{
	[TestFixture]
	public class SimpleContentExtensionBaseTypeWithAttributeTestFixture : SchemaTestFixtureBase
	{
		XmlCompletionItemCollection linkElementAttributes;
		string schemaNamespace = "http://ddue.schemas.microsoft.com/authoring/2003/5";
		
		public override void FixtureInit()
		{
			XmlElementPath path = new XmlElementPath();
			path.AddElement(new QualifiedName("link", schemaNamespace));
			path.NamespacesInScope.Add(new XmlNamespace("xlink", "http://www.w3.org/1999/xlink"));
			
			linkElementAttributes = SchemaCompletion.GetAttributeCompletion(path);
			linkElementAttributes.Sort();
		}
		
		[Test]
		public void LinkElementHasAddressAndXlinkHrefAttribute()
		{
			XmlCompletionItemCollection expectedAttributes = new XmlCompletionItemCollection();
			expectedAttributes.Add(new XmlCompletionItem("address", XmlCompletionItemType.XmlAttribute));
			expectedAttributes.Add(new XmlCompletionItem("xlink:href", XmlCompletionItemType.XmlAttribute));
			
			Assert.AreEqual(expectedAttributes.ToArray(), linkElementAttributes.ToArray());
		}
		
		protected override string GetSchema()
		{
			return 
				"<schema xmlns='http://www.w3.org/2001/XMLSchema'\r\n" +
				"    xmlns:maml='http://ddue.schemas.microsoft.com/authoring/2003/5' \r\n" +
				"    xmlns:doc='http://ddue.schemas.microsoft.com/authoring/internal'\r\n" +
				"    xmlns:xlink='http://www.w3.org/1999/xlink'\r\n" +
				"    targetNamespace='http://ddue.schemas.microsoft.com/authoring/2003/5' \r\n" +
				"    elementFormDefault='qualified'\r\n" +
				"    attributeFormDefault='unqualified'>\r\n" +
				"\r\n" +
				"<element ref='maml:link' />\r\n" +
				"<element name='link' type='maml:inlineLinkType' />\r\n" +
 				"<element name='legacyLink' type='maml:inlineLinkType' />\r\n" +
				"\r\n" +
				"<complexType name='inlineLinkType' mixed='true'>\r\n" +
				"    <simpleContent>\r\n" +
				"        <extension base='maml:textType'>\r\n" +
				"            <attributeGroup ref='maml:linkingGroup' />\r\n" +
				"        </extension>\r\n" +
				"    </simpleContent>\r\n" +
				"</complexType>\r\n" +
				"\r\n" +
				"<complexType name='textType'>\r\n" +
				"    <simpleContent>\r\n" +
				"        <extension base='normalizedString'>\r\n" +
				"            <attributeGroup ref='maml:contentIdentificationSharingAndConditionGroup'/>\r\n" +
				"        </extension>\r\n" +
				"    </simpleContent>\r\n" +
				"</complexType>\r\n" +
				"\r\n" +
				"<attributeGroup name='contentIdentificationSharingAndConditionGroup'>\r\n" +
				"    <attributeGroup ref='maml:addressAttributeGroup'/>\r\n" +
				"</attributeGroup>\r\n" +
				"\r\n" +
				"    <attributeGroup name='addressAttributeGroup'>\r\n" +
				"        <attribute name='address' type='ID'/>\r\n" +
				"    </attributeGroup>\r\n" +
				"\r\n" +
				"    <attributeGroup name='linkingGroup'>\r\n" +
				"        <attribute ref='xlink:href'/>\r\n" +
				"    </attributeGroup>\r\n" +
				"</schema>";
		}
	}
}
