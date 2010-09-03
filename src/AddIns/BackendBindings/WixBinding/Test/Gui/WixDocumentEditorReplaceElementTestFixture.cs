// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.Gui
{
	[TestFixture]
	public class WixDocumentEditorReplaceElementTestFixture
	{
		MockTextEditor textEditor;
		DomRegion replacementRegion;
		WixDocumentEditor wixDocumentEditor;
		
		[SetUp]
		public void Init()
		{
			textEditor = new MockTextEditor();
			textEditor.Document.Text = GetWixXml();
			
			string replacementXml = 
				"<NewElement>\r\n" +
				"</NewElement>";
			
			wixDocumentEditor = new WixDocumentEditor(textEditor);
			replacementRegion = wixDocumentEditor.ReplaceElement("TARGETDIR", "Directory", replacementXml);
		}
		
		string GetWixXml()
		{
			return 
				"<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Product Name='Test' \r\n" +
				"\t         Version='1.0' \r\n" +
				"\t         Language='1013' \r\n" +
				"\t         Manufacturer='#develop' \r\n" +
				"\t         Id='????????-????-????-????-????????????'>\r\n" +
				"\t\t<Package/>\r\n" +
				"\t\t\t<Directory Id=\"TARGETDIR\" SourceName=\"SourceDir\">\r\n" +
				"\t\t\t</Directory>\r\n" +
				"\t</Product>\r\n" +
				"</Wix>";
		}
		
		[Test]
		public void XmlIsUpdatedInTextEditor()
		{
			string expectedXml =
				"<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Product Name='Test' \r\n" +
				"\t         Version='1.0' \r\n" +
				"\t         Language='1013' \r\n" +
				"\t         Manufacturer='#develop' \r\n" +
				"\t         Id='????????-????-????-????-????????????'>\r\n" +
				"\t\t<Package/>\r\n" +
				"\t\t\t<NewElement>\r\n" +
				"\t\t\t</NewElement>\r\n" +
				"\t</Product>\r\n" +
				"</Wix>";
			
			Assert.AreEqual(expectedXml, textEditor.Document.Text);
		}
		
		[Test]
		public void ReplacementRegionIsNotEmpty()
		{
			Assert.IsFalse(replacementRegion.IsEmpty);
		}
		
		[Test]
		public void ReplacingUnknownElementReturnsEmptyRegion()
		{
			DomRegion region = wixDocumentEditor.ReplaceElement("TARGETDIR", "unknown-element", "<test/>");
			Assert.IsTrue(region.IsEmpty);
		}
	}
}
