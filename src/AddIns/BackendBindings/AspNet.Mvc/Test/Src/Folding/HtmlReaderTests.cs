// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
