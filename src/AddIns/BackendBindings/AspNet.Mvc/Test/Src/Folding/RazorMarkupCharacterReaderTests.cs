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
		
		void CreateReader(string html)
		{
			reader = new RazorMarkupCharacterReader(html);
		}
		
		[Test]
		public void IsHtml_CurrentCharacterIsAtStartOfHtml_ReturnsTrue()
		{
			CreateReader("<br/>");
			
			bool htmlCharacterRead = reader.IsHtml;
			
			Assert.IsTrue(htmlCharacterRead);
		}
		
		[Test]
		public void IsHtml_FirstCharacterReadIsRazorTransitionSymbol_ReturnsFalse()
		{
			CreateReader("@model");
			reader.Read();
			
			bool result = reader.IsHtml;
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void IsHtml_ReadFirstRazorTransitionSymbolAfterParagraphTag_ReturnsFalse()
		{
			CreateReader("<p>@model.Message</p>");
			reader.ReadCharacters(4);
			
			bool htmlCharacterRead = reader.IsHtml;
			
			Assert.IsFalse(htmlCharacterRead);
		}
		
		[Test]
		public void IsHtml_ReadFirstCharacterAfterRazorTransitionSymbol_ReturnsFalse()
		{
			CreateReader("<p>@model.Message</p>");
			reader.ReadCharacters(5);
			
			bool htmlCharacterRead = reader.IsHtml;
			
			Assert.IsFalse(htmlCharacterRead);
		}
	}
}
