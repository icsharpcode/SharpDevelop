// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 2128 $</version>
// </file>

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
