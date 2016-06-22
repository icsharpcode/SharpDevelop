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

namespace ICSharpCode.AspNet.Mvc.Folding
{
	public class WebFormsMarkupCharacterReader : CharacterReader
	{
		bool isInAspxMarkup;
		bool isAtEndAspxMarkup;
		int previousCharacter = EndOfCharacters;
		
		public WebFormsMarkupCharacterReader(string html)
			: base(html)
		{
		}
		
		protected override void OnCharacterRead()
		{
			if (isInAspxMarkup) {
				CheckForAspxEndTag();
			} else {
				CheckForAspxMarkupStartTag();
			}
		}
		
		void CheckForAspxEndTag()
		{
			if (isAtEndAspxMarkup) {
				OutsideOfMarkup();
			}
			if (IsGreaterThanSign() && IsPreviousCharacterPercentSign()) {
				isAtEndAspxMarkup = true;
			}
			previousCharacter = CurrentCharacter;
		}
		
		void OutsideOfMarkup()
		{
			isAtEndAspxMarkup = false;
			isInAspxMarkup = false;
		}
		
		bool IsPreviousCharacterPercentSign()
		{
			return previousCharacter == '%';
		}
		
		void CheckForAspxMarkupStartTag()
		{
			isInAspxMarkup = IsAspxMarkupStartTag();
		}
		
		public bool IsHtml {
			get { return !isInAspxMarkup; }
		}
		
		bool IsAspxMarkupStartTag()
		{
			return IsLessThanSign() && IsNextCharacterPercentSign();
		}
	}
}
