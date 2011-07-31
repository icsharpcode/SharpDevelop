// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using ICSharpCode.AvalonEdit.AddIn.MyersDiff;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Utils;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;

namespace ICSharpCode.AvalonEdit.AddIn
{
	public class DefaultChangeWatcher : IChangeWatcher, ILineTracker
	{
		CompressingTreeList<LineChangeInfo> changeList;
		IDocument document;
		TextDocument textDocument;
		IDocument baseDocument;
		
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
		
		public void Initialize(IDocument document)
		{
			if (changeList != null && changeList.Any())
				return;
			
			this.document = document;
			this.textDocument = (TextDocument)document.GetService(typeof(TextDocument));
			this.changeList = new CompressingTreeList<LineChangeInfo>((x, y) => x.Equals(y));
			
			Stream baseFileStream = GetBaseVersion();
			
			// TODO : update baseDocument on VCS actions
			if (baseFileStream != null) {
				// ReadAll() is taking care of closing the stream
				baseDocument = DocumentUtilitites.LoadReadOnlyDocumentFromBuffer(new StringTextBuffer(ReadAll(baseFileStream)));
			} else {
				if (baseDocument == null) {
					// if the file is not under subversion, the document is the opened document
					var doc = new TextDocument(textDocument.Text);
					baseDocument = new AvalonEditDocumentAdapter(doc, null);
				}
			}
			
			SetupInitialFileState(false);
			
			this.textDocument.LineTrackers.Add(this);
			this.textDocument.UndoStack.PropertyChanged += UndoStackPropertyChanged;
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
					changeList.InsertRange(0, document.TotalNumberOfLines + 1, LineChangeInfo.Empty);
			} else {
				changeList.Clear();
				
				Dictionary<string, int> hashes = new Dictionary<string, int>();
				
				MyersDiff.MyersDiff diff = new MyersDiff.MyersDiff(
					new DocumentSequence(baseDocument, hashes),
					new DocumentSequence(document, hashes)
				);
				
				changeList.Add(LineChangeInfo.Empty);
				int lastEndLine = 0;
				
				foreach (Edit edit in diff.GetEdits()) {
					int beginLine = edit.BeginB;
					int endLine = edit.EndB;
					
					changeList.InsertRange(changeList.Count, beginLine - lastEndLine, LineChangeInfo.Empty);
					
					LineChangeInfo change = new LineChangeInfo(edit.EditType, edit.BeginA, edit.BeginB, edit.EndA, edit.EndB);
					if (endLine == beginLine)
						changeList[changeList.Count - 1] = change;
					else
						changeList.InsertRange(changeList.Count, endLine - beginLine, change);
					
					lastEndLine = endLine;
				}
				
				changeList.InsertRange(changeList.Count, textDocument.LineCount - lastEndLine, LineChangeInfo.Empty);
			}
			
			OnChangeOccurred(EventArgs.Empty);
		}
		
		string ReadAll(Stream stream)
		{
			using (StreamReader reader = new StreamReader(stream)) {
				return reader.ReadToEnd();
			}
		}
		
		Stream GetBaseVersion()
		{
			string fileName = ((ITextEditor)document.GetService(typeof(ITextEditor))).FileName;
			
			foreach (IDocumentVersionProvider provider in VersioningServices.Instance.DocumentVersionProviders) {
				var result = provider.OpenBaseVersion(fileName);
				if (result != null)
					return result;
			}
			
			return null;
		}
		
		void UndoStackPropertyChanged(object sender, PropertyChangedEventArgs e)
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
			var newLineInfo = new LineChangeInfo(ChangeType.Unsaved, index, index, newLine.LineNumber, newLine.LineNumber);
			
			changeList[index] = newLineInfo;
			changeList.Insert(index + 1, newLineInfo);
		}
		
		void ILineTracker.RebuildDocument()
		{
			changeList.Clear();
			changeList.InsertRange(0, document.TotalNumberOfLines + 1, new LineChangeInfo(ChangeType.Unsaved, 1, 1, baseDocument.TotalNumberOfLines, document.TotalNumberOfLines));
		}
		
		bool disposed = false;
		
		public void Dispose()
		{
			if (!disposed) {
				this.textDocument.LineTrackers.Remove(this);
				this.textDocument.UndoStack.PropertyChanged -= UndoStackPropertyChanged;
				disposed = true;
			}
		}
		
		public string GetOldVersionFromLine(int lineNumber, out int newStartLine, out bool added)
		{
			LineChangeInfo info = changeList[lineNumber];
			
			added = info.Change == ChangeType.Added;
			
			if (info.Change != ChangeType.None && info.Change != ChangeType.Unsaved) {
				newStartLine = info.NewStartLineNumber + 1;
				
				if (info.Change == ChangeType.Added)
					return "";
				
				var startDocumentLine = baseDocument.GetLine(info.OldStartLineNumber + 1);
				var endLine = baseDocument.GetLine(info.OldEndLineNumber);
				
				return baseDocument.GetText(startDocumentLine.Offset, endLine.EndOffset - startDocumentLine.Offset);
			}
			
			newStartLine = 0;
			return null;
		}
		
		public bool GetNewVersionFromLine(int lineNumber, out int offset, out int length)
		{
			LineChangeInfo info = changeList[lineNumber];
			
			if (info.Change != ChangeType.None && info.Change != ChangeType.Unsaved) {
				var startLine = document.GetLine(info.NewStartLineNumber + 1);
				var endLine = document.GetLine(info.NewEndLineNumber);
				
				offset = startLine.Offset;
				length = endLine.EndOffset - startLine.Offset;
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