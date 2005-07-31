// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Andrea Paatz" email="andrea@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Collections;
using System.Drawing;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using ICSharpCode.NRefactory.Parser;

namespace ICSharpCode.NRefactory.Parser.CSharp
{
	internal sealed class Lexer : AbstractLexer
	{
		public Lexer(TextReader reader) : base(reader)
		{
		}
		
		protected override Token Next()
		{
			int nextChar;
			while ((nextChar = ReaderRead()) != -1) {
				char ch = (char)nextChar;
				
				if (Char.IsWhiteSpace(ch)) {
					HandleLineEnd(ch);
					continue;
				}
				
				if (Char.IsLetter(ch) || ch == '_') {
					int x = col - 1; // col was incremented above, but we want the start of the identifier
					int y = line;
					string s = ReadIdent(ch);
					int keyWordToken = Keywords.GetToken(s);
					if (keyWordToken >= 0) {
						return new Token(keyWordToken, x, y);
					}
					return new Token(Tokens.Identifier, x, y, s);
				}
				
				if (Char.IsDigit(ch)) {
					return ReadDigit(ch, col - 1);
				}
				
				switch (ch) {
					case '/':
						int peek = ReaderPeek();
						if (peek == '/' || peek == '*') {
							ReadComment();
							continue;
						}
						break;
					case '#':
						Point start = new Point(col - 1, line);
						string directive = ReadIdent('#');
						string argument  = ReadToEOL();
						this.specialTracker.AddPreProcessingDirective(directive, argument.Trim(), start, new Point(start.X + directive.Length + argument.Length, start.Y));
						continue;
					case '"':
						return ReadString();
					case '\'':
						return ReadChar();
					case '@':
						int next = ReaderRead();
						if (next == -1) {
							errors.Error(line, col, String.Format("EOF after @"));
						} else {
							int x = col;
							int y = line;
							ch = (char)next;
							if (ch == '"') {
								return ReadVerbatimString();
							}
							if (Char.IsLetterOrDigit(ch)) {
								return new Token(Tokens.Identifier, x, y, ReadIdent(ch));
							}
							errors.Error(y, x, String.Format("Unexpected char in Lexer.Next() : {0}", ch));
						}
						break;
				}
				
				Token token = ReadOperator(ch);
				
				// try error recovery :)
				if (token == null) {
					return Next();
				}
				return token;
			}
			
			return new Token(Tokens.EOF, col, line, String.Empty);
		}
		
		// The C# compiler has a fixed size length therefore we'll use a fixed size char array for identifiers
		// it's also faster than using a string builder.
		const int MAX_IDENTIFIER_LENGTH = 512;
		char[] identBuffer = new char[MAX_IDENTIFIER_LENGTH];
		
		string ReadIdent(char ch)
		{
			int curPos     = 1;
			identBuffer[0] = ch;
			int peek;
			while ((peek = ReaderPeek()) != -1 && (Char.IsLetterOrDigit(ch = (char)peek) || ch == '_')) {
				ReaderRead();
				
				if (curPos < MAX_IDENTIFIER_LENGTH) {
					identBuffer[curPos++] = ch;
				} else {
					errors.Error(line, col, String.Format("Identifier too long"));
					while ((peek = ReaderPeek()) != -1 && (Char.IsLetterOrDigit(ch = (char)peek) || ch == '_')) {
						ReaderRead();
					}
					break;
				}
			}
			return new String(identBuffer, 0, curPos);
		}
		
