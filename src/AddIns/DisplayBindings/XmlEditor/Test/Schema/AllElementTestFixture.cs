// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.XmlEditor;
using NUnit.Framework;

namespace XmlEditor.Tests.Schema
{
	/// <summary>
	/// Tests that element completion works for any child elements
	/// inside an xs:all schema element.
	/// </summary>
	[TestFixture]
	public class AllElementTestFixture : SchemaTestFixtureBase
	{		
		XmlCompletionItemCollection personElementChildren;
		XmlCompletionItemCollection firstNameAttributes;
		XmlCompletionItemCollection firstNameElementChildren;
		
		public override void FixtureInit()
		{
			XmlElementPath path = new XmlElementPath();
			path.Elements.Add(new QualifiedName("person", "http://foo"));
			personElementChildren = SchemaCompletion.GetChildElementCompletion(path);
			
			path.Elements.Add(new QualifiedName("firstname", "http://foo"));
			firstNameAttributes = SchemaCompletion.GetAttributeCompletion(path);
			firstNameElementChildren = SchemaCompletion.GetChildElementCompletion(path);
		}
		
		[Test]
		public void PersonElementHasTwoChildElements()
		{
			Assert.AreEqual(2, personElementChildren.Count, 
			                "Should be 2 child elements.");
		}
		
		[Test]
		public void FirstNameElementHasAttribute()
		{
			Assert.AreEqual(1, firstNameAttributes.Count, "Should have one attribute.");
		}
		
		[Test]
		public void FirstNameElementHasChildren()
		{
			Assert.AreEqual(2, firstNameElementChildren.Count, 
			                "Should be 2 child elements.");
		}
		
		protected override string GetSchema()
		{
			return "<xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" elementFormDefault=\"qualified\" targetNamespace=\"http://foo\">\r\n" +
				"    <xs:element name=\"person\">\r\n" +
				"      <xs:complexType>\r\n" +
				"        <xs:all>\r\n" +
				"          <xs:element name=\"firstname\">\r\n" +
				"            <xs:complexType>\r\n" +
                "              <xs:sequence>\r\n" +
                "                <xs:element name=\"short\" type=\"xs:string\"/>\r\n" +
                "                <xs:element name=\"title\" type=\"xs:string\"/>\r\n" +
                "              </xs:sequence>\r\n" +
                "              <xs:attribute name=\"id\"/>\r\n" +
                "            </xs:complexType>\r\n" +
				"          </xs:element>\r\n" +
				"          <xs:element name=\"lastname\" type=\"xs:string\"/>\r\n" +
				"        </xs:all>\r\n" +
				"      </xs:complexType>\r\n" +
				"    </xs:element>\r\n" +
				"</xs:schema>";
		}
	}
}
