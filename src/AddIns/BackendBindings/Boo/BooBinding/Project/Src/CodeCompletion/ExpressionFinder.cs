// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Text;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;

namespace Grunwald.BooBinding.CodeCompletion
{
	public class ExpressionFinder : IExpressionFinder
	{
		public ExpressionFinder()
		{
		}
		
		public ExpressionFinder(string fileName)
		{
		}
		
		#region RemoveLastPart
		/// <summary>
		/// Removes the last part of the expression.
		/// </summary>
		/// <example>
		/// "arr[i]" => "arr"
		/// "obj.Field" => "obj"
		/// "obj.Method(args,...)" => "obj.Method"
		/// </example>
		public string RemoveLastPart(string expression)
		{
			int state = 0;
			int pos = 0;
			int lastFinishPos = 0;
			int brackets = 0;
			while (state >= 0) {
				state = FindNextCodeCharacter(state, expression, ref pos);
				if (pos >= expression.Length)
					break;
				char c = expression[pos];
				if (c == '[' || c == '(' || c == '{') {
					if (brackets == 0)
						lastFinishPos = pos;
					brackets += 1;
				}
				if (brackets == 0 && c == '.') {
					lastFinishPos = pos;
				}
				if (brackets > 0 && (c == ']' || c == ')' || c == '}')) {
					brackets -= 1;
				}
			}
			return expression.Substring(0, lastFinishPos);
		}
		#endregion
		
		#region Find Expression
		// The expression finder can find an expression in a text
		// inText is the full source code, offset the cursor position
		
		// example: "_var = 'bla'\n_var^\nprint _var"
		// where ^ is the cursor position
		// in that simple case the expression finder should return 'n_var'.
		
		// but also complex expressions like
		// 'filename.Substring(filename.IndexOf("var="))'
		// should be returned if the cursor is after the last ).
		
		// implementation note: the text after offset is irrelevant, so
		// every operation on the string aborts after reaching offset
		
		const string _closingBrackets = "}])";
		const string _openingBrackets = "{[(";
		
		public ExpressionResult FindExpression(string inText, int offset)
		{
			ExpressionResult r = FindExpressionInternal(inText, offset);
			if (string.IsNullOrEmpty(r.Expression))
				r.Expression = null;
			return r;
		}
		
		ExpressionResult FindExpressionInternal(string inText, int offset)
		{
			offset--; // earlier all ExpressionFinder calls had an inexplicable "cursor - 1".
			// The IExpressionFinder API now uses normal cursor offsets, so we need to adjust the offset
			// because Boo ExpressionFinder still uses an implementation that expects old offsets
			
			if (inText == null || offset >= inText.Length)
				return ExpressionResult.Empty;
			// OK, first try a kind of "quick find"
			int i = offset + 1;
			const string forbidden = "\"\'/#)]}";
			const string finish = "([{=+*<,:";
			int start = -1;
			while (i > 0) {
				i -= 1;
				char c = inText[i];
				if (finish.IndexOf(c) >= 0) {
					start = i + 1;
					break;
				}
				if (forbidden.IndexOf(c) >= 0) {
					//LoggingService.Debug("Quickfind failed: got " + c);
					break;
				}
				if (char.IsWhiteSpace(c)) {
					break;
				}
				if (start >= 0) {
					if (CheckString(inText, start, "/#\"\'", "\r\n")) {
						return GetExpression(inText, start, offset + 1);
					}
				}
			}
			
			inText = SimplifyCode(inText, offset);
			if (inText == null) {
				return ExpressionResult.Empty;
			}
			// inText now has no comments or string literals, but the same meaning in
			// terms of the type system
			// Now go back until a finish-character or a whitespace character
			Stack<int> bracketStack = new Stack<int>();
			i = inText.Length;
			while (i > 0) {
				i -= 1;
				char c = inText[i];
				if (bracketStack.Count == 0 && (finish.IndexOf(c) >= 0 || Char.IsWhiteSpace(c))) {
					// SUCCESS!
					return GetExpression(inText, i + 1, inText.Length);
				}
				int bracket = _closingBrackets.IndexOf(c);
				if (bracket >= 0) {
					bracketStack.Push(bracket);
				}
				bracket = _openingBrackets.IndexOf(c);
				if (bracket >= 0) {
					while (bracketStack.Count > 0 && bracketStack.Pop() > bracket);
				}
			}
			return ExpressionResult.Empty;
		}
		
