// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 1929 $</version>
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
	/// Tests that an element is inserted before the selected element by the
	/// XmlTreeEditor.
	/// </summary>
	[TestFixture]
	public class InsertElementBeforeTestFixture : XmlTreeViewTestFixtureBase
	{
		XmlElement paragraphElement;
		XmlElement bodyElement;
			
		[SetUp]
		public void Init()
		{
			base.InitFixture();
			bodyElement = (XmlElement)editor.Document.SelectSingleNode("/html/body");
			paragraphElement = (XmlElement)bodyElement.FirstChild;
			mockXmlTreeView.SelectedElement = paragraphElement;
			mockXmlTreeView.SelectedNewElementsToReturn.Add("h1");

			editor.InsertElementBefore();
		}

		[Test]
		public void HeadlineElementAdded()
		{
			XmlElement headlineElement = (XmlElement)bodyElement.FirstChild;
			Assert.AreEqual("h1", headlineElement.LocalName);
		}
		
		[Test]
		public void BodyHasTwoChildElements()
		{
			Assert.AreEqual(2, bodyElement.ChildNodes.Count);
		}
		
		[Test]
		public void IsDirty()
		{
			Assert.IsTrue(mockXmlTreeView.IsDirty);
		}
		
		[Test]
		public void ViewSelectElementsCalled()
		{
			Assert.IsTrue(mockXmlTreeView.IsSelectNewElementsCalled);
		}
		
		[Test]
		public void NoElementsToAdd()
		{
			mockXmlTreeView.SelectedNewElementsToReturn.Clear();
			mockXmlTreeView.IsDirty = false;
			editor.InsertElementBefore();
			Assert.IsFalse(mockXmlTreeView.IsDirty);
		}
		
		[Test]
		public void NoElementSelected()
		{
			mockXmlTreeView.SelectedElement = null;
			mockXmlTreeView.IsSelectNewElementsCalled = false;
			editor.InsertElementBefore();
			Assert.IsFalse(mockXmlTreeView.IsSelectNewElementsCalled);
		}
		
		[Test]
		public void ElementListContainsForm()
		{
			Assert.IsTrue(mockXmlTreeView.SelectNewElementsList.Contains("form"));
		}
		
		[Test]
		public void NewElementAddedToView()
		{
			Assert.AreEqual(1, mockXmlTreeView.ElementsInsertedBefore.Count);
		}
		
		/// <summary>
		/// Cannot insert before the root element.
		/// </summary>
		[Test]
		public void RootElementSelected()
		{
			mockXmlTreeView.SelectedElement = editor.Document.DocumentElement;
			mockXmlTreeView.IsDirty = false;
			
			editor.InsertElementBefore();
			Assert.IsFalse(mockXmlTreeView.IsDirty);
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
				"\t\t<p/>\r\n" +
				"\t</body>\r\n" +
				"</html>";
		}
	}
}
