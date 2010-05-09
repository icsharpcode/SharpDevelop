// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com" />
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.VB;
using ICSharpCode.SharpDevelop.Editor;

namespace VBNetBinding
{
	public class VBNetBracketSearcher : IBracketSearcher
	{
		string openingBrackets = "([{";
		string closingBrackets = ")]}";
		
		public BracketSearchResult SearchBracket(IDocument document, int offset)
		{
			if (offset > 0) {
				char c = document.GetCharAt(offset - 1);
				int index = openingBrackets.IndexOf(c);
				int otherOffset = -1;
				if (index > -1)
					otherOffset = SearchBracketForward(document, offset, openingBrackets[index], closingBrackets[index]);
				index = closingBrackets.IndexOf(c);
				if (index > -1)
					otherOffset = SearchBracketBackward(document, offset - 2, openingBrackets[index], closingBrackets[index]);
				
				if (otherOffset > -1)
					return new BracketSearchResult(Math.Min(offset - 1, otherOffset), 1, Math.Max(offset - 1, otherOffset), 1);
				
				int length;
				VBStatement statement;
				
				int startIndex = FindBeginStatementAroundOffset(document, offset, out statement, out length);
				int endIndex = 0;
				
				if (statement != null)
					endIndex = FindEndStatement(document, statement);
				else {
					endIndex = FindEndStatementAroundOffset(document, offset, out statement);
					
					if (statement != null)
						startIndex = FindBeginStatement(document, statement, document.OffsetToPosition(endIndex), out length);
				}
				
				if (startIndex > -1 && endIndex > -1)
					return new BracketSearchResult(startIndex, length, endIndex, statement.EndStatement.Length);
			}
			
			return null;
		}
		
		#region bracket search
		int SearchBracketBackward(IDocument document, int offset, char openBracket, char closingBracket)
		{
			bool inString  = false;
			char ch;
			int brackets = -1;
			for (int i = offset; i > 0; --i) {
				ch = document.GetCharAt(i);
				if (ch == openBracket && !inString) {
					++brackets;
					if (brackets == 0) return i;
				} else if (ch == closingBracket && !inString) {
					--brackets;
				} else if (ch == '"') {
					inString = !inString;
				} else if (ch == '\n') {
					int lineStart = ScanLineStart(document, i);
					if (lineStart >= 0) { // line could have a comment
						inString = false;
						for (int j = lineStart; j < i; ++j) {
							ch = document.GetCharAt(j);
							if (ch == '"') inString = !inString;
							if (ch == '\'' && !inString) {
								// comment found!
								// Skip searching in the comment:
								i = j;
								break;
							}
						}
					}
					inString = false;
				}
			}
			return -1;
		}
		
		static int ScanLineStart(IDocument document, int offset)
		{
			bool hasComment = false;
			for (int i = offset - 1; i > 0; --i) {
				char ch = document.GetCharAt(i);
				if (ch == '\n') {
					if (!hasComment) return -1;
					return i + 1;
				} else if (ch == '\'') {
					hasComment = true;
				}
			}
			return 0;
		}
		
		int SearchBracketForward(IDocument document, int offset, char openBracket, char closingBracket)
		{
			bool inString  = false;
			bool inComment = false;
			int  brackets  = 1;
			for (int i = offset; i < document.TextLength; ++i) {
				char ch = document.GetCharAt(i);
				if (ch == '\n') {
					inString  = false;
					inComment = false;
				}
				if (inComment) continue;
				if (ch == '"') inString = !inString;
				if (inString)  continue;
				if (ch == '\'') {
					inComment = true;
				} else if (ch == openBracket) {
					++brackets;
				} else if (ch == closingBracket) {
					--brackets;
					if (brackets == 0) return i;
				}
			}
			return -1;
		}
		#endregion
		
		#region statement search
		int FindBeginStatementAroundOffset(IDocument document, int offset, out VBStatement statement, out int length)
		{
			length = 0;
			statement = null;
			return -1;
		}
		
		int FindEndStatementAroundOffset(IDocument document, int offset, out VBStatement statement)
		{
			IDocumentLine line = document.GetLineForOffset(offset);
			
			string interestingText = VBNetFormattingStrategy.TrimLine(line.Text).Trim(' ', '\t');
			
			//LoggingService.Debug("text: '" + interestingText + "'");
			
			foreach (VBStatement s in VBNetFormattingStrategy.Statements) {
				Match match = Regex.Matches(interestingText, s.EndRegex, RegexOptions.Singleline | RegexOptions.IgnoreCase).OfType<Match>().FirstOrDefault();
				if (match != null) {
					//LoggingService.DebugFormatted("Found end statement at offset {1}: {0}", s, offset);
					statement = s;
					int result = match.Index + (line.Length - line.Text.TrimStart(' ', '\t').Length) + line.Offset;
					if (offset >= result && offset <= (result + match.Length))
						return result;
				}
			}
			
			statement = null;
			return -1;
		}
		
		int FindBeginStatement(IDocument document, VBStatement statement, Location endLocation, out int length)
		{
			ILexer lexer = ParserFactory.CreateLexer(SupportedLanguage.VBNet, document.CreateReader());
			
			Token currentToken = null;
			Token prevToken = null;
			
			int lookFor = statement.StatementToken;
			Stack<Token> tokens = new Stack<Token>();
			
			if (statement.EndStatement == "Next") {
				lookFor = Tokens.For;
			}
			
			Token result = null;
			
			while ((currentToken = lexer.NextToken()).Kind != Tokens.EOF) {
				if (prevToken == null)
					prevToken = currentToken;
				
				if (VBNetFormattingStrategy.IsBlockStart(lexer, currentToken, prevToken))
					tokens.Push(currentToken);
				if (VBNetFormattingStrategy.IsBlockEnd(currentToken, prevToken)) {
					while (tokens.Count > 0 && !VBNetFormattingStrategy.IsMatchingEnd(tokens.Peek(), currentToken))
						tokens.Pop();
					if (tokens.Count > 0) {
						Token t = tokens.Pop();
						if (currentToken.Location.Line == endLocation.Line) {
							result = t;
							break;
						}
					}
				}
				
				prevToken = currentToken;
			}
			
			if (result != null) {
				
				IDocumentLine line = document.GetLine(result.Location.Line);
				string interestingText = VBNetFormattingStrategy.TrimLine(line.Text).Trim(' ', '\t');
				
				//LoggingService.Debug("text2: '" + interestingText + "'");
				
				Match matchResult = Regex.Match(interestingText, statement.StartRegex, RegexOptions.Singleline | RegexOptions.IgnoreCase);
				
				if (matchResult != null) {
					length = matchResult.Value.TrimEnd(' ', '\t').Length;
					int diff = line.Length - line.Text.TrimStart(' ', '\t').Length;
					int start = diff + line.Offset;
					if (IsDeclaration(result.Kind)) {
						length += diff + matchResult.Index;
						return start;
					}
					return matchResult.Index + start;
				}
			}
			
			length = 0;
			
			return -1;
		}
		
		int FindEndStatement(IDocument document, VBStatement statement)
		{
			return -1;
		}
		#endregion
		
		bool IsDeclaration(int kind)
		{
			return kind == Tokens.Sub || kind == Tokens.Function || kind == Tokens.Operator || VBNetFormattingStrategy.IsDeclaration(kind);
		}
	}
}