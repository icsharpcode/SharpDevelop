// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 2111 $</version>
// </file>

using System;
using System.Windows.Forms;
using System.Xml;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.XmlEditor;
using NUnit.Framework;

namespace XmlEditor.Tests.Tree
{
	[TestFixture]
	public class RemoveTextNodesFromTreeControlTestFixture
	{
		XmlDocument doc;
		XmlTreeViewContainerControl treeViewContainerControl; 
		XmlTreeViewControl treeView;
		XmlElementTreeNode topElementTreeNode;
		XmlElementTreeNode childElementTreeNode;
		XmlTextTreeNode topTextTreeNode;
		
		[SetUp]
		public void SetUp()
		{
			XmlCompletionDataProvider completionDataProvider = new XmlCompletionDataProvider(new XmlSchemaCompletionDataCollection(), null, String.Empty);
			treeViewContainerControl = new XmlTreeViewContainerControl();
			treeView = treeViewContainerControl.TreeView;
			treeViewContainerControl.LoadXml("<root><top>text</top><bottom><child>text</child></bottom></root>", completionDataProvider);
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
