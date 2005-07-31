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

namespace ICSharpCode.NRefactory.Parser.VB
{
	internal class Lexer : AbstractLexer
	{
		bool lineEnd = false;
		
		public Lexer(TextReader reader) : base(reader)
		{
		}
		
		public override Token NextToken()
		{
			if (curToken == null) { // first call of NextToken()
				curToken = Next();
				specialTracker.InformToken(curToken.kind);
//				Console.WriteLine("Tok:" + Tokens.GetTokenString(curToken.kind) + " --- " + curToken.val);
				return curToken;
			}
			
			lastToken = curToken;
			
			if (curToken.next == null) {
				curToken.next = Next();
				specialTracker.InformToken(curToken.next.kind);
			}
			
			curToken = curToken.next;
			
			if (curToken.kind == Tokens.EOF && !(lastToken.kind == Tokens.EOL)) { // be sure that before EOF there is an EOL token
				curToken = new Token(Tokens.EOL, curToken.col, curToken.line, "\n");
				specialTracker.InformToken(curToken.kind);
				curToken.next = new Token(Tokens.EOF, curToken.col, curToken.line, "\n");
				specialTracker.InformToken(curToken.next.kind);
			}
//			Console.WriteLine("Tok:" + Tokens.GetTokenString(curToken.kind) + " --- " + curToken.val);
			return curToken;
		}
		
