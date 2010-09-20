// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.XmlEditor;
using NUnit.Framework;
using System;
using System.Xml;

namespace XmlEditor.Tests.Tree
{
	[TestFixture]
	public class XmlTextTreeNodeTextTests
	{
		XmlDocument doc;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			doc = new XmlDocument();
			doc.LoadXml("<root/>");
		}

		[Test]
		public void WhiteSpaceRemoved()
		{
			XmlText text = doc.CreateTextNode("  \t\tTest\t\t   ");
			XmlTextTreeNode node = new XmlTextTreeNode(text);
			Assert.AreEqual("Test", node.Text);
		}
		
		[Test]
		public void TextOnSecondLine()
		{
			XmlText text = doc.CreateTextNode("\r\nTest");
			XmlTextTreeNode node = new XmlTextTreeNode(text);
			Assert.AreEqual("Test", node.Text);
		}
		
		[Test]
		public void ThreeLinesTextOnSecondLine()
		{
			XmlText text = doc.CreateTextNode("\r\nTest\r\n");
			XmlTextTreeNode node = new XmlTextTreeNode(text);
			Assert.AreEqual("Test", node.Text);
		}

		[Test]
		public void TwoLines()
		{
			XmlText text = doc.CreateTextNode("Test\r\nSecond Line");
			XmlTextTreeNode node = new XmlTextTreeNode(text);
			Assert.AreEqual("Test...", node.Text);
		}
		
		[Test]
		public void EmptyLines()
		{
			XmlText text = doc.CreateTextNode("\r\n\r\n\r\n");
			XmlTextTreeNode node = new XmlTextTreeNode(text);
			Assert.AreEqual(String.Empty, node.Text);
		}
		
		[Test]
		public void GhostImage()
		{
			XmlText text = doc.CreateTextNode("\r\n\r\n\r\n");
			XmlTextTreeNode node = new XmlTextTreeNode(text);

			Assert.IsFalse(node.ShowGhostImage);
			
			node.ShowGhostImage = false;
			Assert.AreEqual(XmlTextTreeNode.XmlTextTreeNodeImageKey, node.ImageKey);
		
			node.ShowGhostImage = true;
			Assert.IsTrue(node.ShowGhostImage);
			Assert.AreEqual(XmlTextTreeNode.XmlTextTreeNodeGhostImageKey, node.ImageKey);
		}
	}
}
