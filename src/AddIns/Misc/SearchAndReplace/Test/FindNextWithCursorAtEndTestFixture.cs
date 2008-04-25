// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.TextEditor.Document;
using SearchAndReplace.Tests.Utils;
using NUnit.Framework;
using SearchAndReplace;

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
			if (!PropertyService.Initialized) {
				PropertyService.InitializeService(String.Empty, String.Empty, String.Empty);
			}
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
			StringTextBufferStrategy textBufferStrategy = new StringTextBufferStrategy();
			textBufferStrategy.SetContent("foo");
			doc.TextBufferStrategy = textBufferStrategy;
			
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
