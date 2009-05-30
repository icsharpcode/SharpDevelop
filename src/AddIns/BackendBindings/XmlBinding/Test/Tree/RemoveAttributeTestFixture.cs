// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 1924 $</version>
// </file>

using ICSharpCode.XmlEditor;
using NUnit.Framework;
using System;
using System.IO;
using System.Xml;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Tree
{
	/// <summary>
	/// Tests that an attribute is removed from the XML document by the
	/// XmlTreeEditor.
	/// </summary>
	[TestFixture]
	public class RemoveAttributeTestFixture : XmlTreeViewTestFixtureBase
	{
		XmlElement bodyElement;
			
		[SetUp]
		public void Init()
		{
			base.InitFixture();
			bodyElement = (XmlElement)editor.Document.SelectSingleNode("/html/body");
			mockXmlTreeView.SelectedElement = bodyElement;
			mockXmlTreeView.SelectedAttribute = "id";

			editor.RemoveAttribute();
		}
		
		[Test]
		public void AttributesRedisplayed()
		{
			Assert.AreEqual(1, mockXmlTreeView.AttributesDisplayed.Count);
			Assert.AreEqual("title", mockXmlTreeView.AttributesDisplayed[0].Name);
		}
		
		[Test]
		public void IdAttributeRemoved()
		{
			Assert.IsFalse(bodyElement.HasAttribute("id"));
		}
		
		[Test]
		public void IsDirty()
		{
			Assert.IsTrue(mockXmlTreeView.IsDirty);
		}
		
		[Test]
		public void NoAttributeSelected()
		{
			mockXmlTreeView.SelectedElement = bodyElement;
			mockXmlTreeView.SelectedAttribute = null;
			mockXmlTreeView.IsDirty = false;
			editor.RemoveAttribute();
			Assert.IsFalse(mockXmlTreeView.IsDirty);
		}
		
		[Test]
		public void NoElementSelected()
		{
			mockXmlTreeView.SelectedElement = null;
			mockXmlTreeView.SelectedAttribute = "title";
			mockXmlTreeView.IsDirty = false;
			editor.RemoveAttribute();
			Assert.IsFalse(mockXmlTreeView.IsDirty);
		}

		/// <summary>
		/// Returns the xhtml strict schema as the default schema.
		/// </summary>
		protected override XmlSchemaCompletionData DefaultSchemaCompletionData {
			get {
				XmlTextReader reader = ResourceManager.GetXhtmlStrictSchema();
				return new XmlSchemaCompletionData(reader);
			}
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
