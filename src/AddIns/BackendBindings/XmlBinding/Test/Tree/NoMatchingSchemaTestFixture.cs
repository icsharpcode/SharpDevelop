// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 2128 $</version>
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
	/// Tests that no errors occur if there is no matching schema for the
	/// xml document being edited.
	/// </summary>
	[TestFixture]
	public class NoMatchingSchemaTestFixture : XmlTreeViewTestFixtureBase
	{
		XmlElement bodyElement;
			
		[SetUp]
		public void Init()
		{
			base.InitFixture();
			bodyElement = (XmlElement)editor.Document.SelectSingleNode("/html/body");
			mockXmlTreeView.SelectedElement = bodyElement;

			editor.AddAttribute();
			editor.AppendChildElement();
		}
		
		/// <summary>
		/// Can still add a new attribute even if there is no associated schema.
		/// </summary>
		[Test]
		public void ViewAddAttributeCalled()
		{
			Assert.IsTrue(mockXmlTreeView.IsSelectNewAttributesCalled);
		}
		
		/// <summary>
		/// Can still add a new element even if there is no associated schema.
		/// </summary>
		[Test]
		public void ViewSelectNewElementsCalled()
		{
			Assert.IsTrue(mockXmlTreeView.IsSelectNewElementsCalled);
		}
		
		protected override string GetXml()
		{
			return "<html>\r\n" +
				"\t<head>\r\n" +
				"\t\t<title></title>\r\n" +
				"\t</head>\r\n" +
				"\t<body>\r\n" +
				"\t</body>\r\n" +
				"</html>";
		}
	}
}
