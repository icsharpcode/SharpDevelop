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
	/// Tests attribute refs
	/// </summary>
	[TestFixture]
	public class EnumAttributeValueTestFixture : SchemaTestFixtureBase
	{
		XmlSchemaCompletionData schemaCompletionData;
		ICompletionData[] attributeValues;
		
		[TestFixtureSetUp]
		public void FixtureInit()
		{
			StringReader reader = new StringReader(GetSchema());
			schemaCompletionData = new XmlSchemaCompletionData(reader);
			XmlElementPath path = new XmlElementPath();
			path.Elements.Add(new QualifiedName("foo", "http://foo.com"));
			attributeValues = schemaCompletionData.GetAttributeValueCompletionData(path, "id");
		}
		
		[Test]
		public void IdAttributeHasValueOne()
		{
			Assert.IsTrue(base.Contains(attributeValues, "one"),
			              "Missing attribute value 'one'");
		}
		
		[Test]
		public void IdAttributeHasValueTwo()
		{
			Assert.IsTrue(base.Contains(attributeValues, "two"),
			              "Missing attribute value 'two'");
		}		
		
		[Test]
		public void IdAttributeValueCount()
		{
			Assert.AreEqual(2, attributeValues.Length, "Expecting 2 attribute values.");
		}
		
		string GetSchema()
		{
			return "<xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" xmlns=\"http://foo.com\" targetNamespace=\"http://foo.com\" elementFormDefault=\"qualified\">\r\n" +
				"\t<xs:element name=\"foo\">\r\n" +
				"\t\t<xs:complexType>\r\n" +
				"\t\t\t<xs:attribute name=\"id\">\r\n" +
				"\t\t\t\t<xs:simpleType>\r\n" +
				"\t\t\t\t\t<xs:restriction base=\"xs:string\">\r\n" +
				"\t\t\t\t\t\t<xs:enumeration value=\"one\"/>\r\n" +
				"\t\t\t\t\t\t<xs:enumeration value=\"two\"/>\r\n" +
				"\t\t\t\t\t</xs:restriction>\r\n" +
				"\t\t\t\t</xs:simpleType>\r\n" +
				"\t\t\t</xs:attribute>\r\n" +
				"\t\t</xs:complexType>\r\n" +
				"\t</xs:element>\r\n" +
				"</xs:schema>";
		}
	}
}
