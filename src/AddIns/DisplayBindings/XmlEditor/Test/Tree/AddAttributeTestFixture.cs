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
	/// Tests that an attribute is added to the XML document by the
	/// XmlTreeEditor.
	/// </summary>
	[TestFixture]
	public class AddAttributeTestFixture : XmlTreeViewTestFixtureBase
	{
		XmlElement bodyElement;
			
		[SetUp]
		public void Init()
		{
			base.InitFixture();
			bodyElement = (XmlElement)editor.Document.SelectSingleNode("/html/body");
			mockXmlTreeView.SelectedElement = bodyElement;
			mockXmlTreeView.SelectedNewAttributesToReturn.Add("id");

			editor.AddAttribute();
		}
		
		[Test]
		public void ViewAddAttributeCalled()
		{
			Assert.IsTrue(mockXmlTreeView.IsSelectNewAttributesCalled);
		}
		
		[Test]
		public void IsDirty()
		{
			Assert.IsTrue(mockXmlTreeView.IsDirty);
		}
		
		[Test]
		public void ShowAttributesCalled()
		{
			XmlAttribute idAttribute = bodyElement.Attributes["id"];
			Assert.IsTrue(mockXmlTreeView.AttributesDisplayed.Contains(idAttribute));
		}
		
		[Test]
		public void NoElementSelected()
		{
			mockXmlTreeView.SelectedElement = null;
			mockXmlTreeView.IsSelectNewAttributesCalled = false;
			editor.AddAttribute();
			Assert.IsFalse(mockXmlTreeView.IsSelectNewAttributesCalled);
		}
		
		[Test]
		public void NoAttributeSelected()
		{
			XmlElement headElement = (XmlElement)editor.Document.SelectSingleNode("/html/head");
			mockXmlTreeView.SelectedElement = headElement;
			mockXmlTreeView.SelectedNewAttributesToReturn.Clear();

			editor.AddAttribute();
			
			Assert.IsFalse(headElement.HasAttribute("id"));
		}
		
		[Test]
		public void IdAttributeAdded()
		{
			Assert.IsTrue(bodyElement.HasAttribute("id"));
		}
		
		[Test]
		public void AttributeListDoesNotContainExistingTitleAttribute()
		{
			Assert.IsFalse(mockXmlTreeView.SelectNewAttributesList.Contains("title"));
		}
		
		[Test]
		public void AttributeListContainsId()
		{
			Assert.IsTrue(mockXmlTreeView.SelectNewAttributesList.Contains("id"));
		}
		
		[Test]
		public void AddTwoAttributes()
		{
			mockXmlTreeView.SelectedNewAttributesToReturn.Add("class");
			mockXmlTreeView.SelectedNewAttributesToReturn.Add("onclick");
			editor.AddAttribute();
			
			XmlAttribute classAttribute = bodyElement.Attributes["class"];
			XmlAttribute onclickAttribute = bodyElement.Attributes["onclick"];
			Assert.IsNotNull(classAttribute);
			Assert.IsNotNull(onclickAttribute);
			Assert.IsTrue(mockXmlTreeView.AttributesDisplayed.Contains(classAttribute));
			Assert.IsTrue(mockXmlTreeView.AttributesDisplayed.Contains(onclickAttribute));	
		}
		
		/// <summary>
		/// Tests that when all the attributes have been added the add
		/// attribute dialog is still displayed.
		/// </summary>
		[Test]
		public void AllAttributesAdded()
		{
			XmlElement htmlElement = (XmlElement)editor.Document.SelectSingleNode("/html");
			mockXmlTreeView.SelectedElement = htmlElement;
			mockXmlTreeView.SelectedNewAttributesToReturn.Clear();
			mockXmlTreeView.IsSelectNewAttributesCalled = false;
			
			editor.AddAttribute();
			Assert.IsTrue(mockXmlTreeView.IsSelectNewAttributesCalled);
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
			return "<html dir='' id='' lang='' xml:lang=''>\r\n" +
				"\t<head>\r\n" +
				"\t\t<title></title>\r\n" +
				"\t</head>\r\n" +
				"\t<body title='body text'>\r\n" +
				"\t</body>\r\n" +
				"</html>";
		}
	}
}
