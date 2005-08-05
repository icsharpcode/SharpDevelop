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
	/// Element that is a simple content type.
	/// </summary>
	[TestFixture]
	public class SimpleContentWithAttributeSchemaTestFixture : SchemaTestFixtureBase
	{
		XmlSchemaCompletionData schemaCompletionData;
		ICompletionData[] attributeCompletionData;
		
		[TestFixtureSetUp]
		public void FixtureInit()
		{
			StringReader reader = new StringReader(GetSchema());
			schemaCompletionData = new XmlSchemaCompletionData(reader);
						
			XmlElementPath path = new XmlElementPath();
			path.Elements.Add(new QualifiedName("foo", "http://foo.com"));
			
			attributeCompletionData = schemaCompletionData.GetAttributeCompletionData(path);
		}
		
		[Test]
		public void BarAttributeExists()
		{
			Assert.IsTrue(base.Contains(attributeCompletionData, "bar"),
			              "Attribute bar does not exist.");
		}		
	
		string GetSchema()
		{
			return "<xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\"\r\n" +
				"\ttargetNamespace=\"http://foo.com\"\r\n" +
				"\txmlns=\"http://foo.com\">\r\n" +
				"\t<xs:element name=\"foo\">\r\n" +
				"\t\t<xs:complexType>\r\n" +
				"\t\t\t<xs:simpleContent>\r\n" +
				"\t\t\t\t<xs:extension base=\"xs:string\">\r\n" +
				"\t\t\t\t\t<xs:attribute name=\"bar\">\r\n" +
				"\t\t\t\t\t\t<xs:simpleType>\r\n" +
				"\t\t\t\t\t\t\t<xs:restriction base=\"xs:NMTOKEN\">\r\n" +
				"\t\t\t\t\t\t\t\t<xs:enumeration value=\"default\"/>\r\n" +
				"\t\t\t\t\t\t\t\t<xs:enumeration value=\"enable\"/>\r\n" +
				"\t\t\t\t\t\t\t\t<xs:enumeration value=\"disable\"/>\r\n" +
				"\t\t\t\t\t\t\t\t<xs:enumeration value=\"hide\"/>\r\n" +
				"\t\t\t\t\t\t\t\t<xs:enumeration value=\"show\"/>\r\n" +
				"\t\t\t\t\t\t\t</xs:restriction>\r\n" +
				"\t\t\t\t\t\t</xs:simpleType>\r\n" +
				"\t\t\t\t\t</xs:attribute>\r\n" +
				"\t\t\t\t\t<xs:attribute name=\"id\" type=\"xs:string\"/>\r\n" +
				"\t\t\t\t\t<xs:attribute name=\"msg\" type=\"xs:string\"/>\r\n" +
				"\t\t\t\t</xs:extension>\r\n" +
				"\t\t\t</xs:simpleContent>\r\n" +
				"\t\t</xs:complexType>\r\n" +
				"\t</xs:element>\r\n" +
				"</xs:schema>";
		}
	}
}
