// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Xml;
using ICSharpCode.XmlEditor;
using NUnit.Framework;

namespace XmlEditor.Tests.Tree
{
	/// <summary>
	/// Tests the XmlTreeEditor when editing xml comments.
	/// </summary>
	[TestFixture]
	public class EditCommentNodesTestFixture : XmlTreeViewTestFixtureBase
	{
		XmlComment rootComment;
		XmlElement childElement;
		
		[SetUp]
		public void SetUpFixture()
		{
			base.InitFixture();
			rootComment = (XmlComment)mockXmlTreeView.Document.FirstChild;
			childElement = (XmlElement)mockXmlTreeView.Document.SelectSingleNode("root/child");
			mockXmlTreeView.SelectedComment = rootComment;
			editor.SelectedNodeChanged();
		}
		
		[Test]
		public void IsViewShowingText()
		{
			Assert.AreEqual(" Root comment ", mockXmlTreeView.TextContent);
		}
		
		[Test]
		public void AttributesClearedAfterCommentSelected()
		{
			mockXmlTreeView.SelectedComment = null;
			mockXmlTreeView.SelectedElement = mockXmlTreeView.Document.DocumentElement;
			editor.SelectedNodeChanged();
			Assert.IsTrue(mockXmlTreeView.AttributesDisplayed.Count > 0);
			
			mockXmlTreeView.SelectedElement = null;
			mockXmlTreeView.SelectedComment = rootComment;
			editor.SelectedNodeChanged();
			Assert.IsFalse(mockXmlTreeView.AttributesDisplayed.Count > 0);
		}
		
		[Test]
		public void CommentTextChanged()
		{
			mockXmlTreeView.TextContent = "new value";
			editor.TextContentChanged();
			
			Assert.AreEqual("new value", rootComment.InnerText);
		}
		
		[Test]
		public void IsDirtyAfterTextChanged()
		{
			editor.TextContentChanged();
			Assert.IsTrue(mockXmlTreeView.IsDirty);
		}
		
		/// <summary>
		/// Check that the Xml tree editor calls the XmlTreeView's
		/// UpdateCommentNode method.
		/// </summary>
		[Test]
		public void TreeNodeTextUpdated()
		{			
			mockXmlTreeView.TextContent = "new value";
			editor.TextContentChanged();

			Assert.AreEqual(1, mockXmlTreeView.CommentNodesUpdated.Count);
			Assert.AreEqual(rootComment, mockXmlTreeView.CommentNodesUpdated[0]);
		}
		
		[Test]
		public void AddChildCommentNodeWhenNothingSelected()
		{
			mockXmlTreeView.SelectedElement = null;
			mockXmlTreeView.SelectedTextNode = null;
			mockXmlTreeView.SelectedComment = null;
			mockXmlTreeView.IsDirty = false;
			editor.AppendChildComment();
			Assert.IsFalse(mockXmlTreeView.IsDirty);
		}
		
		[Test]
		public void AddChildCommentToChildElement()
		{
			mockXmlTreeView.SelectedTextNode = null;
			mockXmlTreeView.SelectedComment = null;
			mockXmlTreeView.SelectedElement = childElement;
			editor.AppendChildComment();
			
			// The second child node should be the comment.
			// The first child is the existing text node.
			XmlComment comment = childElement.ChildNodes[1] as XmlComment;
			Assert.AreEqual(2, childElement.ChildNodes.Count);
			Assert.IsNotNull(comment);
			Assert.IsTrue(mockXmlTreeView.IsDirty);
			Assert.AreEqual(1, mockXmlTreeView.ChildCommentNodesAdded.Count);
		}
		
		[Test]
		public void RemoveComment()
		{
			mockXmlTreeView.SelectedComment = rootComment;
			mockXmlTreeView.SelectedNode = rootComment;
			editor.Delete();
			
			Assert.AreEqual(1, mockXmlTreeView.CommentNodesRemoved.Count);
			Assert.AreSame(rootComment, mockXmlTreeView.CommentNodesRemoved[0]);
			Assert.AreEqual(1, mockXmlTreeView.Document.ChildNodes.Count);
			Assert.IsInstanceOf(typeof(XmlElement), mockXmlTreeView.Document.FirstChild);
			Assert.IsTrue(mockXmlTreeView.IsDirty);
		}
		
		[Test]
		public void RemoveCommentWhenNoNodeSelected()
		{
			mockXmlTreeView.SelectedComment = null;
			mockXmlTreeView.SelectedNode = null;
			editor.Delete();
			
			Assert.IsFalse(mockXmlTreeView.IsDirty);
		}
		