		protected override Token Next()
		{
			int nextChar;
			while ((nextChar = reader.Read()) != -1) {
				char ch = (char)nextChar;
				
				++col;
				if (Char.IsWhiteSpace(ch)) {
					if (ch == '\n') {
						int x = col - 1;
						int y = line;
						++line;
						col = 1;
						if (reader.Peek() == '\r') {
							reader.Read();
							if (!lineEnd) {
								lineEnd = true;
								return new Token(Tokens.EOL, x -1 , y, "\n\r");
							}
						}
						if (!lineEnd) {
							lineEnd = true;
							return new Token(Tokens.EOL, x, y, "\n");
						}
					}
					continue;

				}
				if (ch == '_') {
					if (reader.Peek() == -1) {
						errors.Error(line, col, String.Format("No EOF expected after _"));
						return new Token(Tokens.EOF);
					}
					++col;
					if (!Char.IsWhiteSpace((char)reader.Peek())) {
						--col;
						int x = col;
						int y = line;
						string s = ReadIdent('_');
						lineEnd = false;

						return new Token(Tokens.Identifier, x, y, s);
					}
					ch = (char)reader.Read();
					
					while (Char.IsWhiteSpace(ch)) {
						if (ch == '\n') {
							++line;
							col = 1;
							break;
						}
						if (reader.Peek() != -1) {
							ch = (char)reader.Read();
							++col;
						}
					}
					if (ch != '\n') {
						errors.Error(line, col, String.Format("Return expected"));
					}
					continue;
				}
				
				if (ch == '#') {
					while (Char.IsWhiteSpace((char)reader.Peek())) {
						++col;
						reader.Read();
					}
					if (Char.IsDigit((char)reader.Peek())) {
						int x = col;
						int y = line;
						string s = ReadDate();
						DateTime time = new DateTime(1, 1, 1, 0, 0, 0);
						try {
							time = DateTime.Parse(s, System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.NoCurrentDateDefault);
						} catch (Exception e) {
							errors.Error(line, col, String.Format("Invalid date time {0}", e));
						}
						return new Token(Tokens.LiteralDate, x, y, s, time);
					} else {
						ReadPreprocessorDirective();
						continue;
					}
				}
				
				if (ch == '[') { // Identifier
					lineEnd = false;
					if (reader.Peek() == -1) {
						errors.Error(line, col, String.Format("Identifier expected"));
					}
					ch = (char)reader.Read();
					++col;
					if (ch == ']' || Char.IsWhiteSpace(ch)) {
						errors.Error(line, col, String.Format("Identifier expected"));
					}
					int x = col - 1;
					int y = line;
					string s = ReadIdent(ch);
					if (reader.Peek() == -1) {
						errors.Error(line, col, String.Format("']' expected"));
					}
					ch = (char)reader.Read();
					++col;
					if (!(ch == ']')) {
						errors.Error(line, col, String.Format("']' expected"));
					}
					return new Token(Tokens.Identifier, x, y, s);
				}
				if (Char.IsLetter(ch)) {
					int x = col - 1;
					int y = line;
					string s = ReadIdent(ch);
					int keyWordToken = Keywords.GetToken(s);
					if (keyWordToken >= 0) {
						lineEnd = false;
						return new Token(keyWordToken, x, y, s);
					}
					
					// handle 'REM' comments
					if (s.ToUpper() == "REM") {
						ReadComment();
						if (!lineEnd) {
							lineEnd = true;
							return new Token(Tokens.EOL, x, y, "\n");
						}
						continue;
					}
					
					lineEnd = false;
					return new Token(Tokens.Identifier, x, y, s);
					
				}
				if (Char.IsDigit(ch)) {
					lineEnd = false;
					return ReadDigit(ch, col);
				}
				if (ch == '&') {
					lineEnd = false;
					if (reader.Peek() == -1) {
						return ReadOperator('&');
					}
					ch = (char)reader.Peek();
					++col;
					if (Char.ToUpper(ch) == 'H' || Char.ToUpper(ch) == 'O') {
						--col;
						return ReadDigit('&', col);
					}
					return ReadOperator('&');
				}
				if (ch == '\'' || ch == '\u2018' || ch == '\u2019') {
					int x = col - 1;
					int y = line;
					ReadComment();
					if (!lineEnd) {
						lineEnd = true;
						return new Token(Tokens.EOL, x, y, "\n");
					}
					continue;
				}
				if (ch == '"') {
					lineEnd = false;
					int x = col - 1;
					int y = line;
					string s = ReadString();
					if (reader.Peek() != -1 && (reader.Peek() == 'C' || reader.Peek() == 'c')) {
						reader.Read();
						++col;
						if (s.Length != 1) {
							errors.Error(line, col, String.Format("Chars can only have Length 1 "));
						}
						return new Token(Tokens.LiteralCharacter, x, y, '"' + s  + "\"C", s[0]);
					}
					return new Token(Tokens.LiteralString, x, y, '"' + s + '"', s);
				}
				Token token = ReadOperator(ch);
				if (token != null) {
					lineEnd = false;
					return token;
				}
				errors.Error(line, col, String.Format("Unknown char({0}) which can't be read", ch));
			}
			
			return new Token(Tokens.EOF);
		}
		
		string ReadIdent(char ch)
		{
			sb.Length = 0;
			sb.Append(ch);
			int peek;
			while ((peek = reader.Peek()) != -1 && (Char.IsLetterOrDigit(ch = (char)peek) || ch == '_')) {
				reader.Read();
				++col;
				sb.Append(ch.ToString());
			}
			++col;
			if (peek == -1) {
				--col;
				return sb.ToString();
			}
			
			--col;
			if (peek != -1 && "%&@!#$".IndexOf((char)peek) != -1) {
				reader.Read();
				++col;
			}
			return sb.ToString();
		}
		
