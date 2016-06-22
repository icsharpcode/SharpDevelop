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
