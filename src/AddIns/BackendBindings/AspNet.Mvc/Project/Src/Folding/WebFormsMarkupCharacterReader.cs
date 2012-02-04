// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