		Token ReadDigit(char ch, int x)
		{
			sb.Length = 0;
			sb.Append(ch);
			
			int y = line;
			string digit = "";
			if (ch != '&') {
				digit += ch;
			}
			
			bool ishex      = false;
			bool isokt      = false;
			bool issingle   = false;
			bool isdouble   = false;
			bool isdecimal  = false;
			
			if (reader.Peek() == -1) {
				if (ch == '&') {
					errors.Error(line, col, String.Format("digit expected"));
				}
				return new Token(Tokens.LiteralInteger, x, y, sb.ToString() ,ch - '0');
			}
			if (ch == '.') {
				if (Char.IsDigit((char)reader.Peek())) {
					isdouble = true; // double is default
					if (ishex || isokt) {
						errors.Error(line, col, String.Format("No hexadecimal or oktadecimal floating point values allowed"));
					}
					++col;
					while (reader.Peek() != -1 && Char.IsDigit((char)reader.Peek())){ // read decimal digits beyond the dot
						digit += (char)reader.Read();
						++col;
					}
				}
			} else if (ch == '&' && Char.ToUpper((char)reader.Peek()) == 'H') {
				const string hex = "0123456789ABCDEF";
				sb.Append((char)reader.Read()); // skip 'H'
				++col;
				while (reader.Peek() != -1 && hex.IndexOf(Char.ToUpper((char)reader.Peek())) != -1) {
					ch = (char)reader.Read();
					sb.Append(ch);
					digit += Char.ToUpper(ch);
					++col;
				}
				ishex = true;
			} else if (reader.Peek() != -1 && ch == '&' && Char.ToUpper((char)reader.Peek()) == 'O') {
				const string okt = "01234567";
				sb.Append((char)reader.Read()); // skip 'O'
				++col;
				while (reader.Peek() != -1 && okt.IndexOf(Char.ToUpper((char)reader.Peek())) != -1) {
					ch = (char)reader.Read();
					sb.Append(ch);
					digit += Char.ToUpper(ch);
					++col;
				}
				isokt = true;
			} else {
				while (reader.Peek() != -1 && Char.IsDigit((char)reader.Peek())) {
					ch = (char)reader.Read();;
					digit += ch;
					sb.Append(ch);
					++col;
				}
			}
			
			if (reader.Peek() != -1 && ("%&SILU".IndexOf(Char.ToUpper((char)reader.Peek())) != -1 || ishex || isokt)) {
				ch = (char)reader.Peek();
				sb.Append(ch);
				ch = Char.ToUpper(ch);
				bool unsigned = ch == 'U';
				if (unsigned) {
					ch = (char)reader.Peek();
					sb.Append(ch);
					ch = Char.ToUpper(ch);
					if (ch != 'I' && ch != 'L' && ch != 'S') {
						errors.Error(line, col, "Invalid type character: U" + ch);
					}
				}
				++col;
				if (isokt) {
					reader.Read();
					ulong number = 0L;
					for (int i = 0; i < digit.Length; ++i) {
						number = number * 8 + digit[i] - '0';
					}
					if (ch == 'S') {
						if (unsigned)
							return new Token(Tokens.LiteralInteger, x, y, sb.ToString(), (ushort)number);
						else
							return new Token(Tokens.LiteralInteger, x, y, sb.ToString(), (short)number);
					} else if (ch == '%' || ch == 'I') {
						if (unsigned)
							return new Token(Tokens.LiteralInteger, x, y, sb.ToString(), (uint)number);
						else
							return new Token(Tokens.LiteralInteger, x, y, sb.ToString(), (int)number);
					} else if (ch == '&' || ch == 'L') {
						if (unsigned)
							return new Token(Tokens.LiteralInteger, x, y, sb.ToString(), (ulong)number);
						else
							return new Token(Tokens.LiteralInteger, x, y, sb.ToString(), (long)number);
					} else {
						if (number > uint.MaxValue) {
							return new Token(Tokens.LiteralInteger, x, y, sb.ToString(), unchecked((long)number));
						} else {
							return new Token(Tokens.LiteralInteger, x, y, sb.ToString(), unchecked((int)number));
						}
					}
				}
				if (ch == 'S') {
					reader.Read();
					if (unsigned)
						return new Token(Tokens.LiteralInteger, x, y, sb.ToString(), UInt16.Parse(digit, ishex ? NumberStyles.HexNumber : NumberStyles.Number));
					else
						return new Token(Tokens.LiteralInteger, x, y, sb.ToString(), Int16.Parse(digit, ishex ? NumberStyles.HexNumber : NumberStyles.Number));
				} else if (ch == '%' || ch == 'I') {
					reader.Read();
					if (unsigned)
						return new Token(Tokens.LiteralInteger, x, y, sb.ToString(), UInt32.Parse(digit, ishex ? NumberStyles.HexNumber : NumberStyles.Number));
					else
						return new Token(Tokens.LiteralInteger, x, y, sb.ToString(), Int32.Parse(digit, ishex ? NumberStyles.HexNumber : NumberStyles.Number));
				} else if (ch == '&' || ch == 'L') {
					reader.Read();
					if (unsigned)
						return new Token(Tokens.LiteralInteger, x, y, sb.ToString(), UInt64.Parse(digit, ishex ? NumberStyles.HexNumber : NumberStyles.Number));
					else
						return new Token(Tokens.LiteralInteger, x, y, sb.ToString(), Int64.Parse(digit, ishex ? NumberStyles.HexNumber : NumberStyles.Number));
				} else if (ishex) {
					--col;
					ulong number = UInt64.Parse(digit, NumberStyles.HexNumber);
					if (number > uint.MaxValue) {
						return new Token(Tokens.LiteralInteger, x, y, sb.ToString(), unchecked((long)number));
					} else {
						return new Token(Tokens.LiteralInteger, x, y, sb.ToString(), unchecked((int)number));
					}
				}
			}
			Token nextToken = null; // if we accedently read a 'dot'
			if (!isdouble && reader.Peek() == '.') { // read floating point number
				reader.Read();
				if (reader.Peek() != -1 && Char.IsDigit((char)reader.Peek())) {
					isdouble = true; // double is default
					if (ishex || isokt) {
						errors.Error(line, col, String.Format("No hexadecimal or oktadecimal floating point values allowed"));
					}
					digit += '.';
					++col;
					while (reader.Peek() != -1 && Char.IsDigit((char)reader.Peek())){ // read decimal digits beyond the dot
						digit += (char)reader.Read();
						++col;
					}
				} else {
					nextToken = new Token(Tokens.Dot, x, y);
				}
			}
			
			if (reader.Peek() != -1 && Char.ToUpper((char)reader.Peek()) == 'E') { // read exponent
				isdouble = true;
				digit +=  (char)reader.Read();
				++col;
				if (reader.Peek() != -1 && (reader.Peek() == '-' || reader.Peek() == '+')) {
					digit += (char)reader.Read();
					++col;
				}
				while (reader.Peek() != -1 && Char.IsDigit((char)reader.Peek())) { // read exponent value
					digit += (char)reader.Read();
					++col;
				}
			}
			
			if (reader.Peek() != -1) {
				switch (char.ToUpper((char)reader.Peek())) {
					case 'R':
					case '#':
						reader.Read();
						++col;
						isdouble = true;
						break;
					case 'D':
					case '@':
						reader.Read();
						++col;
						isdecimal = true;
						break;
					case 'F':
					case '!':
						reader.Read();
						++col;
						issingle = true;
						break;
				}
			}
			
			try {
				if (issingle) {
					return new Token(Tokens.LiteralSingle, x, y, sb.ToString(), Single.Parse(digit, CultureInfo.InvariantCulture));
				}
				if (isdecimal) {
					return new Token(Tokens.LiteralDecimal, x, y, sb.ToString(), Decimal.Parse(digit, NumberStyles.Currency | NumberStyles.AllowExponent, CultureInfo.InvariantCulture));
				}
				if (isdouble) {
					return new Token(Tokens.LiteralDouble, x, y, sb.ToString(), Double.Parse(digit, CultureInfo.InvariantCulture));
				}
			} catch (FormatException) {
				errors.Error(line, col, String.Format("{0} is not a parseable number", digit));
				if (issingle)
					return new Token(Tokens.LiteralSingle, x, y, sb.ToString(), 0f);
				if (isdecimal)
					return new Token(Tokens.LiteralDecimal, x, y, sb.ToString(), 0m);
				if (isdouble)
					return new Token(Tokens.LiteralDouble, x, y, sb.ToString(), 0.0);
			}
			Token token;
			try {
				token = new Token(Tokens.LiteralInteger, x, y, sb.ToString(), Int32.Parse(digit, ishex ? NumberStyles.HexNumber : NumberStyles.Number));
			} catch (Exception) {
				try {
					token = new Token(Tokens.LiteralInteger, x, y, sb.ToString(), Int64.Parse(digit, ishex ? NumberStyles.HexNumber : NumberStyles.Number));
				} catch (FormatException) {
					errors.Error(line, col, String.Format("{0} is not a parseable number", digit));
					// fallback, when nothing helps :)
					token = new Token(Tokens.LiteralInteger, x, y, sb.ToString(), 0);
				} catch (OverflowException) {
					errors.Error(line, col, String.Format("{0} is too long for a integer literal", digit));
					// fallback, when nothing helps :)
					token = new Token(Tokens.LiteralInteger, x, y, sb.ToString(), 0);
				}
			}
			token.next = nextToken;
			return token;
		}
		
