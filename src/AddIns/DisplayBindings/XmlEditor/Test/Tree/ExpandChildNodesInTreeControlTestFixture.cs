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
