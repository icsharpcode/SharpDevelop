// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.XmlEditor;
using NUnit.Framework;

namespace XmlEditor.Tests.Editor
{
	[TestFixture]
	public class IsEmptySchemaAssociationTests
	{
		[Test]
		public void SchemaAssociationWithBlankFileExtensionAndNamespaceUriAndNamespacePrefixIsEmpty()
		{
			XmlSchemaFileAssociation schemaAssociation = new XmlSchemaFileAssociation(String.Empty, String.Empty, String.Empty);
			Assert.IsTrue(schemaAssociation.IsEmpty);
		}
		
		[Test]
		public void SchemaAssociationWithFileExtensionIsNotEmpty()
		{
			XmlSchemaFileAssociation schemaAssociation = new XmlSchemaFileAssociation(".fileExt", String.Empty, String.Empty);
			Assert.IsFalse(schemaAssociation.IsEmpty);
		}

		[Test]
		public void SchemaAssociationWithNamespaceUriIsNotEmpty()
		{
			XmlSchemaFileAssociation schemaAssociation = new XmlSchemaFileAssociation(String.Empty, "namespace-uri", String.Empty);
			Assert.IsFalse(schemaAssociation.IsEmpty);
		}
		
		[Test]
		public void SchemaAssociationWithNamespacePrefixIsNotEmpty()
		{
			XmlSchemaFileAssociation schemaAssociation = new XmlSchemaFileAssociation(String.Empty, String.Empty, "prefix");
			Assert.IsFalse(schemaAssociation.IsEmpty);
		}		
	}
}