		void ReadPreprocessorDirective()
		{
			Point start = new Point(col - 1, line);
			string directive = ReadIdent('#');
			string argument  = ReadToEOL();
			this.specialTracker.AddPreProcessingDirective(directive, argument.Trim(), start, new Point(start.X + directive.Length + argument.Length, start.Y));
		}
		
		string ReadDate()
		{
			char ch = '\0';
			sb.Length = 0;
			int nextChar;
			while ((nextChar = reader.Read()) != -1) {
				ch = (char)nextChar;
				++col;
				if (ch == '#') {
					break;
				} else if (ch == '\n') {
					errors.Error(line, col, String.Format("No return allowed inside Date literal"));
				} else {
					sb.Append(ch);
				}
			}
			if (ch != '#') {
				errors.Error(line, col, String.Format("End of File reached before Date literal terminated"));
			}
			return sb.ToString();
		}
		
		string ReadString()
		{
			char ch = '\0';
			sb.Length = 0;
			int nextChar;
			while ((nextChar = reader.Read()) != -1) {
				ch = (char)nextChar;
				++col;
				if (ch == '"') {
					if (reader.Peek() != -1 && reader.Peek() == '"') {
						sb.Append('"');
						reader.Read();
						++col;
					} else {
						break;
					}
				} else if (ch == '\n') {
					errors.Error(line, col, String.Format("No return allowed inside String literal"));
				} else {
					sb.Append(ch);
				}
			}
			if (ch != '"') {
				errors.Error(line, col, String.Format("End of File reached before String terminated "));
			}
			return sb.ToString();
		}
		
