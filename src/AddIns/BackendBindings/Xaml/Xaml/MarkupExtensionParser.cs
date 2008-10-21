using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICSharpCode.Xaml
{
	class MarkupExtensionParser
	{
		public static MarkupExtensionAst Parse(string text)
		{
			var tokens = MarkupExtensionTokenizer.Tokenize(text);

			if (tokens.Count < 3 ||
				tokens[0].Kind != MarkupExtensionTokenKind.OpenBrace ||
				tokens[1].Kind != MarkupExtensionTokenKind.TypeName ||
				tokens[tokens.Count - 1].Kind != MarkupExtensionTokenKind.CloseBrace) {
				throw new XamlException("Invalid markup extension");
			}

			var result = new MarkupExtensionAst();
			result.TypeName = tokens[1].Value;

			for (int i = 2; i < tokens.Count - 1; i++) {
				if (tokens[i].Kind == MarkupExtensionTokenKind.String) {
					result.PositionalArgs.Add(tokens[i].Value);
				}
				else if (tokens[i].Kind == MarkupExtensionTokenKind.Membername) {
					if (tokens[i + 1].Kind != MarkupExtensionTokenKind.Equals ||
						tokens[i + 2].Kind != MarkupExtensionTokenKind.String) {
						throw new XamlException("Invalid markup extension");
					}
					var namedArg = new KeyValuePair<string, string>(tokens[i].Value, tokens[i + 2].Value);
					result.NamedArgs.Add(namedArg);
					i += 2;
				}
			}
			return result;
		}
	}

	class MarkupExtensionAst
	{
		public string TypeName;
		public List<string> PositionalArgs = new List<string>();
		public List<KeyValuePair<string, string>> NamedArgs = new List<KeyValuePair<string, string>>();
	}

	class MarkupExtensionTokenizer
	{
		private MarkupExtensionTokenizer() { }

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
				while (!(text[pos] == quote && text[pos - 1] != '\\')) {
					char c = text[pos++];
					if (c != '\\')
						b.Append(c);
					CheckNotEOF();
				}
				pos++; // consume closing quote
				ConsumeWhitespace();
			}
			else {
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
			stop: ;
			}
			CheckNotEOF();
			string valueText = b.ToString();
			if (text[pos] == '=') {
				AddToken(MarkupExtensionTokenKind.Membername, valueText.Trim());
			}
			else {
				AddToken(MarkupExtensionTokenKind.String, valueText);
			}
		}

		void Expect(char c)
		{
			CheckNotEOF();
			if (text[pos] != c)
				throw new XamlException("Expected '" + c + "'");
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
				throw new XamlException("Unexpected end of markup extension");
		}
	}

	class MarkupExtensionToken
	{
		public MarkupExtensionToken(MarkupExtensionTokenKind kind, string value)
		{
			this.Kind = kind;
			this.Value = value;
		}

		public readonly MarkupExtensionTokenKind Kind;
		public readonly string Value;

		public override string ToString()
		{
			return "[" + Kind + " " + Value + "]";
		}
	}

	enum MarkupExtensionTokenKind
	{
		OpenBrace,
		CloseBrace,
		Equals,
		Comma,
		TypeName,
		Membername,
		String
	}
}
