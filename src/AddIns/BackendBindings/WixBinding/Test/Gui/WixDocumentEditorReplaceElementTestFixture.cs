// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
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
			SD.InitializeForUnitTests();
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
