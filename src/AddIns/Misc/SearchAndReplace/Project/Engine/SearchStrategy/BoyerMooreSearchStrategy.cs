// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

namespace SearchAndReplace
{
	/// <summary>
	/// This interface is the basic interface which all 
	/// search algorithms must implement.
	/// </summary>
	public class BoyerMooreSearchStrategy
	{
		// Shift table for chars present in the pattern
		Dictionary<char, int[]> patternCharShifts;
		// Shifts for all other chars
		int[] otherCharShifts;
		// the patern to search
		string searchPattern;
		// Length of the search pattern
		int patternLength;
		
		/// <remarks>
		/// Only with a call to this method the search strategy must
		/// update their pattern information. This method will be called 
		/// before the FindNext function.
		/// </remarks>
		public void CompilePattern()
		{
			searchPattern = SearchOptions.MatchCase ? SearchOptions.FindPattern : SearchOptions.FindPattern.ToUpper();
			
			// Building shift table
			patternLength = searchPattern.Length;
			int maxShift = patternLength;
			patternCharShifts = new Dictionary<char, int[]>();
			// Constructing the table where number
			// of columns is equal to PatternLength
			// and number of rows is equal to the
			// number of distinct chars in the pattern
			for (int i = 0; i < patternLength; ++i) {
				if (!patternCharShifts.ContainsKey(searchPattern[i])) {
					patternCharShifts.Add(searchPattern[i], new int[patternLength]);
				}
			}
			otherCharShifts = new int[patternLength];
			// Filling the last column of the
			// table with maximum shifts (pattern length)
			foreach(KeyValuePair<char, int[]> row in patternCharShifts) {
				row.Value[patternLength - 1] = maxShift;
			}
			otherCharShifts[patternLength - 1] = maxShift;
			// Calculating other shifts (filling each column
			// from PatternLength - 2 to 0 (from right to left)
			for (int i = patternLength - 1; i >= 0; --i)
			{
				// Suffix string contains the characters
				// right to the character being processsed
				string suffix = new String(searchPattern.ToCharArray(),
				                           i + 1,  patternLength - i - 1);
				// if Pattern begins with Suffix
				// the maximum shift is equal to i + 1
				if (searchPattern.StartsWith(suffix)) {
					maxShift = i + 1;
				}
				// Store shift for characters not present in the pattern
				otherCharShifts[i] = maxShift;
				// We shorten patter by one char in NewPattern.
				string newPattern = new string(searchPattern.ToCharArray(),
				                               0, searchPattern.Length -1);
				if ((newPattern.LastIndexOf(suffix) > 0) || (suffix.Length == 0)) {
					foreach(KeyValuePair<char, int[]> row in patternCharShifts)
					{
						string newSuffix  = row.Key + suffix;
						// Calculate shifts:
						//Check if there are other occurences
						//of the new suffix in the pattern
						// If several occurences exist, we need the rightmost one
						int newSuffixPos = newPattern.LastIndexOf(newSuffix);
						if (newSuffixPos >= 0) {
							row.Value[i] = i - newSuffixPos;
						} else {
							row.Value[i] = maxShift;
						}
						// Storing 0 if characters
						// in a row and a columnt are the same
						if (row.Key == searchPattern[i]) {
							row.Value[i] = 0;
						}
					}
				} else {
					foreach(KeyValuePair<char, int[]> row in patternCharShifts)
					{
						// if Suffix doesn't occure in NewPattern
						// we simply use previous shift value
						row.Value[i] = maxShift;
						if (row.Key == searchPattern[i]) {
							row.Value[i] = 0;
						}
					}
				}
			}
		}
		
		int InternalFindNext(ITextIterator textIterator)
		{
//			while (textIterator.MoveAhead(1)) {
//				if (SearchOptions.MatchCase ? MatchCaseSensitive(textIterator.TextBuffer, textIterator.Position, searchPattern) : MatchCaseInsensitive(textIterator.TextBuffer, textIterator.Position, searchPattern)) {
//					if (!SearchOptions.MatchWholeWord || IsWholeWordAt(textIterator.TextBuffer, textIterator.Position, searchPattern.Length)) {
//						return textIterator.Position;
//					}
//				}
//			}
			return -1;
		}
		
		/// <remarks>
		/// The find next method should search the next occurrence of the 
		/// compiled pattern in the text using the textIterator and options.
		/// </remarks>
		public SearchResultMatch FindNext(ITextIterator textIterator)
		{
			int offset = InternalFindNext(textIterator);
			return offset >= 0 ? new SearchResultMatch(offset, searchPattern.Length) : null;
		}
	}
}
