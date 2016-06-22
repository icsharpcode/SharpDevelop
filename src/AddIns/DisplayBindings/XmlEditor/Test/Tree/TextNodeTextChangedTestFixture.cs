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

using ICSharpCode.XmlEditor;
using NUnit.Framework;
using System;
using System.Xml;

namespace XmlEditor.Tests.Tree
{
	/// <summary>
	/// Tests that the text for a text node being changed updates the xml document and
	/// sets the view to be dirty.
	/// </summary>
	[TestFixture]
	public class TextNodeTextChangedTestFixture : XmlTreeViewTestFixtureBase
	{
		XmlText textNode;
		
		[SetUp]
		public void SetUpFixture()
		{
			base.InitFixture();
			
			// User selects text node and alters its text.
			textNode = (XmlText)mockXmlTreeView.Document.DocumentElement.FirstChild;
			mockXmlTreeView.SelectedTextNode = textNode;
			editor.SelectedNodeChanged();
			mockXmlTreeView.TextContent = "new value";
			editor.TextContentChanged();
			
			// The user then selects another element and then switches
			// back to the text node.
			mockXmlTreeView.SelectedElement = mockXmlTreeView.Document.DocumentElement;
			editor.SelectedNodeChanged();
			mockXmlTreeView.TextContent = String.Empty;
			mockXmlTreeView.SelectedTextNode = textNode;
			editor.SelectedNodeChanged();
		}
		
		[Test]
		public void IsDirty()
		{
			Assert.IsTrue(mockXmlTreeView.IsDirty);
		}
		
		[Test]
		public void TextContentDisplayed()
		{
			Assert.AreEqual("new value", mockXmlTreeView.TextContent);
		}
		
		[Test]
		public void TextNodeUpdated()
		{
			Assert.AreEqual("new value", textNode.Value);
		}
		
		/// <summary>
		/// Tests that no exception occurs when the view's text node is null and
		/// that the IsDirty flag is not set to true when there is no text node.
		/// </summary>
		[Test]
		public void NoTextNodeSelected()
		{
			mockXmlTreeView.IsDirty = false;
			mockXmlTreeView.SelectedTextNode = null;
			editor.TextContentChanged();
			
			Assert.IsFalse(mockXmlTreeView.IsDirty);
		}
		
		/// <summary>
		/// Check that the Xml tree editor calls the XmlTreeView's
		/// UpdateTextNode method.
		/// </summary>
		[Test]
		public void TreeNodeTextUpdated()
		{
			Assert.AreEqual(1, mockXmlTreeView.TextNodesUpdated.Count);
			Assert.AreEqual(textNode, mockXmlTreeView.TextNodesUpdated[0]);
		}
		
		[Test]
		public void TextContentClearedAfterElementSelected()
		{
			mockXmlTreeView.SelectedTextNode = null;
			mockXmlTreeView.ShowTextContent("Test");
			mockXmlTreeView.SelectedElement = mockXmlTreeView.Document.DocumentElement;
			editor.SelectedNodeChanged();
			Assert.AreEqual(String.Empty, mockXmlTreeView.TextContent);
		}
		
		[Test]
		public void AttributesClearedAfterTextNodeSelected()
		{
			mockXmlTreeView.SelectedTextNode = null;
			mockXmlTreeView.SelectedElement = mockXmlTreeView.Document.DocumentElement;
			editor.SelectedNodeChanged();
			Assert.IsTrue(mockXmlTreeView.AttributesDisplayed.Count > 0);
			
			mockXmlTreeView.SelectedElement = null;
			mockXmlTreeView.SelectedTextNode = textNode;
			editor.SelectedNodeChanged();
			Assert.IsFalse(mockXmlTreeView.AttributesDisplayed.Count > 0);
		}
		
		protected override string GetXml()
		{
			return "<root a='b'>text</root>";
		}
	}
}
