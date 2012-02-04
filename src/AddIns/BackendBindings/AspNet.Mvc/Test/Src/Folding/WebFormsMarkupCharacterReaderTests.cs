// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AspNet.Mvc.Folding;
using NUnit.Framework;

namespace AspNet.Mvc.Tests.Folding
{
	[TestFixture]
	public class WebFormsMarkupCharacterReaderTests
	{
		WebFormsMarkupCharacterReader reader;
		
		void CreateReader(string html)
		{
			reader = new WebFormsMarkupCharacterReader(html);
		}
		
		[Test]
		public void IsHtml_CurrentCharacterIsAtStartOfHtml_ReturnsTrue()
		{
			CreateReader("<br/>");
			
			bool htmlCharacterRead = reader.IsHtml;
			
			Assert.IsTrue(htmlCharacterRead);
		}
		
		[Test]
		public void IsHtml_FirstCharacterReadIsPartOfAspxMarkup_ReturnsFalse()
		{
			CreateReader("<%@ Page Language=\"VB\" %>");
			reader.Read();
			
			bool result = reader.IsHtml;
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void IsHtml_FirstCharacterReadIsPartOfBreakTag_ReturnsTrue()
		{
			CreateReader("<br/>");
			reader.Read();
			
			bool readHtmlCharacter = reader.IsHtml;
			
			Assert.IsTrue(readHtmlCharacter);
		}
		
		[Test]
		public void IsHtml_FirstCharacterReadAfterAspxMarkupStartOpenBracketTag_ReturnsFalse()
		{
			CreateReader("<%= model.Message %>");
			reader.Read();
			reader.Read();
			
			bool readHtmlCharacter = reader.IsHtml;
			
			Assert.IsFalse(readHtmlCharacter);
		}
		
		[Test]
		public void IsHtml_ReadFirstCharacterAfterAspxEndMarkupTag_ReturnsTrue()
		{
			CreateReader("<%= %><br/>");
			reader.ReadCharacters(7);
			
			bool readHtmlCharacter = reader.IsHtml;
			
			Assert.IsTrue(readHtmlCharacter);
		}
	}
}