		[Test]
		public void InsertCommentBeforeWhenNoNodeSelected()
		{
			mockXmlTreeView.SelectedTextNode = null;
			mockXmlTreeView.SelectedElement = null;
			mockXmlTreeView.SelectedComment = null;
			
			editor.InsertCommentBefore();
			
			Assert.IsFalse(mockXmlTreeView.IsDirty);
		}
		
		[Test]
		public void InsertCommentBeforeComment()
		{
			mockXmlTreeView.SelectedTextNode = null;
			mockXmlTreeView.SelectedElement = null;
			mockXmlTreeView.SelectedComment = rootComment;
			
			editor.InsertCommentBefore();
			
			XmlNode parentNode = rootComment.ParentNode;
			XmlComment comment = (XmlComment)parentNode.FirstChild;
			XmlComment secondComment = parentNode.ChildNodes[1] as XmlComment;
			
			Assert.AreEqual(String.Empty, comment.InnerText);
			Assert.AreSame(rootComment, secondComment);
			Assert.IsTrue(mockXmlTreeView.IsDirty);
			Assert.AreEqual(1, mockXmlTreeView.CommentNodesInsertedBefore.Count);
			Assert.AreSame(comment, mockXmlTreeView.CommentNodesInsertedBefore[0]);
		}
		
		[Test]
		public void InsertCommentBeforeElement()
		{
			XmlDocument doc = mockXmlTreeView.Document;
			mockXmlTreeView.SelectedTextNode = null;
			mockXmlTreeView.SelectedElement = doc.DocumentElement;
			mockXmlTreeView.SelectedComment = null;
			
			editor.InsertCommentBefore();
			
			XmlComment comment = (XmlComment)doc.DocumentElement.PreviousSibling;
			XmlComment originalComment = (XmlComment)comment.PreviousSibling;
			
			Assert.AreEqual(String.Empty, comment.InnerText);
			Assert.AreSame(rootComment, originalComment);
			Assert.IsTrue(mockXmlTreeView.IsDirty);
			Assert.AreEqual(1, mockXmlTreeView.CommentNodesInsertedBefore.Count);
			Assert.AreSame(comment, mockXmlTreeView.CommentNodesInsertedBefore[0]);
		}

		[Test]
		public void InsertCommentBeforeTextNode()
		{
			XmlText textNode = (XmlText)mockXmlTreeView.Document.SelectSingleNode("/root/child/text()");
			mockXmlTreeView.SelectedTextNode = textNode;
			mockXmlTreeView.SelectedElement = null;
			mockXmlTreeView.SelectedComment = null;
			
			editor.InsertCommentBefore();
			
			XmlComment comment = (XmlComment)textNode.PreviousSibling;
			
			Assert.AreEqual(String.Empty, comment.InnerText);
			Assert.IsTrue(mockXmlTreeView.IsDirty);
			Assert.AreEqual(1, mockXmlTreeView.CommentNodesInsertedBefore.Count);
			Assert.AreSame(comment, mockXmlTreeView.CommentNodesInsertedBefore[0]);
		}
		
				[Test]
		public void InsertCommentAfterWhenNoNodeSelected()
		{
			mockXmlTreeView.SelectedTextNode = null;
			mockXmlTreeView.SelectedElement = null;
			mockXmlTreeView.SelectedComment = null;
			
			editor.InsertCommentAfter();
			
			Assert.IsFalse(mockXmlTreeView.IsDirty);
		}
		
		[Test]
		public void InsertCommentAfterComment()
		{
			mockXmlTreeView.SelectedTextNode = null;
			mockXmlTreeView.SelectedElement = null;
			mockXmlTreeView.SelectedComment = rootComment;
			
			editor.InsertCommentAfter();
			
			XmlComment comment = rootComment.NextSibling as XmlComment;
			
			Assert.AreEqual(String.Empty, comment.InnerText);
			Assert.IsTrue(mockXmlTreeView.IsDirty);
			Assert.AreEqual(1, mockXmlTreeView.CommentNodesInsertedAfter.Count);
			Assert.AreSame(comment, mockXmlTreeView.CommentNodesInsertedAfter[0]);
		}
		
		[Test]
		public void InsertCommentAfterElement()
		{
			XmlDocument doc = mockXmlTreeView.Document;
			mockXmlTreeView.SelectedTextNode = null;
			mockXmlTreeView.SelectedElement = doc.DocumentElement;
			mockXmlTreeView.SelectedComment = null;
			
			editor.InsertCommentAfter();
			
			XmlComment comment = (XmlComment)doc.DocumentElement.NextSibling;
			
			Assert.AreEqual(String.Empty, comment.InnerText);
			Assert.IsTrue(mockXmlTreeView.IsDirty);
			Assert.AreEqual(1, mockXmlTreeView.CommentNodesInsertedAfter.Count);
			Assert.AreSame(comment, mockXmlTreeView.CommentNodesInsertedAfter[0]);
		}