		bool CheckString(string text, int offset, string forbidden, string finish)
		{
			int i = offset;
			while (i > 0) {
				i -= 1;
				char c = text[i];
				if (forbidden.IndexOf(c) >= 0) return false;
				if (finish.IndexOf(c) >= 0) return true;
			}
			return true;
		}
		
		ExpressionResult GetExpression(string inText, int start, int end)
		{
			if (start == end)
				return new ExpressionResult(string.Empty);
			StringBuilder b = new StringBuilder();
			bool wasSpace = true;
			int i = start;
			while (i < end) {
				char c = inText[i];
				if (Char.IsWhiteSpace(c)) {
					if (!wasSpace) b.Append(' ');
					wasSpace = true;
				} else {
					wasSpace = false;
					b.Append(c);
				}
				i += 1;
			}
			ExpressionResult result = new ExpressionResult(b.ToString());
			// Now try to find the context of the expression
			while (--start > 0 && char.IsWhiteSpace(inText, start));
			if (start > 2 && char.IsWhiteSpace(inText, start - 2)
			    && inText[start - 1] == 'a' && inText[start] == 's')
			{
				result.Context = ExpressionContext.Type;
			} else if (start > 6 && char.IsWhiteSpace(inText, start - 6)
			           && inText[start - 5] == 'i' && inText[start - 4] == 'm'
			           && inText[start - 3] == 'p' && inText[start - 2] == 'o'
			           && inText[start - 1] == 'r' && inText[start] == 't')
			{
				result.Context = ExpressionContext.Importable;
			} else {
				bool wasSquareBracket = false;
				int brackets = 0;
				while (start > 0) {
					char c = inText[start];
					if (c == '\n') break;
					if (brackets == 0) {
						if (c == '(' || c == ',')
							break;
						if (!char.IsWhiteSpace(inText, start))
							wasSquareBracket = inText[start] == '[';
					} else {
						if (c == '[' || c == '(')
							brackets -= 1;
					}
					if (c == ')' || c == ']')
						brackets += 1;
					start -= 1;
				}
				if (wasSquareBracket) {
					result.Context = BooAttributeContext.Instance;
				}
			}
			
			return result;
		}
		
		internal class BooAttributeContext : ExpressionContext
		{
			public static BooAttributeContext Instance = new BooAttributeContext();
			
			public override bool ShowEntry(ICompletionEntry o)
			{
				IClass c = o as IClass;
				if (c != null && c.IsAbstract)
					return false;
				if (ExpressionContext.Attribute.ShowEntry(o))
					return true;
				if (c == null)
					return false;
				foreach (IClass baseType in c.ClassInheritanceTree) {
					if (baseType.FullyQualifiedName == "Boo.Lang.Compiler.AbstractAstAttribute")
						return true;
				}
				return false;
			}
		}
		#endregion
		
		#region Find Full Expression
		public ExpressionResult FindFullExpression(string inText, int offset)
		{
			ExpressionResult result = FindExpressionInternal(inText, offset);
			if (result.Expression == null)
				return result;
			StringBuilder b = new StringBuilder(result.Expression);
			//  accepting current identifier
			int i;
			for (i = offset; i < inText.Length; i++) {
				char c = inText[i];
				if (!char.IsLetterOrDigit(c) && c != '_') {
					break;
				}
			}
			i -= 1;
			// accepting brackets/parenthesis
			int state = 0;
			ResetStateMachine();
			Stack<int> bracketStack = new Stack<int>();
			while (state >= 0) {
				state = FindNextCodeCharacter(state, inText, ref i);
				if (state < 0) break;
				char c = (i < inText.Length) ? inText[i] : '\0';
				int bracket = _openingBrackets.IndexOf(c);
				if (bracket >= 0) {
					bracketStack.Push(bracket);
				} else {
					if (bracketStack.Count == 0) {
						b.Append(inText, offset, i - offset);
						if (b.Length == 0)
							result.Expression = null;
						else
							result.Expression = b.ToString();
						return result;
					} else if (c == '\0') {
						// end of document
						break;
					}
				}
				bracket = _closingBrackets.IndexOf(c);
				if (bracket >= 0) {
					while (bracketStack.Count > 0 && bracketStack.Pop() > bracket);
				}
			}
			return ExpressionResult.Empty;
		}
		#endregion
		
