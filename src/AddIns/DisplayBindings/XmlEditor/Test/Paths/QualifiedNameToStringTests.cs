// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.XmlEditor;
using NUnit.Framework;

namespace XmlEditor.Tests.Paths
{
	[TestFixture]
	public class QualifiedNameToStringTests
	{
		[Test]
		public void EmptyQualifiedName()
		{
			QualifiedName name = new QualifiedName();
			Assert.AreEqual(String.Empty, name.ToString());
		}
		
		[Test]
		public void NameOnly()
		{
			QualifiedName name = new QualifiedName("root", String.Empty);
			Assert.AreEqual("root", name.ToString());
		}
		
		[Test]
		public void NameAndNamespace()
		{
			QualifiedName name = new QualifiedName("root", "urn:my-uri");
			Assert.AreEqual("root [urn:my-uri]", name.ToString());
		}

		[Test]
		public void NameAndNamespaceAndPrefixS()
		{
			QualifiedName name = new QualifiedName("root", "urn:my-uri", "a");
			Assert.AreEqual("a:root [urn:my-uri]", name.ToString());
		}
		
		[Test]
		public void NameAndPrefixOnly()
		{
			QualifiedName name = new QualifiedName("root", String.Empty, "b");
			Assert.AreEqual("b:root", name.ToString());
		}
		
		[Test]
		public void NullPrefix()
		{
			QualifiedName name = new QualifiedName("root", String.Empty, null);
			Assert.AreEqual("root", name.ToString());
		}
		
		[Test]
		public void NullPrefixAndNullNamespace()
		{
			QualifiedName name = new QualifiedName("root", null, null);
			Assert.AreEqual("root", name.ToString());
		}
		
		[Test]
		public void NullPrefixAndNonEmptyNamespace()
		{
			QualifiedName name = new QualifiedName("root", "urn:my-uri", null);
			Assert.AreEqual("root [urn:my-uri]", name.ToString());
		}
		
		[Test]
		public void CreateQualifiedNameWithXmlNamespaceObject()
		{
			XmlNamespace ns = new XmlNamespace("prefix", "namespace");
			QualifiedName name = new QualifiedName("elementName", ns);
			Assert.AreEqual("prefix:elementName [namespace]", name.ToString());
		}
	}
}
