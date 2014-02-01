// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
