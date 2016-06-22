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
	/// Tests that an element is removed from the XML document by the
	/// XmlTreeEditor.
	/// </summary>
	[TestFixture]
	public class RemoveElementTestFixture : XmlTreeViewTestFixtureBase
	{
		XmlElement bodyElement;
		
		[SetUp]
		public void Init()
		{
			base.InitFixture();
			bodyElement = (XmlElement)editor.Document.SelectSingleNode("/html/body");
			mockXmlTreeView.SelectedElement = bodyElement;

			editor.Delete();
		}
		
		[Test]
		public void BodyElementRemoved()
		{
			Assert.IsNull(editor.Document.SelectSingleNode("/html/body"));
		}
		
		[Test]
		public void BodyElementRemovedFromTree()
		{
			Assert.AreEqual(1, mockXmlTreeView.ElementsRemoved.Count);
			Assert.AreSame(bodyElement, mockXmlTreeView.ElementsRemoved[0]);
		}
		
		[Test]
		public void IsDirty()
		{
			Assert.IsTrue(mockXmlTreeView.IsDirty);
		}
	
		[Test]
		public void NoElementSelected()
		{
			mockXmlTreeView.SelectedElement = null;
			mockXmlTreeView.IsDirty = false;
			editor.Delete();
			Assert.IsFalse(mockXmlTreeView.IsDirty);
		}
		
		[Test]
		public void RemoveRootElement()
		{
			XmlElement htmlElement = editor.Document.DocumentElement;
			mockXmlTreeView.SelectedElement = htmlElement;
			mockXmlTreeView.IsDirty = false;
			editor.Delete();
			Assert.IsTrue(mockXmlTreeView.IsDirty);
			Assert.IsNull(editor.Document.DocumentElement);
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
				"\t<body id='body' title='abc'>\r\n" +
				"\t</body>\r\n" +
				"</html>";
		}
	}
}
