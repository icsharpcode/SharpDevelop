// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
