// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Folding
{
	[TestFixture]
	public class EmptyCommentDoesNotCreateFoldTestFixture
	{
		XmlFoldParserHelper helper;
		
		[SetUp]
		public void Init()
		{
			string xml = 
				"<!---->\r\n" +
				"<root>\r\n" +
				"</root>";
			
			helper = new XmlFoldParserHelper();
			helper.CreateParser();
			helper.GetFolds(xml);
		}
		
		[Test]
		public void GetFolds_CommentAndSingleRootElement_ReturnsOneFold()
		{
			int count = helper.Folds.Count;
			Assert.AreEqual(1, count);
		}
		
		[Test]
		public void GetFolds_CommentAndSingleRootElement_FoldRegionContainsRootElement()
		{
			DomRegion region = helper.GetFirstFoldRegion();

			int beginLine = 2;
			int beginColumn = 1;
			int endLine = 3;
			int endColumn = 8;
			DomRegion expectedRegion = new DomRegion(beginLine, beginColumn, endLine, endColumn);
			
			Assert.AreEqual(expectedRegion, region);
		}
	}
}
