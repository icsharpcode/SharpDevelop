// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.AspNet.Mvc.Folding;
using ICSharpCode.AvalonEdit.Folding;
using NUnit.Framework;

namespace AspNet.Mvc.Tests.Folding
{
	[TestFixture]
	public class WebFormsHtmlFoldParserTests
	{
		WebFormsHtmlFoldParser parser;
		List<NewFolding> folds;
		
		void CreateParser()
		{
			parser = new WebFormsHtmlFoldParser();
		}
		
		void GetFolds(string text)
		{
			folds = parser.GetFolds(text).ToList();
		}
		
		[Test]
		public void GetFolds_ParagraphStartAndEndTag_ReturnsOneFoldForParagraphTag()
		{
			CreateParser();
			
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
		public void GetFolds_HtmlWithSingleParagraphStartAndEndTag_ReturnsTwoFolds()
		{
			CreateParser();
			
			string text =
				"<html>\r\n" +
				"<p>\r\n" +
				"</p>\r\n" +
				"</html>";
			
			GetFolds(text);
			
			var expectedFolds = new HtmlElementFold[] {
				new HtmlElementFold() {
					ElementName = "html",
					StartOffset = 0,
					EndOffset = 26
				},
				new HtmlElementFold() {
					ElementName = "p",
					StartOffset = 8,
					EndOffset = 17
				}
			};
			
			CollectionAssert.AreEqual(expectedFolds, folds);
		}
		
		[Test]
		public void GetFolds_GetFoldsCalledTwiceForParagraphStartAndEndTag_ReturnsOneFoldForParagraphTag()
		{
			CreateParser();
			
			string text =
				"<p>\r\n" +
				"</p>";
			
			GetFolds(text);
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
		public void GetFolds_ParagraphTagsWithUnclosedBoldTagInside_ReturnsOneFoldForParagraphTag()
		{
			CreateParser();
			
			string text =
				"<p>\r\n" +
				"<b>\r\n" +
				"</p>";
			
			GetFolds(text);
			
			var expectedFolds = new HtmlElementFold[] {
				new HtmlElementFold() {
					ElementName = "p",
					StartOffset = 0,
					EndOffset = 14
				}
			};
			
			CollectionAssert.AreEqual(expectedFolds, folds);
		}
		
		[Test]
		public void GetFolds_EndParagraphTag_ReturnsNoFolds()
		{
			CreateParser();
			
			GetFolds("</p>");
			
			Assert.AreEqual(0, folds.Count);
		}
		
		[Test]
		public void GetFolds_DivTagWithClassAttribute_ReturnsOneFold()
		{
			CreateParser();
			
			string text =
				"<div class=\"foo\">\r\n" +
				"</div>\r\n";
			
			GetFolds(text);
			
			var expectedFolds = new HtmlElementFold[] {
				new HtmlElementFold() {
					ElementName = "div",
					StartOffset = 0,
					EndOffset = 25
				}
			};
			
			CollectionAssert.AreEqual(expectedFolds, folds);
		}
		
		[Test]
		public void GetFolds_ScriptTagAttributeHasAspxTagsInsideWithDoubleQuotes_ReturnsOneFoldIgnoringDoubleQuotesInsideScriptTagAttribute()
		{
			CreateParser();
			
			string text =
				"<script src=\"<%: Url.Content(\"~/Scripts/jquery.js\") %>\" type=\"text/javascript\">\r\n" +
				"</script>\r\n";
			
			GetFolds(text);
			
			var expectedFolds = new HtmlElementFold[] {
				new HtmlElementFold() {
					ElementName = "script",
					StartOffset = 0,
					EndOffset = 90
				}
			};
			
			CollectionAssert.AreEqual(expectedFolds, folds);
		}
		
		[Test]
		public void GetFolds_ScriptStartAndEndTagsOnSameLine_ReturnsNoFolds()
		{
			CreateParser();
			GetFolds("<script></script>");
			
			Assert.AreEqual(0, folds.Count);
		}
	}
}
