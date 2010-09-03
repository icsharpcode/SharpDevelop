// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.XmlEditor;
using NUnit.Framework;
using System;
using System.Xml;

namespace XmlEditor.Tests.Tree
{
	/// <summary>
	/// Tests that the XmlTreeEditor tells the view to show the text when
	/// an XmlText node is selected.
	/// </summary>
	[TestFixture]
	public class XmlTextSelectedTestFixture : XmlTreeViewTestFixtureBase
	{
		[SetUp]
		public void SetUpFixture()
		{
			base.InitFixture();
			mockXmlTreeView.SelectedTextNode = (XmlText)mockXmlTreeView.Document.DocumentElement.FirstChild;
			editor.SelectedNodeChanged();
		}
		
		[Test]
		public void IsViewShowingText()
		{
			Assert.AreEqual("text here", mockXmlTreeView.TextContent);
		}
		
		[Test]
		public void TextNodeNotSelected()
		{
			mockXmlTreeView.SelectedTextNode = null;
			editor.SelectedNodeChanged();
			Assert.AreEqual(String.Empty, mockXmlTreeView.TextContent);
		}
	
		protected override string GetXml()
		{
			return "<root>text here</root>";
		}
	}
}
