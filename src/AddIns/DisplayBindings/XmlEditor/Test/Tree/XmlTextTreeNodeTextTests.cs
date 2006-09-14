// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
	}
}
