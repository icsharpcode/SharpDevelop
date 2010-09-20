// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.XmlEditor;
using NUnit.Framework;
using System;
using System.Xml;

namespace XmlEditor.Tests.Tree
{
	/// <summary>
	/// Tests that an attribute being changed updates the xml document and
	/// sets the view to be dirty.
	/// </summary>
	[TestFixture]
	public class AttributeChangedTestFixture : XmlTreeViewTestFixtureBase
	{
		[SetUp]
		public void SetUpFixture()
		{
			base.InitFixture();
			mockXmlTreeView.SelectedElement = mockXmlTreeView.Document.DocumentElement;
			editor.SelectedNodeChanged();
			XmlAttribute attribute = mockXmlTreeView.Document.DocumentElement.Attributes["first"];
			attribute.Value = "new value";
			editor.AttributeValueChanged();
		}
		
		[Test]
		public void IsDirty()
		{
			Assert.IsTrue(mockXmlTreeView.IsDirty);
		}
		
		protected override string GetXml()
		{
			return "<root first='a'/>";
		}
	}
}
