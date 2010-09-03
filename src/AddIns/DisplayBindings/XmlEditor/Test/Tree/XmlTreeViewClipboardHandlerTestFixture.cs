// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Tests.Utils;
using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Tree
{
	/// <summary>
	/// Tests the XmlTreeView's IClipboardHandler implementation. Here
	/// we are just checking that it calls the XmlTreeViewContainerControl
	/// class.
	/// </summary>
	[TestFixture]
	public class XmlTreeViewClipboardHandlerTestFixture
	{
		MockXmlViewContent xmlView;
		XmlTreeView view;
		XmlTreeViewContainerControl treeViewContainer;
		XmlTreeViewControl treeView;
		IClipboardHandler clipboardHandler;
		XmlElementTreeNode htmlTreeNode;
		XmlElementTreeNode bodyTreeNode;
		XmlElementTreeNode paragraphTreeNode;
		
		[SetUp]
		public void SetUp()
		{
			MockOpenedFile openedFile = new MockOpenedFile("test.xml");
			XmlSchemaCompletionCollection schemas = new XmlSchemaCompletionCollection();
			xmlView = new MockXmlViewContent(openedFile);
			view = new XmlTreeView(xmlView, schemas, null);
			treeViewContainer = (XmlTreeViewContainerControl)view.Control;
			treeView = treeViewContainer.TreeView;
			clipboardHandler = view as IClipboardHandler;
			xmlView.GetDocumentForFile(null).Text = "<html><body><p></p></body></html>";
			openedFile.SwitchToView(view);
			
			htmlTreeNode = treeView.Nodes[0] as XmlElementTreeNode;
			htmlTreeNode.PerformInitialization();
			bodyTreeNode = htmlTreeNode.Nodes[0] as XmlElementTreeNode;
			bodyTreeNode.PerformInitialization();
			paragraphTreeNode = bodyTreeNode.Nodes[0] as XmlElementTreeNode;

			treeView.SelectedNode = null;
		}
		
		[TearDown]
		public void TearDown()
		{
			if (view != null) {
				view.Dispose();
			}
			if (xmlView != null) {
				xmlView.Dispose();
			}
		}
		
		[Test]
		public void CopyDisabledWhenNoNodeSelected()
		{
			Assert.IsFalse(clipboardHandler.EnableCopy);
		}
		
		[Test]
		public void CutDisabledWhenNoNodeSelected()
		{
			Assert.IsFalse(clipboardHandler.EnableCut);
		}
		
		[Test]
		public void PasteDisabledWhenNoNodeSelected()
		{
			Assert.IsFalse(clipboardHandler.EnablePaste);
		}
		
		[Test]
		public void DeleteDisabledWhenNoNodeSelected()
		{
			Assert.IsFalse(clipboardHandler.EnableDelete);
		}
		
		[Test]
		public void SelectAllDisabled()
		{
			Assert.IsFalse(clipboardHandler.EnableSelectAll);
		}
		
		[Test]
		public void CopyAndPaste()
		{
			treeView.SelectedNode = htmlTreeNode;
			view.Copy();
			view.Paste();
			
			Assert.IsTrue(view.IsDirty);
			Assert.AreEqual(htmlTreeNode.Text, htmlTreeNode.LastNode.Text);
		}
		
		[Test]
		public void CutAndPaste()
		{
			treeView.SelectedNode = paragraphTreeNode;
			view.Cut();
			treeView.SelectedNode = htmlTreeNode;
			view.Paste();
			
			Assert.IsTrue(view.IsDirty);
			Assert.AreEqual(paragraphTreeNode.Text, htmlTreeNode.LastNode.Text);
		}
		
		[Test]
		public void DeleteRootElement()
		{
			treeView.SelectedNode = htmlTreeNode;
			
			clipboardHandler.Delete();
			
			Assert.AreEqual(0, treeView.Nodes.Count);
			Assert.IsTrue(view.IsDirty);
		}
	}
}
