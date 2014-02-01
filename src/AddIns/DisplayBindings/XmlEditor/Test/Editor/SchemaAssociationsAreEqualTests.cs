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
