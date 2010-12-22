// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Editing;
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
		
		public ChangeMarkerMargin()
		{
			changeWatcher = new DefaultChangeWatcher();
			changeWatcher.ChangeOccurred += new EventHandler(ChangeOccurred);
		}
		
		protected override void OnRender(DrawingContext drawingContext)
		{
			using (DebugTimer.Time("ChangeMarkerMargin.OnRender")) {
				Size renderSize = this.RenderSize;
				TextView textView = this.TextView;
				
				if (textView != null && textView.VisualLinesValid) {
					ITextEditor editor = textView.Services.GetService(typeof(ITextEditor)) as ITextEditor;
					changeWatcher.Initialize(editor.Document);
					
					foreach (VisualLine line in textView.VisualLines) {
						Rect rect = new Rect(0, line.VisualTop - textView.ScrollOffset.Y, 5, line.Height);
						
						LineChangeInfo info = changeWatcher.GetChange(editor.Document.GetLine(line.FirstDocumentLine.LineNumber));
						
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
						
						if (!string.IsNullOrEmpty(info.DeletedLinesAfterThisLine)) {
							Point pt1 = new Point(5,  line.VisualTop + line.Height - textView.ScrollOffset.Y - 4);
							Point pt2 = new Point(10, line.VisualTop + line.Height - textView.ScrollOffset.Y);
							Point pt3 = new Point(5,  line.VisualTop + line.Height - textView.ScrollOffset.Y + 4);
							
							drawingContext.DrawGeometry(Brushes.Red, null, new PathGeometry(new List<PathFigure>() { CreateNAngle(pt1, pt2, pt3) }));
						}
						
						// special case for line 0
						if (line.FirstDocumentLine.LineNumber == 1) {
							info = changeWatcher.GetChange(null);
							
							if (!string.IsNullOrEmpty(info.DeletedLinesAfterThisLine)) {
								Point pt1 = new Point(5,  line.VisualTop - textView.ScrollOffset.Y - 4);
								Point pt2 = new Point(10, line.VisualTop - textView.ScrollOffset.Y);
								Point pt3 = new Point(5,  line.VisualTop - textView.ScrollOffset.Y + 4);
								
								drawingContext.DrawGeometry(Brushes.Red, null, new PathGeometry(new List<PathFigure>() { CreateNAngle(pt1, pt2, pt3) }));
							}
						}
					}
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
}