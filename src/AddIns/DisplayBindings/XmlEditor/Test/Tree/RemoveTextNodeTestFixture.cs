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
