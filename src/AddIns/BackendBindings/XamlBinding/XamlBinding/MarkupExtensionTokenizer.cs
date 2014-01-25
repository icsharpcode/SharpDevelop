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
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ICSharpCode.XamlBinding
{
	/// <summary>
	/// Tokenizer for markup extension attributes.
	/// [MS-XAML 6.6.7.1]
	/// </summary>
	public sealed class MarkupExtensionTokenizer
	{
		public MarkupExtensionTokenizer(string text)
		{
			if (text == null)
				throw new ArgumentNullException("text");
			this.text = text;
		}
		
		string text;
		int pos;
		int startPos;
		
		Queue<MarkupExtensionToken> tokens = new Queue<MarkupExtensionToken>();
		
		/// <summary>
		/// Retrieves the next token.
		/// </summary>
		/// <exception cref="MarkupExtensionParseException">A parse error occurred.</exception>
		public MarkupExtensionToken NextToken()
		{
			if (pos == 0)
				ParseBeginning();
			if (tokens.Count == 0) {
				// produce new tokens on demand
				ParseStep();
				// a parse step must produce tokens unless we're at EOF
				Debug.Assert(tokens.Count > 0 || pos == text.Length);
			}
			if (tokens.Count > 0)
				return tokens.Dequeue();
			else
				return new MarkupExtensionToken(MarkupExtensionTokenKind.EndOfFile, "") { StartOffset = pos, EndOffset = pos };
		}
		
		void AddToken(MarkupExtensionTokenKind kind, string val)
		{
			tokens.Enqueue(new MarkupExtensionToken(kind, val) { StartOffset = startPos, EndOffset = startPos + val.Length });
		}
		
		void ParseBeginning()
		{
			AddToken(MarkupExtensionTokenKind.OpenBrace, "{");
			Expect('{');
			ConsumeWhitespace();
			CheckNotEOF();
			
			StringBuilder b = new StringBuilder();
			while (pos < text.Length && !char.IsWhiteSpace(text, pos) && text[pos] != '}')
				b.Append(text[pos++]);
			AddToken(MarkupExtensionTokenKind.TypeName, b.ToString());
		}
		
		void ParseStep()
		{
			ConsumeWhitespace();
			if (pos < text.Length) {
				startPos = pos;
				switch (text[pos]) {
					case '}':
						AddToken(MarkupExtensionTokenKind.CloseBrace, "}");
						pos++;
						break;
					case '=':
						AddToken(MarkupExtensionTokenKind.Equals, "=");
						pos++;
						break;
					case ',':
						AddToken(MarkupExtensionTokenKind.Comma, ",");
						pos++;
						break;
					default:
						MembernameOrString();
						break;
				}
			}
		}
		
		void MembernameOrString()
		{
			StringBuilder b = new StringBuilder();
			if (text[pos] == '"' || text[pos] == '\'') {
				char quote = text[pos++];
				CheckNotEOF();
				while (!(text[pos] == quote && text[pos-1] != '\\')) {
					char c = text[pos++];
					if (c != '\\')
						b.Append(c);
					CheckNotEOF();
				}
				pos++; // consume closing quote
				ConsumeWhitespace();
			} else {
				int braceTotal = 0;
				while (pos < text.Length) {
					switch (text[pos]) {
						case '\\':
							pos++;
							CheckNotEOF();
							b.Append(text[pos++]);
							break;
						case '{':
							b.Append(text[pos++]);
							braceTotal++;
							break;
						case '}':
							if (braceTotal == 0) goto stop;
							b.Append(text[pos++]);
							braceTotal--;
							break;
						case ',':
						case '=':
							if (braceTotal == 0) goto stop;
							b.Append(text[pos++]);
							break;
						default:
							b.Append(text[pos++]);
							break;
					}
				}
				stop:;
			}
			string valueText = b.ToString();
			if (pos < text.Length && text[pos] == '=') {
				int splitPos = valueText.LastIndexOf(' ');
				if (splitPos > -1) {
					string valueString = valueText.Substring(0, splitPos).Trim();
					string memberString = valueText.Substring(splitPos).Trim();
					AddToken(MarkupExtensionTokenKind.String, valueString);
					startPos += splitPos;
					AddToken(MarkupExtensionTokenKind.MemberName, memberString);
				} else
					AddToken(MarkupExtensionTokenKind.MemberName, valueText.Trim());
			} else {
				AddToken(MarkupExtensionTokenKind.String, valueText);
			}
		}
		
		void Expect(char c)
		{
			CheckNotEOF();
			if (text[pos] != c)
				throw new MarkupExtensionParseException("Expected '" + c + "'");
			pos++;
		}
		
		void ConsumeWhitespace()
		{
			while (pos < text.Length && char.IsWhiteSpace(text, pos))
				pos++;
		}
		
		void CheckNotEOF()
		{
			if (pos >= text.Length)
				throw new MarkupExtensionParseException("Unexpected end of markup extension");
		}
	}
}
