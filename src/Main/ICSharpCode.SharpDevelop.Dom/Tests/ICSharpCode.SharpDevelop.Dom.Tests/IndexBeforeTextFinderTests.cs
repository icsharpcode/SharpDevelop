// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Dom.Tests.NUnitHelpers;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Dom.Tests
{
	[TestFixture]
	public class IndexBeforeTextFinderTests
	{
		TextFinder textFinder;
		IndexBeforeTextFinderHelper helper;
		
		void CreateIndexBeforeTextFinderWithSearchTextOf(string searchText)
		{
			helper = new IndexBeforeTextFinderHelper();
			textFinder = helper.CreateIndexBeforeTextFinder(searchText);
		}
		
		void AssertTextFindMatchesAreEqual(TextFinderMatch expectedMatch, TextFinderMatch actualMatch)
		{
			string expectedMatchAsString = GetTextFinderMatchAsString(expectedMatch);
			string actualMatchAsString = GetTextFinderMatchAsString(actualMatch);
			Assert.AreEqual(expectedMatchAsString, actualMatchAsString);
		}
		
		string GetTextFinderMatchAsString(TextFinderMatch match)
		{
			return String.Format(
				"Position: {0}, Length: {1}, ResolvePosition: {2}",
				match.Position,
				match.Length,
				match.ResolvePosition);
		}

		[Test]
		public void Find_SearchingForSquareBracketCharacterAndInputTextHasNoSquareBracketCharacter_ReturnsEmptyTextFinderMatch()
		{
			CreateIndexBeforeTextFinderWithSearchTextOf("[");
			TextFinderMatch match = textFinder.Find("abc", 0);
			
			AssertTextFindMatchesAreEqual(TextFinderMatch.Empty, match);
		}
		
		[Test]
		public void Find_SearchingForSquareBracketCharacterAndInputTextHasSquareBracketAtPositionOne_ReturnsTextFinderMatchForPositionOne()
		{
			CreateIndexBeforeTextFinderWithSearchTextOf("[");
			TextFinderMatch match = textFinder.Find("a[0]", 0);
			
			TextFinderMatch expectedMatch = 
				new TextFinderMatch(position: 1, length: 1, resolvePosition: 0);
			
			AssertTextFindMatchesAreEqual(expectedMatch, match);
		}
		
		[Test]
		public void Find_SearchingForSquareBracketCharacterAndInputTextHasSquareBracketCharacterAtPositionZero_ReturnsEmptyTextFinderMatch()
		{
			CreateIndexBeforeTextFinderWithSearchTextOf("[");
			TextFinderMatch match = textFinder.Find("[assembly: AssemblyCulture(\"\")]", 0);
			
			AssertTextFindMatchesAreEqual(TextFinderMatch.Empty, match);
		}
	}
}