		void ReadComment()
		{
			Point startPos = new Point(col, line);
			sb.Length = 0;
			StringBuilder curWord = specialCommentHash != null ? new StringBuilder() : null;
			int missingApostrophes = 2; // no. of ' missing until it is a documentation comment
			int x = col;
			int y = line;
			int nextChar;
			while ((nextChar = reader.Read()) != -1) {
				char ch = (char)nextChar;
				++col;
				
				if (HandleLineEnd(ch)) {
					break;
				}
				
				sb.Append(ch);
				
				if (missingApostrophes > 0) {
					if (ch == '\'' || ch == '\u2018' || ch == '\u2019') {
						if (--missingApostrophes == 0) {
							specialTracker.StartComment(CommentType.Documentation, startPos);
							sb.Length = 0;
						}
					} else {
						specialTracker.StartComment(CommentType.SingleLine, startPos);
						missingApostrophes = 0;
					}
				}
				
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
			if (missingApostrophes > 0) {
				specialTracker.StartComment(CommentType.SingleLine, startPos);
			}
			specialTracker.AddString(sb.ToString());
			specialTracker.FinishComment(new Point(col, line));
		}
		
		Token ReadOperator(char ch)
		{
			int x = col;
			int y = line;
			switch(ch) {
				case '+':
					switch (reader.Peek()) {
						case '=':
							reader.Read();
							++col;
							return new Token(Tokens.PlusAssign, x, y);
						default:
							break;
					}
					return new Token(Tokens.Plus, x, y);
				case '-':
					switch (reader.Peek()) {
						case '=':
							reader.Read();
							++col;
							return new Token(Tokens.MinusAssign, x, y);
						default:
							break;
					}
					return new Token(Tokens.Minus, x, y);
				case '*':
					switch (reader.Peek()) {
						case '=':
							reader.Read();
							++col;
							return new Token(Tokens.TimesAssign, x, y);
						default:
							break;
					}
					return new Token(Tokens.Times, x, y, "*");
				case '/':
					switch (reader.Peek()) {
						case '=':
							reader.Read();
							++col;
							return new Token(Tokens.DivAssign, x, y);
						default:
							break;
					}
					return new Token(Tokens.Div, x, y);
				case '\\':
					switch (reader.Peek()) {
						case '=':
							reader.Read();
							++col;
							return new Token(Tokens.DivIntegerAssign, x, y);
						default:
							break;
					}
					return new Token(Tokens.DivInteger, x, y);
				case '&':
					switch (reader.Peek()) {
						case '=':
							reader.Read();
							++col;
							return new Token(Tokens.ConcatStringAssign, x, y);
						default:
							break;
					}
					return new Token(Tokens.ConcatString, x, y);
				case '^':
					switch (reader.Peek()) {
						case '=':
							reader.Read();
							++col;
							return new Token(Tokens.PowerAssign, x, y);
						default:
							break;
					}
					return new Token(Tokens.Power, x, y);
				case ':':
					return new Token(Tokens.Colon, x, y);
				case '=':
					return new Token(Tokens.Assign, x, y);
				case '<':
					switch (reader.Peek()) {
						case '=':
							reader.Read();
							++col;
							return new Token(Tokens.LessEqual, x, y);
						case '>':
							reader.Read();
							++col;
							return new Token(Tokens.NotEqual, x, y);
						case '<':
							reader.Read();
							switch (reader.Peek()) {
								case '=':
									reader.Read();
									col += 2;
									return new Token(Tokens.ShiftLeftAssign, x, y);
								default:
									++col;
									break;
							}
							return new Token(Tokens.ShiftLeft, x, y);
					}
					return new Token(Tokens.LessThan, x, y);
				case '>':
					switch (reader.Peek()) {
						case '=':
							reader.Read();
							++col;
							return new Token(Tokens.GreaterEqual, x, y);
						case '>':
							reader.Read();
							if (reader.Peek() != -1) {
								switch (reader.Peek()) {
									case '=':
										reader.Read();
										col += 2;
										return new Token(Tokens.ShiftRightAssign, x, y);
									default:
										++col;
										break;
								}
							}
							return new Token(Tokens.ShiftRight, x, y);
					}
					return new Token(Tokens.GreaterThan, x, y);
				case ',':
					return new Token(Tokens.Comma, x, y);
				case '.':
					// Prevent OverflowException when Peek returns -1
					int tmp = reader.Peek();
					if (tmp > 0 && Char.IsDigit((char)tmp)) {
						--col;
						return ReadDigit('.', col);
					}
					return new Token(Tokens.Dot, x, y);
				case '(':
					return new Token(Tokens.OpenParenthesis, x, y);
				case ')':
					return new Token(Tokens.CloseParenthesis, x, y);
				case '{':
					return new Token(Tokens.OpenCurlyBrace, x, y);
				case '}':
					return new Token(Tokens.CloseCurlyBrace, x, y);
//				case '[':
//					return new Token(Tokens.OpenSquareBracket, x, y);
//				case ']':
//					return new Token(Tokens.CloseSquareBracket, x, y);
			}
			return null;
		}
	}
}
