// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Xml;

using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Tree
{
	/// <summary>
	/// Tests that an element is removed from the XML document by the
	/// XmlTreeEditor.
	/// </summary>
	[TestFixture]
	public class RemoveElementTestFixture : XmlTreeViewTestFixtureBase
	{
		XmlElement bodyElement;
		
		[SetUp]
		public void Init()
		{
			base.InitFixture();
			bodyElement = (XmlElement)editor.Document.SelectSingleNode("/html/body");
			mockXmlTreeView.SelectedElement = bodyElement;

			editor.Delete();
		}
		
		[Test]
		public void BodyElementRemoved()
		{
			Assert.IsNull(editor.Document.SelectSingleNode("/html/body"));
		}
		
		[Test]
		public void BodyElementRemovedFromTree()
		{
			Assert.AreEqual(1, mockXmlTreeView.ElementsRemoved.Count);
			Assert.AreSame(bodyElement, mockXmlTreeView.ElementsRemoved[0]);
		}
		
		[Test]
		public void IsDirty()
		{
			Assert.IsTrue(mockXmlTreeView.IsDirty);
		}
	
		[Test]
		public void NoElementSelected()
		{
			mockXmlTreeView.SelectedElement = null;
			mockXmlTreeView.IsDirty = false;
			editor.Delete();
			Assert.IsFalse(mockXmlTreeView.IsDirty);
		}
		
		[Test]
		public void RemoveRootElement()
		{
			XmlElement htmlElement = editor.Document.DocumentElement;
			mockXmlTreeView.SelectedElement = htmlElement;
			mockXmlTreeView.IsDirty = false;
			editor.Delete();
			Assert.IsTrue(mockXmlTreeView.IsDirty);
			Assert.IsNull(editor.Document.DocumentElement);
		}

		/// <summary>
		/// Returns the xhtml strict schema as the default schema.
		/// </summary>
		protected override XmlSchemaCompletion DefaultSchemaCompletion {
			get { return new XmlSchemaCompletion(ResourceManager.ReadXhtmlStrictSchema()); }
		}
		
		protected override string GetXml()
		{
			return "<html>\r\n" +
				"\t<head>\r\n" +
				"\t\t<title></title>\r\n" +
				"\t</head>\r\n" +
				"\t<body id='body' title='abc'>\r\n" +
				"\t</body>\r\n" +
				"</html>";
		}
	}
}
