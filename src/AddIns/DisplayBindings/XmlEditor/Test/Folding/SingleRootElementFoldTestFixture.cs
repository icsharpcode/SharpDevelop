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
	public class SingleRootElementFoldTestFixture
	{
		XmlFoldParser parser;
		ICompilationUnit unit;
		DefaultXmlFileExtensions extensions;
		DefaultProjectContent projectContent;
		
		[SetUp]
		public void Init()
		{
			string xml = 
				"<root>\r\n" +
				"</root>";
			
			projectContent = new DefaultProjectContent();
			MockTextBuffer textBuffer = new MockTextBuffer(xml);
			
			extensions = new DefaultXmlFileExtensions(null);
			extensions.Add(".wxs");
			XmlEditorOptions options = new XmlEditorOptions(new Properties());
			parser = new XmlFoldParser(extensions, options);
			unit = parser.Parse(projectContent, @"d:\projects\a.xml", textBuffer);
		}
		
		[Test]
		public void XmlFoldParserCanParseProjectReturnsTrue()
		{
			IProject project = null;
			Assert.IsTrue(parser.CanParse(project));
		}
		
		[Test]
		public void XmlFoldParserLexerTagsHasNoItems()
		{
			Assert.AreEqual(0, parser.LexerTags.Length);
		}
		
		[Test]
		public void XmlFoldParserCanParseReturnsTrueIfFileExtensionInDefaultXmlFileExtensions()
		{
			Assert.IsTrue(parser.CanParse("TEST.WXS"));
		}
		
		[Test]
		public void XmlFoldParserCanParseReturnsFalseForUnknownFileExtension()
		{
			Assert.IsFalse(parser.CanParse("test.unknown"));
		}
		
		[Test]
		public void CompilationUnitUsesProjectContentPassedToParseMethod()
		{
			Assert.AreSame(projectContent, unit.ProjectContent);
		}
		
		[Test]
		public void CompilationUnitFileNameIsFileNamePassedToParseMethod()
		{
			Assert.AreEqual(@"d:\projects\a.xml", unit.FileName);
		}
		
		[Test]
		public void FoldingRegionsHasOneFold()
		{
			Assert.AreEqual(1, unit.FoldingRegions.Count);
		}
		
		[Test]
		public void FoldNameIsElementNameSurroundedByAngledBrackets()
		{
			Assert.AreEqual("<root>", unit.FoldingRegions[0].Name);
		}
		
		[Test]
		public void FoldRegionContainsRootElement()
		{
			int beginLine = 1;
			int beginColumn = 1;
			int endLine = 2;
			int endColumn = 8;
			DomRegion expectedRegion = new DomRegion(beginLine, beginColumn, endLine, endColumn);
			Assert.AreEqual(expectedRegion, unit.FoldingRegions[0].Region);
		}
	}
}
