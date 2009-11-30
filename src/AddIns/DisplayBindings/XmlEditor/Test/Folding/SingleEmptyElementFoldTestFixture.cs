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
	public class SingleEmptyElementFoldTestFixture
	{
		XmlFoldParser parser;
		ICompilationUnit unit;
		DefaultProjectContent projectContent;
		
		[SetUp]
		public void Init()
		{
			string xml = 
				"<root>\r\n" +
				"    <child />\r\n" +
				"</root>";
			
			projectContent = new DefaultProjectContent();
			MockTextBuffer textBuffer = new MockTextBuffer(xml);
			
			DefaultXmlFileExtensions extensions = new DefaultXmlFileExtensions(null);
			XmlEditorOptions options = new XmlEditorOptions(new Properties());
			MockParserService parserService = new MockParserService();
			parser = new XmlFoldParser(extensions, options, parserService);
			unit = parser.Parse(projectContent, @"d:\projects\a.xml", textBuffer);
		}
		
		[Test]
		public void FoldRegionCoversRootElement()
		{
			int beginLine = 1;
			int endLine = 3;
			int beginCol = 1;
			int endCol = 8;
			DomRegion expectedRegion = new DomRegion(beginLine, beginCol, endLine, endCol);
			
			Assert.AreEqual(expectedRegion, unit.FoldingRegions[0].Region);
		}
	}
}
