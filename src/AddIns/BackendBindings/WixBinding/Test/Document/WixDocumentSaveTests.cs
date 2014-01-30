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
using System.IO;
using System.Text;
using System.Xml;
using ICSharpCode.SharpDevelop;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.Document
{
	[TestFixture]
	public class WixDocumentSaveTests
	{
		WixDocument document;
		StringBuilder xmlBuilder;
		XmlWriter xmlWriter;
		
		[SetUp]
		public void Init()
		{
			SD.InitializeForUnitTests();
			WixProject project = WixBindingTestsHelper.CreateEmptyWixProject();
			document = new WixDocument(project, new DefaultFileLoader());
			string xml = "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'><Product Name='MySetup'></Product></Wix>";
			document.LoadXml(xml);
			
			MockTextEditorOptions options = new MockTextEditorOptions();
			options.ConvertTabsToSpaces = false;
			options.IndentationSize = 1;
			
			WixTextWriter wixWriter = new WixTextWriter(options);
			xmlBuilder = new StringBuilder();
			xmlWriter = wixWriter.Create(new StringWriter(xmlBuilder));
			document.Save(xmlWriter);
		}
		
		[Test]
		public void WixDocumentSaveCreatesExpectedXml()
		{
			string expectedXml = 
				"<Wix xmlns=\"http://schemas.microsoft.com/wix/2006/wi\">\r\n" +
				"\t<Product Name=\"MySetup\"></Product>\r\n" +
				"</Wix>";			
			
			Assert.AreEqual(expectedXml, xmlBuilder.ToString());
		}
		
		[Test]
		public void XmlWriterSettingsIndentIsTrue()
		{
			Assert.IsTrue(xmlWriter.Settings.Indent);
		}
		
		[Test]
		public void XmlWriterSettingsCloseOutputIsTrue()
		{
			Assert.IsTrue(xmlWriter.Settings.CloseOutput);
		}
		
		[Test]
		public void XmlWriterSettingsOmitXmlDeclarationIsTrue()
		{
			Assert.IsTrue(xmlWriter.Settings.OmitXmlDeclaration);
		}

		[Test]
		public void XmlWriterSettingsNewLineCharsAreCarriageReturnAndLineFeed()
		{
			Assert.AreEqual("\r\n", xmlWriter.Settings.NewLineChars);
		}
		
		[Test]
		public void XmlWriterSettingsIndentCharsIsTab()
		{
			Assert.AreEqual("\t", xmlWriter.Settings.IndentChars);
		}
			
		[Test]
		public void XmlWriterSettingsIndentCharsIsTabTakenFromSaveMethod()
		{
			int indentLength = 3;
			string indent = " ".PadRight(indentLength);
			
			MockTextEditorOptions options = new MockTextEditorOptions();
			options.ConvertTabsToSpaces = true;
			options.IndentationSize = indentLength;
			
			WixTextWriter wixWriter = new WixTextWriter(options);
			xmlWriter = wixWriter.Create(new StringWriter(new StringBuilder()));
			
			Assert.AreEqual(indent, xmlWriter.Settings.IndentChars);
		}		
	}
}
