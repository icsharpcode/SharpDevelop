// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// Description of ChangeMarkerMargin.
	/// </summary>
	public class ChangeMarkerMargin : AbstractMargin, IDisposable
	{
		IChangeWatcher changeWatcher;
		
		public ChangeMarkerMargin()
		{
			changeWatcher = new DefaultChangeWatcher();
			changeWatcher.ChangeOccurred += new EventHandler(ChangeOccurred);
		}
		
		protected override void OnRender(DrawingContext drawingContext)
		{
			Size renderSize = this.RenderSize;
			TextView textView = this.TextView;
			
			if (textView != null && textView.VisualLinesValid) {
				ITextEditor editor = textView.Services.GetService(typeof(ITextEditor)) as ITextEditor;
				changeWatcher.Initialize(editor.Document);
				
				foreach (VisualLine line in textView.VisualLines) {
					Rect rect = new Rect(0, line.VisualTop - textView.ScrollOffset.Y, renderSize.Width, line.Height);
					
					ChangeType type = changeWatcher.GetChange(editor.Document.GetLine(line.FirstDocumentLine.LineNumber));
					
					switch (type) {
						case ChangeType.None:
							break;
						case ChangeType.Added:
							drawingContext.DrawRectangle(Brushes.LightGreen, null, rect);
							break;
						case ChangeType.Modified:
							drawingContext.DrawRectangle(Brushes.Blue, null, rect);
							break;
						case ChangeType.Unsaved:
							drawingContext.DrawRectangle(Brushes.Yellow, null, rect);
							break;
						default:
							throw new Exception("Invalid value for ChangeType");
					}
				}
			}
		}
		
		protected override void OnTextViewChanged(TextView oldTextView, TextView newTextView)
		{
			if (oldTextView != null) {
				oldTextView.VisualLinesChanged -= VisualLinesChanged;
				oldTextView.ScrollOffsetChanged -= ScrollOffsetChanged;
			}
			
			if (newTextView != null) {
				newTextView.VisualLinesChanged += VisualLinesChanged;
				newTextView.ScrollOffsetChanged += ScrollOffsetChanged;
			}
		}
		
		void ChangeOccurred(object sender, EventArgs e)
		{
			InvalidateVisual();
		}
		
		void VisualLinesChanged(object sender, EventArgs e)
		{
			InvalidateVisual();
		}
		
		void ScrollOffsetChanged(object sender, EventArgs e)
		{
			InvalidateVisual();
		}
		
		protected override Size MeasureOverride(Size availableSize)
		{
			return new Size(5, 0);
		}
		
		bool disposed = false;
		
		public void Dispose()
		{
			if (!disposed) {
				OnTextViewChanged(TextView, null);
				changeWatcher.Dispose();
				changeWatcher = null;
				disposed = true;
			}
		}
	}
	
	public interface IChangeWatcher : IDisposable
	{
		event EventHandler ChangeOccurred;
		ChangeType GetChange(IDocumentLine line);
		void Initialize(IDocument document);
	}
	
	public enum ChangeType
	{
		None,
		Added,
		Modified,
		Unsaved
	}
	
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
