// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Utils;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Widgets.MyersDiff;

namespace ICSharpCode.AvalonEdit.AddIn
{
	public class DefaultChangeWatcher : IChangeWatcher, ILineTracker
	{
		CompressingTreeList<LineChangeInfo> changeList;
		IDocument document;
		TextDocument textDocument;
		IDocument baseDocument;
		IDocumentVersionProvider usedProvider;
		IDisposable watcher;
		FileName currentFileName;
		
		public event EventHandler ChangeOccurred;
		
		protected void OnChangeOccurred(EventArgs e)
		{
			if (ChangeOccurred != null) {
				ChangeOccurred(this, e);
			}
		}
		
		public LineChangeInfo GetChange(int lineNumber)
		{
			return changeList[lineNumber];
		}
		
		public void Initialize(IDocument document, FileName fileName)
		{
			if (this.document == null) {
				this.document = document;
				this.textDocument = (TextDocument)document.GetService(typeof(TextDocument));
				this.changeList = new CompressingTreeList<LineChangeInfo>((x, y) => x.Equals(y));
			}
			
			
			InitializeBaseDocument(fileName);
			if (watcher != null)
				watcher.Dispose();
			
			if (usedProvider != null)
				watcher = usedProvider.WatchBaseVersionChanges(fileName, HandleBaseVersionChanges);
			
			SetupInitialFileState(fileName != currentFileName);
			currentFileName = fileName;
			
			if (!this.textDocument.LineTrackers.Contains(this)) {
				this.textDocument.LineTrackers.Add(this);
				this.textDocument.UndoStack.PropertyChanged += UndoStackPropertyChanged;
			}
		}
		
		void HandleBaseVersionChanges(object sender, EventArgs e)
		{
			InitializeBaseDocument(currentFileName);
			SetupInitialFileState(true);
		}
		
		void InitializeBaseDocument(FileName fileName)
		{
			Stream baseFileStream = GetBaseVersion(fileName);
			if (baseFileStream != null) {
				// ReadAll() is taking care of closing the stream
				baseDocument = new ReadOnlyDocument(new StringTextSource(ReadAll(baseFileStream)), fileName);
			} else {
				// if the file is not under subversion, the document is the opened document
				if (baseDocument == null) {
					baseDocument = document.CreateDocumentSnapshot();
				}
			}
		}
		
		LineChangeInfo TransformLineChangeInfo(LineChangeInfo info)
		{
			if (info.Change == ChangeType.Unsaved)
				info.Change = ChangeType.Added;
			
			return info;
		}
		
		void SetupInitialFileState(bool update)
		{
			if (baseDocument == null) {
				if (update)
					changeList.Transform(TransformLineChangeInfo);
				else
					changeList.InsertRange(0, document.LineCount + 1, LineChangeInfo.EMPTY);
			} else {
				changeList.Clear();
				
				Dictionary<string, int> hashes = new Dictionary<string, int>();
				
				MyersDiffAlgorithm diff = new MyersDiffAlgorithm(
					new DocumentSequence(baseDocument, hashes),
					new DocumentSequence(document, hashes)
				);
				
				changeList.Add(LineChangeInfo.EMPTY);
				int lastEndLine = 0;
				
				foreach (Edit edit in diff.GetEdits()) {
					int beginLine = edit.BeginB;
					int endLine = edit.EndB;
					
					changeList.InsertRange(changeList.Count, beginLine - lastEndLine, LineChangeInfo.EMPTY);
					
					if (endLine == beginLine)
						changeList[changeList.Count - 1] = new LineChangeInfo(edit.EditType, edit.BeginA, edit.EndA);
					else
						changeList.InsertRange(changeList.Count, endLine - beginLine, new LineChangeInfo(edit.EditType, edit.BeginA, edit.EndA));
					lastEndLine = endLine;
				}
				
				changeList.InsertRange(changeList.Count, textDocument.LineCount - lastEndLine, LineChangeInfo.EMPTY);
			}
			
			OnChangeOccurred(EventArgs.Empty);
		}
		
		string ReadAll(Stream stream)
		{
			var memory = new MemoryStream();
			stream.CopyTo(memory);
			stream.Close();
			memory.Position = 0;
			return FileReader.ReadFileContent(memory, SD.FileService.DefaultFileEncoding);
		}
		
