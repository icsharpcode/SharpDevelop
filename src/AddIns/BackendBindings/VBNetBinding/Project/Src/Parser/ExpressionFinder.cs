using System;
using System.Text;
using ICSharpCode.SharpDevelop.Dom;

namespace VBNetBinding.Parser
{
	/// <summary>
	/// Description of ExpressionFinder.	
	/// </summary>
	public class ExpressionFinder : IExpressionFinder
	{
		public string FindExpression(string inText, int offset)
		{
			this.text = FilterComments(inText, ref offset);
			this.offset = this.lastAccept = offset;
			this.state  = START;
			if (this.text == null) {
				return null;
			}
			
			while (state != ERROR) {
				ReadNextToken();
				state = stateTable[state, curTokenType];
				
				if (state == ACCEPT || state == ACCEPT2) {
					lastAccept = this.offset;
				}
				if (state == ACCEPTNOMORE) {
					return this.text.Substring(this.offset + 1, offset - this.offset);
				}
			}
			
			return this.text.Substring(this.lastAccept + 1, offset - this.lastAccept);
		}
		
		#region Comment Filter and 'inside string watcher'
		int initialOffset;
		public string FilterComments(string text, ref int offset)
		{
			this.initialOffset = offset;
			StringBuilder outText = new StringBuilder();
			int curOffset = 0;
			
			while (curOffset <= initialOffset) {
				char ch = text[curOffset];
				
				switch (ch) {
					case '@':
						if (curOffset + 1 < text.Length && text[curOffset + 1] == '"') {
							outText.Append(text[curOffset++]); // @
							outText.Append(text[curOffset++]); // "
							if (!ReadVerbatimString(outText, text, ref curOffset)) {
								return null;
							}
						}else{
							outText.Append(ch);
							++curOffset;
						}
						break;
					case '\'':
						outText.Append(ch);
						curOffset++;
						if(! ReadChar(outText, text, ref curOffset)) {
							return null;
						}
						break;
					case '"':
						outText.Append(ch);
						curOffset++;
						if (!ReadString(outText, text, ref curOffset)) {
							return null;
						}
						break;
					case '/':
						if (curOffset + 1 < text.Length && text[curOffset + 1] == '/') {
							offset    -= 2;
							curOffset += 2;
							if (!ReadToEOL(text, ref curOffset, ref offset)) {
								return null;
							}
						} else if (curOffset + 1 < text.Length && text[curOffset + 1] == '*') {
							offset    -= 2;
							curOffset += 2;
							if (!ReadMultiLineComment(text, ref curOffset, ref offset)) {
								return null;
							}
						} else {
							goto default;
						}
						break;
					default:
						outText.Append(ch);
						++curOffset;
						break;
				}
			}
			
			return outText.ToString();
		}
		
		bool ReadToEOL(string text, ref int curOffset, ref int offset)
		{
			while (curOffset <= initialOffset) {
				char ch = text[curOffset++];
				--offset;
				if (ch == '\n') {
					return true;
				}
			}
			return false;
		}
		
		bool ReadChar(StringBuilder outText, string text, ref int curOffset)
		{
			char first = text[curOffset];
			
			if (curOffset <= initialOffset) {
				outText.Append(text[curOffset++]);
			}
			if (curOffset <= initialOffset) {
				outText.Append(text[curOffset++]);
			}
			
			// special case: '\''
			if(first == '\\' && curOffset <= initialOffset) {
				outText.Append(text[curOffset++]);
			}
			return text[curOffset - 1] == '\'';
		}
		
		bool ReadString(StringBuilder outText, string text, ref int curOffset)
		{
			while (curOffset <= initialOffset) {
				char ch = text[curOffset++];
				outText.Append(ch);
				if (ch == '"') {
					return true;
				} else if (ch == '\\') {
					outText.Append(text[curOffset++]);
				}
			}
			return false;
		}
		