		Token ReadDigit(char ch, int x)
		{
			unchecked { // prevent exception when ReaderPeek() = -1 is cast to char
				int y = line;
				sb.Length = 0;
				sb.Append(ch);
				string prefix = null;
				string suffix = null;
				
				bool ishex      = false;
				bool isunsigned = false;
				bool islong     = false;
				bool isfloat    = false;
				bool isdouble   = false;
				bool isdecimal  = false;
				
				char peek = (char)ReaderPeek();
				
				if (ch == '.')  {
					isdouble = true;
					
					while (Char.IsDigit((char)ReaderPeek())) { // read decimal digits beyond the dot
						sb.Append((char)ReaderRead());
					}
					peek = (char)ReaderPeek();
				} else if (ch == '0' && (peek == 'x' || peek == 'X')) {
					ReaderRead(); // skip 'x'
					sb.Length = 0; // Remove '0' from 0x prefix from the stringvalue
					while (IsHex((char)ReaderPeek())) {
						sb.Append(Char.ToUpper((char)ReaderRead()));
					}
					ishex = true;
					prefix = "0x";
					peek = (char)ReaderPeek();
				} else {
					while (Char.IsDigit((char)ReaderPeek())) {
						sb.Append((char)ReaderRead());
					}
					peek = (char)ReaderPeek();
				}
				
				Token nextToken = null; // if we accedently read a 'dot'
				if (peek == '.') { // read floating point number
					ReaderRead();
					peek = (char)ReaderPeek();
					if (!Char.IsDigit(peek)) {
						nextToken = new Token(Tokens.Dot, x, y);
						peek = '.';
					} else {
						isdouble = true; // double is default
						if (ishex) {
							errors.Error(y, x, String.Format("No hexadecimal floating point values allowed"));
						}
						sb.Append('.');
						
						while (Char.IsDigit((char)ReaderPeek())) { // read decimal digits beyond the dot
							sb.Append((char)ReaderRead());
						}
						peek = (char)ReaderPeek();
					}
				}
				
				if (peek == 'e' || peek == 'E') { // read exponent
					isdouble = true;
					sb.Append((char)ReaderRead());
					peek = (char)ReaderPeek();
					if (peek == '-' || peek == '+') {
						sb.Append((char)ReaderRead());
					}
					while (Char.IsDigit((char)ReaderPeek())) { // read exponent value
						sb.Append((char)ReaderRead());
					}
					isunsigned = true;
					peek = (char)ReaderPeek();
				}
				
				if (peek == 'f' || peek == 'F') { // float value
					ReaderRead();
					suffix = "f";
					isfloat = true;
				} else if (peek == 'd' || peek == 'D') { // double type suffix (obsolete, double is default)
					ReaderRead();
					suffix = "d";
					isdouble = true;
				} else if (peek == 'm' || peek == 'M') { // decimal value
					ReaderRead();
					suffix = "m";
					isdecimal = true;
				} else if (!isdouble) {
					if (peek == 'u' || peek == 'U') {
						ReaderRead();
						suffix = "u";
						isunsigned = true;
						peek = (char)ReaderPeek();
					}
					
					if (peek == 'l' || peek == 'L') {
						ReaderRead();
						peek = (char)ReaderPeek();
						islong = true;
						if (!isunsigned && (peek == 'u' || peek == 'U')) {
							ReaderRead();
							suffix = "lu";
							isunsigned = true;
						} else {
							suffix = isunsigned ? "ul" : "l";
						}
					}
				}
				
				string digit       = sb.ToString();
				string stringValue = prefix + digit + suffix;
				
				if (isfloat) {
					try {
						return new Token(Tokens.Literal, x, y, stringValue, Single.Parse(digit, CultureInfo.InvariantCulture));
					} catch (Exception) {
						errors.Error(y, x, String.Format("Can't parse float {0}", digit));
						return new Token(Tokens.Literal, x, y, stringValue, 0f);
					}
				}
				if (isdecimal) {
					try {
						return new Token(Tokens.Literal, x, y, stringValue, Decimal.Parse(digit, CultureInfo.InvariantCulture));
					} catch (Exception) {
						errors.Error(y, x, String.Format("Can't parse decimal {0}", digit));
						return new Token(Tokens.Literal, x, y, stringValue, 0m);
					}
				}
				if (isdouble) {
					try {
						return new Token(Tokens.Literal, x, y, stringValue, Double.Parse(digit, CultureInfo.InvariantCulture));
					} catch (Exception) {
						errors.Error(y, x, String.Format("Can't parse double {0}", digit));
						return new Token(Tokens.Literal, x, y, stringValue, 0d);
					}
				}
				
				// Try to determine a parsable value using ranges. (Quick hack!)
				double d = 0;
				if (!ishex  && !Double.TryParse(digit,NumberStyles.Integer, null, out d)) {
					errors.Error(y, x, String.Format("Can't parse integral constant {0}", digit));
					return new Token(Tokens.Literal, x, y, stringValue.ToString(), 0);
				}
				
				if (d < long.MinValue || d > long.MaxValue) {
					islong     = true;
					isunsigned = true;	
				} else if (d < uint.MinValue || d > uint.MaxValue) {
					islong = true;	
				} else if (d < int.MinValue || d > int.MaxValue) {
					isunsigned = true;	
				}
				
				Token token;
				
				if (islong) {
					if (isunsigned) {
						try {
							token = new Token(Tokens.Literal, x, y, stringValue, UInt64.Parse(digit, ishex ? NumberStyles.HexNumber : NumberStyles.Number));
						} catch (Exception) {
							errors.Error(y, x, String.Format("Can't parse unsigned long {0}", digit));
							token = new Token(Tokens.Literal, x, y, stringValue, 0UL);
						}
					} else {
						try {
							token = new Token(Tokens.Literal, x, y, stringValue, Int64.Parse(digit, ishex ? NumberStyles.HexNumber : NumberStyles.Number));
						} catch (Exception) {
							errors.Error(y, x, String.Format("Can't parse long {0}", digit));
							token = new Token(Tokens.Literal, x, y, stringValue, 0L);
						}
					}
				} else {
					if (isunsigned) {
						try {
							token = new Token(Tokens.Literal, x, y, stringValue, UInt32.Parse(digit, ishex ? NumberStyles.HexNumber : NumberStyles.Number));
						} catch (Exception) {
							errors.Error(y, x, String.Format("Can't parse unsigned int {0}", digit));
							token = new Token(Tokens.Literal, x, y, stringValue, 0U);
						}
					} else {
						try {
							token = new Token(Tokens.Literal, x, y, stringValue, Int32.Parse(digit, ishex ? NumberStyles.HexNumber : NumberStyles.Number));
						} catch (Exception) {
							errors.Error(y, x, String.Format("Can't parse int {0}", digit));
							token = new Token(Tokens.Literal, x, y, stringValue, 0);
						}
					}
				}
				token.next = nextToken;
				return token;
			}
		}
		
