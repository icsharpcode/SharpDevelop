// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Editor
{
	/// <summary>
	/// Allows language specific search for matching brackets.
	/// </summary>
	public interface IBracketSearcher
	{
		/// <summary>
		/// Searches for a matching bracket from the given offset to the start of the document.
		/// </summary>
		/// <returns>A BracketSearchResult that contains the positions and lengths of the brackets.</returns>
		BracketSearchResult SearchBracket(IDocument document, int offset);
	}
	
	public class DefaultBracketSearcher : IBracketSearcher
	{
		public static readonly DefaultBracketSearcher DefaultInstance = new DefaultBracketSearcher();
		
		public BracketSearchResult SearchBracket(IDocument document, int offset)
		{
			return null;
		}
	}
	
	/// <summary>
	/// Describes a pair of matching brackets found by IBracketSearcher.
	/// </summary>
	public class BracketSearchResult
	{
		public int OpeningBracketOffset { get; private set; }
		
		public int OpeningBracketLength { get; private set; }
		
		public int ClosingBracketOffset { get; private set; }

		public int ClosingBracketLength { get; private set; }
		
		public BracketSearchResult(int openingBracketOffset, int openingBracketLength,
		                           int closingBracketOffset, int closingBracketLength)
		{
			this.OpeningBracketOffset = openingBracketOffset;
			this.OpeningBracketLength = openingBracketLength;
			this.ClosingBracketOffset = closingBracketOffset;
			this.ClosingBracketLength = closingBracketLength;
		}
	}
}
