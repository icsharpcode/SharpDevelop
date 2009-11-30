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
	public class EmptyCommentDoesNotCreateFoldTestFixture
	{
		XmlFoldParser parser;
		ICompilationUnit unit;
		DefaultXmlFileExtensions extensions;
		DefaultProjectContent projectContent;
		
		[SetUp]
		public void Init()
		{
			string xml = 
				"<!---->\r\n" +
				"<root>\r\n" +
				"</root>";
			
			projectContent = new DefaultProjectContent();
			MockTextBuffer textBuffer = new MockTextBuffer(xml);
			
			extensions = new DefaultXmlFileExtensions(null);
			XmlEditorOptions options = new XmlEditorOptions(new Properties());
			MockParserService parserService = new MockParserService();
			parser = new XmlFoldParser(extensions, options, parserService);
			unit = parser.Parse(projectContent, @"d:\projects\a.xml", textBuffer);
		}
		
		[Test]
		public void CompilationUnitHasOneFold()
		{
			Assert.AreEqual(1, unit.FoldingRegions.Count);
		}
		
		[Test]
		public void FoldRegionContainsRootElement()
		{
			int beginLine = 2;
			int beginColumn = 1;
			int endLine = 3;
			int endColumn =8;
			DomRegion expectedRegion = new DomRegion(beginLine, beginColumn, endLine, endColumn);
			Assert.AreEqual(expectedRegion, unit.FoldingRegions[0].Region);
		}
	}
}
