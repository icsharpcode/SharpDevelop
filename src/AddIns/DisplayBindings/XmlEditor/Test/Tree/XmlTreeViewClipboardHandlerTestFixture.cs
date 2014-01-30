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
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Tests.Utils;
using ICSharpCode.SharpDevelop.WinForms;
using ICSharpCode.SharpDevelop.Workbench;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using Rhino.Mocks;
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
			SD.InitializeForUnitTests();
			
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
			SD.TearDownForUnitTests();
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
