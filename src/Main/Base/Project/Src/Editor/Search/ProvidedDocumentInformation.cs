// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.AvalonEdit.Document;
using System;
using ICSharpCode.SharpDevelop.Refactoring;

namespace ICSharpCode.SharpDevelop.Editor.Search
{
	public class ProvidedDocumentInformation
	{
		ITextEditor textEditor;
		IDocument document;
		string textBuffer;
		string fileName;
		int currentOffset;
		
		public string FileName {
			get {
				return fileName;
			}
		}
		
		public IDocument Document {
			get {
				if (document == null) {
					TextDocument textDocument = new TextDocument();
					textDocument.Text = textBuffer;
					textDocument.UndoStack.ClearAll();
					document = new AvalonEditDocumentAdapter(textDocument, null);
					this.textBuffer = null;
				}
				return document;
			}
		}
		
		public bool IsDocumentCreated {
			get { return document != null; }
		}
		
		public int CurrentOffset {
			get {
				if (textEditor != null) {
					return textEditor.Caret.Offset;
				}
				return currentOffset;
			}
			set {
				if (textEditor != null) {
					textEditor.Caret.Position = document.OffsetToPosition(value + 1);
				} else {
					currentOffset = value;
				}
			}
		}
		
		int endOffset = 0;
		public int EndOffset {
			get {
//				if (document != null) {
//					return SearchReplaceUtilities.CalcCurrentOffset(document);
//				}
				return endOffset;
			}
		}
		
		public void Replace(int offset, int length, string pattern)
		{
			this.Document.Replace(offset, length, pattern);
			
			if (offset <= CurrentOffset) {
				CurrentOffset = CurrentOffset - length + pattern.Length;
			}
		}
		
		public override bool Equals(object obj)
		{
			ProvidedDocumentInformation info = obj as ProvidedDocumentInformation;
			if (info == null) {
				return false;
			}
			return this.fileName == info.fileName &&
				this.textEditor == info.textEditor;
		}
		
		public override int GetHashCode()
		{
			return fileName.GetHashCode();
		}
		
		public ProvidedDocumentInformation(IDocument document, string fileName, int currentOffset)
		{
			this.document      = document;
			this.fileName      = fileName;
			this.endOffset = this.currentOffset = currentOffset;
		}
		
		public ProvidedDocumentInformation(IDocument document, string fileName, ITextEditor textEditor)
		{
			this.document   = document;
			this.fileName   = fileName;
			this.textEditor = textEditor;
			this.endOffset = this.CurrentOffset;
		}
		
		public ProvidedDocumentInformation(string textBuffer, string fileName, int currentOffset)
		{
			this.textBuffer    = textBuffer;
			this.fileName      = fileName;
			this.endOffset = this.currentOffset = currentOffset;
		}
	}
}
