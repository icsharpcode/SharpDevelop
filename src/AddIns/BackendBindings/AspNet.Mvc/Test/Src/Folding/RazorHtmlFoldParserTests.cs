// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.AspNet.Mvc.Folding;
using ICSharpCode.AvalonEdit.Folding;
using NUnit.Framework;

namespace AspNet.Mvc.Tests.Folding
{
	[TestFixture]
	public class RazorHtmlFoldParserTests
	{
		RazorHtmlFoldParser parser;
		List<NewFolding> folds;
		
		void CreateCSharpParser()
		{
			parser = new RazorHtmlFoldParser("cshtml");
		}
		
		void GetFolds(string text)
		{
			folds = parser.GetFolds(text).ToList();
		}
		
		[Test]
		public void GetFolds_ParagraphStartAndEndTag_ReturnsOneFoldForParagraphTag()
		{
			CreateCSharpParser();
			
			string text =
				"<p>\r\n" +
				"</p>";
			
			GetFolds(text);
			
			var expectedFolds = new HtmlElementFold[] {
				new HtmlElementFold() {
					ElementName = "p",
					StartOffset = 0,
					EndOffset = 9
				}
			};
			
			CollectionAssert.AreEqual(expectedFolds, folds);
		}
		
		[Test]
		public void GetFolds_EndAnchorTagInsideIfStatement_ReturnsOneFoldForParagraphTagAndNotAnchorTag()
		{
			CreateCSharpParser();
			
			string text =
				"@if (i<a || b>i) {\r\n" +
				"    </a>\r\n" +
				"    <p>\r\n" +
				"    </p>\r\n" +
				"}\r\n";

			GetFolds(text);
			
			var expectedFolds = new HtmlElementFold[] {
				new HtmlElementFold() {
					ElementName = "p",
					StartOffset = 34,
					EndOffset = 47
				}
			};
			
			CollectionAssert.AreEqual(expectedFolds, folds);
		}
	}
}
