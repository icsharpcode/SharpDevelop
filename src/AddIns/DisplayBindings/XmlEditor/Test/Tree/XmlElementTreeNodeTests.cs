// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.XmlEditor;
using NUnit.Framework;
using System;
using System.Xml;

namespace XmlEditor.Tests.Tree
{
	[TestFixture]
	public class XmlElementTreeNodeTextTests
	{
		XmlDocument doc;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			doc = new XmlDocument();
			doc.LoadXml("<root/>");
		}
		
		[Test]
		public void UnqualifiedName()
		{
			XmlElement element = doc.CreateElement("child");
			XmlElementTreeNode node = new XmlElementTreeNode(element);
			Assert.AreEqual("child", node.Text);
		}

		[Test]
		public void QualifiedName()
		{
			XmlElement element = doc.CreateElement("child", "http://test.com");
			XmlElementTreeNode node = new XmlElementTreeNode(element);
			Assert.AreEqual("child", node.Text);
		}
		
		[Test]
		public void PrefixedName()
		{
			XmlElement element = doc.CreateElement("t", "child", "http://test.com");
			XmlElementTreeNode node = new XmlElementTreeNode(element);
			Assert.AreEqual("t:child", node.Text);
		}

	}
}
