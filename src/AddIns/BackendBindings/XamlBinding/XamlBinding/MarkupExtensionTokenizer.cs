// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision: 3494 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.Text;

namespace ICSharpCode.XamlBinding
{
	/// <summary>
	/// Tokenizer for markup extension attributes.
	/// [MS-XAML 6.6.7.1]
	/// </summary>
	public sealed class MarkupExtensionTokenizer
	{
		private MarkupExtensionTokenizer() {}
		
		string text;
		int pos;
		List<MarkupExtensionToken> tokens = new List<MarkupExtensionToken>();
		
		public static List<MarkupExtensionToken> Tokenize(string text)
		{
			MarkupExtensionTokenizer t = new MarkupExtensionTokenizer();
			t.text = text;
			t.Parse();
			return t.tokens;
		}
		
		void AddToken(MarkupExtensionTokenKind kind, string val)
		{
			tokens.Add(new MarkupExtensionToken(kind, val));
		}
		
		void Parse()
		{
			AddToken(MarkupExtensionTokenKind.OpenBrace, "{");
			Expect('{');
			ConsumeWhitespace();
			CheckNotEOF();
			
			StringBuilder b = new StringBuilder();
			while (pos < text.Length && !char.IsWhiteSpace(text, pos) && text[pos] != '}')
				b.Append(text[pos++]);
			AddToken(MarkupExtensionTokenKind.TypeName, b.ToString());
			
			ConsumeWhitespace();
			while (pos < text.Length) {
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
				ConsumeWhitespace();
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
				while (true) {
					CheckNotEOF();
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
			CheckNotEOF();
			string valueText = b.ToString();
			if (text[pos] == '=') {
				AddToken(MarkupExtensionTokenKind.Membername, valueText.Trim());
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
