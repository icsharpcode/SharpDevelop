// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
