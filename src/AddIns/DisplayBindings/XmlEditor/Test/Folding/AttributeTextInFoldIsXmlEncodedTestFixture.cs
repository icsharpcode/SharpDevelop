// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Folding
{
	[TestFixture]
	public class AttributeTextInFoldIsXmlEncodedTestFixture
	{
		XmlFoldParser parser;
		ICompilationUnit unit;
		DefaultXmlFileExtensions extensions;
		DefaultProjectContent projectContent;
		MockTextBuffer textBuffer;
		XmlEditorOptions options;
		
		[SetUp]
		public void Init()
		{			
			projectContent = new DefaultProjectContent();
			extensions = new DefaultXmlFileExtensions(null);
			options = new XmlEditorOptions(new Properties());
			MockParserService parserService = new MockParserService();
			parser = new XmlFoldParser(extensions, options, parserService);
		}

		void ParseWithShowAttributesSetToTrue(string xml)
		{
			options.ShowAttributesWhenFolded = true;
		
			textBuffer = new MockTextBuffer(xml);
			unit = parser.Parse(projectContent, @"d:\projects\a.xml", textBuffer);
		}
		
		[Test]
		public void FoldAttributeTextHasSingleQuoteEncoded()
		{
			string xml = 
				"<root a='Single &apos; Quote'>\r\n" +
				"</root>";
			ParseWithShowAttributesSetToTrue(xml);
			
			Assert.AreEqual("<root a='Single &apos; Quote'>", unit.FoldingRegions[0].Name);
		}
		
		[Test]
		public void FoldAttributeTextHasDoubleQuoteEncoded()
		{
			string xml = 
				"<root a=\"Double &quot; Quote\">\r\n" +
				"</root>";
			ParseWithShowAttributesSetToTrue(xml);
			
			Assert.AreEqual("<root a=\"Double &quot; Quote\">", unit.FoldingRegions[0].Name);
		}
		
		[Test]
		public void FoldAttributeTextHasAmpersandEncoded()
		{
			string xml = 
				"<root a='Ampersand &amp;'>\r\n" +
				"</root>";
			ParseWithShowAttributesSetToTrue(xml);
			
			Assert.AreEqual("<root a='Ampersand &amp;'>", unit.FoldingRegions[0].Name);
		}
		
		[Test]
		public void FoldAttributeTextHasLessThanTagEncoded()
		{
			string xml = 
				"<root a='&lt;'>\r\n" +
				"</root>";
			ParseWithShowAttributesSetToTrue(xml);
			
			Assert.AreEqual("<root a='&lt;'>", unit.FoldingRegions[0].Name);
		}
		
		[Test]
		public void FoldAttributeTextHasGreaterThanTagEncoded()
		{
			string xml = 
				"<root a='&gt;'>\r\n" +
				"</root>";
			ParseWithShowAttributesSetToTrue(xml);
			
			Assert.AreEqual("<root a='&gt;'>", unit.FoldingRegions[0].Name);
		}
	}
}
