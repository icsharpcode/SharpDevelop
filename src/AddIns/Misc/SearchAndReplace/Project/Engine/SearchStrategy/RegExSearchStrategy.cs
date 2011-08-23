// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Editor.Search;
using System;
using System.Text.RegularExpressions;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace SearchAndReplace
{
	public class RegExSearchStrategy : ISearchStrategy
	{
		Regex regex = null;
		
		public bool CompilePattern(IProgressMonitor monitor)
		{
			RegexOptions regexOptions = RegexOptions.Compiled | RegexOptions.Multiline;
			if (!SearchOptions.MatchCase) {
				regexOptions |= RegexOptions.IgnoreCase;
			}
			try {
				regex = new Regex(SearchOptions.FindPattern, regexOptions);
				return true;
			} catch (ArgumentException ex) {
				if (monitor != null) monitor.ShowingDialog = true;
				MessageService.ShowError("${res:Dialog.NewProject.SearchReplace.ErrorParsingRegex}\n" + ex.Message);
				if (monitor != null) monitor.ShowingDialog = false;
				return false;
			}
		}
		
		public SearchResultMatch FindNext(ITextIterator textIterator)
		{
			string document = textIterator.Document.Text;
			
			while (textIterator.MoveAhead(1)) {
				Match m = regex.Match(document, textIterator.Position);
				if (m == null || !m.Success) {
					while (textIterator.Position < document.Length - 1) {
						if (!textIterator.MoveAhead(1))
							return null;
					}
				} else {
					int delta = m.Index - textIterator.Position;
					if (delta <= 0 || textIterator.MoveAhead(delta)) {
						return new RegexSearchResult(m);
					} else {
						return null;
					}
				}
			}
			
			return null;
		}
		
		public SearchResultMatch FindNext(ITextIterator textIterator, int offset, int length)
		{
			string document = textIterator.Document.GetText(0, textIterator.Document.TextLength);
			
			while (textIterator.MoveAhead(1)) {
				if (textIterator.Position >= offset + length) {
					textIterator.Position = offset;
				}
				Match m = regex.Match(document, textIterator.Position);
				if (m == null || !m.Success) {
					while (textIterator.Position < document.Length - 1) {
						if (!textIterator.MoveAhead(1))
							return null;
					}
				} else {
					int delta = m.Index - textIterator.Position;
					if (delta <= 0 || textIterator.MoveAhead(delta)) {
						if (TextSelection.IsInsideRange(m.Index + m.Length - 1, offset, length)) {
							return new RegexSearchResult(m);
						} else {
							return null;
						}
					} else {
						return null;
					}
				}
			}
			
			return null;
		}
		
		private sealed class RegexSearchResult : SearchResultMatch
		{
			Match m;
			
			internal RegexSearchResult(Match m) : base(m.Index, m.Length)
			{
				this.m = m;
			}
			
			public override string TransformReplacePattern(string pattern)
			{
				return m.Result(pattern);
			}
		}
	}
}
