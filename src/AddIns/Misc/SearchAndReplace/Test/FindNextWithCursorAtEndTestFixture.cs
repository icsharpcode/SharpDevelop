// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.AvalonEdit.Search;
using ICSharpCode.SharpDevelop.Editor.Search;
using System;
using ICSharpCode.Core;
using NUnit.Framework;
using SearchAndReplace;
using SearchAndReplace.Tests.Utils;

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
	public class FindNextWithCursorAtEndTestFixture
	{
		SearchResultMatch result = null;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			PropertyService.InitializeServiceForUnitTests();
		}
		
		[SetUp]
		public void SetUp()
		{
			// Set up SearchOptions required by the BruteForceSearchStrategy.
			SearchOptions.CurrentFindPattern = "foo";
			SearchOptions.MatchCase = false;
			SearchOptions.MatchWholeWord = false;
			
			// Create the document to be searched.
			MockDocument doc = new MockDocument();
			doc.Text = "foo";
			
			// Create a doc info with an initial end offset right 
			// at the end of the text.
			ProvidedDocumentInformation docInfo = new ProvidedDocumentInformation(doc, @"C:\Temp\test.txt", doc.TextLength);
			
			// Search the document.
//			result = SearchManager.FindNext(SearchOptions.CurrentFindPattern, !SearchOptions.MatchCase, SearchOptions.MatchWholeWord, SearchMode.Normal, 
//			);
		}
		
		[Test, Ignore]
		public void FoundMatch()
		{
			Assert.IsNotNull(result);
		}
		
		[Test, Ignore]
		public void FoundTextOffset()
		{
			Assert.AreEqual(0, result.StartOffset);
		}
		
		[Test, Ignore]
		public void FoundTextLength()
		{
			Assert.AreEqual(3, result.Length);
		}
	}
}
