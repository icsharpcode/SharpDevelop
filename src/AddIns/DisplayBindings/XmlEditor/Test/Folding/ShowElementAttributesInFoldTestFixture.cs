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
	public class ShowElementAttributesInFoldTestFixture
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
			string xml = 
				"<root a=\"1st\" b=\"2nd\" c='3rd'>\r\n" +
				"</root>";
			
			projectContent = new DefaultProjectContent();
			textBuffer = new MockTextBuffer(xml);
			extensions = new DefaultXmlFileExtensions(null);
			options = new XmlEditorOptions(new Properties());
		}

		void ParseWithShowAttributesSetToTrue()
		{
			options.ShowAttributesWhenFolded = true;
			ParseXml();
		}

		void ParseXml()
		{
			parser = new XmlFoldParser(extensions, options);
			unit = parser.Parse(projectContent, @"d:\projects\a.xml", textBuffer);
		}

		void ParseWithShowAttributesSetToFalse()
		{
			options.ShowAttributesWhenFolded = false;
			ParseXml();
		}
		
		[Test]
		public void FoldNameIsElementNameWithAttributesWhenShowAttributesIsTrue()
		{
			ParseWithShowAttributesSetToTrue();
			Assert.AreEqual("<root a=\"1st\" b=\"2nd\" c='3rd'>", unit.FoldingRegions[0].Name);
		}

		[Test]
		public void FoldNameIsElementNameOnlyWhenShowAttributesIsFalse()
		{
			ParseWithShowAttributesSetToFalse();
			Assert.AreEqual("<root>", unit.FoldingRegions[0].Name);
		}
		
		[Test]
		public void FoldRegionContainsRootElement()
		{
			ParseWithShowAttributesSetToTrue();

			int beginLine = 1;
			int beginColumn = 1;
			int endLine = 2;
			int endColumn = 8;
			DomRegion expectedRegion = new DomRegion(beginLine, beginColumn, endLine, endColumn);
			Assert.AreEqual(expectedRegion, unit.FoldingRegions[0].Region);
		}
	}
}
