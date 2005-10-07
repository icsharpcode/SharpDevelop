// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;

namespace Grunwald.BooBinding.CodeCompletion
{
	public class ExpressionFinder : IExpressionFinder
	{
		string fileName;
		
		public ExpressionFinder(string fileName)
		{
			this.fileName = fileName;
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
			throw new NotImplementedException();
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
			if (inText == null) return new ExpressionResult(null);
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
					LoggingService.Debug("Quickfind failed: got " + c);
					break;
				}
				if (char.IsWhiteSpace(c)) {
					if (i > 6 && inText.Substring(i - 6, 6) == "import") {
						i -= 7; // include 'import' in the expression
					}
					start = i + 1;
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
				LoggingService.Debug("SimplifyCode returned null (cursor is in comment/string???)");
				return new ExpressionResult(null);
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
			return new ExpressionResult(null);
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
			if (start == end) return new ExpressionResult(null);
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
			return new ExpressionResult(b.ToString());
		}
		// TODO: We could need some unit tests for this.
		#endregion
		
		#region Find Full Expression
		public ExpressionResult FindFullExpression(string inText, int offset)
		{
			ExpressionResult result = FindExpression(inText, offset);
			if (result.Expression == null)
				return result;
			StringBuilder b = new StringBuilder(result.Expression);
			ResetStateMachine();
			int state = -1;
			// state = -1 : accepting current identifier
			// state >= 0 : accepting brackets/parenthesis
			Stack<int> bracketStack = new Stack<int>();
			for (int i = offset + 1; i < inText.Length; i++) {
				char c = inText[i];
				if (state == -1) {
					if (char.IsLetterOrDigit(c) || c == '_') {
						// continue reading identifier
					} else {
						state = 0;
					}
				}
				if (state >= 0) {
					state = FeedStateMachine(state, c);
					if (IsInNormalCode(state)) {
						int bracket = _openingBrackets.IndexOf(c);
						if (bracket >= 0) {
							bracketStack.Push(bracket);
						} else {
							if (bracketStack.Count == 0) {
								result.Expression = b.ToString();
								return result;
							}
						}
						bracket = _closingBrackets.IndexOf(c);
						if (bracket >= 0) {
							while (bracketStack.Count > 0 && bracketStack.Pop() > bracket);
						}
					}
				}
				b.Append(c);
			}
			return new ExpressionResult(null);
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
		
		void ResetStateMachine()
		{
			commentblocks = 0;
		}
		
		int FeedStateMachine(int oldState, char c)
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
				} else if (action == 0 || action == 12) {
					// go to normal code
					if (action == 12) {
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
						result.Append("string.Empty");
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
		int SkipRegularExpression(string inText, int pos, int maxOffset)
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
