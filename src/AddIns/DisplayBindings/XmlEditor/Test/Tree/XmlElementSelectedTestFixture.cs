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
using System.Xml;

namespace XmlEditor.Tests.Tree
{
	/// <summary>
	/// A tree node that represents an xml element is selected. This test
	/// checks that the XmlTreeEditor tells the view to display the 
	/// element's attributes.
	/// </summary>
	[TestFixture]
	public class XmlElementSelectedTestFixture : XmlTreeViewTestFixtureBase
	{
		[SetUp]
		public void SetUpFixture()
		{
			base.InitFixture();
			mockXmlTreeView.SelectedElement = mockXmlTreeView.Document.DocumentElement;
			editor.SelectedNodeChanged();
		}
		
		[Test]
		public void AttributesDisplayed()
		{
			Assert.AreEqual(2, mockXmlTreeView.AttributesDisplayed.Count);
		}
		
		[Test]
		public void FirstAttributeName()
		{
			Assert.AreEqual("first", mockXmlTreeView.AttributesDisplayed[0].Name);
		}
		
		[Test]
		public void SecondAttributeName()
		{
			Assert.AreEqual("second", mockXmlTreeView.AttributesDisplayed[1].Name);
		}
		
		[Test]
		public void NoElementSelected()
		{
			mockXmlTreeView.SelectedElement = null;
			editor.SelectedNodeChanged();
			Assert.AreEqual(0, mockXmlTreeView.AttributesDisplayed.Count);
		}
		
		/// <summary>
		/// When the node has changed the text content should
		/// not have been updated otherwise the TextBox control
		/// may be on top instead of the attributes grid.
		/// </summary>
		[Test]
		public void ShowTextContentNotCalled()
		{
			Assert.IsFalse(mockXmlTreeView.IsShowTextContentCalled);
		}
		
		protected override string GetXml()
		{
			return "<root first='a' second='b'/>";
		}
	}
}
