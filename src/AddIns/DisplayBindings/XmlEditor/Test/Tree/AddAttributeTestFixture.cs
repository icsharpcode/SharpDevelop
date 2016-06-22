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
	/// Tests that an attribute is added to the XML document by the
	/// XmlTreeEditor.
	/// </summary>
	[TestFixture]
	public class AddAttributeTestFixture : XmlTreeViewTestFixtureBase
	{
		XmlElement bodyElement;
			
		[SetUp]
		public void Init()
		{
			base.InitFixture();
			bodyElement = (XmlElement)editor.Document.SelectSingleNode("/html/body");
			mockXmlTreeView.SelectedElement = bodyElement;
			mockXmlTreeView.SelectedNewAttributesToReturn.Add("id");

			editor.AddAttribute();
		}
		
		[Test]
		public void ViewAddAttributeCalled()
		{
			Assert.IsTrue(mockXmlTreeView.IsSelectNewAttributesCalled);
		}
		
		[Test]
		public void IsDirty()
		{
			Assert.IsTrue(mockXmlTreeView.IsDirty);
		}
		
		[Test]
		public void ShowAttributesCalled()
		{
			XmlAttribute idAttribute = bodyElement.Attributes["id"];
			Assert.IsTrue(mockXmlTreeView.AttributesDisplayed.Contains(idAttribute));
		}
		
		[Test]
		public void NoElementSelected()
		{
			mockXmlTreeView.SelectedElement = null;
			mockXmlTreeView.IsSelectNewAttributesCalled = false;
			editor.AddAttribute();
			Assert.IsFalse(mockXmlTreeView.IsSelectNewAttributesCalled);
		}
		
		[Test]
		public void NoAttributeSelected()
		{
			XmlElement headElement = (XmlElement)editor.Document.SelectSingleNode("/html/head");
			mockXmlTreeView.SelectedElement = headElement;
			mockXmlTreeView.SelectedNewAttributesToReturn.Clear();

			editor.AddAttribute();
			
			Assert.IsFalse(headElement.HasAttribute("id"));
		}
		
		[Test]
		public void IdAttributeAdded()
		{
			Assert.IsTrue(bodyElement.HasAttribute("id"));
		}
		
		[Test]
		public void AttributeListDoesNotContainExistingTitleAttribute()
		{
			Assert.IsFalse(mockXmlTreeView.SelectNewAttributesList.Contains("title"));
		}
		
		[Test]
		public void AttributeListContainsId()
		{
			Assert.IsTrue(mockXmlTreeView.SelectNewAttributesList.Contains("id"));
		}
		
		[Test]
		public void AddTwoAttributes()
		{
			mockXmlTreeView.SelectedNewAttributesToReturn.Add("class");
			mockXmlTreeView.SelectedNewAttributesToReturn.Add("onclick");
			editor.AddAttribute();
			
			XmlAttribute classAttribute = bodyElement.Attributes["class"];
			XmlAttribute onclickAttribute = bodyElement.Attributes["onclick"];
			Assert.IsNotNull(classAttribute);
			Assert.IsNotNull(onclickAttribute);
			Assert.IsTrue(mockXmlTreeView.AttributesDisplayed.Contains(classAttribute));
			Assert.IsTrue(mockXmlTreeView.AttributesDisplayed.Contains(onclickAttribute));	
		}
		
		/// <summary>
		/// Tests that when all the attributes have been added the add
		/// attribute dialog is still displayed.
		/// </summary>
		[Test]
		public void AllAttributesAdded()
		{
			XmlElement htmlElement = (XmlElement)editor.Document.SelectSingleNode("/html");
			mockXmlTreeView.SelectedElement = htmlElement;
			mockXmlTreeView.SelectedNewAttributesToReturn.Clear();
			mockXmlTreeView.IsSelectNewAttributesCalled = false;
			
			editor.AddAttribute();
			Assert.IsTrue(mockXmlTreeView.IsSelectNewAttributesCalled);
		}
		
		/// <summary>
		/// Returns the xhtml strict schema as the default schema.
		/// </summary>
		protected override XmlSchemaCompletion DefaultSchemaCompletion {
			get { return new XmlSchemaCompletion(ResourceManager.ReadXhtmlStrictSchema()); }
		}
		
		protected override string GetXml()
		{
			return "<html dir='' id='' lang='' xml:lang=''>\r\n" +
				"\t<head>\r\n" +
				"\t\t<title></title>\r\n" +
				"\t</head>\r\n" +
				"\t<body title='body text'>\r\n" +
				"\t</body>\r\n" +
				"</html>";
		}
	}
}
