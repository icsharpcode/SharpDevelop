// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.Diagnostics;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.AvalonEdit.AddIn
{
	public class DefaultChangeWatcher : IChangeWatcher, ILineTracker
	{
		struct LineChangeInfo : IEquatable<LineChangeInfo>
		{
			ChangeType change;
			
			public ChangeType Change {
				get { return change; }
				set { change = value; }
			}
			
			string deletedLinesAfterThisLine;
			
			public string DeletedLinesAfterThisLine {
				get { return deletedLinesAfterThisLine; }
				set { deletedLinesAfterThisLine = value; }
			}
			
			public LineChangeInfo(ChangeType change, string deletedLinesAfterThisLine)
			{
				this.change = change;
				this.deletedLinesAfterThisLine = deletedLinesAfterThisLine;
			}
			
			#region Equals and GetHashCode implementation
			public override bool Equals(object obj)
			{
				return (obj is DefaultChangeWatcher.LineChangeInfo) && Equals((DefaultChangeWatcher.LineChangeInfo)obj);
			}
			
			public bool Equals(DefaultChangeWatcher.LineChangeInfo other)
			{
				return this.change == other.change && this.deletedLinesAfterThisLine == other.deletedLinesAfterThisLine;
			}
			
			public override int GetHashCode()
			{
				int hashCode = 0;
				unchecked {
					hashCode += 1000000007 * change.GetHashCode();
					if (deletedLinesAfterThisLine != null)
						hashCode += 1000000009 * deletedLinesAfterThisLine.GetHashCode();
				}
				return hashCode;
			}
			
			public static bool operator ==(DefaultChangeWatcher.LineChangeInfo lhs, DefaultChangeWatcher.LineChangeInfo rhs)
			{
				return lhs.Equals(rhs);
			}
			
			public static bool operator !=(DefaultChangeWatcher.LineChangeInfo lhs, DefaultChangeWatcher.LineChangeInfo rhs)
			{
				return !(lhs == rhs);
			}
			#endregion
		}
		
		WeakLineTracker lineTracker;
		CompressingTreeList<LineChangeInfo> changeList;
		TextDocument document;
		
		public event EventHandler ChangeOccurred;
		
		protected void OnChangeOccurred(EventArgs e)
		{
			if (ChangeOccurred != null) {
				ChangeOccurred(this, e);
			}
		}
		
		public ChangeType GetChange(IDocumentLine line)
		{
			return changeList[line.LineNumber].Change;
		}
		
		public void Initialize(IDocument document)
		{
			if (this.document != null)
				return;
			
			this.document = ((TextView)document.GetService(typeof(TextView))).Document;
			this.changeList = new CompressingTreeList<LineChangeInfo>((x, y) => x.Equals(y));
			
			SetupInitialFileState();
			
			lineTracker = WeakLineTracker.Register(this.document, this);
			this.document.UndoStack.PropertyChanged += UndoStackPropertyChanged;
		}
	
		void SetupInitialFileState()
		{
			changeList.Clear();
			changeList.InsertRange(0, this.document.LineCount + 1, new LineChangeInfo(ChangeType.None, ""));
			OnChangeOccurred(EventArgs.Empty);
		}
	
		void UndoStackPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (document.UndoStack.IsOriginalFile)
				SetupInitialFileState();
		}
		
		void ILineTracker.BeforeRemoveLine(DocumentLine line)
		{
			int index = line.LineNumber;
			LineChangeInfo info = changeList[index];
			LineChangeInfo lineBefore = changeList[index - 1];
			
			lineBefore.DeletedLinesAfterThisLine
				+= (document.GetText(line.Offset, line.Length)
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
				this.document.UndoStack.PropertyChanged -= UndoStackPropertyChanged;
				disposed = true;
			}
		}
	}
}
