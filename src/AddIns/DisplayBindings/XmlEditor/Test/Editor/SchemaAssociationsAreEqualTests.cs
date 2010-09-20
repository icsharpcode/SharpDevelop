// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.XmlEditor;
using NUnit.Framework;

namespace XmlEditor.Tests.Editor
{
	[TestFixture]
	public class SchemaAssociationsAreEqualTests
	{
		[Test]
		public void DoNotMatchIfNamespacePrefixesAreDifferent()
		{
			XmlSchemaFileAssociation lhs = new XmlSchemaFileAssociation("ext", "namespaceUri", "prefix");
			XmlSchemaFileAssociation rhs = new XmlSchemaFileAssociation("ext", "namespaceUri", "different-prefix");
			Assert.IsFalse(lhs.Equals(rhs));
		}
		
		[Test]
		public void MatchIfFileExtensionAndNamespaceUriAndPrefixMatch()
		{
			XmlSchemaFileAssociation lhs = new XmlSchemaFileAssociation("ext", "namespaceUri", "prefix");
			XmlSchemaFileAssociation rhs = new XmlSchemaFileAssociation("ext", "namespaceUri", "prefix");
			Assert.IsTrue(lhs.Equals(rhs));
		}
		
		[Test]
		public void DoNotMatchIfFileExtensionsAreDifferent()
		{
			XmlSchemaFileAssociation lhs = new XmlSchemaFileAssociation("ext", "namespaceUri", "prefix");
			XmlSchemaFileAssociation rhs = new XmlSchemaFileAssociation("different-ext", "namespaceUri", "prefix");
			Assert.IsFalse(lhs.Equals(rhs));
		}
		
		[Test]
		public void DoNotMatchNamespacesDifferent()
		{
			XmlSchemaFileAssociation lhs = new XmlSchemaFileAssociation("ext", "namespaceUri", "prefix");
			XmlSchemaFileAssociation rhs = new XmlSchemaFileAssociation("ext", "different-namespaceUri", "prefix");
			Assert.IsFalse(lhs.Equals(rhs));
		}
		
		[Test]
		public void DoesNotMatchAStringObject()
		{
			XmlSchemaFileAssociation lhs = new XmlSchemaFileAssociation("ext", "namespaceUri", "prefix");
			Assert.IsFalse(lhs.Equals("String"));
		}
	}
}
