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
using System.IO;
using System.Xml;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Tree
{
	/// <summary>
	/// Checks that a text node is added by the XmlTreeEditor as
	/// a child of the selected element.
	/// </summary>
	[TestFixture]
	public class AddChildTextNodeTestFixture : XmlTreeViewTestFixtureBase
	{
		XmlElement paragraphElement;
		
		[SetUp]
		public void Init()
		{
			base.InitFixture();
			paragraphElement = (XmlElement)editor.Document.SelectSingleNode("/html/body/p");
			mockXmlTreeView.SelectedElement = paragraphElement;
			editor.AppendChildTextNode();
		}
		
		[Test]
		public void ParagraphElementHasChildNodes()
		{
			Assert.AreEqual(1, paragraphElement.ChildNodes.Count);
		}
		
		[Test]
		public void NewTextNodeAdded()
		{
			XmlNode node = paragraphElement.FirstChild;
			Assert.IsInstanceOf(typeof(XmlText), node);
		}
		
		[Test]
		public void IsDirty()
		{
			Assert.IsTrue(mockXmlTreeView.IsDirty);
		}
		
		[Test]
		public void AddChildTextNodeWhenNoElementSelected()
		{
			mockXmlTreeView.SelectedElement = null;
			mockXmlTreeView.IsDirty = false;
			editor.AppendChildTextNode();
			Assert.IsFalse(mockXmlTreeView.IsDirty);
		}
		
		[Test]
		public void TextNodeAddedToView()
		{
			Assert.AreEqual(1, mockXmlTreeView.ChildTextNodesAdded.Count);
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
				"\t\t<p/>\r\n" +
				"\t</body>\r\n" +
				"</html>";
		}
	}
}