		Token ReadString()
		{
			int x = col - 1;
			int y = line;
			
			sb.Length = 0;
			originalValue.Length = 0;
			originalValue.Append('"');
			bool doneNormally = false;
			int nextChar;
			while ((nextChar = ReaderRead()) != -1) {
				char ch = (char)nextChar;
				
				if (ch == '"') {
					doneNormally = true;
					originalValue.Append('"');
					break;
				}
				
				if (ch == '\\') {
					originalValue.Append('\\');
					originalValue.Append(ReadEscapeSequence(out ch));
					sb.Append(ch);
				} else if (ch == '\n') {
					errors.Error(y, x, String.Format("No new line is allowed inside a string literal"));
					break;
				} else {
					originalValue.Append(ch);
					sb.Append(ch);
				}
			}
			
			if (!doneNormally) {
				errors.Error(y, x, String.Format("End of file reached inside string literal"));
			}
			
			return new Token(Tokens.Literal, x, y, originalValue.ToString(), sb.ToString());
		}
		
		Token ReadVerbatimString()
		{
			int x = col;
			int y = line;
			int nextChar;
			sb.Length            = 0;
			originalValue.Length = 0;
			originalValue.Append("@\"");
			while ((nextChar = ReaderRead()) != -1) {
				char ch = (char)nextChar;
				
				if (ch == '"') {
					if (ReaderPeek() != '"') {
						originalValue.Append('"');
						break;
					}
					originalValue.Append("\"\"");
					sb.Append('"');
					ReaderRead();
				}
				if (HandleLineEnd(ch)) {
					sb.Append('\n');
					originalValue.Append('\n');
				} else {
					sb.Append(ch);
					originalValue.Append(ch);
				}
			}
			
			if (nextChar == -1) {
				errors.Error(y, x, String.Format("End of file reached inside verbatim string literal"));
			}
			
			return new Token(Tokens.Literal, x, y, originalValue.ToString(), sb.ToString());
		}
		
		char[] escapeSequenceBuffer = new char[12];
		string ReadEscapeSequence(out char ch)
		{
			int nextChar = ReaderRead();
			if (nextChar == -1) {
				errors.Error(line, col, String.Format("End of file reached inside escape sequence"));
				ch = '\0';
				return String.Empty;
			}
			char c = (char)nextChar;
			int curPos              = 1;
			escapeSequenceBuffer[0] = c;
			switch (c)  {
				case '\'':
					ch = '\'';
					break;
				case '\"':
					ch = '\"';
					break;
				case '\\':
					ch = '\\';
					break;
				case '0':
					ch = '\0';
					break;
				case 'a':
					ch = '\a';
					break;
				case 'b':
					ch = '\b';
					break;
				case 'f':
					ch = '\f';
					break;
				case 'n':
					ch = '\n';
					break;
				case 'r':
					ch = '\r';
					break;
				case 't':
					ch = '\t';
					break;
				case 'v':
					ch = '\v';
					break;
				case 'u':
				case 'x':
					c = (char)ReaderRead();
					int number = GetHexNumber(c);
					escapeSequenceBuffer[curPos++] = c;
					
					if (number < 0) {
						errors.Error(line, col - 1, String.Format("Invalid char in literal : {0}", c));
					}
					for (int i = 0; i < 3; ++i) {
						if (IsHex((char)ReaderPeek())) {
							c = (char)ReaderRead();
							int idx = GetHexNumber(c);
							escapeSequenceBuffer[curPos++] = c;
							number = 16 * number + idx;
						} else {
							break;
						}
					}
					ch = (char)number;
					break;
				default:
					errors.Error(line, col, String.Format("Unexpected escape sequence : {0}", c));
					ch = '\0';
					break;
			}
			return new String(escapeSequenceBuffer, 0, curPos);
		}
		
