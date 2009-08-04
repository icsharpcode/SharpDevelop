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
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	using IDocument = ICSharpCode.TextEditor.Document.IDocument;
	
	/// <summary>
	/// Use this class to pass a text editor document to the refactoring API.
	/// </summary>
	public sealed class TextEditorDocument : ICSharpCode.SharpDevelop.Editor.IDocument
	{
		ICSharpCode.TextEditor.Document.IDocument doc;
		
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
			
			public int DelimiterLength {
				get { return line.DelimiterLength; }
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
		
		public event EventHandler TextChanged {
			add { doc.TextContentChanged += value; }
			remove { doc.TextContentChanged -= value; }
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
		
		public ITextAnchor CreateAnchor(int offset)
		{
			LineSegment lineSegment = doc.GetLineSegmentForOffset(offset);
			return new AnchorAdapter(lineSegment.CreateAnchor(offset - lineSegment.Offset));
		}
		
		public object GetService(Type serviceType)
		{
			if (serviceType == typeof(ICSharpCode.TextEditor.Document.IDocument))
				return doc;
			else
				return null;
		}
		
		public System.IO.TextReader CreateReader()
		{
			throw new NotImplementedException();
		}
		
		public ITextBuffer CreateSnapshot()
		{
			throw new NotImplementedException();
		}
		
		public ITextBuffer CreateSnapshot(int offset, int length)
		{
			throw new NotImplementedException();
		}
		
		sealed class AnchorAdapter : ITextAnchor
		{
			readonly TextAnchor anchor;
			
			public AnchorAdapter(TextAnchor anchor)
			{
				this.anchor = anchor;
			}
			
			#region Forward Deleted Event
			EventHandler deleted;
			
			public event EventHandler Deleted {
				add {
					// we cannot simply forward the event handler because
					// that would raise the event with an incorrect sender
					if (deleted == null && value != null)
						anchor.Deleted += OnDeleted;
					deleted += value;
				}
				remove {
					deleted -= value;
					if (deleted == null)
						anchor.Deleted -= OnDeleted;
				}
			}
			
			void OnDeleted(object sender, EventArgs e)
			{
				// raise event with correct sender
				if (deleted != null)
					deleted(this, e);
			}
			#endregion
			
			public ICSharpCode.NRefactory.Location Location {
				get {
					TextLocation loc = anchor.Location;
					return new ICSharpCode.NRefactory.Location(
						loc.Column + 1,
						loc.Line + 1
					);
				}
			}
			
			public int Offset {
				get { return anchor.Offset; }
			}
			
			public ICSharpCode.SharpDevelop.Editor.AnchorMovementType MovementType {
				get {
					switch (anchor.MovementType) {
						case ICSharpCode.TextEditor.Document.AnchorMovementType.AfterInsertion:
							return ICSharpCode.SharpDevelop.Editor.AnchorMovementType.AfterInsertion;
						case ICSharpCode.TextEditor.Document.AnchorMovementType.BeforeInsertion:
							return ICSharpCode.SharpDevelop.Editor.AnchorMovementType.BeforeInsertion;
						default:
							throw new NotSupportedException();
					}
				}
				set {
					switch (value) {
						case ICSharpCode.SharpDevelop.Editor.AnchorMovementType.AfterInsertion:
							anchor.MovementType = ICSharpCode.TextEditor.Document.AnchorMovementType.AfterInsertion;
							break;
						case ICSharpCode.SharpDevelop.Editor.AnchorMovementType.BeforeInsertion:
							anchor.MovementType = ICSharpCode.TextEditor.Document.AnchorMovementType.BeforeInsertion;
							break;
						default:
							throw new NotSupportedException();
					}
				}
			}
			
			public bool SurviveDeletion {
				get { return false; }
				set {
					if (value)
						throw new NotSupportedException();
				}
			}
			
			public bool IsDeleted {
				get { return anchor.IsDeleted; }
			}
			
			public int Line {
				get { return anchor.LineNumber + 1; }
			}
			
			public int Column {
				get { return anchor.ColumnNumber + 1; }
			}
		}
	}
}
