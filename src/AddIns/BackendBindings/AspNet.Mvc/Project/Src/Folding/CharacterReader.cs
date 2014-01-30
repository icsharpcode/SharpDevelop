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
using System.IO;

namespace ICSharpCode.AspNet.Mvc.Folding
{
	public class CharacterReader
	{
		public const int EndOfCharacters = -1;
		public const int BeforeStartOffset = -1;
		
		TextReader reader;
		
		public CharacterReader(string html)
			: this(new StringReader(html))
		{
		}
		
		public CharacterReader(TextReader reader)
		{
			this.reader = reader;
			this.CurrentCharacterOffset = BeforeStartOffset;
		}
		
		public bool Read()
		{
			CurrentCharacter = reader.Read();
			CurrentCharacterOffset++;
			OnCharacterRead();
			return HasMoreCharactersToRead();
		}
		
		protected virtual void OnCharacterRead()
		{
		}
		
		bool HasMoreCharactersToRead()
		{
			return CurrentCharacter != EndOfCharacters;
		}
		
		public int CurrentCharacterOffset { get; private set; }
		
		public int NextCharacterOffset {
			get { return CurrentCharacterOffset + 1; }
		}
		
		public int CurrentCharacter { get; private set; }
		
		public bool IsLetterOrDigit()
		{
			return Char.IsLetterOrDigit((char)CurrentCharacter);
		}
		
		public bool IsLessThanSign()
		{
			return CurrentCharacter == '<';
		}
		
		public bool IsGreaterThanSign()
		{
			return CurrentCharacter == '>';
		}
		
		public bool IsForwardSlash()
		{
			return CurrentCharacter == '/';
		}
		
		public bool IsNextCharacterForwardSlash()
		{
			return reader.Peek() == '/';
		}
		
		public bool IsSpace()
		{
			return CurrentCharacter == ' ';
		}
		
		public bool IsDoubleQuote()
		{
			return CurrentCharacter == '\"';
		}
		
		public bool IsSingleQuote()
		{
			return CurrentCharacter == '\'';
		}
		
		public void ReadCharacters(int howMany)
		{
			for (int i = 0; i < howMany; ++i) {
				Read();
			}
		}
		
		public bool IsNextCharacterPercentSign()
		{
			return reader.Peek() == '%';
		}
		
		public bool IsLineFeed()
		{
			return CurrentCharacter == '\n';
		}
		
		public bool IsColon()
		{
			return CurrentCharacter == ':';
		}
	}
}
