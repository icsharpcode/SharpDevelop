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
