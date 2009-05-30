// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 2164 $</version>
// </file>

using System;
using System.Xml;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.XmlEditor;
using NUnit.Framework;

namespace XmlEditor.Tests.Tree
{
	/// <summary>
	/// Tests pasting in the XmlTreeViewContainerControl.
	/// </summary>
	[TestFixture]
	public class PasteInTreeControlTestFixture
	{
		XmlDocument doc;
		XmlTreeViewContainerControl treeViewContainerControl; 
		XmlTreeViewControl treeView;
		IClipboardHandler clipboardHandler;
		XmlElementTreeNode htmlTreeNode;
		XmlElementTreeNode bodyTreeNode;
		XmlElementTreeNode paragraphTreeNode;
		XmlElement htmlElement;
		XmlElement bodyElement;
		XmlElement paragraphElement;
		XmlTextTreeNode paragraphTextTreeNode;
		XmlText paragraphText;
		XmlCommentTreeNode bodyCommentTreeNode;
		XmlComment bodyComment;
		
		[SetUp]
		public void SetUp()
		{
			XmlCompletionDataProvider completionDataProvider = new XmlCompletionDataProvider(new XmlSchemaCompletionDataCollection(), null, String.Empty);
			treeViewContainerControl = new XmlTreeViewContainerControl();
			treeView = treeViewContainerControl.TreeView;
			treeViewContainerControl.LoadXml(GetXml(), completionDataProvider);
			doc = treeViewContainerControl.Document;
			
			clipboardHandler = treeViewContainerControl as IClipboardHandler;
			
			htmlElement = doc.DocumentElement;
			bodyElement = htmlElement.FirstChild as XmlElement;
			paragraphElement = bodyElement.SelectSingleNode("p") as XmlElement;
			paragraphText = paragraphElement.SelectSingleNode("text()") as XmlText;
			bodyComment = bodyElement.SelectSingleNode("comment()") as XmlComment;
			
			htmlTreeNode = treeView.Nodes[0] as XmlElementTreeNode;
			htmlTreeNode.PerformInitialization();
			bodyTreeNode = htmlTreeNode.FirstNode as XmlElementTreeNode;
			bodyTreeNode.PerformInitialization();
			bodyCommentTreeNode = bodyTreeNode.FirstNode as XmlCommentTreeNode;
			paragraphTreeNode = bodyTreeNode.LastNode as XmlElementTreeNode;
			paragraphTreeNode.PerformInitialization();
			paragraphTextTreeNode = paragraphTreeNode.FirstNode as XmlTextTreeNode;
		}
		
		[TearDown]
		public void TearDown()
		{
			if (treeViewContainerControl != null) {
				treeViewContainerControl.Dispose();
			}
		}
		
		[Test]
		public void ClipboardCopyDisabledWhenNoNodeSelected()
		{
			treeView.SelectedNode = null;
			Assert.IsFalse(clipboardHandler.EnableCopy);
		}
		
		[Test]
		public void ClipboardPasteDisabledWhenNoNodeSelected()
		{
			treeView.SelectedNode = null;
			Assert.IsFalse(clipboardHandler.EnablePaste);
		}
		
		/// <summary>
		/// Select all is not currently implemented since the tree 
		/// only supports single node selection.
		/// </summary>
		[Test]
		public void SelectAllDisabled()
		{
			Assert.IsFalse(clipboardHandler.EnableSelectAll);
		}
		
		[Test]
		public void ClipboardCutDisabledWhenNoNodeSelected()
		{
			treeView.SelectedNode = null;
			Assert.IsFalse(clipboardHandler.EnableCut);
		}
		
		[Test]
		public void ClipboardDeleteDisabledWhenNoNodeSelected()
		{
			treeView.SelectedNode = null;
			Assert.IsFalse(clipboardHandler.EnableDelete);
		}
		
		[Test]
		public void ClipboardDeleteEnabledWhenNodeSelected()
		{
			treeView.SelectedNode = treeView.Nodes[0];
			Assert.IsTrue(clipboardHandler.EnableDelete);
		}
		
		/// <summary>
		/// The cut tree node should have be showing its ghost image
		/// after being cut.
		/// </summary>
		[Test]
		public void CutBodyElement()
		{
			treeView.SelectedNode = bodyTreeNode;
			treeViewContainerControl.Cut();
						
			Assert.AreEqual(XmlElementTreeNode.XmlElementTreeNodeGhostImageKey, bodyTreeNode.ImageKey);
			Assert.AreEqual(XmlElementTreeNode.XmlElementTreeNodeGhostImageKey, bodyTreeNode.SelectedImageKey);
			Assert.IsTrue(bodyTreeNode.ShowGhostImage);
		}
		
		/// <summary>
		/// This should append a copy of the root element
		/// as a child of the existing root element. All child nodes of the
		/// root element should be copied too.
		/// </summary>
		[Test]
		public void CopyRootElementAndPasteToRootElement()
		{
			treeView.SelectedNode = htmlTreeNode;
			treeViewContainerControl.Copy();
			bool pasteEnabled = treeViewContainerControl.EnablePaste;
			treeViewContainerControl.Paste();
			
			XmlElement pastedElement = (XmlElement)htmlElement.LastChild;
			XmlElementTreeNode pastedTreeNode = htmlTreeNode.LastNode as XmlElementTreeNode;
			Assert.AreEqual(htmlElement.Name, pastedElement.Name);
			Assert.AreEqual(pastedElement.Name, pastedTreeNode.Text);
			Assert.AreSame(pastedElement, pastedTreeNode.XmlElement);
			Assert.IsTrue(pasteEnabled);
		}
		
