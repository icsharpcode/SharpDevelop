// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using System.Diagnostics;

namespace ICSharpCode.SharpDevelop.Editor
{
	/// <summary>
	/// Implements IRefactoringDocument by forwarding calls to an IDocument.
	/// </summary>
	public class RefactoringDocumentAdapter : IRefactoringDocument
	{
		readonly IDocument document;
		
		public RefactoringDocumentAdapter(IDocument document)
		{
			if (document == null)
				throw new ArgumentNullException("document");
			this.document = document;
		}
		
		sealed class LineAdapter : IRefactoringDocumentLine
		{
			readonly IDocumentLine line;
			
			public LineAdapter(IDocumentLine line)
			{
				Debug.Assert(line != null);
				this.line = line;
			}
			
			public int Offset {
				get { return line.Offset; }
			}
			
			public int Length {
				get { return line.Length; }
			}
			
			public int TotalLength {
				get { return line.TotalLength; }
			}
			
			public int DelimiterLength {
				get { return line.DelimiterLength; }
			}
			
			public int LineNumber {
				get { return line.LineNumber; }
			}
			
			public string Text {
				get { return line.Text; }
			}
		}
		
		public int TextLength {
			get { return document.TextLength; }
		}
		
		public int TotalNumberOfLines {
			get { return document.TotalNumberOfLines; }
		}
		
		public string Text {
			get { return document.Text; }
			set { document.Text = value; }
		}
		
		public IRefactoringDocumentLine GetLine(int lineNumber)
		{
			return new LineAdapter(document.GetLine(lineNumber));
		}
		
		public IRefactoringDocumentLine GetLineForOffset(int offset)
		{
			return new LineAdapter(document.GetLineForOffset(offset));
		}
		
		public int PositionToOffset(int line, int column)
		{
			return document.PositionToOffset(line, column);
		}
		
		public ICSharpCode.NRefactory.Location OffsetToPosition(int offset)
		{
			return document.OffsetToPosition(offset);
		}
		
		public void Insert(int offset, string text)
		{
			document.Insert(offset, text);
		}
		
		public void Remove(int offset, int length)
		{
			document.Remove(offset, length);
		}
		
		public void Replace(int offset, int length, string newText)
		{
			document.Replace(offset, length, newText);
		}
		
		public char GetCharAt(int offset)
		{
			return document.GetCharAt(offset);
		}
		
		public string GetText(int offset, int length)
		{
			return document.GetText(offset, length);
		}
		
		public void StartUndoableAction()
		{
			document.StartUndoableAction();
		}
		
		public void EndUndoableAction()
		{
			document.EndUndoableAction();
		}
		
		public IDisposable OpenUndoGroup()
		{
			return document.OpenUndoGroup();
		}
		
		public object GetService(Type serviceType)
		{
			return document.GetService(serviceType);
		}
	}
}
