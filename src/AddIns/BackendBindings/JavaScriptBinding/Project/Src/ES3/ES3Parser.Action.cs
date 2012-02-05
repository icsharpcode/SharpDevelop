using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using Antlr.Runtime;
using Antlr.Runtime.Tree;

namespace Xebic.Parsers.ES3
{
	using IToken = Antlr.Runtime.IToken;

	/// <summary>
	/// This partial class is complementary to the parser generated with ANTLR from the JavaScript.g grammar.
	/// It implements the actions used in the parser.
	/// </summary>
	public partial class ES3Parser
	{
		/// <summary>
		/// Is a RuleReturnScope node candidate for the left-hand-side of an assignment expression?
		/// </summary>
		/// <param name="lhs">The RuleReturnScope node</param>
		/// <param name="cached">The cached result of a former call to this method</param>
		/// <returns>True if so, false otherwise</returns>
		public bool IsLeftHandSideAssign(IAstRuleReturnScope lhs, ref bool? cached)
		{
			if (cached.HasValue)
			{
				return cached.Value;
			}

			bool result;
			if (IsLeftHandSideExpression(lhs))
			{
				switch (input.LA(1))
				{
					case ASSIGN:
					case MULASS:
					case DIVASS:
					case MODASS:
					case ADDASS:
					case SUBASS:
					case SHLASS:
					case SHRASS:
					case SHUASS:
					case ANDASS:
					case XORASS:
					case ORASS:
						result = true;
						break;

					default:
						result = false;
						break;
				}
			}
			else
			{
				result = false;
			}

			cached = result;
			return result;
		}

		/// <summary>
		/// Is a RuleReturnScope node candidate a left-hand-side expression?
		/// </summary>
		/// <param name="lhs">The RuleReturnScope node</param>
		/// <returns>True if so, false otherwise</returns>
		private bool IsLeftHandSideExpression(IAstRuleReturnScope lhs)
		{
			if (lhs.Tree == null) // e.g. during backtracking
			{
				return true;
			}
			else
			{
				switch (((ITree)lhs.Tree).Type)
				{
					// primaryExpression
					case THIS:
					case Identifier:
					case NULL:
					case TRUE:
					case FALSE:
					case DecimalLiteral:
					case OctalIntegerLiteral:
					case HexIntegerLiteral:
					case StringLiteral:
					case RegularExpressionLiteral:
					case ARRAY:
					case OBJECT:
					case PAREXPR:
					// functionExpression
					case FUNCTION:
					// newExpression
					case NEW:
					// leftHandSideExpression
					case CALL:
					case BYFIELD:
					case BYINDEX:
						return true;

					default:
						return false;
				}
			}
		}

		/// <summary>
		/// Is a RuleReturnScope node candidate for the left-hand-side of an in expression?
		/// </summary>
		/// <param name="lhs">The RuleReturnScope node</param>
		/// <param name="cached">The cached result of a former call to this method</param>
		/// <returns>True if so, false otherwise</returns>
		public bool IsLeftHandSideIn(IAstRuleReturnScope lhs, ref bool? cached)
		{
			if (cached.HasValue)
			{
				return cached.Value;
			}

			bool result = IsLeftHandSideExpression(lhs) && (input.LA(1) == IN);

			cached = result;
			return result;
		}

		/// <summary>
		/// This method handles promotion of an EOL token to on channel in situations where the ECMA 3 specification 
		/// states there should be a semicolon inserted because of an EOL between the current (offending) token
		/// and the previous token.
		/// So an semicolon is not actually inserted but the EOL present is switched from off to on channel. In this
		/// way that EOL gets the notion of an "virtual" semicolon.
		/// As a side effect a given rule's return scope starting point is set to the found EOL and the input stream is repositioned on it.
		/// A multi line comment with an EOL is also promoted.
		/// </summary>
		/// <param name="rule">The invoking rule's return scope</param>
		public void PromoteEOL(ParserRuleReturnScope<IToken> rule)
		{
			// Get current token and its type (the possibly offending token).
			IToken lt = input.LT(1);
			int la = lt.Type;

			// We only need to promote an EOL when the current token is offending (not a SEMIC, EOF, RBRACE or EOL).
			// Promoting an EOL means switching it from off channel to on channel.
			if (!(la == SEMIC || la == EOF || la == RBRACE || la == EOL))
			{
				// Start on the possition before the current token and scan backwards off channel tokens until the previous on channel token.
				for (int ix = lt.TokenIndex - 1; ix > 0; ix--)
				{
					lt = input.Get(ix);
					if (lt.Channel == DefaultTokenChannel)
					{
						// On channel token found: stop scanning.
						break;
					}
					else if (lt.Type == EOL || (lt.Type == MultiLineComment && Regex.IsMatch(lt.Text, "/.*\r\n|\r|\n")))
					{
						// We found our EOL: promote it to on channel, position the input on it and reset the rule start.
						lt.Channel = DefaultTokenChannel;
						input.Seek(lt.TokenIndex);
						if (rule != null)
						{
							rule.Start = lt;
						}
						break;
					}
				}
			}
		}
	}
}