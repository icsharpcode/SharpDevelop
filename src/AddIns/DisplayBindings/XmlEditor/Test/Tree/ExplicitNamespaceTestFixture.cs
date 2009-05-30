// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 2128 $</version>
// </file>

using ICSharpCode.XmlEditor;
using NUnit.Framework;
using System;
using System.IO;
using System.Xml;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Tree
{
	/// <summary>
	/// Tests that a child element is added to the XML document by the
	/// XmlTreeEditor with the correct namespace.
	/// </summary>
	[TestFixture]
	public class ExplicitNamespaceTestFixture : XmlTreeViewTestFixtureBase
	{
		XmlElement bodyElement;
			
		[SetUp]
		public void Init()
		{
			base.InitFixture();
			XmlNamespaceManager nsManager = new XmlNamespaceManager(editor.Document.NameTable);
			nsManager.AddNamespace("h", "http://www.w3.org/1999/xhtml");
			bodyElement = (XmlElement)editor.Document.SelectSingleNode("/h:html/h:body", nsManager);
			mockXmlTreeView.SelectedElement = bodyElement;
			mockXmlTreeView.SelectedNewElementsToReturn.Add("h1");

			editor.AppendChildElement();
		}
		
		[Test]
		public void HeadlineElementNamespace()
		{
			XmlElement element = (XmlElement)bodyElement.FirstChild;
			Assert.AreEqual("http://www.w3.org/1999/xhtml", element.NamespaceURI);
		}
		
		[Test]
		public void InsertBeforeDifferentNamespaceElementSelected()
		{
			XmlNamespaceManager nsManager = new XmlNamespaceManager(editor.Document.NameTable);
			nsManager.AddNamespace("a", "http://asp.net");
			XmlElement buttonElement = (XmlElement)editor.Document.SelectSingleNode("//a:button", nsManager);
			XmlElement headElement = (XmlElement)buttonElement.ParentNode;
			mockXmlTreeView.SelectedElement = buttonElement;
			mockXmlTreeView.SelectedNewElementsToReturn.Clear();
			mockXmlTreeView.SelectedNewElementsToReturn.Add("title");
			
			editor.InsertElementBefore();
			
			XmlElement titleElement = (XmlElement)headElement.FirstChild;
			Assert.AreEqual(2, headElement.ChildNodes.Count);
			Assert.AreEqual("title", titleElement.LocalName);
			Assert.AreEqual("http://www.w3.org/1999/xhtml", titleElement.NamespaceURI);
		}
		
		[Test]
		public void InsertAfterDifferentNamespaceElementSelected()
		{
			XmlNamespaceManager nsManager = new XmlNamespaceManager(editor.Document.NameTable);
			nsManager.AddNamespace("a", "http://asp.net");
			XmlElement buttonElement = (XmlElement)editor.Document.SelectSingleNode("//a:button", nsManager);
			XmlElement headElement = (XmlElement)buttonElement.ParentNode;
			mockXmlTreeView.SelectedElement = buttonElement;
			mockXmlTreeView.SelectedNewElementsToReturn.Clear();
			mockXmlTreeView.SelectedNewElementsToReturn.Add("title");
			
			editor.InsertElementAfter();
			
			XmlElement titleElement = (XmlElement)headElement.LastChild;
			Assert.AreEqual(2, headElement.ChildNodes.Count);
			Assert.AreEqual("title", titleElement.LocalName);
			Assert.AreEqual("http://www.w3.org/1999/xhtml", titleElement.NamespaceURI);
		}

		/// <summary>
		/// Returns the xhtml strict schema as the default schema.
		/// </summary>
		protected override XmlSchemaCompletionData DefaultSchemaCompletionData {
			get {
				XmlTextReader reader = ResourceManager.GetXhtmlStrictSchema();
				return new XmlSchemaCompletionData(reader);
			}
		}
		
		protected override string GetXml()
		{
			return "<html xmlns='http://www.w3.org/1999/xhtml'>\r\n" +
				"\t<head>\r\n" +
				"\t\t<asp:button xmlns:asp='http://asp.net'/>\r\n" +
				"\t</head>\r\n" +
				"\t<body>\r\n" +
				"\t</body>\r\n" +
				"</html>";
		}
	}
}