		bool ReadVerbatimString(StringBuilder outText, string text, ref int curOffset)
		{
			while (curOffset <= initialOffset) {
				char ch = text[curOffset++];
				outText.Append(ch);
				if (ch == '"') {
					if (curOffset < text.Length && text[curOffset] == '"') {
						outText.Append(text[curOffset++]);
					} else {
						return true;
					}
				}
			}
			return false;
		}
		
		bool ReadMultiLineComment(string text, ref int curOffset, ref int offset)
		{
			while (curOffset <= initialOffset) {
				char ch = text[curOffset++];
				--offset;
				if (ch == '*') {
					if (curOffset < text.Length && text[curOffset] == '/') {
						++curOffset;
						--offset;
						return true;
					}
				}
			}
			return false;
		}
		#endregion
		
		#region mini backward lexer
		string text;
		int    offset;
		
		char GetNext()
		{
			if (offset >= 0) {
				return text[offset--];
			}
			return '\0';
		}
		
		char Peek(int n)
		{
			if (offset - n >= 0) {
				return text[offset - n];
			}
			return '\0';
		}
		
		char Peek()
		{
			if (offset >= 0) {
				return text[offset];
			}
			return '\0';
		}
		
		void UnGet()
		{
			++offset;
		}
		
		// tokens for our lexer
		static int Err     = 0;
		static int Dot     = 1;
		static int StrLit  = 2;
		static int Ident   = 3;
		static int New     = 4;
		static int Bracket = 5;
		static int Parent  = 6;
		static int Curly   = 7;
		static int Using   = 8;
		static int Digit   = 9;
		int curTokenType;
		
		readonly static string[] tokenStateName = new string[] {
			"Err", "Dot", "StrLit", "Ident", "New", "Bracket", "Paren", "Curly", "Using", "Digit"
		};
		
		string GetTokenName(int state)
		{
			return tokenStateName[state];
		}
		
		void ReadNextToken()
		{
			char ch = GetNext();
				
			curTokenType = Err;
			if (ch == '\0') {
				return;
			}
			while (Char.IsWhiteSpace(ch)) {
				ch = GetNext();
			}
			
			switch (ch) {
				case '}':
					if (ReadBracket('{', '}')) {
						curTokenType = Curly;
					}
					break;
				case ')':
					if (ReadBracket('(', ')')) {
						curTokenType = Parent;
					}
					break;
				case ']':
					if (ReadBracket('[', ']')) {
						curTokenType = Bracket;
					}
					break;
				case '.':
					curTokenType = Dot;
					break;
				case '\'':
				case '"':
					if (ReadStringLiteral(ch)) {
						curTokenType = StrLit;
					}
					break;
				default:
					if (IsDigit()) {
						string digit = ReadDigit(ch);
						curTokenType = Digit;
					} else if (IsIdentifierPart(ch)) {
						string ident = ReadIdentifier(ch);
						if (ident != null) {
							switch (ident) {
								case "new":
									curTokenType = New;
									break;
								case "using":
									curTokenType = Using;
									break;
								default:
									curTokenType = Ident;
									break;
							}
						}
					}

					break;
			}
		}
		bool IsDigit()
		{
			int n = 0;
			while (true) {
				char ch = Peek(n);
				if (Char.IsDigit(ch)) {
					n++;
					continue;
				}
				return n > 0 && !Char.IsLetter(ch);
			}
		}
		bool ReadStringLiteral(char litStart)
		{
			while (true) {
				char ch = GetNext();
				if (ch == '\0') {
					return false;
				}
				if (ch == litStart) {
					if (Peek() == '@' && litStart == '"') {
						GetNext();
					}
					return true;
				}
			}
		}
		
		bool ReadBracket(char openBracket, char closingBracket)
		{
			int curlyBraceLevel    = 0;
			int squareBracketLevel = 0;
			int parenthesisLevel   = 0;
			switch (openBracket) {
				case '(':
					parenthesisLevel++;
					break;
				case '[':
					squareBracketLevel++;
					break;
				case '{':
					curlyBraceLevel++;
					break;
			}
			
			while (parenthesisLevel != 0 || squareBracketLevel != 0 || curlyBraceLevel != 0) {
				char ch = GetNext();
				if (ch == '\0') {
					return false;
				}
				switch (ch) {
					case '(':
						parenthesisLevel--;
						break;
					case '[':
						squareBracketLevel--;
						break;
					case '{':
						curlyBraceLevel--;
						break;
					case ')':
						parenthesisLevel++;
						break;
					case ']':
						squareBracketLevel++;
						break;
					case '}':
						curlyBraceLevel++;
						break;
				}
			}
			return true;
		}
		