		Token ReadChar()
		{
			int x = col - 1;
			int y = line;
			int nextChar = ReaderRead();
			if (nextChar == -1) {
				errors.Error(y, x, String.Format("End of file reached inside character literal"));
				return null;
			}
			char ch = (char)nextChar;
			char chValue = ch;
			string escapeSequence = String.Empty;
			if (ch == '\\') {
				escapeSequence = ReadEscapeSequence(out chValue);
			}
			
			unchecked {
				if ((char)ReaderRead() != '\'') {
					errors.Error(y, x, String.Format("Char not terminated"));
				}
			}
			return new Token(Tokens.Literal, x, y, "'" + ch + escapeSequence + "'", chValue);
		}
		
		Token ReadOperator(char ch)
		{
			int x = col;
			int y = line;
			switch (ch) {
				case '+':
					switch (ReaderPeek()) {
						case '+':
							ReaderRead();
							return new Token(Tokens.Increment, x, y);
						case '=':
							ReaderRead();
							return new Token(Tokens.PlusAssign, x, y);
					}
					return new Token(Tokens.Plus, x, y);
				case '-':
					switch (ReaderPeek()) {
						case '-':
							ReaderRead();
							return new Token(Tokens.Decrement, x, y);
						case '=':
							ReaderRead();
							return new Token(Tokens.MinusAssign, x, y);
						case '>':
							ReaderRead();
							return new Token(Tokens.Pointer, x, y);
					}
					return new Token(Tokens.Minus, x, y);
				case '*':
					switch (ReaderPeek()) {
						case '=':
							ReaderRead();
							return new Token(Tokens.TimesAssign, x, y);
						default:
							break;
					}
					return new Token(Tokens.Times, x, y);
				case '/':
					switch (ReaderPeek()) {
						case '=':
							ReaderRead();
							return new Token(Tokens.DivAssign, x, y);
					}
					return new Token(Tokens.Div, x, y);
				case '%':
					switch (ReaderPeek()) {
						case '=':
							ReaderRead();
							return new Token(Tokens.ModAssign, x, y);
					}
					return new Token(Tokens.Mod, x, y);
				case '&':
					switch (ReaderPeek()) {
						case '&':
							ReaderRead();
							return new Token(Tokens.LogicalAnd, x, y);
						case '=':
							ReaderRead();
							return new Token(Tokens.BitwiseAndAssign, x, y);
					}
					return new Token(Tokens.BitwiseAnd, x, y);
				case '|':
					switch (ReaderPeek()) {
						case '|':
							ReaderRead();
							return new Token(Tokens.LogicalOr, x, y);
						case '=':
							ReaderRead();
							return new Token(Tokens.BitwiseOrAssign, x, y);
					}
					return new Token(Tokens.BitwiseOr, x, y);
				case '^':
					switch (ReaderPeek()) {
						case '=':
							ReaderRead();
							return new Token(Tokens.XorAssign, x, y);
						default:
							break;
					}
					return new Token(Tokens.Xor, x, y);
				case '!':
					switch (ReaderPeek()) {
						case '=':
							ReaderRead();
							return new Token(Tokens.NotEqual, x, y);
					}
					return new Token(Tokens.Not, x, y);
				case '~':
					return new Token(Tokens.BitwiseComplement, x, y);
				case '=':
					switch (ReaderPeek()) {
						case '=':
							ReaderRead();
							return new Token(Tokens.Equal, x, y);
					}
					return new Token(Tokens.Assign, x, y);
				case '<':
					switch (ReaderPeek()) {
						case '<':
							ReaderRead();
							switch (ReaderPeek()) {
								case '=':
									ReaderRead();
									return new Token(Tokens.ShiftLeftAssign, x, y);
								default:
									break;
							}
							return new Token(Tokens.ShiftLeft, x, y);
						case '=':
							ReaderRead();
							return new Token(Tokens.LessEqual, x, y);
					}
					return new Token(Tokens.LessThan, x, y);
				case '>':
					switch (ReaderPeek()) {
// Removed because of generics:
//						case '>':
//							ReaderRead();
//							if (ReaderPeek() != -1) {
//								switch ((char)ReaderPeek()) {
//									case '=':
//										ReaderRead();
//										return new Token(Tokens.ShiftRightAssign, x, y);
//									default:
//										break;
//								}
//							}
//							return new Token(Tokens.ShiftRight, x, y);
						case '=':
							ReaderRead();
							return new Token(Tokens.GreaterEqual, x, y);
					}
					return new Token(Tokens.GreaterThan, x, y);
				case '?':
					return new Token(Tokens.Question, x, y);
				case ';':
					return new Token(Tokens.Semicolon, x, y);
				case ':':
					if (ReaderPeek() == ':') {
						ReaderRead();
						return new Token(Tokens.DoubleColon, x, y);
					}
					return new Token(Tokens.Colon, x, y);
				case ',':
					return new Token(Tokens.Comma, x, y);
				case '.':
					// Prevent OverflowException when ReaderPeek returns -1
					int tmp = ReaderPeek();
					if (tmp > 0 && Char.IsDigit((char)tmp)) {
						 return ReadDigit('.', col - 1);
					}
					return new Token(Tokens.Dot, x, y);
				case ')':
					return new Token(Tokens.CloseParenthesis, x, y);
				case '(':
					return new Token(Tokens.OpenParenthesis, x, y);
				case ']':
					return new Token(Tokens.CloseSquareBracket, x, y);
				case '[':
					return new Token(Tokens.OpenSquareBracket, x, y);
				case '}':
					return new Token(Tokens.CloseCurlyBrace, x, y);
				case '{':
					return new Token(Tokens.OpenCurlyBrace, x, y);
				default:
					return null;
			}
		}
		
