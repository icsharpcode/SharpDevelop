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
using System.IO;
using System.Xml;

using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Tree
{
	/// <summary>
	/// Tests that a text node is inserted after the selected node
	/// by the XmlTreeEditor.
	/// </summary>
	[TestFixture]
	public class InsertTextNodeAfterTestFixture : XmlTreeViewTestFixtureBase
	{
		XmlElement paragraphElement;
		XmlText textNode;		
		
		[SetUp]
		public void Init()
		{
			base.InitFixture();
			paragraphElement = (XmlElement)editor.Document.SelectSingleNode("/html/body/p");
			textNode = (XmlText)paragraphElement.SelectSingleNode("text()");
			mockXmlTreeView.SelectedTextNode = textNode;

			editor.InsertTextNodeAfter();
		}
		
		[Test]
		public void IsDirty()
		{
			Assert.IsTrue(mockXmlTreeView.IsDirty);
		}
		
		[Test]
		public void ParagraphNodeHasTwoChildNodes()
		{
			Assert.AreEqual(2, paragraphElement.ChildNodes.Count);
		}
		
		[Test]
		public void TextNodeInserted()
		{
			XmlText lastTextNode = (XmlText)paragraphElement.LastChild;
			Assert.AreEqual(String.Empty, lastTextNode.Value);
		}
		
		/// <summary>
		/// Makes sure that nothing happens if we try to insert a 
		/// text node when a text node is not already selected.
		/// </summary>
		[Test]
		public void NoNodeSelected()
		{
			mockXmlTreeView.SelectedTextNode = null;
			mockXmlTreeView.IsDirty = false;
			editor.InsertTextNodeAfter();
			ParagraphNodeHasTwoChildNodes();
			Assert.IsFalse(mockXmlTreeView.IsDirty);
		}
		
		[Test]
		public void TextNodeAddedToView()
		{
			Assert.AreEqual(1, mockXmlTreeView.TextNodesInsertedAfter.Count);
		}
		
		/// <summary>
		/// Tests that we can insert a text node after the 
		/// an element if it is not the root element.
		/// </summary>
		[Test]
		public void ElementSelected()
		{
			mockXmlTreeView.SelectedTextNode = null;
			mockXmlTreeView.SelectedElement = paragraphElement;
			mockXmlTreeView.IsDirty = false;
			editor.InsertTextNodeAfter();
			
			XmlElement bodyElement = (XmlElement)paragraphElement.ParentNode;
			Assert.AreEqual(2, bodyElement.ChildNodes.Count);
			Assert.IsInstanceOf(typeof(XmlText), bodyElement.LastChild);
			Assert.IsTrue(mockXmlTreeView.IsDirty);		
		}
		
		/// <summary>
		/// Tests that we cannot insert a text node after the 
		/// root element.
		/// </summary>
		[Test]
		public void RootElementSelected()
		{
			mockXmlTreeView.SelectedTextNode = null;
			mockXmlTreeView.SelectedElement = editor.Document.DocumentElement;
			mockXmlTreeView.IsDirty = false;
			editor.InsertTextNodeAfter();
			ParagraphNodeHasTwoChildNodes();
			Assert.IsFalse(mockXmlTreeView.IsDirty);		
		}
		
		/// <summary>
		/// Returns the xhtml strict schema as the default schema.
		/// </summary>
		protected override XmlSchemaCompletion DefaultSchemaCompletion {
			get { return new XmlSchemaCompletion(ResourceManager.ReadXhtmlStrictSchema()); }
		}
		
		protected override string GetXml()
		{
			return "<html>\r\n" +
				"\t<head>\r\n" +
				"\t\t<title></title>\r\n" +
				"\t</head>\r\n" +
				"\t<body>\r\n" +
				"\t\t<p>text</p>\r\n" +
				"\t</body>\r\n" +
				"</html>";
		}
	}
}
