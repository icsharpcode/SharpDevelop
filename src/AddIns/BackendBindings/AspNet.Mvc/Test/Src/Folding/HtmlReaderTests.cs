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
using ICSharpCode.AspNet.Mvc.Folding;
using NUnit.Framework;

namespace AspNet.Mvc.Tests.Folding
{
	[TestFixture]
	public class HtmlReaderTests
	{
		HtmlReader reader;
		
		void CreateReader(string html)
		{
			reader = new HtmlReader(html);
		}
		
		[Test]
		public void Line_ReadParagraphStartTagOnFirstLine_ReturnsOne()
		{
			CreateReader("<p>");
			reader.Read();
			
			int line = reader.Line;
			
			Assert.AreEqual(1, line);
		}
		
		[Test]
		public void Line_ReadParagraphEndTagOnSecondLine_ReturnsTwo()
		{
			string html =
				"<p>\r\n" +
				"</p>\r\n";
			
			CreateReader(html);
			reader.Read();
			reader.Read();
			
			int line = reader.Line;
			
			Assert.AreEqual(2, line);
		}
	}
}
