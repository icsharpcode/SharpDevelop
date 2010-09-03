// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Xml;

using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Tree
{
	/// <summary>
	/// Tests that a text node is inserted before the selected node
	/// by the XmlTreeEditor.
	/// </summary>
	[TestFixture]
	public class InsertTextNodeBeforeTestFixture : XmlTreeViewTestFixtureBase
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

			editor.InsertTextNodeBefore();
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
			XmlText firstTextNode = (XmlText)paragraphElement.ChildNodes[0];
			Assert.AreEqual(String.Empty, firstTextNode.Value);
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
			editor.InsertTextNodeBefore();
			ParagraphNodeHasTwoChildNodes();
			Assert.IsFalse(mockXmlTreeView.IsDirty);
		}
		
		[Test]
		public void TextNodeAddedToView()
		{
			Assert.AreEqual(1, mockXmlTreeView.TextNodesInsertedBefore.Count);
		}
		
		/// <summary>
		/// Tests that we can insert a text node before the 
		/// an element if it is not the root element.
		/// </summary>
		[Test]
		public void ElementSelected()
		{
			mockXmlTreeView.SelectedTextNode = null;
			mockXmlTreeView.SelectedElement = paragraphElement;
			mockXmlTreeView.IsDirty = false;
			editor.InsertTextNodeBefore();
			
			XmlElement bodyElement = (XmlElement)paragraphElement.ParentNode;
			Assert.AreEqual(2, bodyElement.ChildNodes.Count);
			Assert.IsInstanceOf(typeof(XmlText), bodyElement.FirstChild);
			Assert.IsTrue(mockXmlTreeView.IsDirty);		
		}
		
		/// <summary>
		/// Tests that we cannot insert a text node before the 
		/// root element.
		/// </summary>
		[Test]
		public void RootElementSelected()
		{
			mockXmlTreeView.SelectedTextNode = null;
			mockXmlTreeView.SelectedElement = editor.Document.DocumentElement;
			mockXmlTreeView.IsDirty = false;
			editor.InsertTextNodeBefore();
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
