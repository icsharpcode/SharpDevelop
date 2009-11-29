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
	public class ShowElementAttributesForElementWithNoAttributesTestFixture
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
				"<root>\r\n" +
				"</root>";
			
			projectContent = new DefaultProjectContent();
			textBuffer = new MockTextBuffer(xml);
			extensions = new DefaultXmlFileExtensions(null);
			options = new XmlEditorOptions(new Properties());
			options.ShowAttributesWhenFolded = true;

			parser = new XmlFoldParser(extensions, options);
			unit = parser.Parse(projectContent, @"d:\projects\a.xml", textBuffer);
		}

		[Test]
		public void FoldNameIsElementNameOnlyWithNoExtraSpaceAtTheEnd()
		{
			Assert.AreEqual("<root>", unit.FoldingRegions[0].Name);
		}		
	}
}
