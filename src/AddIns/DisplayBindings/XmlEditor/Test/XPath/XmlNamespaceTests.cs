// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.XmlEditor;
using NUnit.Framework;
using System;

namespace XmlEditor.Tests.XPath
{
	[TestFixture]
	public class XmlNamespaceTests
	{
		[Test]
		public void SimpleNamespace()
		{
			XmlNamespace ns = new XmlNamespace("s", "http://sharpdevelop.com");
			Assert.AreEqual("s", ns.Prefix);
			Assert.AreEqual("http://sharpdevelop.com", ns.Name);
		}
		
		[Test]
		public void SettingPrefixPropertyUpdatesPrefix()
		{
			XmlNamespace ns = new XmlNamespace("s", "http://sharpdevelop.com");
			ns.Prefix = "a";
			Assert.AreEqual("a", ns.Prefix);
		}
		
		[Test]
		public void SettingNamePropertyUpdatesNamespace()
		{
			XmlNamespace ns = new XmlNamespace("s", "http://sharpdevelop.com");
			ns.Name = "new-ns";
			Assert.AreEqual("new-ns", ns.Name);
		}
		
		[Test]
		public void NullNamespaceAndPrefixReplacedByEmptyStringsWhenCreatingXmlNamespaceInstance()
		{
			XmlNamespace ns = new XmlNamespace(null, null);
			Assert.AreEqual(String.Empty, ns.Name);
			Assert.AreEqual(String.Empty, ns.Prefix);
		}
		
		[Test]
		public void SettingNamePropertyToNullUpdatesNamespaceToEmptyString()
		{
			XmlNamespace ns = new XmlNamespace("s", "http://sharpdevelop.com");
			ns.Name = null;
			Assert.AreEqual(String.Empty, ns.Name);
		}
		
		[Test]
		public void SettingPrefixPropertyToNullUpdatesPrefixToEmptyString()
		{
			XmlNamespace ns = new XmlNamespace("s", "http://sharpdevelop.com");
			ns.Prefix = null;
			Assert.AreEqual(String.Empty, ns.Prefix);
		}		

		[Test]
		public void HasNamePropertyReturnsFalseWhenNamespaceIsEmptyString()
		{
			XmlNamespace ns = new XmlNamespace("s", String.Empty);
			Assert.IsFalse(ns.HasName);
		}
		
		[Test]
		public void HasNamePropertyReturnsTrueWhenNamespaceIsNotEmptyString()
		{
			XmlNamespace ns = new XmlNamespace("s", "ns");
			Assert.IsTrue(ns.HasName);
		}
	}
}
