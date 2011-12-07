// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;

namespace ICSharpCode.AspNet.Mvc.Folding
{
	public class CharacterReader
	{
		public const int EndOfCharacters = -1;
		
		TextReader reader;
		
		public CharacterReader(string html)
			: this(new StringReader(html))
		{
		}
		
		public CharacterReader(TextReader reader)
		{
			this.reader = reader;
		}
		
		public bool Read()
		{
			CurrentCharacter = reader.Read();
			Offset++;
			return HasMoreCharactersToRead();
		}
		
		bool HasMoreCharactersToRead()
		{
			return CurrentCharacter != EndOfCharacters;
		}
		
		public int Offset { get; private set; }
		
		public int PreviousOffset {
			get { return Offset - 1; }
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
	}
}
