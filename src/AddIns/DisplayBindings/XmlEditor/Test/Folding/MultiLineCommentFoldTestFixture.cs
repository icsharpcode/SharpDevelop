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
	public class MultiLineCommentFoldTestFixture
	{
		XmlFoldParser parser;
		ICompilationUnit unit;
		DefaultXmlFileExtensions extensions;
		DefaultProjectContent projectContent;
		
		[SetUp]
		public void Init()
		{
			string xml = 
				"<!-- first line\r\n" +
				"second line -->";
			
			projectContent = new DefaultProjectContent();
			MockTextBuffer textBuffer = new MockTextBuffer(xml);
			
			extensions = new DefaultXmlFileExtensions(null);
			XmlEditorOptions options = new XmlEditorOptions(new Properties());
			parser = new XmlFoldParser(extensions, options);
			unit = parser.Parse(projectContent, @"d:\projects\a.xml", textBuffer);
		}
		
		[Test]
		public void FoldingRegionsHasOneFold()
		{
			Assert.AreEqual(1, unit.FoldingRegions.Count);
		}
		
		[Test]
		public void FoldNameIsCommentTagsWithFirstLineOfTextBetween()
		{
			Assert.AreEqual("<!-- first line -->", unit.FoldingRegions[0].Name);
		}
		
		[Test]
		public void FoldRegionContainsEntireComment()
		{
			int beginLine = 1;
			int beginColumn = 1;
			int endLine = 2;
			int endColumn = 16;
			DomRegion expectedRegion = new DomRegion(beginLine, beginColumn, endLine, endColumn);
			Assert.AreEqual(expectedRegion, unit.FoldingRegions[0].Region);
		}
	}
}
