// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
