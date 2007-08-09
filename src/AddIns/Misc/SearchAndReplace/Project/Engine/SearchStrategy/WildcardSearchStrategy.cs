// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using ICSharpCode.TextEditor.Document;

namespace SearchAndReplace
{
	/// <summary>
	/// Implements a wildcard search strategy.
	/// 
	/// Wildcard search has following pattern code :
	///      * = Zero or more of any character
	///      ? = Any single character
	///      # = Any single digit
	///  [...] = Any one character in the set
	/// [!...] = Any one character not in the set
	/// </summary>
	public class WildcardSearchStrategy : ISearchStrategy
	{
		enum CommandType {
			Match,
			AnyZeroOrMore,
			AnySingle,
			AnyDigit,
			AnyInList,
			NoneInList
		}
		
		class Command {
			public CommandType CommandType = CommandType.Match;
			public char        SingleChar  = '\0';
			public string      CharList    = String.Empty;
		}
		
		ArrayList patternProgram = null;
		int       curMatchEndOffset = -1;
		
		void CompilePattern(string pattern, bool ignoreCase)
		{
			patternProgram = new ArrayList();
			for (int i = 0; i < pattern.Length; ++i) {
				Command newCommand = new Command();
				switch (pattern[i]) {
					case '#':
						newCommand.CommandType = CommandType.AnyDigit;
						break;
					case '*':
						newCommand.CommandType = CommandType.AnyZeroOrMore;
						break;
					case '?':
						newCommand.CommandType = CommandType.AnySingle;
						break;
					case '[':
						int index = pattern.IndexOf(']', i);
						if (index > 0) {
							newCommand.CommandType = CommandType.AnyInList;
							string list = pattern.Substring(i + 1, index - i - 1);
							if (list[0] == '!') {
								newCommand.CommandType = CommandType.NoneInList;
								list = list.Substring(1);
							}
							newCommand.CharList = ignoreCase ? list.ToUpper() : list;
							i = index;
						} else {
							goto default;
						}
						break;
					default:
						newCommand.CommandType = CommandType.Match;
						newCommand.SingleChar  = ignoreCase ? Char.ToUpper(pattern[i]) : pattern[i];
						break;
				}
				patternProgram.Add(newCommand);
			}
		}
		
		bool Match(ITextBufferStrategy document, 
		           int  offset, 
		           bool ignoreCase,
		           int  programStart)
		{
			int curOffset = offset;
			curMatchEndOffset = -1;
			
			for (int pc = programStart; pc < patternProgram.Count; ++pc) {
				if (curOffset >= document.Length) {
					return false;
				}
				
				char    ch  = ignoreCase ? Char.ToUpper(document.GetCharAt(curOffset)) : document.GetCharAt(curOffset);
				Command cmd = (Command)patternProgram[pc];
				
				switch (cmd.CommandType) {
					case CommandType.Match:
						if (ch != cmd.SingleChar) {
							return false;
						}
						break;
					case CommandType.AnyZeroOrMore:
						if (ch == '\n') {
							return false;
						}
						return Match(document, curOffset, ignoreCase, pc + 1) ||
						       Match(document, curOffset + 1, ignoreCase, pc);
					case CommandType.AnySingle:
						break;
					case CommandType.AnyDigit:
						if (!Char.IsDigit(ch) && ch != '#') {
							return false;
						}
						break;
					case CommandType.AnyInList:
						if (cmd.CharList.IndexOf(ch) < 0) {
							return false;
						}
						break;
					case CommandType.NoneInList:
						if (cmd.CharList.IndexOf(ch) >= 0) {
							return false;
						}
						break;
				}
				++curOffset;
			}
			curMatchEndOffset = curOffset;
			return true;
		}
		
		int InternalFindNext(ITextIterator textIterator)
		{
			while (textIterator.MoveAhead(1)) {
				int position = textIterator.Position;
				if (Match(textIterator.TextBuffer, position, !SearchOptions.MatchCase, 0)) {
					if (!SearchOptions.MatchWholeWord || SearchReplaceUtilities.IsWholeWordAt(textIterator.TextBuffer, position, curMatchEndOffset - position)) {
						textIterator.MoveAhead(curMatchEndOffset - position - 1);
						return position;
					}
				}
			}
			return -1;
		}
		
		int InternalFindNext(ITextIterator textIterator, int offset, int length)
		{
			while (textIterator.MoveAhead(1) && TextSelection.IsInsideRange(textIterator.Position, offset, length)) {
				int position = textIterator.Position;
				if (Match(textIterator.TextBuffer, position, !SearchOptions.MatchCase, 0)) {
					if (!SearchOptions.MatchWholeWord || SearchReplaceUtilities.IsWholeWordAt(textIterator.TextBuffer, position, curMatchEndOffset - position)) {
						if (TextSelection.IsInsideRange(curMatchEndOffset - 1, offset, length)) {
							textIterator.MoveAhead(curMatchEndOffset - position - 1);
							return position;
						} else {
							return -1;
						}
					}
				}
			}
			return -1;
		}
		
		public bool CompilePattern(ICSharpCode.SharpDevelop.Gui.IProgressMonitor monitor)
		{
			CompilePattern(SearchOptions.FindPattern, !SearchOptions.MatchCase);
			return true;
		}
		
		public SearchResultMatch FindNext(ITextIterator textIterator)
		{
			int offset = InternalFindNext(textIterator);
			return GetSearchResult(offset);
		}
		
		public SearchResultMatch FindNext(ITextIterator textIterator, int offset, int length)
		{
			int foundOffset = InternalFindNext(textIterator, offset, length);
			return GetSearchResult(foundOffset);
		}
		
		SearchResultMatch GetSearchResult(int offset)
		{
			return offset >= 0 ? new SearchResultMatch(offset, curMatchEndOffset - offset) : null;
		}
	}
}
