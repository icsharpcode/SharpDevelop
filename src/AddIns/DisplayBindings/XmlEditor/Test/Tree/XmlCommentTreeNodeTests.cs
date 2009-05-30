// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 2164 $</version>
// </file>

using System;
using System.Xml;
using ICSharpCode.XmlEditor;
using NUnit.Framework;

namespace XmlEditor.Tests.Tree
{
	/// <summary>
	/// Tests the XmlCommentTreeNode class.
	/// </summary>
	[TestFixture]
	public class XmlCommentTreeNodeTests
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
			XmlComment comment = doc.CreateComment("  \t\tTest\t\t   ");
			XmlCommentTreeNode node = new XmlCommentTreeNode(comment);
			Assert.AreEqual("Test", node.Text);
		}
		
		[Test]
		public void ImageKey()
		{
			XmlComment comment = doc.CreateComment(String.Empty);
			XmlCommentTreeNode node = new XmlCommentTreeNode(comment);
			Assert.AreEqual(XmlCommentTreeNode.XmlCommentTreeNodeImageKey, node.ImageKey);
		}
		
		[Test]
		public void SelectedImageKey()
		{
			XmlComment comment = doc.CreateComment(String.Empty);
			XmlCommentTreeNode node = new XmlCommentTreeNode(comment);
			Assert.AreEqual(XmlCommentTreeNode.XmlCommentTreeNodeImageKey, node.SelectedImageKey);
		}
		
		[Test]
		public void XmlCommentIsSame()
		{
			XmlComment comment = doc.CreateComment(String.Empty);
			XmlCommentTreeNode node = new XmlCommentTreeNode(comment);
			Assert.AreSame(comment, node.XmlComment);
		}
		
		[Test]
		public void GhostImage()
		{
			XmlComment comment = doc.CreateComment(String.Empty);
			XmlCommentTreeNode node = new XmlCommentTreeNode(comment);

			Assert.IsFalse(node.ShowGhostImage);
			
			node.ShowGhostImage = false;
			Assert.AreEqual(XmlCommentTreeNode.XmlCommentTreeNodeImageKey, node.ImageKey);
		
			node.ShowGhostImage = true;
			Assert.IsTrue(node.ShowGhostImage);
			Assert.AreEqual(XmlCommentTreeNode.XmlCommentTreeNodeGhostImageKey, node.ImageKey);
		}
	}
}