		[Test]
		public void InsertCommentAfterTextNode()
		{
			XmlText textNode = (XmlText)mockXmlTreeView.Document.SelectSingleNode("/root/child/text()");
			mockXmlTreeView.SelectedTextNode = textNode;
			mockXmlTreeView.SelectedElement = null;
			mockXmlTreeView.SelectedComment = null;
			
			editor.InsertCommentAfter();
			
			XmlComment comment = (XmlComment)textNode.NextSibling;
			
			Assert.AreEqual(String.Empty, comment.InnerText);
			Assert.IsTrue(mockXmlTreeView.IsDirty);
			Assert.AreEqual(1, mockXmlTreeView.CommentNodesInsertedAfter.Count);
			Assert.AreSame(comment, mockXmlTreeView.CommentNodesInsertedAfter[0]);
		}
		
		[Test]
		public void InsertTextNodeBeforeWhenCommentSelected()
		{
			AddChildCommentToChildElement();
			
			XmlComment comment = childElement.ChildNodes[1] as XmlComment;

			mockXmlTreeView.SelectedTextNode = null;
			mockXmlTreeView.SelectedElement = null;
			mockXmlTreeView.SelectedComment = comment;
			
			mockXmlTreeView.IsDirty = false;
			editor.InsertTextNodeBefore();
			
			XmlText text = comment.PreviousSibling as XmlText;
			Assert.IsTrue(mockXmlTreeView.IsDirty);
			Assert.IsNotNull(text);
			Assert.AreEqual(1, mockXmlTreeView.TextNodesInsertedBefore.Count);
			Assert.AreEqual(text, mockXmlTreeView.TextNodesInsertedBefore[0]);
		}
		
		[Test]
		public void InsertTextNodeAfterWhenCommentSelected()
		{
			AddChildCommentToChildElement();
			
			XmlComment comment = childElement.ChildNodes[1] as XmlComment;

			mockXmlTreeView.SelectedTextNode = null;
			mockXmlTreeView.SelectedElement = null;
			mockXmlTreeView.SelectedComment = comment;
			
			mockXmlTreeView.IsDirty = false;
			editor.InsertTextNodeAfter();
			
			XmlText text = comment.NextSibling as XmlText;
			Assert.IsTrue(mockXmlTreeView.IsDirty);
			Assert.IsNotNull(text);
			Assert.AreEqual(1, mockXmlTreeView.TextNodesInsertedAfter.Count);
			Assert.AreEqual(text, mockXmlTreeView.TextNodesInsertedAfter[0]);
		}
		
		[Test]
		public void InsertElementBeforeWhenCommentSelected()
		{
			AddChildCommentToChildElement();
			
			XmlComment comment = childElement.ChildNodes[1] as XmlComment;

			mockXmlTreeView.SelectedTextNode = null;
			mockXmlTreeView.SelectedElement = null;
			mockXmlTreeView.SelectedComment = comment;
			
			mockXmlTreeView.SelectedNewElementsToReturn.Add("new");
			mockXmlTreeView.IsDirty = false;
			editor.InsertElementBefore();
			
			XmlElement element = comment.PreviousSibling as XmlElement;
			Assert.IsTrue(mockXmlTreeView.IsDirty);
			Assert.IsNotNull(element);
			Assert.AreEqual("new", element.LocalName);
			Assert.AreEqual(1, mockXmlTreeView.ElementsInsertedBefore.Count);
			Assert.AreEqual(element, mockXmlTreeView.ElementsInsertedBefore[0]);
		}
		
		[Test]
		public void InsertElementAfterWhenCommentSelected()
		{
			AddChildCommentToChildElement();
			
			XmlComment comment = childElement.ChildNodes[1] as XmlComment;

			mockXmlTreeView.SelectedTextNode = null;
			mockXmlTreeView.SelectedElement = null;
			mockXmlTreeView.SelectedComment = comment;
			
			mockXmlTreeView.SelectedNewElementsToReturn.Add("new");
			mockXmlTreeView.IsDirty = false;
			editor.InsertElementAfter();
			
			XmlElement element = comment.NextSibling as XmlElement;
			Assert.IsTrue(mockXmlTreeView.IsDirty);
			Assert.IsNotNull(element);
			Assert.AreEqual("new", element.LocalName);
			Assert.AreEqual(1, mockXmlTreeView.ElementsInsertedAfter.Count);
			Assert.AreEqual(element, mockXmlTreeView.ElementsInsertedAfter[0]);
		}
		
		protected override string GetXml()
		{
			return "<!-- Root comment --><root a='b'><!-- Child comment --><child>text</child></root>";
		}
	}
}
