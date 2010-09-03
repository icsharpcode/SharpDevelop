// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Text;
using System.Xml;

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
