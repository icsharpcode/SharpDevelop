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
	/// Tests duplicate elements in the schema.
	/// </summary>
	[TestFixture]
	public class DuplicateElementTestFixture : SchemaTestFixtureBase
	{
		XmlSchemaCompletionData schemaCompletionData;
		ICompletionData[] htmlChildElements;
		
		[TestFixtureSetUp]
		public void FixtureInit()
		{
			StringReader reader = new StringReader(GetSchema());
			schemaCompletionData = new XmlSchemaCompletionData(reader);

			XmlElementPath path = new XmlElementPath();
			path.Elements.Add(new QualifiedName("html", "http://foo/xhtml"));
		
			htmlChildElements = schemaCompletionData.GetChildElementCompletionData(path);
		}		
		
		[Test]
		public void HtmlHasTwoChildElements()
		{
			Assert.AreEqual(2, htmlChildElements.Length, 
			                "Should be 2 child elements.");
		}
		
		[Test]
		public void HtmlChildElementHead()
		{
			Assert.IsTrue(base.Contains(htmlChildElements, "head"), 
			              "Should have a child element called head.");
		}
		
		[Test]
		public void HtmlChildElementBody()
		{
			Assert.IsTrue(base.Contains(htmlChildElements, "body"), 
			              "Should have a child element called body.");
		}		
		
		string GetSchema()
		{
			return "<xs:schema version=\"1.0\" xml:lang=\"en\"\r\n" +
					"    xmlns:xs=\"http://www.w3.org/2001/XMLSchema\"\r\n" +
					"    targetNamespace=\"http://foo/xhtml\"\r\n" +
					"    xmlns=\"http://foo/xhtml\"\r\n" +
					"    elementFormDefault=\"qualified\">\r\n" +
					"\r\n" +
					"  <xs:element name=\"html\">\r\n" +
					"    <xs:complexType>\r\n" +
					"      <xs:choice>\r\n" +
					"        <xs:sequence>\r\n" +
					"          <xs:element name=\"head\"/>\r\n" +
					"          <xs:element name=\"body\"/>\r\n" +
					"        </xs:sequence>\r\n" +
					"        <xs:sequence>\r\n" +
					"          <xs:element name=\"body\"/>\r\n" +
					"        </xs:sequence>\r\n" +
					"      </xs:choice>\r\n" +
					"    </xs:complexType>\r\n" +
					"  </xs:element>\r\n" +
					"\r\n" +
					"</xs:schema>";
		}
	}
}
