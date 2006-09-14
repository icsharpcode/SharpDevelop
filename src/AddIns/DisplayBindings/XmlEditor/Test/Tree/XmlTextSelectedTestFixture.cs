// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
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
			mockXmlTreeView.SelectedTextNode = (XmlText)mockXmlTreeView.DocumentElement.FirstChild;
			editor.SelectedTextNodeChanged();
		}
		
		[Test]
		public void IsViewShowingText()
		{
			Assert.AreEqual("text here", mockXmlTreeView.TextContentDisplayed);
		}
		
		[Test]
		public void TextNodeNotSelected()
		{
			mockXmlTreeView.SelectedTextNode = null;
			editor.SelectedTextNodeChanged();
			Assert.AreEqual(String.Empty, mockXmlTreeView.TextContentDisplayed);
		}
	
		protected override string GetXml()
		{
			return "<root>text here</root>";
		}
	}
}
