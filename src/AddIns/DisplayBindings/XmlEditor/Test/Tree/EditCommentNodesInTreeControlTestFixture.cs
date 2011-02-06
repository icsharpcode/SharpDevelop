// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;
using System.Xml;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Tree
{
	/// <summary>
	/// Tests the display of XmlComment nodes in the Xml Tree
	/// and the removal and addition of new xml comment nodes.
	/// </summary>
	[TestFixture]
	public class EditCommentNodesInTreeControlTestFixture
	{
		XmlDocument doc;
		DerivedXmlTreeViewContainerControl treeViewContainer; 
		XmlTreeViewControl treeView;
		XmlCommentTreeNode rootCommentTreeNode;
		XmlComment rootCommentNode;
		XmlCommentTreeNode childCommentTreeNode;
		XmlComment childCommentNode;
		XmlElementTreeNode childElementTreeNode;
		XmlElement childElement;
		XmlElementTreeNode rootElementTreeNode;
		XmlElement rootElement;
		
		[SetUp]
		public void SetUp()
		{
			treeViewContainer = new DerivedXmlTreeViewContainerControl();
			string xml = "<!-- Root comment --><root><!-- Child comment --><child></child></root>";
			treeViewContainer.LoadXml(xml);
			
			doc = treeViewContainer.Document;
			treeView = treeViewContainer.TreeView;
			
			// Get the root comment node in the tree.
			rootCommentNode = (XmlComment)doc.FirstChild;
			rootCommentTreeNode = treeView.Nodes[0] as XmlCommentTreeNode;
			
			// Get the child comment node in the tree.
			rootElementTreeNode = (XmlElementTreeNode)treeView.Nodes[1];
			rootElementTreeNode.Expanding();
			rootElement = rootElementTreeNode.XmlElement;
			
			childCommentTreeNode = rootElementTreeNode.Nodes[0] as XmlCommentTreeNode;
			childCommentNode = (XmlComment)rootElementTreeNode.XmlElement.FirstChild;
			
			childElementTreeNode = rootElementTreeNode.Nodes[1] as XmlElementTreeNode;
			childElement = childElementTreeNode.XmlElement;
		}
		
		[TearDown]
		public void TearDown()
		{
			if (treeViewContainer != null) {
				treeViewContainer.Dispose();
			}
		}
		
		/// <summary>
		/// Should have an xml element and xml comment as root
		/// nodes in the XmlTreeViewControl.
		/// </summary>
		[Test]
		public void TwoRootNodes()
		{
			Assert.AreEqual(2, treeView.Nodes.Count);
		}
		
		[Test]
		public void RootCommentNodeExists()
		{
			Assert.IsNotNull(rootCommentTreeNode);
		}
		
		[Test]
		public void RootCommentNodeText()
		{
			Assert.AreEqual("Root comment", rootCommentTreeNode.Text);
		}
		
		[Test]
		public void RootCommentXmlNode()
		{
			Assert.AreSame(rootCommentNode, rootCommentTreeNode.XmlComment);
		}
		
		[Test]
		public void ChildCommentNodeExists()
		{
			Assert.IsNotNull(childCommentTreeNode);
		}
		
		[Test]
		public void ChildCommentNodeText()
		{
			Assert.AreEqual("Child comment", childCommentTreeNode.Text);
		}
		
		[Test]
		public void ChildCommentXmlNode()
		{
			Assert.AreSame(childCommentNode, childCommentTreeNode.XmlComment);
		}
		
		[Test]
		public void CommentTextChanged()
		{
			// Select the comment node.
			treeView.SelectedNode = rootCommentTreeNode;
			
			string newText = "changed text";
			treeViewContainer.TextContent = newText;
			
			treeViewContainer.CallTextBoxTextChanged();
			
			Assert.AreEqual(newText, rootCommentTreeNode.XmlComment.InnerText);
			Assert.AreEqual(newText, rootCommentTreeNode.Text, "Tree node text should be updated with new XmlComment's inner text");
		}
		
		/// <summary>
		/// Updates the comment node when no text node is selected in the
		/// tree.
		/// </summary>
		[Test]
		public void UpdateCommentNodeText()
		{
			treeView.SelectedNode = null;
			
			rootCommentNode.InnerText = "New value";
			treeView.UpdateComment(rootCommentNode);
			Assert.AreEqual("New value", rootCommentTreeNode.Text);
		}
		
		/// <summary>
		/// Tests that when the XmlTreeView's UpdateCommentNode method
		/// is called we do not get a null exception if the
		/// text node cannot be found in the tree.
		/// </summary>
		[Test]
		public void UpdateUnknownTextNodeText()
		{
			// Select the root comment node.
			treeView.SelectedNode = rootCommentTreeNode;
			
			XmlComment comment = doc.CreateComment(String.Empty);
			treeView.UpdateComment(comment);
		}
		
		[Test]
		public void UpdateChildCommentNodeText()
		{
			treeView.SelectedNode = null;
			
			childCommentNode.InnerText = "New value";
			treeView.UpdateComment(childCommentNode);
			Assert.AreEqual("New value", childCommentTreeNode.Text);
		}
		
		/// <summary>
		/// Makes sure that nothing happens if we try to add a text
		/// node to the currently selected element node when there
		/// is no selected element node.
		/// </summary>
		[Test]
		public void AddChildCommentNodeWhenNoElementSelected()
		{
			treeView.SelectedNode = null;
			XmlComment newCommentNode = doc.CreateComment(String.Empty);
			treeView.AppendChildComment(newCommentNode);
			
			Assert.IsFalse(treeViewContainer.IsDirty);
		}
		
		[Test]
		public void RemoveCommentWhenNoNodeSelected()
		{
			treeView.SelectedNode = null;
			treeView.RemoveComment(rootCommentNode);
			
			Assert.AreEqual(1, treeView.Nodes.Count);
			Assert.IsInstanceOf(typeof(XmlElementTreeNode), treeView.Nodes[0]);
		}
		
		[Test]
		public void RemoveUnknownCommentNode()
		{
			treeView.SelectedNode = rootCommentTreeNode;
			XmlComment newCommentNode = doc.CreateComment(String.Empty);
			treeView.RemoveComment(newCommentNode);
		}
		
		[Test]
		public void InsertCommentBeforeRootComment()
		{
			treeView.SelectedNode = rootCommentTreeNode;
			XmlComment newCommentNode = doc.CreateComment(String.Empty);
			treeView.InsertCommentBefore(newCommentNode);
			
			XmlCommentTreeNode commentNode = treeView.Nodes[0] as XmlCommentTreeNode;
			XmlCommentTreeNode originalCommentNode = treeView.Nodes[1] as XmlCommentTreeNode;
			Assert.AreEqual(3, treeView.Nodes.Count);
			Assert.AreEqual(String.Empty, commentNode.Text);
			Assert.AreEqual(newCommentNode, commentNode.XmlComment);
			Assert.AreSame(rootCommentTreeNode, originalCommentNode);
			Assert.IsNull(commentNode.Parent);
		}
		
		[Test]
		public void InsertCommentBeforeChildElement()
		{
			treeView.SelectedNode = childElementTreeNode;
			XmlComment newCommentNode = doc.CreateComment(String.Empty);
			treeView.InsertCommentBefore(newCommentNode);
			
			XmlCommentTreeNode commentNode = childElementTreeNode.PrevNode as XmlCommentTreeNode;
			Assert.AreEqual(String.Empty, commentNode.Text);
			Assert.AreEqual(newCommentNode, commentNode.XmlComment);
			Assert.IsNotNull(commentNode.Parent);
		}
		
		[Test]
		public void InsertCommentBeforeWhenNothingSelected()
		{
			treeView.SelectedNode = null;
			XmlComment newCommentNode = doc.CreateComment(String.Empty);
			treeView.InsertCommentBefore(newCommentNode);
		}
		
		[Test]
		public void InsertCommentAfterRootComment()
		{
			treeView.SelectedNode = rootCommentTreeNode;
			XmlComment newCommentNode = doc.CreateComment(String.Empty);
			treeView.InsertCommentAfter(newCommentNode);
			
			XmlCommentTreeNode commentNode = treeView.Nodes[1] as XmlCommentTreeNode;
			XmlCommentTreeNode originalCommentNode = treeView.Nodes[0] as XmlCommentTreeNode;
			Assert.AreEqual(3, treeView.Nodes.Count);
			Assert.AreEqual(String.Empty, commentNode.Text);
			Assert.AreEqual(newCommentNode, commentNode.XmlComment);
			Assert.AreSame(rootCommentTreeNode, originalCommentNode);
			Assert.IsNull(commentNode.Parent);
		}
		
		[Test]
		public void InsertCommentAfterChildElement()
		{
			treeView.SelectedNode = childElementTreeNode;
			XmlComment newCommentNode = doc.CreateComment(String.Empty);
			treeView.InsertCommentAfter(newCommentNode);
			
			XmlCommentTreeNode commentNode = childElementTreeNode.NextNode as XmlCommentTreeNode;
			Assert.AreEqual(String.Empty, commentNode.Text);
			Assert.AreEqual(newCommentNode, commentNode.XmlComment);
		}
		
		[Test]
		public void InsertElementAfterWhenCommentSelected()
		{
			treeView.SelectedNode = childCommentTreeNode;
			XmlElement element = doc.CreateElement("new");
			treeView.InsertElementAfter(element);
			
			XmlElementTreeNode node = childCommentTreeNode.NextNode as XmlElementTreeNode;
			Assert.AreEqual("new", node.Text);
			Assert.AreEqual(element, node.XmlElement);
		}
		
		/// <summary>
		/// Tests that when a comment node is selected the tree view container
		/// returns this from the SelectedNode property.
		/// </summary>
		[Test]
		public void SelectedNodeWhenCommentNodeSelected()
		{
			treeView.SelectedNode = childCommentTreeNode;
			Assert.AreEqual(childCommentTreeNode.XmlComment, treeViewContainer.SelectedNode);
		}
	}
}
