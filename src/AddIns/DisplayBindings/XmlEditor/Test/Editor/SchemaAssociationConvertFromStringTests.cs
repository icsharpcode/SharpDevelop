// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.XmlEditor;
using NUnit.Framework;

namespace XmlEditor.Tests.Editor
{
	[TestFixture]
	public class SchemaAssociationConvertFromStringTests
	{
		[Test]
		public void FileExtensionAndNamespaceUriAndNamespaceConvertedFromString()
		{
			XmlSchemaFileAssociation expectedSchemaAssociation = new XmlSchemaFileAssociation("ext", "namespace-uri", "prefix");
			XmlSchemaFileAssociation schemaAssociation = XmlSchemaFileAssociation.ConvertFromString("ext|namespace-uri|prefix");
			Assert.AreEqual(expectedSchemaAssociation, schemaAssociation);
		}
		
		[Test]
		public void FileExtensionAndNamespaceUriConvertedFromString()
		{
			XmlSchemaFileAssociation expectedSchemaAssociation = new XmlSchemaFileAssociation("ext", "namespace-uri");
			XmlSchemaFileAssociation schemaAssociation = XmlSchemaFileAssociation.ConvertFromString("ext|namespace-uri|");
			Assert.AreEqual(expectedSchemaAssociation, schemaAssociation);
		}

		[Test]
		public void FileExtensionConvertedFromString()
		{
			XmlSchemaFileAssociation expectedSchemaAssociation = new XmlSchemaFileAssociation("ext", String.Empty);
			XmlSchemaFileAssociation schemaAssociation = XmlSchemaFileAssociation.ConvertFromString("ext||");
			Assert.AreEqual(expectedSchemaAssociation, schemaAssociation);
		}
		
		[Test]
		public void EmpyStringConverted()
		{
			XmlSchemaFileAssociation expectedSchemaAssociation = new XmlSchemaFileAssociation(String.Empty, String.Empty);
			Assert.AreEqual(expectedSchemaAssociation, XmlSchemaFileAssociation.ConvertFromString(String.Empty));
		}
	}
}
