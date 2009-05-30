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
	/// XmlTreeEditor.
	/// </summary>
	[TestFixture]
	public class AddChildElementTestFixture : XmlTreeViewTestFixtureBase
	{
		XmlElement bodyElement;
			
		[SetUp]
		public void Init()
		{
			base.InitFixture();
			bodyElement = (XmlElement)editor.Document.SelectSingleNode("/html/body");
			mockXmlTreeView.SelectedElement = bodyElement;
			mockXmlTreeView.SelectedNewElementsToReturn.Add("h1");
			mockXmlTreeView.SelectedNewElementsToReturn.Add("p");

			editor.AppendChildElement();
		}
		
		[Test]
		public void ViewSelectElementsCalled()
		{
			Assert.IsTrue(mockXmlTreeView.IsSelectNewElementsCalled);
		}
		
		[Test]
		public void NoElementSelected()
		{
			mockXmlTreeView.SelectedElement = null;
			mockXmlTreeView.IsSelectNewElementsCalled = false;
			editor.AppendChildElement();
			Assert.IsFalse(mockXmlTreeView.IsSelectNewElementsCalled);
		}
		
		[Test]
		public void IsDirty()
		{
			Assert.IsTrue(mockXmlTreeView.IsDirty);
		}
		
		[Test]
		public void NoElementsToAdd()
		{
			mockXmlTreeView.SelectedNewElementsToReturn.Clear();
			mockXmlTreeView.IsDirty = false;
			editor.AppendChildElement();
			Assert.IsFalse(mockXmlTreeView.IsDirty);
		}
		
		[Test]
		public void ParagraphElementAdded()
		{
			Assert.AreEqual(1, bodyElement.GetElementsByTagName("p").Count);
		}
		
		[Test]
		public void HeadlineElementAdded()
		{
			Assert.AreEqual(1, bodyElement.GetElementsByTagName("h1").Count);
		}
		
		[Test]
		public void HeadlineElementNamespace()
		{
			XmlElement element = (XmlElement)(bodyElement.GetElementsByTagName("h1")[0]);
			Assert.AreEqual(String.Empty, element.NamespaceURI);
		}
		
		[Test]
		public void ElementListContainsForm()
		{
			Assert.IsTrue(mockXmlTreeView.SelectNewElementsList.Contains("form"));
		}
		
		[Test]
		public void TwoChildElementsAddedToView()
		{
			Assert.AreEqual(2, mockXmlTreeView.ChildElementsAdded.Count);
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
			return "<html>\r\n" +
				"\t<head>\r\n" +
				"\t\t<title></title>\r\n" +
				"\t</head>\r\n" +
				"\t<body>\r\n" +
				"\t</body>\r\n" +
				"</html>";
		}
	}
}
