// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.XmlEditor;
using NUnit.Framework;

namespace XmlEditor.Tests.Paths
{
	[TestFixture]
	public class XmlNamespaceCollectionTests
	{
		XmlNamespaceCollection namespaceCollection;
		
		[SetUp]
		public void Init()
		{
			namespaceCollection = new XmlNamespaceCollection();
		}
		
		[Test]
		public void ToArrayReturnsXmlNamespaceItemsInCollection()
		{
			XmlNamespace a = new XmlNamespace("a", "a-ns");
			XmlNamespace b = new XmlNamespace("b", "b-ns");
			
			namespaceCollection.Add(a);
			namespaceCollection.Add(b);
			
			XmlNamespace[] expectedArray = new XmlNamespace[] { a, b };
			
			Assert.AreEqual(expectedArray, namespaceCollection.ToArray());
		}
		
		[Test]
		public void GetNamespaceForPrefixReturnsPrefixForKnownNamespaceInCollection()
		{
			XmlNamespace ns = new XmlNamespace("prefix", "namespace");
			namespaceCollection.Add(ns);
			
			Assert.AreEqual("namespace", namespaceCollection.GetNamespaceForPrefix("prefix"));
		}
		
		[Test]
		public void GetNamespaceForPrefixReturnsEmptyStringForUnknownNamespaceInCollection()
		{
			XmlNamespace ns = new XmlNamespace("prefix", "namespace");
			namespaceCollection.Add(ns);
			
			Assert.AreEqual(String.Empty, namespaceCollection.GetNamespaceForPrefix("unknown-prefix"));
		}
		
		[Test]
		public void ContainsReturnsFalseForKnownNamespaceWhenPrefixDoesNotMatch()
		{
			XmlNamespace ns = new XmlNamespace("prefix", "namespace");
			namespaceCollection.Add(ns);
			XmlNamespace namespaceWithDifferentPrefix = new XmlNamespace(String.Empty, "namespace");
			
			Assert.IsFalse(namespaceCollection.Contains(namespaceWithDifferentPrefix));
		}
		
		[Test]
		public void ContainsReturnsTrueForKnownNamespaceWhenPrefixIsEmptyString()
		{
			XmlNamespace ns = new XmlNamespace(String.Empty, "namespace");
			namespaceCollection.Add(ns);
			
			Assert.IsTrue(namespaceCollection.Contains(ns));
		}
		
		[Test]
		public void ContainsReturnsFalseForUnknownNamespace()
		{
			XmlNamespace ns = new XmlNamespace("prefix", "namespace");
			namespaceCollection.Add(ns);
			
			XmlNamespace unknownNamespace = new XmlNamespace("prefix", "unknown-namespace");
			
			Assert.IsFalse(namespaceCollection.Contains(unknownNamespace));
		}
		
		[Test]
		public void GetPrefixReturnsNamespaceForKnownNamespaceInCollection()
		{
			XmlNamespace ns = new XmlNamespace("prefix", "namespace");
			namespaceCollection.Add(ns);
			
			Assert.AreEqual("prefix", namespaceCollection.GetPrefix("namespace"));
		}
		
		[Test]
		public void GetPrefixReturnsEmptyStringForUnknownNamespaceInCollection()
		{
			XmlNamespace ns = new XmlNamespace("prefix", "namespace");
			namespaceCollection.Add(ns);
			
			Assert.AreEqual(String.Empty, namespaceCollection.GetPrefix("unknown-namespace"));
		}
	}
}
