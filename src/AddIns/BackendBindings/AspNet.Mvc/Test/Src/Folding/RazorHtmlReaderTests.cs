// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AspNet.Mvc.Folding;
using NUnit.Framework;

namespace AspNet.Mvc.Tests.Folding
{
	[TestFixture]
	public class RazorHtmlReaderTests
	{
		RazorHtmlReader htmlReader;
		
		void CreateHtmlReader(string html)
		{
			htmlReader = new RazorHtmlReader(html, ".cshtml");
		}
		
		[Test]
		public void Value_ReadFirstParagraphTagInsideIfStatementWithLessThanSign_ReturnsParagraphTagName()
		{
			string html = 
				"@if (i<a || b>i) {\r\n" +
				"    <p></p>\r\n" +
				"}\r\n";
				
			CreateHtmlReader(html);
			htmlReader.Read();
			
			string value = htmlReader.Value;
			
			Assert.AreEqual("p", value);
		}
	}
}
