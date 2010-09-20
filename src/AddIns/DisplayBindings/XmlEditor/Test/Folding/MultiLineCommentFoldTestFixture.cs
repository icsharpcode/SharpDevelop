// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Folding
{
	[TestFixture]
	public class MultiLineCommentFoldTestFixture
	{
		XmlFoldParserHelper helper;
		
		[SetUp]
		public void Init()
		{
			string xml = 
				"<!-- first line\r\n" +
				"second line -->\r\n" +
				"<a />";
			
			helper = new XmlFoldParserHelper();
			helper.CreateParser();
			helper.GetFolds(xml);
		}
		
		[Test]
		public void GetFolds_MultiLineComment_ReturnsOneFold()
		{
			int count = helper.Folds.Count;
			Assert.AreEqual(1, count);
		}
		
		[Test]
		public void GetFolds_MultiLineComment_FoldNameIsCommentTagsWithFirstLineOfTextBetween()
		{
			string name = helper.GetFirstFoldName();
			string expectedName = "<!-- first line -->";
			Assert.AreEqual(expectedName, name);
		}
		
		[Test]
		public void GetFolds_MultiLineComment_FoldRegionContainsEntireComment()
		{
			DomRegion region = helper.GetFirstFoldRegion();
			
			int beginLine = 1;
			int beginColumn = 1;
			int endLine = 2;
			int endColumn = 16;
			DomRegion expectedRegion = new DomRegion(beginLine, beginColumn, endLine, endColumn);
			
			Assert.AreEqual(expectedRegion, region);
		}
	}
}
