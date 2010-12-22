// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

using ICSharpCode.AvalonEdit.AddIn.MyersDiff;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.AvalonEdit.AddIn
{
	public class DefaultChangeWatcher : IChangeWatcher, ILineTracker
	{
		WeakLineTracker lineTracker;
		CompressingTreeList<LineChangeInfo> changeList;
		IDocument document;
		TextDocument textDocument;
		
		public event EventHandler ChangeOccurred;
		
		protected void OnChangeOccurred(EventArgs e)
		{
			if (ChangeOccurred != null) {
				ChangeOccurred(this, e);
			}
		}
		
		public LineChangeInfo GetChange(IDocumentLine line)
		{
			if (line == null)
				return changeList[0];
			
			return changeList[line.LineNumber];
		}
		
		public void Initialize(IDocument document)
		{
			if (changeList != null && changeList.Any())
				return;
			
			this.document = document;
			this.textDocument = ((TextView)document.GetService(typeof(TextView))).Document;
			this.changeList = new CompressingTreeList<LineChangeInfo>((x, y) => x.Equals(y));
			
			SetupInitialFileState();
			
			lineTracker = WeakLineTracker.Register(this.textDocument, this);
			this.textDocument.UndoStack.PropertyChanged += UndoStackPropertyChanged;
		}
		
		void SetupInitialFileState()
		{
			changeList.Clear();
			
			Stream baseFileStream = GetBaseVersion();
			string baseFile = ReadAll(baseFileStream);
			
			Stream currentFileStream = GetCurrentVersion();
			string currentFile = ReadAll(currentFileStream);
			
			List<int> baseFileOffsets = CalculateLineOffsets(baseFile);
			List<int> currentFileOffsets = CalculateLineOffsets(currentFile);
			
			MyersDiff.MyersDiff diff = new MyersDiff.MyersDiff(new StringSequence(baseFile), new StringSequence(currentFile));
			
			if (diff == null)
				changeList.InsertRange(0, document.TotalNumberOfLines + 1, new LineChangeInfo(ChangeType.None, ""));
			else {
				changeList.Add(new LineChangeInfo(ChangeType.None, ""));
				int lastEndLine = 0;
				foreach (Edit edit in diff.GetEdits()) {
					int beginLine = OffsetToLineNumber(currentFileOffsets, edit.BeginB);
					int endLine = OffsetToLineNumber(currentFileOffsets, edit.EndB);
					changeList.InsertRange(changeList.Count, beginLine - lastEndLine, new LineChangeInfo(ChangeType.None, ""));
					changeList.InsertRange(changeList.Count, endLine - beginLine, new LineChangeInfo(edit.EditType, ""));
					lastEndLine = endLine;
				}
				changeList.InsertRange(changeList.Count, document.TotalNumberOfLines - lastEndLine, new LineChangeInfo(ChangeType.None, ""));
			}
			
			OnChangeOccurred(EventArgs.Empty);
		}
		
		List<int> CalculateLineOffsets(string fileContent)
		{
			List<int> offsets = new List<int>();
			
			int current = 0;
			offsets.Add(current);
			
			while ((current = fileContent.IndexOfAny(new[] { '\r', '\n' }, current + 1)) != -1) {
				switch (fileContent[current]) {
					case '\r':
						if (current + 1 < fileContent.Length && fileContent[current + 1] != '\n')
							offsets.Add(current + 1);
						break;
					case '\n':
						offsets.Add(current + 1);
						break;
				}
			}
			
			return offsets;
		}
		
		int OffsetToLineNumber(List<int> lineOffsets, int offset)
		{
			int lineNumber = lineOffsets.BinarySearch(offset);
			if (lineNumber < 0)
				lineNumber = (~lineNumber) - 1;
			return lineNumber + 1;
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
			
			return new DefaultVersionProvider().OpenBaseVersion(fileName);
		}
		
		Stream GetCurrentVersion()
		{
			string fileName = ((ITextEditor)document.GetService(typeof(ITextEditor))).FileName;
			
			foreach (IDocumentVersionProvider provider in VersioningServices.Instance.DocumentVersionProviders) {
				var result = provider.OpenCurrentVersion(fileName);
				if (result != null)
					return result;
			}
			
			return new DefaultVersionProvider().OpenCurrentVersion(fileName);
		}
		
		void UndoStackPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
//			if (textDocument.UndoStack.IsOriginalFile)
//				SetupInitialFileState();
		}
		
		void ILineTracker.BeforeRemoveLine(DocumentLine line)
		{
			int index = line.LineNumber;
			LineChangeInfo info = changeList[index];
			LineChangeInfo lineBefore = changeList[index - 1];
			
			// TODO : add deleted text (GetText not allowed in ILineTracker callbacks)
//			lineBefore.DeletedLinesAfterThisLine
//				+= (textDocument.GetText(line.Offset, line.Length)
//				    + Environment.NewLine + info.DeletedLinesAfterThisLine);
//
//			Debug.Assert(lineBefore.DeletedLinesAfterThisLine.EndsWith(Environment.NewLine));
			
			changeList[index - 1] = lineBefore;
			changeList.RemoveAt(index);
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
			var firstLine = changeList[index];
			var newLineInfo = new LineChangeInfo(ChangeType.Unsaved, firstLine.DeletedLinesAfterThisLine);
			
			firstLine.Change = ChangeType.Unsaved;
			firstLine.DeletedLinesAfterThisLine = "";
			
			changeList.Insert(index + 1, newLineInfo);
			changeList[index] = firstLine;
		}
		
		void ILineTracker.RebuildDocument()
		{
		}
		
		bool disposed = false;
		
		public void Dispose()
		{
			if (!disposed) {
				lineTracker.Deregister();
				this.textDocument.UndoStack.PropertyChanged -= UndoStackPropertyChanged;
				disposed = true;
			}
		}
	}
}