		[Test]
		public void CutAndPasteElement()
		{
			treeView.SelectedNode = paragraphTreeNode;
			treeViewContainerControl.Cut();
			treeView.SelectedNode = htmlTreeNode;
			bool pasteEnabled = treeViewContainerControl.EnablePaste;
			treeViewContainerControl.Paste();
			
			XmlElement pastedElement = htmlElement.LastChild as XmlElement;
			XmlElementTreeNode pastedTreeNode = htmlTreeNode.LastNode as XmlElementTreeNode;
			
			Assert.AreSame(paragraphElement, pastedElement);
			Assert.IsNull(bodyElement.SelectSingleNode("p"));
			Assert.AreSame(paragraphElement, pastedTreeNode.XmlElement);
			Assert.IsTrue(pasteEnabled);
			Assert.AreEqual(XmlElementTreeNode.XmlElementTreeNodeImageKey, 
			                pastedTreeNode.ImageKey, 
			                "Should not be ghost image.");
		}
		
		/// <summary>
		/// Check that the ghost image is removed when the user pastes the
		/// cut node back onto itself.
		/// </summary>
		[Test]
		public void CutAndPasteElementBackOntoItself()
		{
			treeView.SelectedNode = paragraphTreeNode;
			treeViewContainerControl.Cut();
			treeView.SelectedNode = paragraphTreeNode;
			treeViewContainerControl.Paste();

			Assert.IsFalse(treeViewContainerControl.IsDirty);
			Assert.AreSame(XmlElementTreeNode.XmlElementTreeNodeImageKey, paragraphTreeNode.ImageKey);
			Assert.AreSame(XmlElementTreeNode.XmlElementTreeNodeImageKey, paragraphTreeNode.SelectedImageKey);
		}
		
		[Test]
		public void CopyAndPasteTextNode()
		{
			treeView.SelectedNode = paragraphTextTreeNode;
			treeViewContainerControl.Copy();
			treeView.SelectedNode = bodyTreeNode;
			treeViewContainerControl.Paste();
			
			XmlText pastedTextNode = bodyElement.LastChild as XmlText;
			XmlTextTreeNode pastedTreeNode = bodyTreeNode.LastNode as XmlTextTreeNode;
			
			Assert.AreEqual(pastedTextNode.InnerText, paragraphText.InnerText);
			Assert.AreEqual(paragraphText.InnerText, pastedTreeNode.Text);
			Assert.IsTrue(treeViewContainerControl.IsDirty);
		}
		
		[Test]
		public void CutAndPasteTextNode()
		{
			treeView.SelectedNode = paragraphTextTreeNode;
			treeViewContainerControl.Cut();
			string paragraphTextTreeNodeImageKeyAfterCut = paragraphTextTreeNode.ImageKey;
			treeView.SelectedNode = bodyTreeNode;
			treeViewContainerControl.Paste();
			
			XmlText pastedTextNode = bodyElement.LastChild as XmlText;
			XmlTextTreeNode pastedTreeNode = bodyTreeNode.LastNode as XmlTextTreeNode;
				
			Assert.AreSame(paragraphText, pastedTextNode);
			Assert.IsNull(paragraphElement.SelectSingleNode("text()"));
			Assert.AreEqual(XmlTextTreeNode.XmlTextTreeNodeGhostImageKey,
			                paragraphTextTreeNodeImageKeyAfterCut);
			Assert.AreEqual(XmlTextTreeNode.XmlTextTreeNodeImageKey, 
			                pastedTreeNode.ImageKey, 
			                "Should not be ghost image.");
		}
		
		[Test]
		public void CopyAndPasteCommentNode()
		{
			treeView.SelectedNode = bodyCommentTreeNode;
			treeViewContainerControl.Copy();
			treeView.SelectedNode = htmlTreeNode;
			treeViewContainerControl.Paste();
			
			XmlComment pastedCommentNode = htmlElement.LastChild as XmlComment;
			XmlCommentTreeNode pastedTreeNode = htmlTreeNode.LastNode as XmlCommentTreeNode;
			
			Assert.AreEqual(pastedCommentNode.InnerText, bodyComment.InnerText);
			Assert.AreEqual(bodyComment.InnerText.Trim(), pastedTreeNode.Text);
			Assert.IsTrue(treeViewContainerControl.IsDirty);
		}
		
		[Test]
		public void CutAndPasteCommentNode()
		{
			treeView.SelectedNode = bodyCommentTreeNode;
			treeViewContainerControl.Cut();
			string bodyCommentTreeNodeImageKeyAfterCut = bodyCommentTreeNode.ImageKey;
			treeView.SelectedNode = htmlTreeNode;
			treeViewContainerControl.Paste();
			
			XmlComment pastedCommentNode = htmlElement.LastChild as XmlComment;
			XmlCommentTreeNode pastedTreeNode = htmlTreeNode.LastNode as XmlCommentTreeNode;
				
			Assert.AreSame(bodyComment, pastedCommentNode);
			Assert.AreSame(bodyComment, pastedTreeNode.XmlComment);
			Assert.IsNull(bodyElement.SelectSingleNode("comment()"));
			Assert.AreEqual(XmlCommentTreeNode.XmlCommentTreeNodeGhostImageKey,
			                bodyCommentTreeNodeImageKeyAfterCut);
			Assert.AreEqual(XmlCommentTreeNode.XmlCommentTreeNodeImageKey, 
			                pastedTreeNode.ImageKey, 
			                "Should not be ghost image.");
		}
		
		string GetXml()
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
