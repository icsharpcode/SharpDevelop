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
	public class FoldParserParsesInvalidXmlTestFixture
	{
		XmlFoldParser parser;
		ICompilationUnit unit;
		DefaultProjectContent projectContent;
		MockTextBuffer textBuffer;
		
		[SetUp]
		public void Init()
		{
			string xml = 
				"<root\r\n" +
				"    <child>\r\n" +
				"</root>";
			
			projectContent = new DefaultProjectContent();
			textBuffer = new MockTextBuffer(xml);
			
			DefaultXmlFileExtensions extensions = new DefaultXmlFileExtensions(null);
			XmlEditorOptions options = new XmlEditorOptions(new Properties());
			MockParserService parserService = new MockParserService();
			parser = new XmlFoldParser(extensions, options, parserService);
		}
		
		[Test]
		public void ParseMethodDoesNotThrowExceptionWhenParsingInvalidXml()
		{
			Assert.DoesNotThrow(delegate { ParseXml(); });
		}
		
		void ParseXml()
		{
			unit = parser.Parse(projectContent, @"d:\projects\a.xml", textBuffer);
		}
		
		[Test]
		public void CompilationUnitFileNameSetByXmlParserParseMethod()
		{
			ParseXml();
			Assert.AreEqual(@"d:\projects\a.xml", unit.FileName);
		}
		
		[Test]
		public void CompilationUnitProjectContentSetByXmlParserParseMethod()
		{
			ParseXml();
			Assert.AreEqual(projectContent, unit.ProjectContent);
		}
	}
}
