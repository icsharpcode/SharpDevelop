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
