// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 915 $</version>
// </file>

using ICSharpCode.TextEditor.Gui.CompletionWindow;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using System;
using System.IO;

namespace XmlEditor.Tests.Schema
{
	/// <summary>
	/// Element that has a single attribute.
	/// </summary>
	[TestFixture]
	public class ElementWithAttributeSchemaTestFixture : SchemaTestFixtureBase
	{
		ICompletionData[] attributeCompletionData;
		string attributeName;
		
		public override void FixtureInit()
		{
			XmlElementPath path = new XmlElementPath();
			path.Elements.Add(new QualifiedName("note", "http://www.w3schools.com"));
			
			attributeCompletionData = SchemaCompletionData.GetAttributeCompletionData(path);
			attributeName = attributeCompletionData[0].Text;
		}

		[Test]
		public void AttributeCount()
		{
			Assert.AreEqual(1, attributeCompletionData.Length, "Should be one attribute.");
		}
		
		[Test]
		public void AttributeName()
		{
			Assert.AreEqual("name", attributeName, "Attribute name is incorrect.");
		}		
		
		[Test]
		public void NoAttributesForUnknownElement()
		{
			XmlElementPath path = new XmlElementPath();
			path.Elements.Add(new QualifiedName("foobar", "http://www.w3schools.com"));
			ICompletionData[] attributes = SchemaCompletionData.GetAttributeCompletionData(path);
			
			Assert.AreEqual(0, attributes.Length, "Should not find attributes for unknown element.");
		}
		
		protected override string GetSchema()
		{
			return "<xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" targetNamespace=\"http://www.w3schools.com\" xmlns=\"http://www.w3schools.com\" elementFormDefault=\"qualified\">\r\n" +
				"    <xs:element name=\"note\">\r\n" +
				"        <xs:complexType>\r\n" +
				"\t<xs:attribute name=\"name\"  type=\"xs:string\"/>\r\n" +
				"        </xs:complexType>\r\n" +
				"    </xs:element>\r\n" +
				"</xs:schema>";
		}
	}
}
