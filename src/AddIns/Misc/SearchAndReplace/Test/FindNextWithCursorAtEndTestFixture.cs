// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		Search search;
		SearchResultMatch result;
		
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
			
			// Create search.
			search = new Search();
			search.TextIteratorBuilder = new ForwardTextIteratorBuilder();
			search.SearchStrategy = new BruteForceSearchStrategy();
			MockCurrentDocumentIterator currentDocIterator = new MockCurrentDocumentIterator();
			search.DocumentIterator = currentDocIterator;
			
			// Create the document to be searched.
			MockDocument doc = new MockDocument();
			doc.Text = "foo";
			
			// Create a doc info with an initial end offset right 
			// at the end of the text.
			ProvidedDocumentInformation docInfo = new ProvidedDocumentInformation(doc, @"C:\Temp\test.txt", doc.TextLength);
			currentDocIterator.Current = docInfo;
			currentDocIterator.CurrentFileName = docInfo.FileName;
			
			// Search the document.
			search.SearchStrategy.CompilePattern(null);
			result = search.FindNext(null);
		}
		
		[Test]
		public void FoundMatch()
		{
			Assert.IsNotNull(result);
		}
		
		[Test]
		public void FoundTextOffset()
		{
			Assert.AreEqual(0, result.Offset);
		}
		
		[Test]
		public void FoundTextLength()
		{
			Assert.AreEqual(3, result.Length);
		}
	}
}
