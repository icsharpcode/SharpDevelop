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

using System;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Tree
{
	[TestFixture]
	public class RemoveTextNodesFromTreeControlTestFixture
	{
		XmlDocument doc;
		DerivedXmlTreeViewContainerControl treeViewContainerControl; 
		XmlTreeViewControl treeView;
		XmlElementTreeNode topElementTreeNode;
		XmlElementTreeNode childElementTreeNode;
		XmlTextTreeNode topTextTreeNode;
		
		[SetUp]
		public void SetUp()
		{
			treeViewContainerControl = new DerivedXmlTreeViewContainerControl();
			treeView = treeViewContainerControl.TreeView;
			string xml = "<root><top>text</top><bottom><child>text</child></bottom></root>";
			treeViewContainerControl.LoadXml(xml);
			doc = treeViewContainerControl.Document;
			
			ExtTreeNode rootNode = (ExtTreeNode)treeView.Nodes[0];
			rootNode.Expanding();
			
			topElementTreeNode = (XmlElementTreeNode)rootNode.Nodes[0];
			topElementTreeNode.Expanding();
			
			topTextTreeNode = (XmlTextTreeNode)topElementTreeNode.Nodes[0];
			
			ExtTreeNode bottomNode = (ExtTreeNode)rootNode.Nodes[1];
			bottomNode.Expanding();
			
			childElementTreeNode = (XmlElementTreeNode)bottomNode.Nodes[0];
			childElementTreeNode.Expanding();
		}
		
		[TearDown]
		public void TearDown()
		{
			if (treeViewContainerControl != null) {
				treeViewContainerControl.Dispose();
			}
		}
		
		[Test]
		public void RemoveTextNode()
		{
			treeView.SelectedNode = topTextTreeNode;
			treeView.RemoveTextNode(topTextTreeNode.XmlText);
			
			Assert.AreEqual(0, topElementTreeNode.Nodes.Count);			
		}
		
		/// <summary>
		/// Makes sure that nothing happens if we try to remove a text
		/// node when there is no currently selected node.
		/// </summary>
		[Test]
		public void RemoveTextNodeWhenNoTreeNodeSelected()
		{
			treeView.SelectedNode = null;
			XmlText newTextNode = doc.CreateTextNode(String.Empty);
			treeView.RemoveTextNode(newTextNode);
			
			Assert.AreEqual(1, topElementTreeNode.Nodes.Count);
		}
		
		/// <summary>
		/// Makes sure that nothing happens if we try to remove a text
		/// node that is not in the tree.
		/// </summary>
		[Test]
		public void RemoveUnknownTextNode()
		{
			treeView.SelectedNode = topElementTreeNode;
			XmlText newTextNode = doc.CreateTextNode(String.Empty);
			treeView.RemoveTextNode(newTextNode);
			
			Assert.AreEqual(1, topElementTreeNode.Nodes.Count);
		}
		
		[Test]
		public void RemoveKnownTextNodeWhenNothingSelected()
		{
			treeView.SelectedNode = null;
			treeView.RemoveTextNode(topTextTreeNode.XmlText);
			
			Assert.AreEqual(0, topElementTreeNode.Nodes.Count);			
		}
	}
}
