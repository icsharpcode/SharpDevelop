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
