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
	/// Tests that an element is inserted after the selected element by the
	/// XmlTreeEditor.
	/// </summary>
	[TestFixture]
	public class InsertElementAfterTestFixture : XmlTreeViewTestFixtureBase
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

			editor.InsertElementAfter();
		}

		[Test]
		public void HeadlineElementAdded()
		{
			XmlElement headlineElement = (XmlElement)bodyElement.LastChild;
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
			editor.InsertElementAfter();
			Assert.IsFalse(mockXmlTreeView.IsDirty);
		}
		
		[Test]
		public void NoElementSelected()
		{
			mockXmlTreeView.SelectedElement = null;
			mockXmlTreeView.IsSelectNewElementsCalled = false;
			editor.InsertElementAfter();
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
			Assert.AreEqual(1, mockXmlTreeView.ElementsInsertedAfter.Count);
		}
		
		/// <summary>
		/// Cannot insert after the root element.
		/// </summary>
		[Test]
		public void RootElementSelected()
		{
			mockXmlTreeView.SelectedElement = editor.Document.DocumentElement;
			mockXmlTreeView.IsDirty = false;
			
			editor.InsertElementAfter();
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
				"\t\t<p/>\r\n" +
				"\t</body>\r\n" +
				"</html>";
		}
	}
}
