// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Editor;
using System;
using System.Collections.Generic;

namespace Grunwald.BooBinding
{
	public class BooFormattingStrategy : DefaultFormattingStrategy
	{
		public override void IndentLine(ITextEditor editor, IDocumentLine line)
		{
			IDocument document = editor.Document;
			int lineNumber = line.LineNumber;
			if (lineNumber > 1) {
				IDocumentLine previousLine = document.GetLine(line.LineNumber - 1);
				
				if (previousLine.Text.EndsWith(":", StringComparison.Ordinal)) {
					string indentation = DocumentUtilitites.GetWhitespaceAfter(document, previousLine.Offset);
					indentation += editor.Options.IndentationString;
					string newIndentation = DocumentUtilitites.GetWhitespaceAfter(document, line.Offset);
					document.Replace(line.Offset, newIndentation.Length, indentation);
					return;
				}
			}
			base.IndentLine(editor, line);
		}
		
		// Deactivate indenting multiple lines with Ctrl-I
		public override void IndentLines(ITextEditor editor, int begin, int end)
		{
		}
		
		public override void SurroundSelectionWithComment(ITextEditor editor)
		{
			SurroundSelectionWithSingleLineComment(editor, "//");
		}
		
		#region Matching bracket search
		/*
		int DoQuickFindBackward(IDocument document, int offset, char openBracket, char closingBracket)
		{
			int brackets = -1;
			// first try "quick find" - find the matching bracket if there is no string/comment in the way
			for (int i = offset; i >= 0; --i) {
				char ch = document.GetCharAt(i);
				if (ch == openBracket) {
					++brackets;
					if (brackets == 0) return i;
				} else if (ch == closingBracket) {
					--brackets;
				} else if (ch == '"') {
					break;
				} else if (ch == '#') {
					break;
				} else if (ch == '\'') {
					break;
				} else if (ch == '/' && i > 0) {
					if (document.GetCharAt(i - 1) == '/') break;
					if (document.GetCharAt(i - 1) == '*') break;
				}
			}
			return -1;
		}
		
		int DoQuickFindForward(IDocument document, int offset, char openBracket, char closingBracket)
		{
			int brackets = 1;
			// try "quick find" - find the matching bracket if there is no string/comment in the way
			for (int i = offset; i < document.TextLength; ++i) {
				char ch = document.GetCharAt(i);
				if (ch == openBracket) {
					++brackets;
				} else if (ch == closingBracket) {
					--brackets;
					if (brackets == 0) return i;
				} else if (ch == '"') {
					break;
				} else if (ch == '#') {
					break;
				} else if (ch == '\'') {
					break;
				} else if (ch == '/' && i > 0) {
					if (document.GetCharAt(i - 1) == '/') break;
				} else if (ch == '*' && i > 0) {
					if (document.GetCharAt(i - 1) == '/') break;
				}
			}
			return -1;
		}
		
		public override int SearchBracketBackward(IDocument document, int offset, char openBracket, char closingBracket)
		{
			// Implemented Boo specific "quick find" of brackets
			// needed because of "#" --> comment character
			int quickFind = DoQuickFindBackward(document, offset, openBracket, closingBracket);
			if (quickFind >= 0)
				return quickFind;
			
			// Same as with C# - can not parse a boo file backwards
			// -> all brackets from the beginning are put on a stack
			
			bool inSingleComment = false;
			int multiCommentLevel = 0; // Boo has nested multiline comments
			bool inMultiString = false;
			bool inSingleString = false;
			bool inSingleString2 = false; // ' ... ' strings
			Stack<int> bracketStack = new Stack<int>();
			
			if (document.TextContent.StartsWith("//")) {
				inSingleComment = true;
			}
			
			for (int position = 0; position <= offset; ++position) {
				try {
					char ch = document.GetCharAt(position);
					
					if (ch == openBracket) {
						if (!(inSingleString || inSingleString2 || inMultiString || inSingleComment || (multiCommentLevel != 0))) {
							bracketStack.Push(position);
						}
					} else {
						if (ch == closingBracket) {
							if (!(inSingleString || inSingleString2 || inMultiString || inSingleComment || (multiCommentLevel != 0))) {
								if (bracketStack.Count > 0) {
									bracketStack.Pop();
								}
							}
						} else {
							switch (ch) {
								case '/':
									if (!(inMultiString || inSingleString2 || inSingleString || inSingleComment)) {
										if (document.GetCharAt(position + 1) == '*') {
											multiCommentLevel++;
										}
										if (document.GetCharAt(position + 1) == '/') {
											inSingleComment = true;
										}
										if (document.GetCharAt(position - 1) == '*') {
											multiCommentLevel--;
										}
									}
									break;
								case '#':
									if (!inMultiString && !inSingleString && !inSingleString2)
										inSingleComment = true;
									break;
								case '\n':
									if (!inMultiString && !inSingleString && !inSingleString2)
										inSingleComment = false;
									break;
								case '"':
									if (IsMultilineString(document, position)) {
										position += 2;
										inMultiString = !inMultiString;
									} else {
										if (!inMultiString && !inSingleString2) {
											inSingleString = !inSingleString;
										}
									}
									break;
								case '\'':
									if (!inMultiString && !inSingleString2) {
										inSingleString2 = !inSingleString2;
									}
									break;
								case '\\':
									if (!inMultiString)
										position++;
									break;
							}
						}
					}
				} catch (ArgumentOutOfRangeException) {
					break;
				}
			}
			
			if (bracketStack.Count > 0)
				return bracketStack.Pop();
			
			return -1;
		}
		
		bool IsMultilineString(IDocument document, int offset)
		{
			if (offset + 2 >= document.TextLength)
				return false;
			
			if ((document.GetCharAt(offset) == document.GetCharAt(offset + 1))
			    && (document.GetCharAt(offset) == document.GetCharAt(offset + 2))) {
				return true;
			}
			
			return false;
		}
		
		public override int SearchBracketForward(IDocument document, int offset, char openBracket, char closingBracket)
		{
			// Implemented Boo specific "quick find" of brackets
			// needed because of "#" --> comment character
			int quickFind = DoQuickFindForward(document, offset, openBracket, closingBracket);
			if (quickFind >= 0)
				return quickFind;
			
			int brackets = 1;
			bool inSingleComment = false;
			int multiCommentLevel = 0;
			bool inMultiString = false;
			bool inSingleString = false;
			
			while (offset < document.TextLength) {
				char ch = document.GetCharAt(offset);
				
				if (ch == openBracket) {
					if (!(inSingleString || inMultiString || inSingleComment || (multiCommentLevel != 0))) {
						brackets++;
					}
				} else {
					if (ch == closingBracket) {
						if (!(inSingleString || inMultiString || inSingleComment || (multiCommentLevel != 0))) {
							brackets--;
							if (brackets == 0) return offset;
						}
					} else {
						switch (ch) {
							case '/':
								if (!inMultiString && !inSingleString) {
									if (offset + 1 < document.TextLength) {
										if (document.GetCharAt(offset + 1) == '*') {
											multiCommentLevel++;
										} else if (document.GetCharAt(offset + 1) == '/') {
											inSingleComment = true;
										}
									}
								}
								break;
							case '*':
								if (!inMultiString && !inSingleString) {
									if (offset + 1 < document.TextLength && document.GetCharAt(offset + 1) == '/') {
										multiCommentLevel--;
									}
								}
								break;
							case '#':
								if (!inMultiString && !inSingleString)
									inSingleComment = true;
								break;
							case '\n':
								if (!inMultiString && !inSingleString)
									inSingleComment = false;
								break;
							case '"':
								if (IsMultilineString(document, offset)) {

									offset += 2;
									inMultiString = !inMultiString;
								} else {
									if (!inMultiString) {
										inSingleString = !inSingleString;
									}
								}
								break;
							case '\\':
								if (!inMultiString)
									offset++;
								break;
						}
					}
				}
				offset++;
			}
			
			return -1;
		}
		*/
		#endregion
	}
}
