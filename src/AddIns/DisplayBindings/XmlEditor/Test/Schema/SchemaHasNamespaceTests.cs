// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.XmlEditor;
using NUnit.Framework;

namespace XmlEditor.Tests.Schema
{
	[TestFixture]
	public class SchemaHasNamespaceTests
	{
		[Test]
		public void SchemaNamespaceReturnsFalseForEmptyNamespaceString()
		{
			string xml = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' targetNamespace='' />";
			XmlSchemaCompletion schema = new XmlSchemaCompletion(new StringReader(xml));
			Assert.IsFalse(schema.HasNamespaceUri);
		}
		
		[Test]
		public void SchemaNamespaceReturnsTrueForNonEmptyNamespaceString()
		{
			string xml = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' targetNamespace='a' />";
			XmlSchemaCompletion schema = new XmlSchemaCompletion(new StringReader(xml));
			Assert.IsTrue(schema.HasNamespaceUri);
		}

		[Test]
		public void SchemaNamespaceReturnsFalseForMissingNamespace()
		{
			string xml = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' />";
			XmlSchemaCompletion schema = new XmlSchemaCompletion(new StringReader(xml));
			Assert.IsFalse(schema.HasNamespaceUri);
		}
		
		[Test]
		public void SchemaNamespaceReturnsFalseForWhitespaceNamespaceString()
		{
			string xml = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' targetNamespace='    ' />";
			XmlSchemaCompletion schema = new XmlSchemaCompletion(new StringReader(xml));
			Assert.IsFalse(schema.HasNamespaceUri);
		}
	}
}