		#region State Machine / SimplifyCode
		static readonly int[] inputTable;
		
		static ExpressionFinder()
		{
			inputTable = new int[128];
			for (int i = 0; i < inputTable.Length; i++) {
				inputTable[i] = _elseIndex;
			}
			inputTable[ 34] = 0; // "
			inputTable[ 39] = 1; // '
			inputTable[ 92] = 2; // \
			inputTable[ 10] = 3; // \n
			inputTable[ 13] = 3; // \r
			inputTable[ 36] = 4; // $
			inputTable[123] = 5; // {
			inputTable[125] = 6; // }
			inputTable[ 35] = 7; // #
			inputTable[ 47] = 8; // /
			inputTable[ 42] = 9; // *
		}
		
		const int _elseIndex = 10;
		public const int PossibleRegexStart = 12;
		public const int LineCommentState = 13;
		
		static readonly
			int[][] _stateTable =         { // "    '    \    \n   $    {    }    #    /    *   else
			/* 0: in Code       */ new int[] { 1  , 7  , 0  , 0  , 0  , 0  , 0  , 13 , 12 , 0  , 0  },
			/* 1: after "       */ new int[] { 2  , 6  , 10 , 0  , 8  , 6  , 6  , 6  , 6  , 6  , 6  },
			/* 2: after ""      */ new int[] { 3  , 7  , 0  , 0  , 0  , 0  , 0  , 13 , 12 , 0  , 0  },
			/* 3: in """        */ new int[] { 4  , 3  , 3  , 3  , 3  , 3  , 3  , 3  , 3  , 3  , 3  },
			/* 4: in """, "     */ new int[] { 5  , 3  , 3  , 3  , 3  , 3  , 3  , 3  , 3  , 3  , 3  },
			/* 5: in """, ""    */ new int[] { 0  , 3  , 3  , 3  , 3  , 3  , 3  , 3  , 3  , 3  , 3  },
			/* 6: in "-string   */ new int[] { 0  , 6  , 10 , 0  , 8  , 6  , 6  , 6  , 6  , 6  , 6  },
			/* 7: in '-string   */ new int[] { 7  , 0  , 11 , 0  , 7  , 7  , 7  , 7  , 7  , 7  , 7  },
			/* 8: after $ in "  */ new int[] { 0  , 6  , 10 , 0  , 8  , 9  , 6  , 6  , 6  , 6  , 6  },
			/* 9: in "{         */ new int[] { 9  , 9  , 9  , 9  , 9  , 9  , 6  , 9  , 9  , 9  , 9  },
			/* 10: after \ in " */ new int[] { 6  , 6  , 6  , 0  , 6  , 6  , 6  , 6  , 6  , 6  , 6  },
			/* 11: after \ in ' */ new int[] { 7  , 7  , 7  , 0  , 7  , 7  , 7  , 7  , 7  , 7  , 7  },
			/* 12: after /      */ new int[] { 1  , 7  , 0  , 0  , 0  , 0  , 0  , 0  , 13 ,-14 , 0  },
			/* 13: line comment */ new int[] { 13 , 13 , 13 , 0  , 13 , 13 , 13 , 13 , 13 , 13 , 13 },
			/* 14: block comment*/ new int[] { 14 , 14 , 14 , 14 , 14 , 14 , 14 , 14 , 14 , 15 , 14 },
			/* 15: after * in bc*/ new int[] { 14 , 14 , 14 , 14 , 14 , 14 , 14 , 14 ,-15 , 15 , 14 }
		};
		
		static bool IsInNormalCode(int state)
		{
			return state == 0 || state == 2 || state == 12;
		}
		
