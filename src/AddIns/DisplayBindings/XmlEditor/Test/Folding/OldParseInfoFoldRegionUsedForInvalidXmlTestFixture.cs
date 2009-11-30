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
	public class OldParseInfoFoldRegionsUsedForInvalidXmlTestFixture
	{
		XmlFoldParser parser;
		ICompilationUnit unit;
		DefaultProjectContent projectContent;
		DomRegion previousCompilationUnitFoldRegion;
		MockParserService parserService;
		
		[SetUp]
		public void Init()
		{
			string xml = 
				"<root\r\n" +
				"    <child>\r\n" +
				"</root>";
			
			int line = 1;
			int column = 1;
			int endLine = 3;
			int endColumn = 8;
			previousCompilationUnitFoldRegion = new DomRegion(line, column, endLine, endColumn);
			
			projectContent = new DefaultProjectContent();
			DefaultCompilationUnit existingUnit = new DefaultCompilationUnit(projectContent);
			existingUnit.FoldingRegions.Add(new FoldingRegion("<root>", previousCompilationUnitFoldRegion));
			ParseInformation parseInfo = new ParseInformation(existingUnit);
			
			parserService = new MockParserService();
			parserService.SetExistingParseInformation(@"d:\projects\test\a.xml", parseInfo);
			
			MockTextBuffer textBuffer = new MockTextBuffer(xml);
			DefaultXmlFileExtensions extensions = new DefaultXmlFileExtensions(null);
			XmlEditorOptions options = new XmlEditorOptions(new Properties());
			parser = new XmlFoldParser(extensions, options, parserService);
			unit = parser.Parse(projectContent, @"d:\projects\test\a.xml", textBuffer);
		}
		
		[Test]
		public void PreviouslyParsedFoldRegionUsedWhenXmlIsInvalid()
		{
			Assert.AreEqual(previousCompilationUnitFoldRegion, unit.FoldingRegions[0].Region);
		}
		
		[Test]
		public void ProjectContentPassedToParserServiceGetExistingParseInfoMethod()
		{
			Assert.AreSame(projectContent, parserService.ProjectContentPassedToGetExistingParseInforMethod);
		}
	}
}