		void ReadComment()
		{
			switch (ReaderRead()) {
				case '*':
					ReadMultiLineComment();
					break;
				case '/':
					if (ReaderPeek() == '/') {
						ReaderRead();
						ReadSingleLineComment(CommentType.Documentation);
					} else {
						ReadSingleLineComment(CommentType.SingleLine);
					}
					break;
				default:
					errors.Error(line, col, String.Format("Error while reading comment"));
					break;
			}
		}
		
		string ReadCommentToEOL()
		{
			sb.Length = 0;
			StringBuilder curWord = specialCommentHash != null ? new StringBuilder() : null;
			
			int nextChar;
			while ((nextChar = ReaderRead()) != -1) {
				char ch = (char)nextChar;
				
				if (HandleLineEnd(ch)) {
					break;
				}
				
				sb.Append(ch);
				if (specialCommentHash != null) {
					if (Char.IsLetter(ch)) {
						curWord.Append(ch);
					} else {
						string tag = curWord.ToString();
						curWord.Length = 0;
						if (specialCommentHash.ContainsKey(tag)) {
							Point p = new Point(col, line);
							string comment = ReadToEOL();
							tagComments.Add(new TagComment(tag, comment, p, new Point(col, line)));
							sb.Append(comment);
							break;
						}
					}
				}
			}
			return sb.ToString();
		}
		
		void ReadSingleLineComment(CommentType commentType)
		{
			specialTracker.StartComment(commentType, new Point(col, line));
			specialTracker.AddString(ReadCommentToEOL());
			specialTracker.FinishComment(new Point(col, line));
		}
		
		void ReadMultiLineComment()
		{
			specialTracker.StartComment(CommentType.Block, new Point(col, line));
			int nextChar;
			while ((nextChar = ReaderRead()) != -1) {
				char ch = (char)nextChar;
				
				if (HandleLineEnd(ch)) {
					specialTracker.AddChar('\n');
					continue;
				}
				
				// End of multiline comment reached ?
				if (ch == '*' && ReaderPeek() == '/') {
					ReaderRead();
					specialTracker.FinishComment(new Point(col, line));
					return;
				}
				specialTracker.AddChar(ch);
			}
			specialTracker.FinishComment(new Point(col, line));
			// Reached EOF before end of multiline comment.
			errors.Error(line, col, String.Format("Reached EOF before the end of a multiline comment"));
		}
		
		/// <summary>
		/// Skips to the end of the current code block.
		/// For this, the lexer must have read the next token AFTER the token opening the
		/// block (so that Lexer.Token is the block-opening token, not Lexer.LookAhead).
		/// After the call, Lexer.LookAhead will be the block-closing token.
		/// </summary>
		public override void SkipCurrentBlock()
		{
			int braceCount = 0;
			Token t;
			while ((t = LookAhead).kind != Tokens.EOF) {
				if (t.kind == Tokens.OpenCurlyBrace) {
					++braceCount;
				} else if (t.kind == Tokens.CloseCurlyBrace) {
					if (--braceCount < 0)
						return;
				}
				NextToken();
			}
		}
	}
}
