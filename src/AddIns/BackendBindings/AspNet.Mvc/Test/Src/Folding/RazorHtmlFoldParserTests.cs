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
