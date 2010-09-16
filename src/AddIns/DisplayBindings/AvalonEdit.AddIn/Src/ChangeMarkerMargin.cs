// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// Description of ChangeMarkerMargin.
	/// </summary>
	public class ChangeMarkerMargin : AbstractMargin, ILineTracker, IDisposable
	{
		WeakLineTracker lineTracker;
		List<DocumentLine> changedLines;
		List<DocumentLine> lastSavedChangedLines;
		bool changed;
		OpenedFile openedFile;
		
		public ChangeMarkerMargin()
		{
			changedLines = new List<DocumentLine>();
			lastSavedChangedLines = new List<DocumentLine>();
			changed = false;
		}
		
		protected override void OnRender(DrawingContext drawingContext)
		{
			Size renderSize = this.RenderSize;
			TextView textView = this.TextView;
			
			if (textView != null && textView.VisualLinesValid) {
				ITextEditor editor = textView.Services.GetService(typeof(ITextEditor)) as ITextEditor;
				openedFile = FileService.GetOpenedFile(editor.FileName);
				openedFile.IsDirtyChanged += IsDirtyChanged;
				textView.ScrollOffsetChanged += ScrollOffsetChanged;
				
				if (lineTracker == null)
					lineTracker = WeakLineTracker.Register(textView.Document, this);
				
				foreach (VisualLine line in textView.VisualLines) {
					Rect rect = new Rect(0, line.VisualTop - textView.ScrollOffset.Y, renderSize.Width, line.Height);
					if (lastSavedChangedLines.Contains(line.FirstDocumentLine))
						drawingContext.DrawRectangle(Brushes.LightGreen, null, rect);
					else if (changedLines.Contains(line.FirstDocumentLine))
						drawingContext.DrawRectangle(Brushes.Yellow, null, rect);
				}
			}
		}

		void ScrollOffsetChanged(object sender, EventArgs e)
		{
			InvalidateVisual();
		}

		void IsDirtyChanged(object sender, EventArgs e)
		{
			if (changed && !openedFile.IsDirty) {
				lastSavedChangedLines = lastSavedChangedLines.Concat(changedLines).Distinct().ToList();
				changedLines.Clear();
				changed = false;
				InvalidateVisual();
			}
		}
		
		protected override Size MeasureOverride(Size availableSize)
		{
			return new Size(5, 0);
		}
		
		public void BeforeRemoveLine(DocumentLine line)
		{
			changedLines.Remove(line);
			lastSavedChangedLines.Remove(line);
			changed = true;
			InvalidateVisual();
		}
		
		public void SetLineLength(DocumentLine line, int newTotalLength)
		{
			if (!changedLines.Contains(line))
				changedLines.Add(line);
			lastSavedChangedLines.Remove(line);
			changed = true;
			InvalidateVisual();
		}
		
		public void LineInserted(DocumentLine insertionPos, DocumentLine newLine)
		{
			if (!changedLines.Contains(insertionPos))
				changedLines.Add(insertionPos);
			lastSavedChangedLines.Remove(insertionPos);
			changedLines.Add(newLine);
			changed = true;
			InvalidateVisual();
		}
		
		public void RebuildDocument()
		{
			InvalidateVisual();
		}
		
		bool disposed = false;
		
		public void Dispose()
		{
			if (!disposed) {
				lineTracker.Deregister();
				openedFile.IsDirtyChanged -= IsDirtyChanged;
				disposed = true;
			}
		}
	}
}
