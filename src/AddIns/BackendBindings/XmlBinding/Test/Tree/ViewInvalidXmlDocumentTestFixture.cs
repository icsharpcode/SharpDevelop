// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 1797 $</version>
// </file>

using ICSharpCode.XmlEditor;
using NUnit.Framework;
using System;
using System.Xml;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Tree
{
	/// <summary>
	/// Tests that the XmlTreeEditor shows an error message when the xml
	/// is not well formed. 
	/// </summary>
	[TestFixture]
	public class ViewInvalidXmlDocumentTestFixture : XmlTreeViewTestFixtureBase
	{
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			base.InitFixture();
		}
		
		[Test]
		public void IsXmlNotWellFormedErrorDisplayed()
		{
			Assert.IsTrue(mockXmlTreeView.IsXmlNotWellFormedMessageDisplayed);
		}
		
		[Test]
		public void XmlExceptionPassedToView()
		{
			Assert.IsNotNull(mockXmlTreeView.NotWellFormedExceptionPassed);
		}
		
		protected override string GetXml()
		{
			return "<BadXml>";
		}
	}
}
