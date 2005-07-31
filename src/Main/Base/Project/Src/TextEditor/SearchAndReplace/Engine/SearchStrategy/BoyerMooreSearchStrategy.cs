// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 230 $</version>
// </file>
/*
using System;
using System.Drawing;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Internal.Undo;

namespace SearchAndReplace
{
	/// <summary>
	/// This interface is the basic interface which all 
	/// search algorithms must implement.
	/// </summary>
	public class BoyerMooreSearchStrategy
	{
		// Shift table for chars present in the pattern
		Hashtable PatternCharShifts;
		// Shifts for all other chars
		int[] OtherCharShifts;
		// the patern to search
		string searchPattern;
		// Length of the search pattern
		int PatternLength;
		
		/// <remarks>
		/// Only with a call to this method the search strategy must
		/// update their pattern information. This method will be called 
		/// before the FindNext function.
		/// </remarks>
		public void CompilePattern()
		{
			searchPattern = SearchOptions.MatchCase ? SearchOptions.FindPattern : SearchOptions.FindPattern.ToUpper();
			PatternCharShifts = new Hashtable();
			
			// Building shift table
			PatternLength = Pattern.Length;
			int MaxShift = PatternLength;
			// Constructing the table where number
			// of columns is equal to PatternLength
			// and number of rows is equal to the
			// number of distinct chars in the pattern
			for (int i = 0; i < PatternLength; ++i) {
				if (!PatternCharShifts.ContainsKey(Pattern[i])) {
					PatternCharShifts.Add(Pattern[i], new int[PatternLength]);
				}
			}
			OtherCharShifts = new int[PatternLength];
			// Filling the last column of the
			// table with maximum shifts (pattern length)
			foreach(DictionaryEntry Row in PatternCharShifts) {
				((int[])Row.Value)[PatternLength - 1] = MaxShift;
			}
			OtherCharShifts[PatternLength - 1] = MaxShift;
			// Calculating other shifts (filling each column
			// from PatternLength - 2 to 0 (from right to left)
			for (int i = PatternLength - 1; i >= 0; --i)
			{
				// Suffix string contains the characters
				// right to the character being processsed
				string Suffix = new String(Pattern.ToCharArray(),
				                           i + 1,  PatternLength - i - 1);
				// if Pattern begins with Suffix
				// the maximum shift is equal to i + 1
				if (Pattern.StartsWith(Suffix)) {
					MaxShift = i + 1;
				}
				// Store shift for characters not present in the pattern
				OtherCharShifts[i] = MaxShift;
				// We shorten patter by one char in NewPattern.
				string NewPattern = new string(Pattern.ToCharArray(),
				                               0, Pattern.Length -1);
				if ((NewPattern.LastIndexOf(Suffix) > 0) || (Suffix.Length == 0)) {
					foreach(DictionaryEntry Row in PatternCharShifts)
					{
						string NewSuffix  = (char)Row.Key + Suffix;
						// Calculate shifts:
						//Check if there are other occurences
						//of the new suffix in the pattern
						// If several occurences exist, we need the rightmost one
						int NewSuffixPos = NewPattern.LastIndexOf(NewSuffix);
						if (NewSuffixPos >= 0) {
							((int[])Row.Value)[i] = i - NewSuffixPos;
						} else {
							((int[])Row.Value)[i] = MaxShift;
						}
						// Storing 0 if characters
						// in a row and a columnt are the same
						if ((char)Row.Key == Pattern[i]) {
							((int[])Row.Value)[i] = 0;
						}
					}
				} else {
					foreach(DictionaryEntry Row in PatternCharShifts)
					{
						// if Suffix doesn't occure in NewPattern
						// we simply use previous shift value
						((int[])Row.Value)[i] = MaxShift;
						if ((char)Row.Key == Pattern[i]) {
							((int[])Row.Value)[i] = 0;
						}
					}
				}
			}
		}
		
		/// <remarks>
		/// The find next method should search the next occurrence of the 
		/// compiled pattern in the text using the textIterator and options.
		/// </remarks>
		public SearchResult FindNext(ITextIterator textIterator)
		{
			return null;
		}
	}
}
*/
