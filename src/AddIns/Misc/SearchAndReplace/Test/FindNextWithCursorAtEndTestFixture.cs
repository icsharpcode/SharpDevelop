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
using ICSharpCode.AvalonEdit.Search;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor.Search;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Workbench;
using NUnit.Framework;
using Rhino.Mocks;

namespace SearchAndReplace.Tests
{
	/// <summary>
	/// SD2-857 - The search fails to find a string if the cursor is
	/// positioned at the end of the last line. For example, if the
	/// text editor has the text:
	/// "foo"
	/// without the quotes and no newline after this text, then
	/// putting the cursor at the end after the last 'o' and
	/// opening the search dialog and doing a find next for 'foo'
	/// the string is not found.
	/// </summary>
	[TestFixture]
	[Ignore("FindNext fails because no editor is open (??)")]
	public class FindNextWithCursorAtEndTestFixture : SDTestFixtureBase
	{
		SearchResultMatch result = null;
		
		public override void FixtureSetUp()
		{
			base.FixtureSetUp();
			SD.Services.AddService(typeof(IWorkbench), MockRepository.GenerateStub<IWorkbench>());
			
			// Set up SearchOptions required by the BruteForceSearchStrategy.
			SearchOptions.CurrentFindPattern = "foo";
			SearchOptions.MatchCase = false;
			SearchOptions.MatchWholeWord = false;
			
			// Create the document to be searched.
			var doc = new ReadOnlyDocument("foo");
			
			var location = MockRepository.GenerateStub<SearchLocation>(SearchOptions.SearchTarget, SearchOptions.LookIn, SearchOptions.LookInFiletypes, SearchOptions.IncludeSubdirectories, null);
			
			location.Stub(l => l.GenerateFileList()).Return(new[] { new FileName(@"C:\Temp\test.txt") });
			
			// Search the document.
			var strategy = SearchStrategyFactory.Create(SearchOptions.FindPattern, !SearchOptions.MatchCase, SearchOptions.MatchWholeWord, SearchOptions.SearchMode);
			result = SearchManager.FindNext(strategy, location);
		}
		
		[Test]
		public void FoundMatch()
		{
			Assert.IsNotNull(result);
		}
		
		[Test]
		public void FoundTextOffset()
		{
			Assert.AreEqual(0, result.StartOffset);
		}
		
		[Test]
		public void FoundTextLength()
		{
			Assert.AreEqual(3, result.Length);
		}
	}
}
