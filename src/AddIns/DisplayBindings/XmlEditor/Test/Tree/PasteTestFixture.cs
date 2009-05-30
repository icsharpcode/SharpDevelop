// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 2229 $</version>
// </file>

using System;
using System.Xml;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Tree
{
	/// <summary>
	/// Tests pasting nodes with the XmlTreeEditor either from a cut or a copy.
	/// Here we are testing the XmlTreeEditor side and now the actual
	/// GUI side - XML Tree.
	/// </summary>
	[TestFixture]
	public class PasteTestFixture : XmlTreeViewTestFixtureBase
	{
		XmlElement rootElement;
		XmlElement bodyElement;
		XmlElement paragraphElement;
		XmlComment bodyComment;
		XmlText paragraphText;
		
		[SetUp]
		public void Init()
		{
			base.InitFixture();
			rootElement = editor.Document.DocumentElement;
			bodyElement = (XmlElement)rootElement.FirstChild;
			paragraphElement = (XmlElement)bodyElement.SelectSingleNode("p");
			bodyComment = (XmlComment)bodyElement.SelectSingleNode("comment()");
			paragraphText = (XmlText)paragraphElement.SelectSingleNode("text()");
		}
		
		/// <summary>
		/// Here we take a copy of the root element of the document and paste
		/// it on top of itself. This should append a copy of the root element
		/// as a child of the existing root element. All child nodes of the
		/// root element should be copied too.
		/// </summary>
		[Test]
		public void CopyRootElementAndPasteToRootElement()
		{
			mockXmlTreeView.SelectedElement = rootElement;
			editor.Copy();
			bool pasteEnabled = editor.IsPasteEnabled;
			editor.Paste();
			
			XmlElement pastedElement = (XmlElement)rootElement.LastChild;
			Assert.AreEqual(rootElement.Name, pastedElement.Name);
			Assert.AreEqual(rootElement.FirstChild.Name, pastedElement.FirstChild.Name);
			Assert.IsTrue(mockXmlTreeView.IsDirty);
			Assert.AreEqual(1, mockXmlTreeView.ChildElementsAdded.Count);
			Assert.AreEqual(pastedElement, mockXmlTreeView.ChildElementsAdded[0]);
			Assert.IsTrue(pasteEnabled);
		}
		
		/// <summary>
		/// The selected node is null when the paste is attempted. Here
		/// nothing should happen.
		/// </summary>
		[Test]
		public void PasteWhenNoNodeSelected()
		{
			mockXmlTreeView.SelectedNode = rootElement;
			editor.Copy();
			mockXmlTreeView.SelectedNode = null;
			bool pasteEnabled = editor.IsPasteEnabled;
			editor.Paste();
			
			Assert.IsFalse(mockXmlTreeView.IsDirty);
			Assert.IsFalse(pasteEnabled);
		}
		
		/// <summary>
		/// Check that the view is informed when a node is cut. This allows the
		/// view to give some sort of indication that a particular node is
		/// being cut.
		/// </summary>
		[Test]
		public void CutElement()
		{
			mockXmlTreeView.SelectedElement = bodyElement;
			editor.Cut();
			
			Assert.AreEqual(1, mockXmlTreeView.CutNodes.Count);
			Assert.AreEqual(bodyElement, mockXmlTreeView.CutNodes[0]);
			Assert.IsFalse(mockXmlTreeView.IsDirty);
		}
		
		[Test]
		public void CutAndPasteElement()
		{
			mockXmlTreeView.SelectedElement = paragraphElement;
			editor.Cut();
			mockXmlTreeView.SelectedElement = rootElement;
			editor.Paste();
			
			XmlElement pastedElement = rootElement.LastChild as XmlElement;
			Assert.IsTrue(mockXmlTreeView.IsDirty);
			Assert.AreSame(paragraphElement, pastedElement);
			Assert.IsNull(bodyElement.SelectSingleNode("p"));
			Assert.AreEqual(1, mockXmlTreeView.ChildElementsAdded.Count);
			Assert.AreSame(paragraphElement, mockXmlTreeView.ChildElementsAdded[0]);
			Assert.AreEqual(1, mockXmlTreeView.ElementsRemoved.Count);
			Assert.AreSame(paragraphElement, mockXmlTreeView.ElementsRemoved[0]);
		}
		
		/// <summary>
		/// Checks that nothing happens when no node is selected in the tree
		/// to cut.
		/// </summary>
		[Test]
		public void NoElementSelectedToCut()
		{
			editor.Cut();
			
			Assert.AreEqual(0, mockXmlTreeView.CutNodes.Count);
		}
		
		/// <summary>
		/// The selected node is null when the copy method is called, but
		/// there is a selected node when the paste is attempted. Here
		/// nothing should happen.
		/// </summary>
		[Test]
		public void CopyWhenNoNodeSelectedThenPaste()
		{
			mockXmlTreeView.SelectedNode = null;
			editor.Copy();
			mockXmlTreeView.SelectedNode = rootElement;
			
			bool pasteEnabled = editor.IsPasteEnabled;
			editor.Paste();
			
			Assert.IsFalse(mockXmlTreeView.IsDirty);
			Assert.IsFalse(pasteEnabled);
		}
		
		[Test]
		public void IsCopyEnabledWhenNoNodeSelected()
		{
			Assert.IsFalse(editor.IsCopyEnabled);
		}		
		
		[Test]
		public void IsCopyEnabledWhenRootElementSelected()
		{
			mockXmlTreeView.SelectedElement = rootElement;
			mockXmlTreeView.SelectedNode = rootElement;
			Assert.IsTrue(editor.IsCopyEnabled);
		}
		
		[Test]
		public void IsCutEnabledWhenChildElementSelected()
		{
			mockXmlTreeView.SelectedElement = bodyElement;
			mockXmlTreeView.SelectedNode = bodyElement;
			Assert.IsTrue(editor.IsCutEnabled);
		}
		
		[Test]
		public void IsCutEnabledWhenRootElementSelected()
		{
			mockXmlTreeView.SelectedElement = rootElement;
			mockXmlTreeView.SelectedNode = rootElement;
			Assert.IsFalse(editor.IsCutEnabled);
		}
		
		/// <summary>
		/// The document should not change if the user decides to paste the
		/// cut node back to itself. All that should happen is the view
		/// updates the cut node so it no longer has the ghost image.
		/// </summary>
		[Test]
		public void CutAndPasteToSameNode()
		{
			mockXmlTreeView.SelectedElement = paragraphElement;
			editor.Cut();
			mockXmlTreeView.SelectedElement = paragraphElement;
			editor.Paste();
			
			Assert.IsFalse(mockXmlTreeView.IsDirty);
			Assert.AreEqual(0, mockXmlTreeView.ChildElementsAdded.Count);
			Assert.AreEqual(1, mockXmlTreeView.HiddenCutNodes.Count);
			Assert.AreSame(paragraphElement, mockXmlTreeView.HiddenCutNodes[0]);
		}
		
		[Test]
		public void CutThenPasteTwiceToSameNode()
		{
			mockXmlTreeView.SelectedElement = paragraphElement;
			editor.Cut();
			mockXmlTreeView.SelectedElement = paragraphElement;
			editor.Paste();
			mockXmlTreeView.IsDirty = false;
			mockXmlTreeView.HiddenCutNodes.Clear();
			mockXmlTreeView.SelectedElement = rootElement;
			bool pasteEnabled = editor.IsPasteEnabled;
			editor.Paste();
			
			Assert.IsFalse(mockXmlTreeView.IsDirty);
			Assert.AreEqual(0, mockXmlTreeView.ChildElementsAdded.Count);
			Assert.IsFalse(pasteEnabled);
		}
		
		[Test]
		public void CannotPasteElementOntoCommentNode()
		{
			mockXmlTreeView.SelectedElement = paragraphElement;
			editor.Copy();
			mockXmlTreeView.SelectedComment = bodyComment;
			
			Assert.IsFalse(editor.IsPasteEnabled);
		}
		
		[Test]
		public void CannotPasteElementOntoTextNode()
		{
			mockXmlTreeView.SelectedElement = paragraphElement;
			editor.Copy();
			mockXmlTreeView.SelectedComment = bodyComment;
			
			Assert.IsFalse(editor.IsPasteEnabled);
		}
		
		[Test]
		public void CopyAndPasteTextNode()
		{
			mockXmlTreeView.SelectedTextNode = paragraphText;
			editor.Copy();
			bool pastedEnabledWhenTextNodeSelected = editor.IsPasteEnabled;
			mockXmlTreeView.SelectedElement = bodyElement;
			bool pasteEnabled = editor.IsPasteEnabled;
			editor.Paste();
			
			XmlText pastedTextNode = bodyElement.LastChild as XmlText;
			
			Assert.IsFalse(pastedEnabledWhenTextNodeSelected);
			Assert.IsTrue(pasteEnabled);
			Assert.IsTrue(mockXmlTreeView.IsDirty);
			Assert.IsNotNull(pastedTextNode);
			Assert.AreEqual(paragraphText.InnerText, pastedTextNode.InnerText);
			Assert.AreEqual(1, mockXmlTreeView.ChildTextNodesAdded.Count);
			Assert.AreEqual(pastedTextNode, mockXmlTreeView.ChildTextNodesAdded[0]);
		}
		
		[Test]
		public void CutAndPasteTextNode()
		{
			mockXmlTreeView.SelectedTextNode = paragraphText;
			editor.Cut();
			mockXmlTreeView.SelectedElement = bodyElement;
			bool pasteEnabled = editor.IsPasteEnabled;
			editor.Paste();
			
			XmlText pastedTextNode = bodyElement.LastChild as XmlText;
			
			Assert.IsTrue(pasteEnabled);
			Assert.IsNotNull(pastedTextNode);
			Assert.IsTrue(mockXmlTreeView.IsDirty);
			Assert.IsNull(paragraphElement.SelectSingleNode("text()"));
			Assert.AreEqual(1, mockXmlTreeView.ChildTextNodesAdded.Count);
			Assert.AreEqual(pastedTextNode, mockXmlTreeView.ChildTextNodesAdded[0]);
			Assert.AreEqual(1, mockXmlTreeView.CutNodes.Count);
			Assert.AreSame(pastedTextNode, mockXmlTreeView.CutNodes[0]);
			Assert.AreEqual(1, mockXmlTreeView.TextNodesRemoved.Count);
			Assert.AreSame(paragraphText, mockXmlTreeView.TextNodesRemoved[0]);
		}
		
		[Test]
		public void CopyAndPasteCommentNode()
		{
			mockXmlTreeView.SelectedComment = bodyComment;
			editor.Copy();
			bool pastedEnabledWhenCommentSelected = editor.IsPasteEnabled;
			mockXmlTreeView.SelectedElement = rootElement;
			bool pasteEnabled = editor.IsPasteEnabled;
			editor.Paste();
			
			XmlComment pastedCommentNode = rootElement.LastChild as XmlComment;
			
			Assert.IsFalse(pastedEnabledWhenCommentSelected);
			Assert.IsTrue(pasteEnabled);
			Assert.IsTrue(mockXmlTreeView.IsDirty);
			Assert.IsNotNull(pastedCommentNode);
			Assert.AreEqual(bodyComment.InnerText, pastedCommentNode.InnerText);
			Assert.AreEqual(1, mockXmlTreeView.ChildCommentNodesAdded.Count);
			Assert.AreEqual(pastedCommentNode, mockXmlTreeView.ChildCommentNodesAdded[0]);
		}
				
		[Test]
		public void CutAndPasteCommentNode()
		{
			mockXmlTreeView.SelectedComment = bodyComment;
			editor.Cut();
			mockXmlTreeView.SelectedElement = rootElement;
			bool pasteEnabled = editor.IsPasteEnabled;
			editor.Paste();
			
			XmlComment pastedCommentNode = rootElement.LastChild as XmlComment;
			
			Assert.IsTrue(pasteEnabled);
			Assert.IsNotNull(pastedCommentNode);
			Assert.IsTrue(mockXmlTreeView.IsDirty);
			Assert.IsNull(bodyElement.SelectSingleNode("comment()"));
			Assert.AreSame(bodyComment, pastedCommentNode);
			Assert.AreEqual(1, mockXmlTreeView.ChildCommentNodesAdded.Count);
			Assert.AreEqual(pastedCommentNode, mockXmlTreeView.ChildCommentNodesAdded[0]);
			Assert.AreEqual(1, mockXmlTreeView.CutNodes.Count);
			Assert.AreSame(pastedCommentNode, mockXmlTreeView.CutNodes[0]);
			Assert.AreEqual(1, mockXmlTreeView.CommentNodesRemoved.Count);
			Assert.AreSame(pastedCommentNode, mockXmlTreeView.CommentNodesRemoved[0]);
		}
		
		/// <summary>
		/// Tests that the cut node has its ghost image removed.
		/// </summary>
		[Test]
		public void CutThenCopyElement()
		{
			mockXmlTreeView.SelectedElement = bodyElement;
			editor.Cut();
			editor.Copy();
			
			Assert.AreEqual(1, mockXmlTreeView.HiddenCutNodes.Count);
			Assert.AreSame(bodyElement, mockXmlTreeView.HiddenCutNodes[0]);
		}
		
		/// <summary>
		/// This test makes sure that the copied node is cleared when
		/// the user decides to do a cut.
		/// </summary>
		[Test]
		public void CopyThenCutAndPasteElement()
		{
			mockXmlTreeView.SelectedElement = paragraphElement;
			editor.Copy();
			editor.Cut();
			mockXmlTreeView.SelectedElement = rootElement;
			editor.Paste();
			
			Assert.IsNull(bodyElement.SelectSingleNode("p"));
		}
		
		[Test]
		public void CopyThenPasteToUnsupportedNode()
		{
			XmlNode node = editor.Document.CreateProcessingInstruction("a", "b");
			mockXmlTreeView.SelectedNode = node;
			editor.Copy();
			mockXmlTreeView.SelectedElement = rootElement;
			
			Assert.IsFalse(editor.IsPasteEnabled);
		}
		
		/// <summary>
		/// Returns the xhtml strict schema as the default schema.
		/// </summary>
		protected override XmlSchemaCompletionData DefaultSchemaCompletionData {
			get {
				XmlTextReader reader = ResourceManager.GetXhtmlStrictSchema();
				return new XmlSchemaCompletionData(reader);
			}
		}
		
		protected override string GetXml()
		{
			return "<html>\r\n" +
				"\t<body>\r\n" +
				"\t\t<!-- Comment -->\r\n" +
				"\t\t<p>some text here</p>\r\n" +
				"\t</body>\r\n" +
				"</html>";
		}
	}
}