		int commentblocks;
		
		public void ResetStateMachine()
		{
			commentblocks = 0;
		}
		
		public int FeedStateMachine(int oldState, char c)
		{
			int charNum = (int)c;
			int input;
			if (charNum < inputTable.Length) {
				input = inputTable[charNum];
			} else {
				input = _elseIndex;
			}
			int action = _stateTable[oldState][input];
			if (action == -14) {
				// enter block comment
				commentblocks += 1;
				return 14;
			} else if (action == -15) {
				// leave block comment
				commentblocks -= 1;
				if (commentblocks == 0)
					return 0;
				else
					return 14;
			}
			return action;
		}
		
		/// <summary>
		/// Goes to the next position in "text" that is code (not comment, string etc.).
		/// Returns a state that has be passed in as <paramref name="state"/> on the
		/// next call.
		/// </summary>
		public int FindNextCodeCharacter(int state, string text, ref int pos)
		{
			ResetStateMachine();
			do {
				pos += 1;
				if (pos >= text.Length)
					break;
				char c = text[pos];
				state = FeedStateMachine(state, c);
				if (state == PossibleRegexStart) {
					// after / could be a regular expression, do a special check for that
					int regexEnd = SkipRegularExpression(text, pos, text.Length - 1);
					if (regexEnd > 0) {
						pos = regexEnd;
					} else if (regexEnd == -1) {
						// cursor is in regex
						return -1;
					} // else: regexEnd is 0 if its not a regex
				}
			} while (!IsInNormalCode(state));
			return state;
		}
		
		/// <summary>This method makes boo source code "simpler" by removing all comments
		/// and replacing all string litarals through string.Empty.
		/// Regular expressions literals are replaced with the simple regex /a/</summary>
		public string SimplifyCode(string inText, int offset)
		{
			StringBuilder result = new StringBuilder();
			StringBuilder inStringResult = new StringBuilder(" ");
			int state = 0;
			ResetStateMachine();
			int i = -1;
			while (i < offset) {
				i += 1;
				char c = inText[i];
				int action = FeedStateMachine(state, c);
				if (action == 9) {
					// enter inner string expression (${...})
					if (state == 9)
						inStringResult.Append(c);
					else
						inStringResult.Length = 1;
					state = action;
				} else if (action == 0 || action == PossibleRegexStart) {
					// go to normal code
					if (action == PossibleRegexStart) {
						// after / could be a regular expression, do a special check for that
						int regexEnd = SkipRegularExpression(inText, i, offset);
						if (regexEnd > 0) {
							i = regexEnd;
							result.Append("/a");
						} else if (regexEnd == -1) {
							// cursor is in regex
							return null;
						}
					}
					if (state == 2 || (state >= 6 && state <= 11))
						result.Append("''");
					if (IsInNormalCode(state))
						result.Append(c);
					state = action;
				} else {
					state = action;
				}
			}
			if (IsInNormalCode(state)) {
				// cursor is in normal code
				return result.ToString();
			} else if (state == 9) {
				// cursor is in inner string expression (${...})
				return inStringResult.ToString();
			} else {
				// cursor is in comment/string
				return null;
			}
		}
		
		/// <summary>Skips the regular expression in inText at position pos. Returns end position of the ending / if
		/// successful or 0 is no regular expression was found at the location.
		/// Return -1 if maxOffset is inside the regular expression.</summary>
		public int SkipRegularExpression(string inText, int pos, int maxOffset)
		{
			bool containsWhitespace;
			if (pos > 0) {
				containsWhitespace = (inText[pos - 1] == '@');
			} else {
				containsWhitespace = false;
			}
			if (pos == maxOffset) return -1;             // cursor is after / -> cursor inside regex
			if (inText[pos + 1] == '/') return 0;        // double // is comment, no regex
			int i = pos;
			while (i < maxOffset) {
				i += 1;
				if (!containsWhitespace && Char.IsWhiteSpace(inText, i))
					return 0; // this is no regex
				if (inText[i] == '/')
					return i;
			}
			return -1; // maxOffset inside regex
		}
		#endregion
	}
}
