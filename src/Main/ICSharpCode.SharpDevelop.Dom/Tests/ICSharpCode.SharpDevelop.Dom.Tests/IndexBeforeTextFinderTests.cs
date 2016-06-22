// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
