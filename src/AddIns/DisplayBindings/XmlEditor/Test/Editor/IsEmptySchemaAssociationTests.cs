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