		string ReadIdentifier(char ch)
		{
			string identifier = ch.ToString();
			while (IsIdentifierPart(Peek())) {
				identifier = GetNext() + identifier;
			}
			return identifier;
		}
		
		string ReadDigit(char ch)
		{
			string digit = ch.ToString();
			while (Char.IsDigit(Peek()) || Peek() == '.') {
				digit = GetNext() + digit;
			}
			return digit;
		}
		
		bool IsIdentifierPart(char ch)
		{
			return Char.IsLetterOrDigit(ch) || ch == '_';
		}
		#endregion
		
		#region finite state machine 
		readonly static int ERROR  = 0;
		readonly static int START  = 1;
		readonly static int DOT    = 2;
		readonly static int MORE   = 3;
		readonly static int CURLY  = 4;
		readonly static int CURLY2 = 5;
		readonly static int CURLY3 = 6;
		
		readonly static int ACCEPT = 7;
		readonly static int ACCEPTNOMORE = 8;
		readonly static int ACCEPT2 = 9;
		
		readonly static string[] stateName = new string[] {
			"ERROR", 
			"START", 
			"DOT", 
			"MORE", 
			"CURLY", 
			"CURLY2", 
			"CURLY3", 
			"ACCEPT", 
			"ACCEPTNOMORE", 
			"ACCEPT2"
		};
			
		string GetStateName(int state)
		{
			return stateName[state];
		}
		
		int state = 0;
		int lastAccept = 0;
		static int[,] stateTable = new int[,] {
			//                   Err,     Dot,     Str,      ID,         New,     Brk,     Par,     Cur,   Using,       digit
			/*ERROR*/        { ERROR,   ERROR,   ERROR,   ERROR,        ERROR,  ERROR,   ERROR,   ERROR,   ERROR,        ERROR},
			/*START*/        { ERROR,     DOT,  ACCEPT,  ACCEPT,        ERROR,   MORE, ACCEPT2,   CURLY,   ACCEPTNOMORE, ERROR},
			/*DOT*/          { ERROR,   ERROR,  ACCEPT,  ACCEPT,        ERROR,   MORE,  ACCEPT,   CURLY,   ERROR,        ACCEPT},
			/*MORE*/         { ERROR,   ERROR,  ACCEPT,  ACCEPT,        ERROR,   MORE, ACCEPT2,   CURLY,   ERROR,        ACCEPT},
			/*CURLY*/        { ERROR,   ERROR,   ERROR,   ERROR,        ERROR, CURLY2,   ERROR,   ERROR,   ERROR,        ERROR},
			/*CURLY2*/       { ERROR,   ERROR,   ERROR,  CURLY3,        ERROR,  ERROR,   ERROR,   ERROR,   ERROR,        CURLY3},
			/*CURLY3*/       { ERROR,   ERROR,   ERROR,   ERROR, ACCEPTNOMORE,  ERROR,   ERROR,   ERROR,   ERROR,        ERROR},
			/*ACCEPT*/       { ERROR,    MORE,   ERROR,   ERROR,       ACCEPT,  ERROR,   ERROR,   ERROR,   ACCEPTNOMORE, ERROR},
			/*ACCEPTNOMORE*/ { ERROR,   ERROR,   ERROR,   ERROR,        ERROR,  ERROR,   ERROR,   ERROR,   ERROR,        ERROR},
			/*ACCEPT2*/      { ERROR,    MORE,   ERROR,  ACCEPT,       ACCEPT,  ERROR,   ERROR,   ERROR,   ERROR,        ACCEPT},
		};
		#endregion 
	}
}
