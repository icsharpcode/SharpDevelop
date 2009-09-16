// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace ICSharpCode.AvalonEdit.Document
{
	[TestFixture]
	public class LineManagerTests
	{
		TextDocument document;
		
		[SetUp]
		public void SetUp()
		{
			document = new TextDocument();
		}
		
		[Test]
		public void CheckEmptyDocument()
		{
			Assert.AreEqual("", document.Text);
			Assert.AreEqual(0, document.TextLength);
			Assert.AreEqual(1, document.LineCount);
		}
		
		[Test]
		public void CheckClearingDocument()
		{
			document.Text = "Hello,\nWorld!";
			Assert.AreEqual(2, document.LineCount);
			document.Text = "";
			Assert.AreEqual("", document.Text);
			Assert.AreEqual(0, document.TextLength);
			Assert.AreEqual(1, document.LineCount);
		}
		
		[Test]
		public void CheckGetLineInEmptyDocument()
		{
			Assert.AreEqual(1, document.Lines.Count);
			List<DocumentLine> lines = new List<DocumentLine>(document.Lines);
			Assert.AreEqual(1, lines.Count);
			DocumentLine line = document.Lines[0];
			Assert.AreSame(line, lines[0]);
			Assert.AreSame(line, document.GetLineByNumber(1));
			Assert.AreSame(line, document.GetLineByOffset(0));
		}
		
		[Test]
		public void CheckLineSegmentInEmptyDocument()
		{
			DocumentLine line = document.GetLineByNumber(1);
			Assert.AreEqual(1, line.LineNumber);
			Assert.AreEqual(0, line.Offset);
			Assert.IsFalse(line.IsDeleted);
			Assert.AreEqual(0, line.Length);
			Assert.AreEqual(0, line.TotalLength);
			Assert.AreEqual(0, line.DelimiterLength);
			Assert.AreEqual("", line.Text);
		}
		
		[Test]
		public void LineIndexOfTest()
		{
			DocumentLine line = document.GetLineByNumber(1);
			Assert.AreEqual(0, document.Lines.IndexOf(line));
			DocumentLine lineFromOtherDocument = new TextDocument().GetLineByNumber(1);
			Assert.AreEqual(-1, document.Lines.IndexOf(lineFromOtherDocument));
			document.Text = "a\nb\nc";
			DocumentLine middleLine = document.GetLineByNumber(2);
			Assert.AreEqual(1, document.Lines.IndexOf(middleLine));
			document.Remove(1, 3);
			Assert.IsTrue(middleLine.IsDeleted);
			Assert.AreEqual(-1, document.Lines.IndexOf(middleLine));
		}
		
		[Test]
		public void InsertInEmptyDocument()
		{
			document.Insert(0, "a");
			Assert.AreEqual(document.LineCount, 1);
			DocumentLine line = document.GetLineByNumber(1);
			Assert.AreEqual("a", line.Text);
		}
		
		[Test]
		public void SetText()
		{
			document.Text = "a";
			Assert.AreEqual(document.LineCount, 1);
			DocumentLine line = document.GetLineByNumber(1);
			Assert.AreEqual("a", line.Text);
		}
		
		[Test]
		public void InsertNothing()
		{
			document.Insert(0, "");
			Assert.AreEqual(document.LineCount, 1);
			Assert.AreEqual(document.TextLength, 0);
		}
		
		[Test, ExpectedException(typeof(ArgumentNullException))]
		public void InsertNull()
		{
			document.Insert(0, null);
		}
		
		[Test, ExpectedException(typeof(ArgumentNullException))]
		public void SetTextNull()
		{
			document.Text = null;
		}
		
		[Test]
		public void RemoveNothing()
		{
			document.Remove(0, 0);
			Assert.AreEqual(document.LineCount, 1);
			Assert.AreEqual(document.TextLength, 0);
		}
		
		[Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void GetCharAt0EmptyDocument()
		{
			document.GetCharAt(0);
		}
		
		[Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void GetCharAtNegativeOffset()
		{
			document.Text = "a\nb";
			document.GetCharAt(-1);
		}
		
		[Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void GetCharAtEndOffset()
		{
			document.Text = "a\nb";
			document.GetCharAt(document.TextLength);
		}
		
		[Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void InsertAtNegativeOffset()
		{
			document.Text = "a\nb";
			document.Insert(-1, "text");
		}
		
		[Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void InsertAfterEndOffset()
		{
			document.Text = "a\nb";
			document.Insert(4, "text");
		}
		
		[Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void RemoveNegativeAmount()
		{
			document.Text = "abcd";
			document.Remove(2, -1);
		}
		
		[Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void RemoveTooMuch()
		{
			document.Text = "abcd";
			document.Remove(2, 10);
		}
		
		[Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void GetLineByNumberNegative()
		{
			document.Text = "a\nb";
			document.GetLineByNumber(-1);
		}
		
		[Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void GetLineByNumberTooHigh()
		{
			document.Text = "a\nb";
			document.GetLineByNumber(3);
		}
		
		[Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void GetLineByOffsetNegative()
		{
			document.Text = "a\nb";
			document.GetLineByOffset(-1);
		}
		
		
		[Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void GetLineByOffsetToHigh()
		{
			document.Text = "a\nb";
			document.GetLineByOffset(10);
		}
		
		[Test]
		public void InsertAtEndOffset()
		{
			document.Text = "a\nb";
			CheckDocumentLines("a",
			                   "b");
			document.Insert(3, "text");
			CheckDocumentLines("a",
			                   "btext");
		}
		
		[Test]
		public void GetCharAt()
		{
			document.Text = "a\r\nb";
			Assert.AreEqual('a', document.GetCharAt(0));
			Assert.AreEqual('\r', document.GetCharAt(1));
			Assert.AreEqual('\n', document.GetCharAt(2));
			Assert.AreEqual('b', document.GetCharAt(3));
		}
		
		[Test]
		public void CheckMixedNewLineTest()
		{
			const string mixedNewlineText = "line 1\nline 2\r\nline 3\rline 4";
			document.Text = mixedNewlineText;
			Assert.AreEqual(mixedNewlineText, document.Text);
			Assert.AreEqual(4, document.LineCount);
			for (int i = 1; i < 4; i++) {
				DocumentLine line = document.GetLineByNumber(i);
				Assert.AreEqual(i, line.LineNumber);
				Assert.AreEqual("line " + i, line.Text);
			}
			Assert.AreEqual(1, document.GetLineByNumber(1).DelimiterLength);
			Assert.AreEqual(2, document.GetLineByNumber(2).DelimiterLength);
			Assert.AreEqual(1, document.GetLineByNumber(3).DelimiterLength);
			Assert.AreEqual(0, document.GetLineByNumber(4).DelimiterLength);
		}
		
		[Test]
		public void LfCrIsTwoNewLinesTest()
		{
			document.Text = "a\n\rb";
			Assert.AreEqual("a\n\rb", document.Text);
			CheckDocumentLines("a",
			                   "",
			                   "b");
		}
		
		[Test]
		public void RemoveFirstPartOfDelimiter()
		{
			document.Text = "a\r\nb";
			document.Remove(1, 1);
			Assert.AreEqual("a\nb", document.Text);
			CheckDocumentLines("a",
			                   "b");
		}
		
		[Test]
		public void RemoveLineContentAndJoinDelimiters()
		{
			document.Text = "a\rb\nc";
			document.Remove(2, 1);
			Assert.AreEqual("a\r\nc", document.Text);
			CheckDocumentLines("a",
			                   "c");
		}
		
		[Test]
		public void RemoveLineContentAndJoinDelimiters2()
		{
			document.Text = "a\rb\nc\nd";
			document.Remove(2, 3);
			Assert.AreEqual("a\r\nd", document.Text);
			CheckDocumentLines("a",
			                   "d");
		}
		
		[Test]
		public void RemoveLineContentAndJoinDelimiters3()
		{
			document.Text = "a\rb\r\nc";
			document.Remove(2, 2);
			Assert.AreEqual("a\r\nc", document.Text);
			CheckDocumentLines("a",
			                   "c");
		}
		
		[Test]
		public void RemoveLineContentAndJoinNonMatchingDelimiters()
		{
			document.Text = "a\nb\nc";
			document.Remove(2, 1);
			Assert.AreEqual("a\n\nc", document.Text);
			CheckDocumentLines("a",
			                   "",
			                   "c");
		}
		
		[Test]
		public void RemoveMultilineUpToFirstPartOfDelimiter()
		{
			document.Text = "0\n1\r\n2";
			document.Remove(1, 3);
			Assert.AreEqual("0\n2", document.Text);
			CheckDocumentLines("0",
			                   "2");
		}
		
		[Test]
		public void RemoveSecondPartOfDelimiter()
		{
			document.Text = "a\r\nb";
			document.Remove(2, 1);
			Assert.AreEqual("a\rb", document.Text);
			CheckDocumentLines("a",
			                   "b");
		}
		
		[Test]
		public void RemoveFromSecondPartOfDelimiter()
		{
			document.Text = "a\r\nb\nc";
			document.Remove(2, 3);
			Assert.AreEqual("a\rc", document.Text);
			CheckDocumentLines("a",
			                   "c");
		}
		
		[Test]
		public void RemoveFromSecondPartOfDelimiterToDocumentEnd()
		{
			document.Text = "a\r\nb";
			document.Remove(2, 2);
			Assert.AreEqual("a\r", document.Text);
			CheckDocumentLines("a",
			                   "");
		}
		
		[Test]
		public void RemoveUpToMatchingDelimiter1()
		{
			document.Text = "a\r\nb\nc";
			document.Remove(2, 2);
			Assert.AreEqual("a\r\nc", document.Text);
			CheckDocumentLines("a",
			                   "c");
		}
		
		[Test]
		public void RemoveUpToMatchingDelimiter2()
		{
			document.Text = "a\r\nb\r\nc";
			document.Remove(2, 3);
			Assert.AreEqual("a\r\nc", document.Text);
			CheckDocumentLines("a",
			                   "c");
		}
		
		[Test]
		public void RemoveUpToNonMatchingDelimiter()
		{
			document.Text = "a\r\nb\rc";
			document.Remove(2, 2);
			Assert.AreEqual("a\r\rc", document.Text);
			CheckDocumentLines("a",
			                   "",
			                   "c");
		}
		
		[Test]
		public void RemoveTwoCharDelimiter()
		{
			document.Text = "a\r\nb";
			document.Remove(1, 2);
			Assert.AreEqual("ab", document.Text);
			CheckDocumentLines("ab");
		}
		
		[Test]
		public void RemoveOneCharDelimiter()
		{
			document.Text = "a\nb";
			document.Remove(1, 1);
			Assert.AreEqual("ab", document.Text);
			CheckDocumentLines("ab");
		}
		
		void CheckDocumentLines(params string[] lines)
		{
			Assert.AreEqual(lines.Length, document.LineCount, "LineCount");
			for (int i = 0; i < lines.Length; i++) {
				Assert.AreEqual(lines[i], document.Lines[i].Text, "Text of line " + (i + 1));
			}
		}
		
		[Test]
		public void FixUpFirstPartOfDelimiter()
		{
			document.Text = "a\n\nb";
			document.Replace(1, 1, "\r");
			Assert.AreEqual("a\r\nb", document.Text);
			CheckDocumentLines("a",
			                   "b");
		}
		
		[Test]
		public void FixUpSecondPartOfDelimiter()
		{
			document.Text = "a\r\rb";
			document.Replace(2, 1, "\n");
			Assert.AreEqual("a\r\nb", document.Text);
			CheckDocumentLines("a",
			                   "b");
		}
		
		[Test]
		public void InsertInsideDelimiter()
		{
			document.Text = "a\r\nc";
			document.Insert(2, "b");
			Assert.AreEqual("a\rb\nc", document.Text);
			CheckDocumentLines("a",
			                   "b",
			                   "c");
		}
		
		[Test]
		public void InsertInsideDelimiter2()
		{
			document.Text = "a\r\nd";
			document.Insert(2, "b\nc");
			Assert.AreEqual("a\rb\nc\nd", document.Text);
			CheckDocumentLines("a",
			                   "b",
			                   "c",
			                   "d");
		}
		
		[Test]
		public void InsertInsideDelimiter3()
		{
			document.Text = "a\r\nc";
			document.Insert(2, "b\r");
			Assert.AreEqual("a\rb\r\nc", document.Text);
			CheckDocumentLines("a",
			                   "b",
			                   "c");
		}
		
		[Test]
		public void ExtendDelimiter1()
		{
			document.Text = "a\nc";
			document.Insert(1, "b\r");
			Assert.AreEqual("ab\r\nc", document.Text);
			CheckDocumentLines("ab",
			                   "c");
		}
		
		[Test]
		public void ExtendDelimiter2()
		{
			document.Text = "a\rc";
			document.Insert(2, "\nb");
			Assert.AreEqual("a\r\nbc", document.Text);
			CheckDocumentLines("a",
			                   "bc");
		}
		
		[Test]
		public void ReplaceLineContentBetweenMatchingDelimiters()
		{
			document.Text = "a\rb\nc";
			document.Replace(2, 1, "x");
			Assert.AreEqual("a\rx\nc", document.Text);
			CheckDocumentLines("a",
			                   "x",
			                   "c");
		}
	}
}
