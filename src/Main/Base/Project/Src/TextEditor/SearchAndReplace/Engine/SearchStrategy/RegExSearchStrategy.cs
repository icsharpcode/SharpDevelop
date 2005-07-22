// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Text.RegularExpressions;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Internal.Undo;

namespace SearchAndReplace
{
	public class RegExSearchStrategy : ISearchStrategy
	{
		Regex regex = null;
		
		public void CompilePattern()
		{
			RegexOptions regexOptions = RegexOptions.Compiled;
			if (!SearchOptions.MatchCase) {
				regexOptions |= RegexOptions.IgnoreCase;
			}
			regex = new Regex(SearchOptions.FindPattern, regexOptions);
		}
		
		public SearchResult FindNext(ITextIterator textIterator)
		{
			string document = textIterator.TextBuffer.GetText(0, textIterator.TextBuffer.Length);
			
			while (textIterator.MoveAhead(1)) {
				Match m = regex.Match(document, textIterator.Position);
				if (m == null || m.Index <= 0 || m.Length <= 0) {
					
				} else {
					int delta = m.Index - textIterator.Position;
					if (delta <= 0 || textIterator.MoveAhead(delta)) {
						return new SearchResult(m.Index, m.Length);
					} else {
						return null;
					}
				}
			}
			
			return null;
		}
	}
}
