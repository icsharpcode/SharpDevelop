// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AvalonEdit.Search;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor.Search;
using ICSharpCode.SharpDevelop.Gui;
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
	public class FindNextWithCursorAtEndTestFixture
	{
		SearchResultMatch result = null;
		
		[SetUp]
		public void SetUp()
		{
			SD.InitializeForUnitTests();
			SD.Services.AddService(typeof(IWorkbench), MockRepository.GenerateStub<IWorkbench>());
			
			// Set up SearchOptions required by the BruteForceSearchStrategy.
			SearchOptions.CurrentFindPattern = "foo";
			SearchOptions.MatchCase = false;
			SearchOptions.MatchWholeWord = false;
			
			// Create the document to be searched.
			var doc = MockRepository.GenerateStub<IDocument>();
			doc.Text = "foo";
			doc.Stub(d => d.TextLength).Return(doc.Text.Length);
			
			var location = MockRepository.GenerateStub<SearchLocation>(SearchOptions.SearchTarget, SearchOptions.LookIn, SearchOptions.LookInFiletypes, SearchOptions.IncludeSubdirectories, SearchOptions.SearchTarget == SearchTarget.CurrentSelection ? SearchManager.GetActiveSelection(true) : null);
			
			location.Stub(l => l.GenerateFileList()).Return(new[] { new FileName(@"C:\Temp\test.txt") });
			
			// Create a doc info with an initial end offset right
			// at the end of the text.
			ProvidedDocumentInformation docInfo = new ProvidedDocumentInformation(doc, @"C:\Temp\test.txt", doc.TextLength);
			
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
