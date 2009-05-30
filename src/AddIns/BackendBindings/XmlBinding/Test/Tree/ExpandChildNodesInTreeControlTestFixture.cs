// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 2128 $</version>
// </file>

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using System;
using System.Windows.Forms;
using System.Xml;

namespace XmlEditor.Tests.Tree
{
	[TestFixture]
	public class ExpandChildNodesInTreeControlTestFixture
	{
		ExtTreeNode rootNode;
		XmlDocument doc;
		ExtTreeNode firstChildNode;
		ExtTreeNode secondChildNode;
		ExtTreeNode textChildNode;
		ExtTreeNode textNode;
		bool rootNodeHadChildrenBeforeExpansion;
		bool firstChildNodeHadChildrenBeforeExpansion;
		XmlElement rootNodeElement;
		XmlElement textNodeElement;
		bool isRootElementSelected;
		bool isTextContentSelectedBeforeTextNodeSelected;
		bool isTextContentSelectedAfterTextNodeSelected;
		XmlText textContentBefore;
		XmlText textContentAfter;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			doc = new XmlDocument();
			doc.LoadXml("<root><firstChild><secondChild/></firstChild><textChild>some text</textChild></root>");
			using (XmlTreeViewControl treeView = new XmlTreeViewControl()) {
				treeView.Document = doc;
				treeView.SelectedNode = treeView.Nodes[0];
				rootNodeElement = treeView.SelectedElement;
				isRootElementSelected = treeView.IsElementSelected;
				rootNode = (ExtTreeNode)treeView.SelectedNode;
				rootNodeHadChildrenBeforeExpansion = rootNode.Nodes.Count > 0;
				rootNode.Expanding();
				firstChildNode = (ExtTreeNode)rootNode.Nodes[0];
				firstChildNodeHadChildrenBeforeExpansion = firstChildNode.Nodes.Count > 0;
				firstChildNode.Expanding();
				secondChildNode = (ExtTreeNode)firstChildNode.Nodes[0];
				textChildNode = (ExtTreeNode)rootNode.Nodes[1];
				textChildNode.Expanding();
				textNode = (ExtTreeNode)textChildNode.Nodes[0];
				isTextContentSelectedBeforeTextNodeSelected = treeView.IsTextNodeSelected;
				textContentBefore = treeView.SelectedTextNode;
				treeView.SelectedNode = textNode;
				isTextContentSelectedAfterTextNodeSelected = treeView.IsTextNodeSelected;
				textContentAfter = treeView.SelectedTextNode;
				textNodeElement = treeView.SelectedElement;
			}
		}
		
		[Test]
		public void RootElementIsDocumentElement()
		{
			Assert.IsTrue(Object.ReferenceEquals(rootNodeElement, doc.DocumentElement));
		}
		
		[Test]
		public void RootNodeHadChildrenBeforeExpansion()
		{
			Assert.IsTrue(rootNodeHadChildrenBeforeExpansion);
		}
		
		[Test]
		public void RootNodeText()
		{
			Assert.AreEqual("root", rootNode.Text);
		}
		
		[Test]
		public void FirstChildNodeHadChildrenBeforeExpansion()
		{
			Assert.IsTrue(firstChildNodeHadChildrenBeforeExpansion);
		}
		
		[Test]
		public void FirstChildNodeText()
		{
			Assert.AreEqual("firstChild", firstChildNode.Text);
		}
		
		[Test]
		public void FirstChildNodeImageKey()
		{
			Assert.AreEqual(XmlElementTreeNode.XmlElementTreeNodeImageKey, firstChildNode.ImageKey);
		}
		
		[Test]
		public void SecondChildNodeText()
		{
			Assert.AreEqual("secondChild", secondChildNode.Text);
		}
		
		[Test]
		public void SecondChildNodeImageKey()
		{
			Assert.AreEqual(XmlElementTreeNode.XmlElementTreeNodeImageKey, secondChildNode.ImageKey);
		}
		
		[Test]
		public void TextChildNodeText()
		{
			Assert.AreEqual("textChild", textChildNode.Text);
		}
		
		[Test]
		public void TextNodeExists()
		{
			Assert.IsNotNull(textNode);
		}
		
		[Test]
		public void TextNodeImageKey()
		{
			Assert.AreEqual(XmlTextTreeNode.XmlTextTreeNodeImageKey, textNode.ImageKey);
		}
		
		[Test]
		public void TextNodeSelectedImageKey()
		{
			Assert.AreEqual(XmlTextTreeNode.XmlTextTreeNodeImageKey, textNode.SelectedImageKey);
		}
		
		[Test]
		public void TextNodeText()
		{
			Assert.AreEqual("some text", textNode.Text);
		}
		
		[Test]
		public void NoElementForSelectedTextNode()
		{
			Assert.IsNull(textNodeElement);
		}
		
		[Test]
		public void IsElementSelectedForRootNode()
		{
			Assert.IsTrue(isRootElementSelected);
		}
		
		[Test]
		public void TextContentNotSelectedBeforeTextNodeSelected()
		{
			Assert.IsFalse(isTextContentSelectedBeforeTextNodeSelected);
		}
		
		[Test]
		public void TextContentSelectedAfterTextNodeSelected()
		{
			Assert.IsTrue(isTextContentSelectedAfterTextNodeSelected);
		}
		
		[Test]
		public void TextNodeNotSelectedBeforeTextNodeSelected()
		{
			Assert.IsNull(textContentBefore);
		}
		
		[Test]
		public void TextNodeSelectedAfterTextNodeSelected()
		{
			Assert.IsNotNull(textContentAfter);
			Assert.AreEqual("some text", textContentAfter.InnerText);
		}
	}
}
