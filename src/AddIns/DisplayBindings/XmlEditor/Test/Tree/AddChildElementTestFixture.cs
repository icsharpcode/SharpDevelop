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
				"\t</body>\r\n" +
				"</html>";
		}
	}
}
