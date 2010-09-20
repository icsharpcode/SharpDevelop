// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.XmlEditor;
using NUnit.Framework;
using System;

namespace XmlEditor.Tests.XPath
{
	[TestFixture]
	public class XmlNamespaceToStringTests
	{
		[Test]
		public void PrefixAndNamespaceToString()
		{
			XmlNamespace ns = new XmlNamespace("f", "http://foo.com");
			Assert.AreEqual("Prefix [f] Uri [http://foo.com]", ns.ToString());
		}
		
		[Test]
		public void PrefixAndNamespaceFromString()
		{
			XmlNamespace ns = XmlNamespace.FromString("Prefix [f] Uri [http://foo.com]");
			Assert.AreEqual("f", ns.Prefix);
			Assert.AreEqual("http://foo.com", ns.Name);
		}
		
		[Test]
		public void EmptyPrefixAndNamespaceFromString()
		{
			XmlNamespace ns = XmlNamespace.FromString("Prefix [] Uri [http://foo.com]");
			Assert.AreEqual(String.Empty, ns.Prefix);
			Assert.AreEqual("http://foo.com", ns.Name);
		}
		
		[Test]
		public void PrefixAndEmptyNamespaceFromString()
		{
			XmlNamespace ns = XmlNamespace.FromString("Prefix [f] Uri []");
			Assert.AreEqual("f", ns.Prefix);
			Assert.AreEqual(String.Empty, ns.Name);
		}
		
		[Test]
		public void FromEmptyString()
		{
			XmlNamespace ns = XmlNamespace.FromString(String.Empty);
			Assert.AreEqual(String.Empty, ns.Prefix);
			Assert.AreEqual(String.Empty, ns.Name);
		}
	}
}
