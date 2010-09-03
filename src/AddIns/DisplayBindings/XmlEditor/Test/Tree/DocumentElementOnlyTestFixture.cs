// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
