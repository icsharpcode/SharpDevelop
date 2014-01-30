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
