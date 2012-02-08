// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AspNet.Mvc.Folding;
using NUnit.Framework;

namespace AspNet.Mvc.Tests.Folding
{
	[TestFixture]
	public class RazorMarkupCharacterReaderTests
	{
		RazorMarkupCharacterReader reader;
		
		void CreateCSharpReader(string html)
		{
			reader = new RazorMarkupCharacterReader(html, ".cshtml");
		}
		
		void CreateVisualBasicReader(string html)
		{
			reader = new RazorMarkupCharacterReader(html, ".vbhtml");
		}
		
		[Test]
		public void IsHtml_CurrentCharacterIsAtStartOfHtml_ReturnsTrue()
		{
			CreateCSharpReader("<br/>");
			
			bool htmlCharacterRead = reader.IsHtml;
			
			Assert.IsTrue(htmlCharacterRead);
		}
		
		[Test]
		public void IsHtml_FirstCharacterReadIsCSharpRazorTransitionSymbol_ReturnsFalse()
		{
			CreateCSharpReader("@model");
			reader.Read();
			
			bool result = reader.IsHtml;
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void IsHtml_ReadFirstCSharpRazorTransitionSymbolAfterParagraphTag_ReturnsFalse()
		{
			CreateCSharpReader("<p>@model.Message</p>");
			reader.ReadCharacters(4);
			
			bool htmlCharacterRead = reader.IsHtml;
			
			Assert.IsFalse(htmlCharacterRead);
		}
		
		[Test]
		public void IsHtml_ReadFirstCharacterAfterCSharpRazorTransitionSymbol_ReturnsFalse()
		{
			CreateCSharpReader("<p>@model.Message</p>");
			reader.ReadCharacters(5);
			
			bool htmlCharacterRead = reader.IsHtml;
			
			Assert.IsFalse(htmlCharacterRead);
		}
		
		[Test]
		public void IsHtml_FirstCharacterReadIsVisualBasicRazorTransitionSymbol_ReturnsFalse()
		{
			CreateVisualBasicReader("@ModelType");
			reader.Read();
			
			bool result = reader.IsHtml;
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void IsHtml_SecondCharacterReadIsVisualBasicIfStatement_ReturnsFalse()
		{
			CreateVisualBasicReader("@If IsPost Then");
			reader.Read();
			reader.Read();
			
			bool result = reader.IsHtml;
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void IsHtml_SecondHtmlCharacterAfterVisualBasicCodeBlockRead_ReturnsTrue()
		{
			string html = 
				"@Code\r\n" +
				"Dim a As Int\r\n" +
				"End Code\r\n" +
				"<p></p>";
			
			CreateVisualBasicReader(html);
			reader.ReadCharacters(32);
			
			bool result = reader.IsHtml;
			
			Assert.IsTrue(result);
		}
	}
}
