// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Editor;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Tests
{
	[TestFixture]
	public class ReadOnlyDocumentTests
	{
		[Test]
		public void EmptyReadOnlyDocument()
		{
			IDocument document = DocumentUtilitites.LoadReadOnlyDocumentFromBuffer(new StringTextBuffer(string.Empty));
			Assert.AreEqual(string.Empty, document.Text);
			Assert.AreEqual(0, document.TextLength);
			Assert.AreEqual(1, document.TotalNumberOfLines);
			Assert.AreEqual(0, document.PositionToOffset(1, 1));
			Assert.AreEqual(new Location(1, 1), document.OffsetToPosition(0));
			
			Assert.AreEqual(0, document.GetLine(1).Offset);
			Assert.AreEqual(0, document.GetLine(1).EndOffset);
			Assert.AreEqual(0, document.GetLine(1).Length);
			Assert.AreEqual(0, document.GetLine(1).TotalLength);
			Assert.AreEqual(0, document.GetLine(1).DelimiterLength);
			Assert.AreEqual(1, document.GetLine(1).LineNumber);
		}
		
		[Test]
		public void SimpleDocument()
		{
			string text = "Hello\nWorld!\r\n";
			IDocument document = DocumentUtilitites.LoadReadOnlyDocumentFromBuffer(new StringTextBuffer(text));
			Assert.AreEqual(text, document.Text);
			Assert.AreEqual(3, document.TotalNumberOfLines);
			
			Assert.AreEqual(0, document.GetLine(1).Offset);
			Assert.AreEqual(5, document.GetLine(1).EndOffset);
			Assert.AreEqual(5, document.GetLine(1).Length);
			Assert.AreEqual(6, document.GetLine(1).TotalLength);
			Assert.AreEqual(1, document.GetLine(1).DelimiterLength);
			Assert.AreEqual(1, document.GetLine(1).LineNumber);
			
			Assert.AreEqual(6, document.GetLine(2).Offset);
			Assert.AreEqual(12, document.GetLine(2).EndOffset);
			Assert.AreEqual(6, document.GetLine(2).Length);
			Assert.AreEqual(8, document.GetLine(2).TotalLength);
			Assert.AreEqual(2, document.GetLine(2).DelimiterLength);
			Assert.AreEqual(2, document.GetLine(2).LineNumber);

			Assert.AreEqual(14, document.GetLine(3).Offset);
			Assert.AreEqual(14, document.GetLine(3).EndOffset);
			Assert.AreEqual(0, document.GetLine(3).Length);
			Assert.AreEqual(0, document.GetLine(3).TotalLength);
			Assert.AreEqual(0, document.GetLine(3).DelimiterLength);
			Assert.AreEqual(3, document.GetLine(3).LineNumber);
		}
	}
}
