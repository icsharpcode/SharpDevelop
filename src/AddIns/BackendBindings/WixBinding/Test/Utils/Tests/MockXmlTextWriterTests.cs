// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Xml;
using NUnit.Framework;

namespace WixBinding.Tests.Utils.Tests
{
	[TestFixture]
	public class MockXmlTextWriterTests
	{
		MockXmlTextWriter mockXmlTextWriter;
		MockTextEditorOptions options;
		
		[SetUp]
		public void Init()
		{
			options = new MockTextEditorOptions();
			options.ConvertTabsToSpaces = false;
			options.IndentationSize = 4;
			mockXmlTextWriter = new MockXmlTextWriter(options);
		}
		
		[Test]
		public void TextWrittenToWriterUsesXmlWriterSettings()
		{
			using (XmlWriter writer = mockXmlTextWriter.Create("test.xml")) {
				writer.WriteStartElement("root");
				writer.WriteStartElement("child");
				writer.WriteEndElement();
				writer.WriteEndElement();
			}
			
			string expectedXml = 
				"<root>\r\n" +
				"\t<child />\r\n" +
				"</root>";
			
			Assert.AreEqual(expectedXml, mockXmlTextWriter.GetXmlWritten());
		}
		
		[Test]
		public void XmlWriterSettingsUsedInCreateMethodIsReturned()
		{
			mockXmlTextWriter.Create("test.xml");
			
			XmlWriterSettings expectedSettings = new XmlWriterSettings();
			expectedSettings.IndentChars = "\t";
			expectedSettings.Indent = true;
			expectedSettings.NewLineChars = "\r\n";
			expectedSettings.OmitXmlDeclaration = true;
			expectedSettings.CloseOutput = true;
			
			XmlWriterSettingsComparison comparison = new XmlWriterSettingsComparison();
			Assert.IsTrue(comparison.AreEqual(expectedSettings, mockXmlTextWriter.XmlWriterSettingsPassedToCreateMethod),
				comparison.ToString());
		}
	}
}
