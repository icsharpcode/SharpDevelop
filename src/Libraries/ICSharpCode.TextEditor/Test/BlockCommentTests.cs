// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Actions;
using ICSharpCode.TextEditor.Document;
using NUnit.Framework;
using System;

namespace ICSharpCode.TextEditor.Tests
{
	[TestFixture]
	public class BlockCommentTests
	{
		IDocument document = null;
		string commentStart = "<!--";
		string commentEnd = "-->";
		
		[SetUp]
		public void Init()
		{
			document = new DocumentFactory().CreateDocument();
			document.HighlightingStrategy = HighlightingManager.Manager.FindHighlighter("XML");
		}
		
		[Test]
		public void NoTextSelected()
		{
			document.TextContent = String.Empty;
			int selectionStartOffset = 0;
			int selectionEndOffset = 0;
			
			BlockCommentRegion commentRegion = ToggleBlockComment.FindSelectedCommentRegion(document, commentStart, commentEnd, selectionStartOffset, selectionEndOffset);
			Assert.IsNull(commentRegion, "Should not be a comment region for an empty document");
		}
		
		[Test]
		public void EntireCommentSelected()
		{
			document.TextContent = "<!---->";
			int selectionStartOffset = 0;
			int selectionEndOffset = 7;
			BlockCommentRegion expectedCommentRegion = new BlockCommentRegion(commentStart, commentEnd, 0, 4);
			
			BlockCommentRegion commentRegion = ToggleBlockComment.FindSelectedCommentRegion(document, commentStart, commentEnd, selectionStartOffset, selectionEndOffset);
			Assert.AreEqual(expectedCommentRegion, commentRegion);
		}
		
		[Test]
		public void EntireCommentAndExtraTextSelected()
		{
			document.TextContent = "a<!-- -->";
			int selectionStartOffset = 0;
			int selectionEndOffset = 9;
			BlockCommentRegion expectedCommentRegion = new BlockCommentRegion(commentStart, commentEnd, 1, 6);
			
			BlockCommentRegion commentRegion = ToggleBlockComment.FindSelectedCommentRegion(document, commentStart, commentEnd, selectionStartOffset, selectionEndOffset);
			Assert.AreEqual(expectedCommentRegion, commentRegion);
		}
		
		[Test]
		public void OnlyCommentStartSelected()
		{
			document.TextContent = "<!-- -->";
			int selectionStartOffset = 0;
			int selectionEndOffset = 4;
			BlockCommentRegion expectedCommentRegion = new BlockCommentRegion(commentStart, commentEnd, 0, 5);
			
			BlockCommentRegion commentRegion = ToggleBlockComment.FindSelectedCommentRegion(document, commentStart, commentEnd, selectionStartOffset, selectionEndOffset);
			Assert.AreEqual(expectedCommentRegion, commentRegion);
		}
		
		[Test]
		public void OnlyCommentEndSelected()
		{
			document.TextContent = "<!-- -->";
			int selectionStartOffset = 5;
			int selectionEndOffset = 8;
			BlockCommentRegion expectedCommentRegion = new BlockCommentRegion(commentStart, commentEnd, 0, 5);
			
			BlockCommentRegion commentRegion = ToggleBlockComment.FindSelectedCommentRegion(document, commentStart, commentEnd, selectionStartOffset, selectionEndOffset);
			Assert.AreEqual(expectedCommentRegion, commentRegion);
		}
		
		[Test]
		public void LastCharacterOfCommentEndSelected()
		{
			document.TextContent = "<!-- -->";
			int selectionStartOffset = 7;
			int selectionEndOffset = 8;
			BlockCommentRegion expectedCommentRegion = new BlockCommentRegion(commentStart, commentEnd, 0, 5);
			
			BlockCommentRegion commentRegion = ToggleBlockComment.FindSelectedCommentRegion(document, commentStart, commentEnd, selectionStartOffset, selectionEndOffset);
			Assert.AreEqual(expectedCommentRegion, commentRegion);
		}		
		
		[Test]
		public void CaretInsideCommentButNoSelectedText()
		{
			document.TextContent = "<!---->";
			int selectionStartOffset = 4;
			int selectionEndOffset = 4;
			BlockCommentRegion expectedCommentRegion = new BlockCommentRegion(commentStart, commentEnd, 0, 4);
			
			BlockCommentRegion commentRegion = ToggleBlockComment.FindSelectedCommentRegion(document, commentStart, commentEnd, selectionStartOffset, selectionEndOffset);
			Assert.AreEqual(expectedCommentRegion, commentRegion);
		}
		
		[Test]
		public void FirstCharacterOfCommentStartSelected()
		{
			document.TextContent = "<!-- -->";
			int selectionStartOffset = 0;
			int selectionEndOffset = 1;
			BlockCommentRegion expectedCommentRegion = new BlockCommentRegion(commentStart, commentEnd, 0, 5);
			
			BlockCommentRegion commentRegion = ToggleBlockComment.FindSelectedCommentRegion(document, commentStart, commentEnd, selectionStartOffset, selectionEndOffset);
			Assert.AreEqual(expectedCommentRegion, commentRegion);
		}	
		
		[Test]
		public void CursorJustOutsideCommentStart()
		{
			document.TextContent = "<!-- -->";
			int selectionStartOffset = 0;
			int selectionEndOffset = 0;
			
			BlockCommentRegion commentRegion = ToggleBlockComment.FindSelectedCommentRegion(document, commentStart, commentEnd, selectionStartOffset, selectionEndOffset);
			Assert.IsNull(commentRegion);
		}
		
		[Test]
		public void CursorJustOutsideCommentEnd()
		{
			document.TextContent = "<!-- -->";
			int selectionStartOffset = 8;
			int selectionEndOffset = 8;
			
			BlockCommentRegion commentRegion = ToggleBlockComment.FindSelectedCommentRegion(document, commentStart, commentEnd, selectionStartOffset, selectionEndOffset);
			Assert.IsNull(commentRegion);
		}
		
		[Test]
		public void TwoExistingBlockComments()
		{
			document.TextContent = "<a>\r\n" +
									"<!--<b></b>-->\r\n" +
									"\t<c></c>\r\n" +
									"<!--<d></d>-->\r\n" +
									"</a>";
			
			string selectedText = "<c></c>";
			int selectionStartOffset = document.TextContent.IndexOf(selectedText);
			int selectionEndOffset = selectionStartOffset + selectedText.Length;
			
			BlockCommentRegion commentRegion = ToggleBlockComment.FindSelectedCommentRegion(document, commentStart, commentEnd, selectionStartOffset, selectionEndOffset);
			Assert.IsNull(commentRegion);
		}
	}
}
