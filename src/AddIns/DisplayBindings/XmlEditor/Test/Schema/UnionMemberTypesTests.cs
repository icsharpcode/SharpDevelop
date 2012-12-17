// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.XmlEditor;
using NUnit.Framework;

namespace XmlEditor.Tests.Schema
{
	/// <summary>
	/// Tests that attribute completion works for union member types:
	/// &lt;xs:union memberTypes="YesNoType PreprocessorVariables"/&gt;
	/// </summary>
	[TestFixture]
	public class UnionMemberTypesTests : SchemaTestFixtureBase
	{
		const string namespaceURI = "http://schemas.microsoft.com/wix/2006/wi";
		const string prefix = "w";
		
		protected override string GetSchema()
		{
			return
				"<xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" \r\n" +
				"  xmlns:xse=\"http://schemas.microsoft.com/wix/2005/XmlSchemaExtension\" \r\n" +
				"  xmlns:html=\"http://www.w3.org/1999/xhtml\" \r\n" +
				"  targetNamespace=\"http://schemas.microsoft.com/wix/2006/wi\" \r\n" +
				"  xmlns=\"http://schemas.microsoft.com/wix/2006/wi\">\r\n" +
				"  <xs:element name=\"Component\">\r\n" +
				"    <xs:complexType>\r\n" +
				"      <xs:attribute name=\"KeyPath\" type=\"YesNoTypeUnion\">\r\n" +
				"      </xs:attribute>\r\n" +
				"    </xs:complexType>\r\n" +
				"  </xs:element>\r\n" +
				"  <xs:simpleType name=\"YesNoTypeUnion\">\r\n" +
				"    <xs:union memberTypes=\"YesNoType Another\"/>\r\n" +
				"  </xs:simpleType>\r\n" +
				"  <xs:simpleType name=\"YesNoType\">\r\n" +
				"    <xs:restriction base=\"xs:NMTOKEN\">\r\n" +
				"      <xs:enumeration value=\"no\" />\r\n" +
				"      <xs:enumeration value=\"yes\" />\r\n" +
				"    </xs:restriction>\r\n" +
				"  </xs:simpleType>\r\n" +
				"  <xs:simpleType name=\"Another\">\r\n" +
				"    <xs:restriction base=\"xs:string\">\r\n" +
				"      <xs:enumeration value=\"1\" />\r\n" +
				"      <xs:enumeration value=\"0\" />\r\n" +
				"    </xs:restriction>\r\n" +
				"  </xs:simpleType>\r\n" +
				"</xs:schema>";
		}
		
		[Test]
		public void GetAttributeValueCompletion_UnionMemberTypesJoinsTwoSimpleTypes_ReturnsUnionOfSimpleTypes()
		{
			var path = new XmlElementPath();
			path.AddElement(new QualifiedName("Component", namespaceURI));
			
			XmlCompletionItemCollection items = SchemaCompletion.GetAttributeValueCompletion(path, "KeyPath");
			
			Assert.AreEqual(4, items.Count);
			Assert.IsTrue(items.Contains("yes"));
			Assert.IsTrue(items.Contains("1"));
		}
	}
}
