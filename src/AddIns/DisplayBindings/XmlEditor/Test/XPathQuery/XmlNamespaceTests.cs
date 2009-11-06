// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.XmlEditor;
using NUnit.Framework;
using System;

namespace XmlEditor.Tests.XPathQuery
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
