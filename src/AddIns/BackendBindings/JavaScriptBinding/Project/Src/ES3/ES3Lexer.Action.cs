using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using Antlr.Runtime;

namespace Xebic.Parsers.ES3
{
	using IToken = Antlr.Runtime.IToken;

	/// <summary>
	/// This partial class is complementary to the lexer generated with ANTLR from the JavaScript.g grammar.
	/// It implements the actions used in the lexer.
	/// </summary>
	public partial class ES3Lexer
	{
		/// <summary>
		/// Containts the last on channel token.
		/// </summary>
		protected IToken last;

		/// <summary>
		/// Indicates whether regular expression (yields true) or division expression recognition (false) in the lexer is enabled.
		/// These are mutual exclusive and the decision which is active in the lexer is based on the previous on channel token.
		/// When the previous token can be identified as a possible left operand for a division this results in false, otherwise true.
		/// </summary>
		private bool AreRegularExpressionsEnabled
		{
			get
			{
				if (last == null)
				{
					return true;
				}

				switch (last.Type)
				{
					// identifier
					case Identifier:
					// literals
					case NULL:
					case TRUE:
					case FALSE:
					case THIS:
					case OctalIntegerLiteral:
					case DecimalLiteral:
					case HexIntegerLiteral:
					case StringLiteral:
					// member access ending 
					case RBRACK:
					// function call or nested expression ending
					case RPAREN:
						return false;

					// otherwise OK
					default:
						return true;
				}
			}
		}

		/// <summary>
		/// Consumes an unicode identifier after validating that the first character can be the starting character.
		/// This method is called by the lexer logic as fallback alternative when a character can not be considered as start of an identifier in the ASCII range.
		/// See the Identfier lexer rule for more details.
		/// </summary>
		private void ConsumeIdentifierUnicodeStart()
		{
			int ch = (char)input.LA(1);
			if (IsIdentifierStartUnicode(ch))
			{
				MatchAny();
				do
				{
					ch = (char)input.LA(1);
					if (ch == '$' || (ch >= '0' && ch <= '9') || (ch >= 'A' && ch <= 'Z') || ch == '\\' || ch == '_' || (ch >= 'a' && ch <= 'z') || IsIdentifierPartUnicode(ch))
					{
						mIdentifierPart();
					}
					else
					{
						return;
					}
				}
				while (true);
			}
			else
			{
				throw new NoViableAltException();
			}
		}

		/// <summary>
		/// Indicates whether a given character can be a part of an unicode identifier.
		/// This method doesn't consider ASCII characters that can be a part of an identifier, that is left to the mIdentifierPart method.
		/// The latter method will call this method to check other characters in the unicode range after evaluating those in the ASCII range.
		/// </summary>
		/// <param name="ch">The character to check.</param>
		/// <returns>True when the character matches, false otherwise.</returns>
		public static bool IsIdentifierPartUnicode(int ch)
		{
			switch (Char.GetUnicodeCategory((char)ch))
			{
				// UnicodeLetter
				case UnicodeCategory.UppercaseLetter: // Lu
				case UnicodeCategory.LowercaseLetter: // Ll
				case UnicodeCategory.TitlecaseLetter: // Lt
				case UnicodeCategory.ModifierLetter: // Lm
				case UnicodeCategory.OtherLetter: // Lo
				case UnicodeCategory.LetterNumber: // Nl
				// UnicodeCombiningMark
				case UnicodeCategory.NonSpacingMark: // Mn
				case UnicodeCategory.SpacingCombiningMark: // Mc
				// UnicodeDigit
				case UnicodeCategory.DecimalDigitNumber: // Nd
				// UnicodeConnectorPuntuation
				case UnicodeCategory.ConnectorPunctuation: // Pc
					return true;

				// Not matching
				default:
					return false;
			}
		}

		/// <summary>
		/// Indicates whether a given character can be the first character of an unicode identifier.
		/// This method doesn't consider ASCII characters as it is used in a fallback scenario after the ASCII range is evaluated.
		/// </summary>
		/// <param name="ch">The character to check.</param>
		/// <returns>True when the character matches, false otherwise.</returns>
		public static bool IsIdentifierStartUnicode(int ch)
		{
			switch (Char.GetUnicodeCategory((char)ch))
			{
				// UnicodeLetter
				case UnicodeCategory.UppercaseLetter: // Lu
				case UnicodeCategory.LowercaseLetter: // Ll
				case UnicodeCategory.TitlecaseLetter: // Lt
				case UnicodeCategory.ModifierLetter: // Lm
				case UnicodeCategory.OtherLetter: // Lo
				case UnicodeCategory.LetterNumber: // Nl
					return true;

				// Not matching
				default:
					return false;
			}
		}

		/// <summary>
		/// Override of base to track previous on channel token.
		/// This token is needed as input to decide whether regular expression or division expression recognition is enabled.
		/// </summary>
		public override IToken NextToken()
		{
			IToken result = base.NextToken();
			if (result.Channel == DefaultTokenChannel)
			{
				last = result;
			}
			return result;
		}
	}
}