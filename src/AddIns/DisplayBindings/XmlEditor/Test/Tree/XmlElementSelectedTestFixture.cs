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
			mockXmlTreeView.SelectedElement = mockXmlTreeView.DocumentElement;
			editor.SelectedElementChanged();
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
			editor.SelectedElementChanged();
			Assert.AreEqual(0, mockXmlTreeView.AttributesDisplayed.Count);
		}
		
		protected override string GetXml()
		{
			return "<root first='a' second='b'/>";
		}
	}
}
