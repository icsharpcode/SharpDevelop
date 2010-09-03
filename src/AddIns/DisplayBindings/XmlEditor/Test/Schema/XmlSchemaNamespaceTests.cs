// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.XmlEditor;
using NUnit.Framework;
using System;

namespace XmlEditor.Tests.Schema
{
	/// <summary>
	/// Tests that the standard W3C namespace for XSD files is recognised.
	/// </summary>
	[TestFixture]
	public class XmlSchemaNamespaceTests
	{
		[Test]
		public void IsXmlSchemaNamespace()
		{
			Assert.IsTrue(XmlSchemaDefinition.IsXmlSchemaNamespace("http://www.w3.org/2001/XMLSchema"));
		}
		
		[Test]
		public void IsNotXmlSchemaNamespace()
		{
			Assert.IsFalse(XmlSchemaDefinition.IsXmlSchemaNamespace("http://foo.com"));
		}
		
		[Test]
		public void EmptyString()
		{
			Assert.IsFalse(XmlSchemaDefinition.IsXmlSchemaNamespace(String.Empty));
		}
	}
}