		Stream GetBaseVersion(FileName fileName)
		{
			foreach (IDocumentVersionProvider provider in VersioningServices.Instance.DocumentVersionProviders) {
				var result = provider.OpenBaseVersionAsync(fileName).GetAwaiter().GetResult();
				if (result != null) {
					usedProvider = provider;
					return result;
				}
			}

			return null;
		}
		
		void UndoStackPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "IsOriginalFile" && textDocument.UndoStack.IsOriginalFile)
				SetupInitialFileState(true);
		}
		
		void ILineTracker.BeforeRemoveLine(DocumentLine line)
		{
			changeList.RemoveAt(line.LineNumber);
		}
		
		void ILineTracker.SetLineLength(DocumentLine line, int newTotalLength)
		{
			int index = line.LineNumber;
			var info = changeList[index];
			info.Change = ChangeType.Unsaved;
			changeList[index] = info;
		}
		
		void ILineTracker.LineInserted(DocumentLine insertionPos, DocumentLine newLine)
		{
			int index = insertionPos.LineNumber;
			var newLineInfo = new LineChangeInfo(ChangeType.Unsaved, index, index);
			
			changeList[index] = newLineInfo;
			changeList.Insert(index + 1, newLineInfo);
		}
		
		void ILineTracker.RebuildDocument()
		{
			changeList.Clear();
			changeList.InsertRange(0, document.LineCount + 1, new LineChangeInfo(ChangeType.Unsaved, 1, baseDocument.LineCount));
		}
		
		void ILineTracker.ChangeComplete(DocumentChangeEventArgs e)
		{
		}
		
		bool disposed;
		
		public void Dispose()
		{
			if (!disposed) {
				if (watcher != null)
					watcher.Dispose();
				if (this.textDocument != null) {
					this.textDocument.LineTrackers.Remove(this);
					this.textDocument.UndoStack.PropertyChanged -= UndoStackPropertyChanged;
				}
				disposed = true;
			}
		}
		
		public string GetOldVersionFromLine(int lineNumber, out int newStartLine, out bool added)
		{
			LineChangeInfo info = changeList[lineNumber];
			
			added = info.Change == ChangeType.Added;
			
			if (info.Change != ChangeType.None && info.Change != ChangeType.Unsaved) {
				newStartLine = CalculateNewStartLineNumber(lineNumber);
				
				if (info.Change == ChangeType.Added)
					return "";
				
				var startDocumentLine = baseDocument.GetLineByNumber(info.OldStartLineNumber + 1);
				var endLine = baseDocument.GetLineByNumber(info.OldEndLineNumber);
				
				return TextUtilities.NormalizeNewLines(baseDocument.GetText(startDocumentLine.Offset, endLine.EndOffset - startDocumentLine.Offset), DocumentUtilities.GetLineTerminator(document, newStartLine == 0 ? 1 : newStartLine));
			}
			
			newStartLine = 0;
			return null;
		}
		
		int CalculateNewStartLineNumber(int lineNumber)
		{
			return changeList.GetStartOfRun(lineNumber);
		}
		
		int CalculateNewEndLineNumber(int lineNumber)
		{
			return changeList.GetEndOfRun(lineNumber) - 1;
		}
		
		public bool GetNewVersionFromLine(int lineNumber, out int offset, out int length)
		{
			LineChangeInfo info = changeList[lineNumber];
			
			if (info.Change != ChangeType.None && info.Change != ChangeType.Unsaved) {
				var startLine = document.GetLineByNumber(CalculateNewStartLineNumber(lineNumber));
				var endLine = document.GetLineByNumber(CalculateNewEndLineNumber(lineNumber));
				
				offset = startLine.Offset;
				length = endLine.EndOffset - startLine.Offset;
				
				if (info.Change == ChangeType.Added)
					length += endLine.DelimiterLength;
				
				return true;
			}
			
			offset = length = 0;
			return false;
		}
		
		public IDocument CurrentDocument {
			get { return document; }
		}
		
		public IDocument BaseDocument {
			get { return baseDocument; }
		}
	}
}
