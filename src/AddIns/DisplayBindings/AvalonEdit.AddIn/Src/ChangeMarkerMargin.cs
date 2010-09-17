// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// Description of ChangeMarkerMargin.
	/// </summary>
	public class ChangeMarkerMargin : AbstractMargin, IDisposable
	{
		IChangeWatcher changeWatcher;
		
		public IChangeWatcher ChangeWatcher {
			get { return changeWatcher; }
			set {
				if (changeWatcher != null)
					changeWatcher.ChangeOccured -= ChangeOccured;
				changeWatcher = value;
				if (changeWatcher != null)
					changeWatcher.ChangeOccured += ChangeOccured;
			}
		}
		
		protected override void OnRender(DrawingContext drawingContext)
		{
			Size renderSize = this.RenderSize;
			TextView textView = this.TextView;
			
			if (textView != null && textView.VisualLinesValid) {
				ITextEditor editor = textView.Services.GetService(typeof(ITextEditor)) as ITextEditor;
				
				foreach (VisualLine line in textView.VisualLines) {
					Rect rect = new Rect(0, line.VisualTop - textView.ScrollOffset.Y, renderSize.Width, line.Height);
					
					ChangeType type = changeWatcher.GetChange(editor.Document.GetLine(line.FirstDocumentLine.LineNumber));
					
					switch (type) {
						case ChangeType.None:
							break;
						case ChangeType.Added:
						case ChangeType.Saved:
							drawingContext.DrawRectangle(Brushes.LightGreen, null, rect);
							break;
						case ChangeType.Deleted:
							// TODO : implement
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
			if (ChangeWatcher == null)
				ChangeWatcher = new LocalChangeWatcher();
			
			if (oldTextView != null) {
				oldTextView.VisualLinesChanged -= VisualLinesChanged;
				oldTextView.ScrollOffsetChanged -= ScrollOffsetChanged;
			}
			
			if (newTextView != null) {
				newTextView.VisualLinesChanged += VisualLinesChanged;
				newTextView.ScrollOffsetChanged += ScrollOffsetChanged;
				
				ITextEditor editor = newTextView.Services.GetService(typeof(ITextEditor)) as ITextEditor;
				changeWatcher.UpdateTextEditor(editor);
			}
		}
		
		protected override void OnDocumentChanged(TextDocument oldDocument, TextDocument newDocument)
		{
			if (ChangeWatcher == null)
				ChangeWatcher = new LocalChangeWatcher();
			
			ITextEditor editor = TextView.Services.GetService(typeof(ITextEditor)) as ITextEditor;
			changeWatcher.UpdateTextEditor(editor);
		}

		void VisualLinesChanged(object sender, EventArgs e)
		{
			InvalidateVisual();
		}
		
		void ScrollOffsetChanged(object sender, EventArgs e)
		{
			InvalidateVisual();
		}
		
		void ChangeOccured(object sender, EventArgs e)
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
				ChangeWatcher = null;
				disposed = true;
			}
		}
	}
	
	public interface IChangeWatcher : IDisposable
	{
		event EventHandler ChangeOccured;
		ChangeType GetChange(IDocumentLine line);
		void UpdateTextEditor(ITextEditor editor);
	}
	
	public enum ChangeType : int
	{
		None       = 0x0000,
		Added      = 0x0001,
		Deleted    = 0x0002,
		Modified   = 0x0004,
		/// <summary>
		/// Only to be used by LocalChangeWatcher. States if a change is unsaved or not.
		/// </summary>
		Saved      = 0x0008,
		/// <summary>
		/// Only to be used by LocalChangeWatcher. States if a change is unsaved or not.
		/// </summary>
		Unsaved    = 0x0016
	}
	
	public class LocalChangeWatcher : IChangeWatcher, ILineTracker
	{
		WeakLineTracker lineTracker;
		List<DocumentLine> changedLines;
		List<DocumentLine> lastSavedChangedLines;
		bool changed;
		OpenedFile openedFile;
		TextView textView;
		ITextEditor editor;
		
		public event EventHandler ChangeOccured;
		
		protected void OnChangeOccured(EventArgs e)
		{
			if (ChangeOccured != null) {
				ChangeOccured(this, e);
			}
		}
		
		public ChangeType GetChange(IDocumentLine line)
		{
			if (openedFile == null && editor.FileName != null) {
				openedFile = FileService.GetOpenedFile(editor.FileName);
				openedFile.IsDirtyChanged += IsDirtyChanged;
			}
			
			DocumentLine documentLine = textView.Document.GetLineByNumber(line.LineNumber);
			
			if (lastSavedChangedLines.Contains(documentLine))
				return ChangeType.Saved;
			if (changedLines.Contains(documentLine))
				return ChangeType.Unsaved;
			
			return ChangeType.None;
		}
		
		void IsDirtyChanged(object sender, EventArgs e)
		{
			if (changed && !openedFile.IsDirty) {
				lastSavedChangedLines = lastSavedChangedLines.Concat(changedLines).Distinct().ToList();
				changedLines.Clear();
				changed = false;
				OnChangeOccured(EventArgs.Empty);
			}
		}
		
		void ILineTracker.BeforeRemoveLine(DocumentLine line)
		{
			changedLines.Remove(line);
			lastSavedChangedLines.Remove(line);
			changed = true;
		}
		
		void ILineTracker.SetLineLength(DocumentLine line, int newTotalLength)
		{
			if (!changedLines.Contains(line))
				changedLines.Add(line);
			lastSavedChangedLines.Remove(line);
			changed = true;
		}
		
		void ILineTracker.LineInserted(DocumentLine insertionPos, DocumentLine newLine)
		{
			if (!changedLines.Contains(insertionPos))
				changedLines.Add(insertionPos);
			lastSavedChangedLines.Remove(insertionPos);
			changedLines.Add(newLine);
			changed = true;
		}
		
		void ILineTracker.RebuildDocument()
		{
		}
		
		bool disposed = false;
		
		public void Dispose()
		{
			if (!disposed) {
				lineTracker.Deregister();
				openedFile.IsDirtyChanged -= IsDirtyChanged;
				FileService.FileCreated -= FileServiceFileCreated;
				disposed = true;
			}
		}
		
		public void UpdateTextEditor(ITextEditor editor)
		{
			if (lineTracker != null)
				lineTracker.Deregister();
			if (openedFile != null)
				openedFile.IsDirtyChanged -= IsDirtyChanged;
			
			if (editor == null)
				throw new ArgumentNullException("editor");
			
			this.editor = editor;
			this.textView = editor.GetService(typeof(TextView)) as TextView;
			FileService.FileCreated += FileServiceFileCreated;
			
			lineTracker = WeakLineTracker.Register(textView.Document, this);
			
			changed = false;
			changedLines = new List<DocumentLine>();
			lastSavedChangedLines = new List<DocumentLine>();
		}
		
		void FileServiceFileCreated(object sender, FileEventArgs e)
		{
			
		}
	}
}
