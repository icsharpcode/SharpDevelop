// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Xml;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.DialogXmlGeneration
{
	[TestFixture]
	public class DialogXmlWritingTestFixture
	{
		WixDialogElement dialogElement;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			WixDocument doc = new WixDocument();
			doc.LoadXml(GetWixXml());
			dialogElement = (WixDialogElement)doc.SelectSingleNode("//w:Dialog", new WixNamespaceManager(doc.NameTable));
			dialogElement.SetAttribute("Id", "id");
			dialogElement.SetAttribute("Title", "title");
			XmlElement controlElement = doc.CreateElement("Control", WixNamespaceManager.Namespace);
			dialogElement.AppendChild(controlElement);
		}
		
		[Test]
		public void WixDocumentGetXmlWithTabs()
		{
			MockTextEditorOptions options = new MockTextEditorOptions();
			options.ConvertTabsToSpaces = false;
			options.IndentationSize = 4;
			
			WixTextWriter wixWriter = new WixTextWriter(options);
			
			string outputXml = dialogElement.GetXml(wixWriter);
			string expectedXml = 
				"<Dialog Id=\"id\" Height=\"270\" Width=\"370\" Title=\"title\">\r\n" +
				"\t<Control />\r\n" +
				"</Dialog>";
			Assert.AreEqual(expectedXml, outputXml);
		}
		
		[Test]
		public void WixDocumentGetXmlWithSpaces()
		{
			MockTextEditorOptions options = new MockTextEditorOptions();
			options.ConvertTabsToSpaces = true;
			options.IndentationSize = 4;
			
			WixTextWriter wixWriter = new WixTextWriter(options);
			
			string outputXml = dialogElement.GetXml(wixWriter);
			string expectedXml = 
				"<Dialog Id=\"id\" Height=\"270\" Width=\"370\" Title=\"title\">\r\n" +
				"    <Control />\r\n" +
				"</Dialog>";
			Assert.AreEqual(expectedXml, outputXml);
		}
		
		string GetWixXml()
		{
			return 
				"<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<UI>\r\n" +
				"\t\t\t<Dialog Id='WelcomeDialog' Height='270' Width='370' Title='Welcome Dialog Title'/>\r\n" +
				"\t\t</UI>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
		}
	}
}
