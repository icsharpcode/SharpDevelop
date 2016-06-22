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
	/// Tests that no errors occur if there is no matching schema for the
	/// xml document being edited.
	/// </summary>
	[TestFixture]
	public class NoMatchingSchemaTestFixture : XmlTreeViewTestFixtureBase
	{
		XmlElement bodyElement;
			
		[SetUp]
		public void Init()
		{
			base.InitFixture();
			bodyElement = (XmlElement)editor.Document.SelectSingleNode("/html/body");
			mockXmlTreeView.SelectedElement = bodyElement;

			editor.AddAttribute();
			editor.AppendChildElement();
		}
		
		/// <summary>
		/// Can still add a new attribute even if there is no associated schema.
		/// </summary>
		[Test]
		public void ViewAddAttributeCalled()
		{
			Assert.IsTrue(mockXmlTreeView.IsSelectNewAttributesCalled);
		}
		
		/// <summary>
		/// Can still add a new element even if there is no associated schema.
		/// </summary>
		[Test]
		public void ViewSelectNewElementsCalled()
		{
			Assert.IsTrue(mockXmlTreeView.IsSelectNewElementsCalled);
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
