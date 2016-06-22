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
	/// Tests that a text node is removed from the XML document by the
	/// XmlTreeEditor.
	/// </summary>
	[TestFixture]
	public class RemoveTextNodeTestFixture : XmlTreeViewTestFixtureBase
	{
		XmlElement paragraphElement;
		
		[SetUp]
		public void Init()
		{
			base.InitFixture();
			paragraphElement = (XmlElement)editor.Document.SelectSingleNode("/html/body/p");
			XmlText textNode = (XmlText)paragraphElement.FirstChild;
			mockXmlTreeView.SelectedTextNode = textNode;
			mockXmlTreeView.SelectedNode = textNode;

			editor.Delete();
		}
		
		[Test]
		public void TextNodeRemovedFromDocument()
		{
			Assert.AreEqual(0, paragraphElement.ChildNodes.Count);
		}
		
		/// <summary>
		/// Tests that the xml tree editor does not throw
		/// an exception if we try to remove a text node when
		/// no node is selected.
		/// </summary>
		[Test]
		public void NoTextNodeSelected()
		{
			mockXmlTreeView.IsDirty = false;
			mockXmlTreeView.SelectedTextNode = null;
			mockXmlTreeView.SelectedNode = null;
			editor.Delete();
			
			Assert.IsFalse(mockXmlTreeView.IsDirty);
		}
		
		[Test]
		public void TextNodeRemovedFromView()
		{
			Assert.AreEqual(1, mockXmlTreeView.TextNodesRemoved.Count);
		}
		
		[Test]
		public void IsDirty()
		{
			Assert.IsTrue(mockXmlTreeView.IsDirty);
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
				"\t<body>\r\n" +
				"\t\t<p>some text here</p>\r\n" +
				"\t</body>\r\n" +
				"</html>";
		}
	}
}
