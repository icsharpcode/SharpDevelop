// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 1662 $</version>
// </file>

using ICSharpCode.XmlEditor;
using NUnit.Framework;
using System;

namespace XmlEditor.Tests.XPathQuery
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
			Assert.AreEqual("http://foo.com", ns.Uri);
		}
		
		[Test]
		public void EmptyPrefixAndNamespaceFromString()
		{
			XmlNamespace ns = XmlNamespace.FromString("Prefix [] Uri [http://foo.com]");
			Assert.AreEqual(String.Empty, ns.Prefix);
			Assert.AreEqual("http://foo.com", ns.Uri);
		}
		
		[Test]
		public void PrefixAndEmptyNamespaceFromString()
		{
			XmlNamespace ns = XmlNamespace.FromString("Prefix [f] Uri []");
			Assert.AreEqual("f", ns.Prefix);
			Assert.AreEqual(String.Empty, ns.Uri);
		}
		
		[Test]
		public void FromEmptyString()
		{
			XmlNamespace ns = XmlNamespace.FromString(String.Empty);
			Assert.AreEqual(String.Empty, ns.Prefix);
			Assert.AreEqual(String.Empty, ns.Uri);
		}
	}
}
