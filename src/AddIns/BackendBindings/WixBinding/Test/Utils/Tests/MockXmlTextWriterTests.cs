// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
