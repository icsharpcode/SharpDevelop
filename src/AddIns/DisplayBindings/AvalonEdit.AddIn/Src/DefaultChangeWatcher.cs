// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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
			byte[] baseFile = new byte[baseFileStream.Length];
			
			baseFileStream.Read(baseFile, 0, baseFile.Length);
			
			Stream currentFileStream = GetCurrentVersion();
			byte[] currentFile = new byte[currentFileStream.Length];
			
			currentFileStream.Read(currentFile, 0, currentFileStream.Length);
			
			MyersDiff diff = new MyersDiff(new RawText(baseFile), new RawText(currentFile));
			
			if (diff == null)
				changeList.InsertRange(0, document.TotalNumberOfLines + 1, new LineChangeInfo(ChangeType.None, ""));
			else {
				changeList.Add(new LineChangeInfo(ChangeType.None, ""));
				foreach (Edit edit in diff.GetEdits()) {
					changeList.InsertRange(changeList.Count, edit.EndB - edit.BeginB, new LineChangeInfo(edit.EditType, ""));
				}
			}
			
			OnChangeOccurred(EventArgs.Empty);
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
			if (textDocument.UndoStack.IsOriginalFile)
				SetupInitialFileState();
		}
		
		void ILineTracker.BeforeRemoveLine(DocumentLine line)
		{
			int index = line.LineNumber;
			LineChangeInfo info = changeList[index];
			LineChangeInfo lineBefore = changeList[index - 1];
			
			lineBefore.DeletedLinesAfterThisLine
				+= (textDocument.GetText(line.Offset, line.Length)
				    + Environment.NewLine + info.DeletedLinesAfterThisLine);
			
			Debug.Assert(lineBefore.DeletedLinesAfterThisLine.EndsWith(Environment.NewLine));
			
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
