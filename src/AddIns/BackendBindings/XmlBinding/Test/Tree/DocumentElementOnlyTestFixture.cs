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
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Tree
{
	[TestFixture]
	public class DocumentElementOnlyTestFixture : XmlTreeViewTestFixtureBase
	{
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			base.InitFixture();
		}
		
		[Test]
		public void RootElementAdded()
		{
			Assert.AreEqual("root", mockXmlTreeView.Document.DocumentElement.Name);
		}
		
		[Test]
		public void RootElementInDocument()
		{
			Assert.IsTrue(Object.ReferenceEquals(editor.Document.DocumentElement, mockXmlTreeView.Document.DocumentElement));
		}
				
		protected override string GetXml()
		{
			return "<root/>";
		}
	}
}
