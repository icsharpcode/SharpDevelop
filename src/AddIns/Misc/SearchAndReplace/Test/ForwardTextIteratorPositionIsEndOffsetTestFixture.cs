// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using SearchAndReplace.Tests.Utils;
using ICSharpCode.TextEditor.Document;
using SearchAndReplace;
using NUnit.Framework;

namespace SearchAndReplace.Tests
{
	/// <summary>
	/// The forward text iterator never finishes if the initial
	/// offset is right at the end of the text.
	/// </summary>
	[TestFixture]
	public class ForwardTextIteratorPositionIsEndOffsetTestFixture
	{
		ForwardTextIterator forwardTextIterator;
		
		[SetUp]
		public void SetUp()
		{
			// Create the document to be iterated through.
			MockDocument doc = new MockDocument();
			StringTextBufferStrategy textBufferStrategy = new StringTextBufferStrategy();
			textBufferStrategy.SetContent("bar");
			doc.TextBufferStrategy = textBufferStrategy;
			
			// Create a doc info with an initial end offset right 
			// at the end of the text.
			ProvidedDocumentInformation docInfo = new ProvidedDocumentInformation(doc, 
				@"C:\Temp\test.txt", 
				doc.TextLength);
			
			// Create the forward iterator.
			forwardTextIterator = new ForwardTextIterator(docInfo);
		}
		
		/// <summary>
		/// Note that we cannot move 4 chars in one go with the first
		/// call to MoveAhead due to another bug in the ForwardTextIterator.
		/// If the iterator has been reset then the first call to MoveAhead
		/// always just moves to the DocInfo's EndOffset ignoring any
		/// number of chars passed in as an argument to MoveAhead.
		/// </summary>
		[Test]
		public void MoveAheadFourChars()
		{
			// First move ahead does nothing if the forward text
			// iterator was reset. I consider this a bug. All this
			// call does is put the iterator at the current position to
			// start the iteration. In this case it is after the last
			// character in the string, offset 3.
			forwardTextIterator.MoveAhead(1);
			
			bool firstMove = forwardTextIterator.MoveAhead(1);
			bool secondMove = forwardTextIterator.MoveAhead(1);
			bool thirdMove = forwardTextIterator.MoveAhead(1);
			bool fourthMove = forwardTextIterator.MoveAhead(1);
			
			Assert.IsTrue(firstMove);
			Assert.IsTrue(secondMove);
			Assert.IsTrue(thirdMove);
			Assert.IsFalse(fourthMove);
		}
		
		/// <summary>
		/// Check that after moving ahead one character the first
		/// char in the string is selected.
		/// </summary>
		[Test]
		public void MoveAheadOneChar()
		{
			// First move ahead does nothing see comment in MoveAheadThreeChars
			// test.
			forwardTextIterator.MoveAhead(1);
			
			// Move one char.
			forwardTextIterator.MoveAhead(1);
			
			Assert.AreEqual(0, forwardTextIterator.Position);
		}
		
		/// <summary>
		/// Tests the unusual scenario when a find all is done for a
		/// single character when the starting iteration position is after the
		/// last character in a string and the last character is a match.
		/// </summary>
		[Test]
		public void ChangePositionAfterThreeMoves()
		{
			// First move does nothing.
			forwardTextIterator.MoveAhead(1);
			
			bool firstMove = forwardTextIterator.MoveAhead(1);
			bool secondMove = forwardTextIterator.MoveAhead(1);
			bool thirdMove = forwardTextIterator.MoveAhead(1);
			
			// Change position to simulate the search when it finds a
			// match at the very last offset. Here the search will
			// set the position to be the end offset.
			forwardTextIterator.Position = 3;
			
			bool fourthMove = forwardTextIterator.MoveAhead(1);
			
			Assert.IsTrue(firstMove);
			Assert.IsTrue(secondMove);
			Assert.IsTrue(thirdMove);
			Assert.IsFalse(fourthMove);
		}
	}
}
