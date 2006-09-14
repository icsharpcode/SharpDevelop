// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
			textNode = (XmlText)mockXmlTreeView.DocumentElement.FirstChild;
			mockXmlTreeView.SelectedTextNode = textNode;
			editor.SelectedTextNodeChanged();
			mockXmlTreeView.TextContentDisplayed = "new value";
			editor.TextContentChanged();
			
			// The user then selects another element and then switches
			// back to the text node.
			mockXmlTreeView.SelectedElement = mockXmlTreeView.DocumentElement;
			editor.SelectedElementChanged();
			mockXmlTreeView.TextContentDisplayed = String.Empty;
			mockXmlTreeView.SelectedTextNode = textNode;
			editor.SelectedTextNodeChanged();
		}
		
		[Test]
		public void IsDirty()
		{
			Assert.IsTrue(mockXmlTreeView.IsDirty);
		}
		
		[Test]
		public void TextContentDisplayed()
		{
			Assert.AreEqual("new value", mockXmlTreeView.TextContentDisplayed);
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
		
		protected override string GetXml()
		{
			return "<root>text</root>";
		}
	}
}
