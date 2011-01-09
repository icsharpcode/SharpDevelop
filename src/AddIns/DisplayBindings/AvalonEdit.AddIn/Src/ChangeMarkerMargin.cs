// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.AddIn.Options;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Widgets;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// Description of ChangeMarkerMargin.
	/// </summary>
	public class ChangeMarkerMargin : AbstractMargin, IDisposable
	{
		IChangeWatcher changeWatcher;
		
		public ChangeMarkerMargin(IChangeWatcher changeWatcher)
		{
			this.changeWatcher = changeWatcher;
			changeWatcher.ChangeOccurred += ChangeOccurred;
		}
		
		bool disposed = false;
		
		public void Dispose()
		{
			if (!disposed) {
				changeWatcher.ChangeOccurred -= ChangeOccurred;
				disposed = true;
			}
		}
		
		protected override void OnRender(DrawingContext drawingContext)
		{
			Size renderSize = this.RenderSize;
			TextView textView = this.TextView;
			
			if (textView != null && textView.VisualLinesValid) {
				foreach (VisualLine line in textView.VisualLines) {
					Rect rect = new Rect(0, line.VisualTop - textView.ScrollOffset.Y, 5, line.Height);
					
					LineChangeInfo info = changeWatcher.GetChange(line.FirstDocumentLine.LineNumber);
					
					switch (info.Change) {
						case ChangeType.None:
							break;
						case ChangeType.Added:
							drawingContext.DrawRectangle(Brushes.LightGreen, null, rect);
							break;
						case ChangeType.Modified:
							drawingContext.DrawRectangle(Brushes.LightBlue, null, rect);
							break;
						case ChangeType.Unsaved:
							drawingContext.DrawRectangle(Brushes.Yellow, null, rect);
							break;
						default:
							throw new Exception("Invalid value for ChangeType");
					}
					
//					if (!string.IsNullOrEmpty(info.DeletedLinesAfterThisLine)) {
//						Point pt1 = new Point(5,  line.VisualTop + line.Height - textView.ScrollOffset.Y - 4);
//						Point pt2 = new Point(10, line.VisualTop + line.Height - textView.ScrollOffset.Y);
//						Point pt3 = new Point(5,  line.VisualTop + line.Height - textView.ScrollOffset.Y + 4);
//
//						drawingContext.DrawGeometry(Brushes.Red, null, new PathGeometry(new List<PathFigure>() { CreateNAngle(pt1, pt2, pt3) }));
//					}
//
//					// special case for line 0
//					if (line.FirstDocumentLine.LineNumber == 1) {
//						info = changeWatcher.GetChange(0);
//
//						if (!string.IsNullOrEmpty(info.DeletedLinesAfterThisLine)) {
//							Point pt1 = new Point(5,  line.VisualTop - textView.ScrollOffset.Y - 4);
//							Point pt2 = new Point(10, line.VisualTop - textView.ScrollOffset.Y);
//							Point pt3 = new Point(5,  line.VisualTop - textView.ScrollOffset.Y + 4);
//
//							drawingContext.DrawGeometry(Brushes.Red, null, new PathGeometry(new List<PathFigure>() { CreateNAngle(pt1, pt2, pt3) }));
//						}
//					}
				}
			}
		}
		
		PathFigure CreateNAngle(params Point[] points)
		{
			if (points == null || points.Length == 0)
				return new PathFigure();
			
			List<PathSegment> segs = new List<PathSegment>();
			PathSegment seg = new PolyLineSegment(points, true);
			segs.Add(seg);
			
			return new PathFigure(points[0], segs, true);
		}
		
		protected override void OnTextViewChanged(TextView oldTextView, TextView newTextView)
		{
			if (oldTextView != null) {
				oldTextView.VisualLinesChanged -= VisualLinesChanged;
				oldTextView.ScrollOffsetChanged -= ScrollOffsetChanged;
			}
			base.OnTextViewChanged(oldTextView, newTextView);
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
		
		#region Diffs tooltip
		
		Popup tooltip = new Popup() { StaysOpen = false };
		ITextMarker marker;
		ITextMarkerService markerService;
		
		protected override void OnMouseMove(MouseEventArgs e)
		{
			int line = GetLineFromMousePosition(e);
			
			if (line == 0)
				return;
			
			int startLine;
			string oldText = changeWatcher.GetOldVersionFromLine(line, out startLine);
			
			int offset, length;
			
			TextEditor editor = this.TextView.Services.GetService(typeof(TextEditor)) as TextEditor;
			markerService = this.TextView.Services.GetService(typeof(ITextMarkerService)) as ITextMarkerService;

			
			if (changeWatcher.GetNewVersionFromLine(line, out offset, out length)) {
				if (marker != null)
					markerService.Remove(marker);
				marker = markerService.Create(offset, length);
				marker.BackgroundColor = Colors.LightGreen;
			}
			
			if (oldText != null) {
				DiffControl differ = new DiffControl();
				differ.editor.SyntaxHighlighting = editor.SyntaxHighlighting;
				differ.editor.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
				differ.editor.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
				differ.editor.Document.Text = oldText;
				differ.Background = Brushes.White;
				
				DocumentHighlighter mainHighlighter = TextView.Services.GetService(typeof(IHighlighter)) as DocumentHighlighter;
				DocumentHighlighter popupHighlighter = differ.editor.TextArea.GetService(typeof(IHighlighter)) as DocumentHighlighter;
				
//				popupHighlighter.InitialSpanStack = mainHighlighter.GetSpanStack(
				
				if (oldText == string.Empty) {
					differ.editor.Visibility = Visibility.Collapsed;
				}
				
				differ.undoButton.Click += delegate {
					if (marker != null) {
						int delimiter = 0;
						if (oldText == string.Empty)
							delimiter = Document.GetLineByOffset(offset + length).DelimiterLength;
						Document.Replace(offset, length + delimiter, oldText);
						tooltip.IsOpen = false;
					}
				};
				
				tooltip.Child = new Border() {
					Child = differ,
					BorderBrush = Brushes.Black,
					BorderThickness = new Thickness(1)
				};
				
				if (tooltip.IsOpen)
					tooltip.IsOpen = false;
				
				tooltip.IsOpen = true;
				
				tooltip.Closed += delegate {
					if (marker != null) markerService.Remove(marker);
				};
				
				tooltip.HorizontalOffset = -10;
				tooltip.VerticalOffset =
					TextView.GetVisualTopByDocumentLine(startLine) - TextView.ScrollOffset.Y;
				tooltip.Placement = PlacementMode.Top;
				tooltip.PlacementTarget = this.TextView;
			}
			
			base.OnMouseMove(e);
		}
		
		protected override void OnMouseLeave(MouseEventArgs e)
		{
			if (marker != null && !tooltip.IsOpen)
				markerService.Remove(marker);
			base.OnMouseLeave(e);
		}
		
		int GetLineFromMousePosition(MouseEventArgs e)
		{
			TextView textView = this.TextView;
			if (textView == null)
				return 0;
			VisualLine vl = textView.GetVisualLineFromVisualTop(e.GetPosition(textView).Y + textView.ScrollOffset.Y);
			if (vl == null)
				return 0;
			return vl.FirstDocumentLine.LineNumber;
		}
		
		#endregion
	}
}