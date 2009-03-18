// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Drawing;

using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using IDocumentLine = ICSharpCode.SharpDevelop.Dom.Refactoring.IDocumentLine;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	/// <summary>
	/// Use this class to pass a text editor document to the refactoring API.
	/// </summary>
	public sealed class TextEditorDocument : ICSharpCode.SharpDevelop.Dom.Refactoring.IDocument
	{
		IDocument doc;
		
		public TextEditorDocument(IDocument doc)
		{
			if (doc == null)
				throw new ArgumentNullException("doc");
			this.doc = doc;
		}
		
		private sealed class TextEditorDocumentLine : IDocumentLine
		{
			IDocument doc;
			LineSegment line;
			
			public TextEditorDocumentLine(IDocument doc, LineSegment line)
			{
				this.doc = doc;
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
			
			public int LineNumber {
				get { return line.LineNumber + 1; }
			}
			
			public string Text {
				get { return doc.GetText(line.Offset, line.Length); }
			}
		}
		
		public int TextLength {
			get {
				return doc.TextLength;
			}
		}
		
		public string Text {
			get { return doc.TextContent; }
			set { doc.TextContent = value; }
		}
		
		public string GetText(int offset, int length)
		{
			return doc.GetText(offset, length);
		}
		
		public IDocumentLine GetLine(int lineNumber)
		{
			return new TextEditorDocumentLine(doc, doc.GetLineSegment(lineNumber - 1));
		}
		
		public IDocumentLine GetLineForOffset(int offset)
		{
			return new TextEditorDocumentLine(doc, doc.GetLineSegmentForOffset(offset));
		}
		
		public int PositionToOffset(int line, int column)
		{
			return doc.PositionToOffset(new TextLocation(column - 1, line - 1));
		}
		
		public ICSharpCode.NRefactory.Location OffsetToPosition(int offset)
		{
			TextLocation loc = doc.OffsetToPosition(offset);
			return new ICSharpCode.NRefactory.Location(loc.Column, loc.Line);
		}
		
		public void Insert(int offset, string text)
		{
			doc.Insert(offset, text);
		}
		
		public void Remove(int offset, int length)
		{
			doc.Remove(offset, length);
		}
		
		public void Replace(int offset, int length, string newText)
		{
			doc.Replace(offset, length, newText);
		}
		
		public char GetCharAt(int offset)
		{
			return doc.GetCharAt(offset);
		}
		
		public void StartUndoableAction()
		{
			doc.UndoStack.StartUndoGroup();
		}
		
		public void EndUndoableAction()
		{
			doc.UndoStack.EndUndoGroup();
		}
		
		public IDisposable OpenUndoGroup()
		{
			StartUndoableAction();
			return new UndoGroup { document = this };
		}
		
		sealed class UndoGroup : IDisposable
		{
			internal TextEditorDocument document;
			
			public void Dispose()
			{
				document.EndUndoableAction();
			}
		}
		
		public void UpdateView()
		{
			doc.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
			doc.CommitUpdate();
		}
		
		public int TotalNumberOfLines {
			get {
				return doc.TotalNumberOfLines;
			}
		}
	}
}
