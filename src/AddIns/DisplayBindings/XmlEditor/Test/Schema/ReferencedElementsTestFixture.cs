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
	[TestFixture]
	public class ReferencedElementsTestFixture : SchemaTestFixtureBase
	{
		XmlSchemaCompletionData schemaCompletionData;
		ICompletionData[] shipOrderAttributes;
		ICompletionData[] shipToAttributes;
		XmlElementPath shipToPath;
		XmlElementPath shipOrderPath;
		
		[TestFixtureSetUp]
		public void FixtureInit()
		{
			StringReader reader = new StringReader(GetSchema());
			schemaCompletionData = new XmlSchemaCompletionData(reader);
			
			// Get shipto attributes.
			shipToPath = new XmlElementPath();
			QualifiedName shipOrderName = new QualifiedName("shiporder", "http://www.w3schools.com");
			shipToPath.Elements.Add(shipOrderName);
			shipToPath.Elements.Add(new QualifiedName("shipto", "http://www.w3schools.com"));

			shipToAttributes = schemaCompletionData.GetAttributeCompletionData(shipToPath);
			
			// Get shiporder attributes.
			shipOrderPath = new XmlElementPath();
			shipOrderPath.Elements.Add(shipOrderName);
			
			shipOrderAttributes = schemaCompletionData.GetAttributeCompletionData(shipOrderPath);
			
		}
		
		[Test]
		public void OneShipOrderAttribute()
		{
			Assert.AreEqual(1, shipOrderAttributes.Length, "Should only have one shiporder attribute.");
		}		
		
		[Test]
		public void ShipOrderAttributeName()
		{
			Assert.IsTrue(base.Contains(shipOrderAttributes,"id"),
			                "Incorrect shiporder attribute name.");
		}

		[Test]
		public void OneShipToAttribute()
		{
			Assert.AreEqual(1, shipToAttributes.Length, "Should only have one shipto attribute.");
		}
		
		[Test]
		public void ShipToAttributeName()
		{
			Assert.IsTrue(base.Contains(shipToAttributes, "address"),
			                "Incorrect shipto attribute name.");
		}					
		
		[Test]
		public void ShipOrderChildElementsCount()
		{
			Assert.AreEqual(1, schemaCompletionData.GetChildElementCompletionData(shipOrderPath).Length, 
			                "Should be one child element.");
		}
		
		[Test]
		public void ShipOrderHasShipToChildElement()
		{
			ICompletionData[] data = schemaCompletionData.GetChildElementCompletionData(shipOrderPath);
			Assert.IsTrue(base.Contains(data, "shipto"), 
			                "Incorrect child element name.");
		}
		
		[Test]
		public void ShipToChildElementsCount()
		{
			Assert.AreEqual(2, schemaCompletionData.GetChildElementCompletionData(shipToPath).Length, 
			                "Should be 2 child elements.");
		}		
		
		[Test]
		public void ShipToHasNameChildElement()
		{
			ICompletionData[] data = schemaCompletionData.GetChildElementCompletionData(shipToPath);
			Assert.IsTrue(base.Contains(data, "name"), 
			                "Incorrect child element name.");
		}		
		
		[Test]
		public void ShipToHasAddressChildElement()
		{
			ICompletionData[] data = schemaCompletionData.GetChildElementCompletionData(shipToPath);
			Assert.IsTrue(base.Contains(data, "address"), 
			                "Incorrect child element name.");
		}		
		
		string GetSchema()
		{
			return "<xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" targetNamespace=\"http://www.w3schools.com\"  xmlns=\"http://www.w3schools.com\">\r\n" +
				"\r\n" +
				"<!-- definition of simple elements -->\r\n" +
				"<xs:element name=\"name\" type=\"xs:string\"/>\r\n" +
				"<xs:element name=\"address\" type=\"xs:string\"/>\r\n" +
				"\r\n" +
				"<!-- definition of complex elements -->\r\n" +
				"<xs:element name=\"shipto\">\r\n" +
				" <xs:complexType>\r\n" +
				"  <xs:sequence>\r\n" +
				"   <xs:element ref=\"name\"/>\r\n" +
				"   <xs:element ref=\"address\"/>\r\n" +
				"  </xs:sequence>\r\n" +
				"  <xs:attribute name=\"address\"/>\r\n" +
				" </xs:complexType>\r\n" +
				"</xs:element>\r\n" +
				"\r\n" +
				"<xs:element name=\"shiporder\">\r\n" +
				" <xs:complexType>\r\n" +
				"  <xs:sequence>\r\n" +
				"   <xs:element ref=\"shipto\"/>\r\n" +
				"  </xs:sequence>\r\n" +
				"  <xs:attribute name=\"id\"/>\r\n" +
				" </xs:complexType>\r\n" +
				"</xs:element>\r\n" +
				"\r\n" +
				"</xs:schema>";
		}
	}
}
