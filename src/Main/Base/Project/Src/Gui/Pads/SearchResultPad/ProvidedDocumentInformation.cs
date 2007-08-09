// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace SearchAndReplace
{
	public class ProvidedDocumentInformation
	{
		IDocument           document;
		ITextBufferStrategy textBuffer;
		string              fileName;
		int                 currentOffset;
		TextAreaControl     textAreaControl = null;
		
		public ITextBufferStrategy TextBuffer {
			get {
				return textBuffer;
			}
			set {
				textBuffer = value;
			}
		}
		
		public string FileName {
			get {
				return fileName;
			}
		}
		
		public IDocument Document {
			get {
				return document;
			}
		}
		
		public int CurrentOffset {
			get {
				if (textAreaControl != null) {
					return textAreaControl.Caret.Offset;
				}
				return currentOffset;
			}
			set {
				if (textAreaControl != null) {
					textAreaControl.Caret.Position = document.OffsetToPosition(value + 1);
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
			if (document != null) {
				document.Replace(offset, length, pattern);
			} else {
				textBuffer.Replace(offset, length, pattern);
			}
			
			if (offset <= CurrentOffset) {
				CurrentOffset = CurrentOffset - length + pattern.Length;
			}
		}
		
		public IDocument CreateDocument()
		{
			if (document != null) {
				return document;
			}
			return new DocumentFactory().CreateFromTextBuffer(textBuffer);
		}		
		
		public override bool Equals(object obj)
		{
			ProvidedDocumentInformation info = obj as ProvidedDocumentInformation;
			if (info == null) {
				return false;
			}
			return this.fileName == info.fileName && 
				this.textAreaControl == info.textAreaControl;
		}
		
		public override int GetHashCode()
		{
			return fileName.GetHashCode();
		}
		
		public ProvidedDocumentInformation(IDocument document, string fileName, int currentOffset)
		{
			this.document      = document;
			this.textBuffer    = document.TextBufferStrategy;
			this.fileName      = fileName;
			this.endOffset = this.currentOffset = currentOffset;
		}
		
		public ProvidedDocumentInformation(IDocument document, string fileName, TextAreaControl textAreaControl)
		{
			this.document   = document;
			this.textBuffer = document.TextBufferStrategy;
			this.fileName   = fileName;
			this.textAreaControl = textAreaControl;
			this.endOffset = this.CurrentOffset;
		}
		
		public ProvidedDocumentInformation(ITextBufferStrategy textBuffer, string fileName, int currentOffset)
		{
			this.textBuffer    = textBuffer;
			this.fileName      = fileName;
			this.endOffset = this.currentOffset = currentOffset;
		}
	}
}
