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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// Scrollbar that shows markers.
	/// </summary>
	[TextEditorService]
	public class EnhancedScrollBar : IDisposable
	{
		readonly TextEditor editor;
		readonly TextMarkerService textMarkerService;
		readonly IChangeWatcher changeWatcher;
		TrackBackground trackBackground;
		TrackAdorner trackAdorner;
		
		public EnhancedScrollBar(TextEditor editor, TextMarkerService textMarkerService, IChangeWatcher changeWatcher)
		{
			if (editor == null)
				throw new ArgumentNullException("editor");
			this.editor = editor;
			this.textMarkerService = textMarkerService;
			this.changeWatcher = changeWatcher;
			
			editor.Loaded += editor_Loaded;
			if (editor.IsLoaded) {
				editor_Loaded(null, null);
			}
		}
		
		public void Dispose()
		{
			editor.Loaded -= editor_Loaded;
			if (trackBackground != null) {
				trackBackground.Remove();
				trackBackground = null;
			}
			if (trackAdorner != null) {
				trackAdorner.Remove();
				trackAdorner = null;
			}
		}
		
		#region Initialize UI
		bool isUIInitialized;
		
		void editor_Loaded(object sender, RoutedEventArgs e)
		{
			if (isUIInitialized)
				return;
			isUIInitialized = true;
			editor.ApplyTemplate();
			var scrollViewer = (ScrollViewer)editor.Template.FindName("PART_ScrollViewer", editor);
			if (scrollViewer == null)
				return;
			scrollViewer.ApplyTemplate();
			var vScrollBar = (ScrollBar)scrollViewer.Template.FindName("PART_VerticalScrollBar", scrollViewer);
			var hScrollBar = (ScrollBar)scrollViewer.Template.FindName("PART_HorizontalScrollBar", scrollViewer);
			// make both scrollbars transparent so that they look consistent
			MakeThumbTransparent(vScrollBar);
			MakeThumbTransparent(hScrollBar);
			if (vScrollBar == null)
				return;
			Track track = (Track)vScrollBar.Template.FindName("PART_Track", vScrollBar);
			if (track == null)
				return;
			Grid grid = VisualTreeHelper.GetParent(track) as Grid;
			if (grid != null) {
				trackBackground = new TrackBackground(this);
				trackAdorner = new TrackAdorner(this);
				Grid.SetColumn(trackBackground, Grid.GetColumn(track));
				Grid.SetRow(trackBackground, Grid.GetRow(track));
				Grid.SetColumnSpan(trackBackground, Grid.GetColumnSpan(track));
				Grid.SetRowSpan(trackBackground, Grid.GetRowSpan(track));
				Grid.SetColumn(trackAdorner, Grid.GetColumn(track));
				Grid.SetRow(trackAdorner, Grid.GetRow(track));
				Grid.SetColumnSpan(trackAdorner, Grid.GetColumnSpan(track));
				Grid.SetRowSpan(trackAdorner, Grid.GetRowSpan(track));
				Panel.SetZIndex(track, 1);
				Panel.SetZIndex(trackAdorner, 2);
				grid.Children.Add(trackBackground);
				grid.Children.Add(trackAdorner);
			}
		}
		#endregion
		
		#region MakeThumbTransparent
		List<Thumb> transparentThumbs = new List<Thumb>();
		const double thumbOpacity = 0.7;
		static readonly Duration animationDuration = new Duration(TimeSpan.FromSeconds(0.25));
		
		void MakeThumbTransparent(ScrollBar scrollBar)
		{
			if (scrollBar == null)
				return;
			Track track = (Track)scrollBar.Template.FindName("PART_Track", scrollBar);
			if (track == null)
				return;
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(track); i++) {
				var thumb = VisualTreeHelper.GetChild(track, i) as Thumb;
				if (thumb != null) {
					thumb.Opacity = thumbOpacity;
					thumb.MouseEnter += thumb_MouseEnter;
					thumb.MouseLeave += thumb_MouseLeave;
					transparentThumbs.Add(thumb);
					break;
				}
			}
		}
		
		void ClearTransparencyFromThumbs()
		{
			foreach (var thumb in transparentThumbs) {
				thumb.MouseEnter -= thumb_MouseEnter;
				thumb.MouseLeave -= thumb_MouseLeave;
				thumb.ClearValue(Thumb.OpacityProperty);
			}
		}
		
		void thumb_MouseEnter(object sender, MouseEventArgs e)
		{
			var thumb = (Thumb)sender;
			thumb.BeginAnimation(Thumb.OpacityProperty, new DoubleAnimation(1, animationDuration, FillBehavior.HoldEnd));
		}
		
		void thumb_MouseLeave(object sender, MouseEventArgs e)
		{
			var thumb = (Thumb)sender;
			thumb.BeginAnimation(Thumb.OpacityProperty, new DoubleAnimation(thumbOpacity, animationDuration, FillBehavior.HoldEnd));
		}
		#endregion
		
		static Brush GetBrush(Color markerColor)
		{
			SolidColorBrush brush = new SolidColorBrush(markerColor);
			brush.Freeze();
			return brush;
		}
		
		#region TrackBackground
		sealed class TrackBackground : UIElement
		{
			readonly TextEditor editor;
			readonly TextMarkerService textMarkerService;
			readonly IChangeWatcher changeWatcher;
			
			public TrackBackground(EnhancedScrollBar enhanchedScrollBar)
			{
				this.editor = enhanchedScrollBar.editor;
				this.textMarkerService = enhanchedScrollBar.textMarkerService;
				this.changeWatcher = enhanchedScrollBar.changeWatcher;
				
				textMarkerService.RedrawRequested += textMarkerService_RedrawRequested;
			}
			
			public void Remove()
			{
				textMarkerService.RedrawRequested -= textMarkerService_RedrawRequested;
				
				Grid grid = (Grid)VisualTreeHelper.GetParent(this);
				grid.Children.Remove(this);
			}
			
			void textMarkerService_RedrawRequested(object sender, EventArgs e)
			{
				InvalidateVisual();
			}
			
			protected override void OnRender(DrawingContext drawingContext)
			{
				var renderSize = this.RenderSize;
				var document = editor.Document;
				var textView = editor.TextArea.TextView;
				double documentHeight = textView.DocumentHeight;
				foreach (var marker in textMarkerService.TextMarkers) {
					if ((marker.MarkerTypes & (TextMarkerTypes.LineInScrollBar | TextMarkerTypes.CircleInScrollBar)) == 0)
						continue;
					var location = document.GetLocation(marker.StartOffset);
					double visualTop = textView.GetVisualTopByDocumentLine(location.Line);
					double renderPos = visualTop / documentHeight * renderSize.Height;
					var brush = GetBrush(marker.MarkerColor);
					if ((marker.MarkerTypes & (TextMarkerTypes.LineInScrollBar)) != 0) {
						drawingContext.DrawRectangle(brush, null, new Rect(3, renderPos - 1, renderSize.Width - 6, 2));
					}
					if ((marker.MarkerTypes & (TextMarkerTypes.CircleInScrollBar)) != 0) {
						const double radius = 3;
						drawingContext.DrawEllipse(brush, null, new Point(renderSize.Width / 2, renderPos), radius, radius);
					}
				}
			}
		}
		#endregion
		
		#region TrackAdorner
		sealed class TrackAdorner : FrameworkElement
		{
			#region TriangleGeometry
			static readonly StreamGeometry triangleGeometry = CreateTriangleGeometry();
			
			static StreamGeometry CreateTriangleGeometry()
			{
				var triangleGeometry = new StreamGeometry();
				using (var ctx = triangleGeometry.Open()) {
					const double triangleSize = 6.5;
					const double right = (triangleSize * 0.866) / 2;
					const double left = -right;
					ctx.BeginFigure(new Point(left, triangleSize / 2), true, true);
					ctx.LineTo(new Point(left, -triangleSize / 2), true, false);
					ctx.LineTo(new Point(right, 0), true, false);
				}
				triangleGeometry.Freeze();
				return triangleGeometry;
			}
			#endregion
			
			readonly TextEditor editor;
			readonly TextMarkerService textMarkerService;
			
			public TrackAdorner(EnhancedScrollBar enhanchedScrollBar)
			{
				this.editor = enhanchedScrollBar.editor;
				this.textMarkerService = enhanchedScrollBar.textMarkerService;
				
				this.Cursor = Cursors.Hand;
				this.ToolTip = string.Empty;
				
				textMarkerService.RedrawRequested += textMarkerService_RedrawRequested;
			}
			
			public void Remove()
			{
				textMarkerService.RedrawRequested -= textMarkerService_RedrawRequested;

				Grid grid = (Grid)VisualTreeHelper.GetParent(this);
				grid.Children.Remove(this);
			}
			
			void textMarkerService_RedrawRequested(object sender, EventArgs e)
			{
				InvalidateVisual();
			}
			
			protected override void OnRender(DrawingContext drawingContext)
			{
				var renderSize = this.RenderSize;
				var document = editor.Document;
				var textView = editor.TextArea.TextView;
				double documentHeight = textView.DocumentHeight;
				foreach (var marker in textMarkerService.TextMarkers) {
					if (!IsVisibleInAdorner(marker))
						continue;
					var location = document.GetLocation(marker.StartOffset);
					double visualTop = textView.GetVisualTopByDocumentLine(location.Line);
					double renderPos = visualTop / documentHeight * renderSize.Height;
					var brush = GetBrush(marker.MarkerColor);
					
					var translateTransform = new TranslateTransform(6, renderPos);
					translateTransform.Freeze();
					drawingContext.PushTransform(translateTransform);
					
					if ((marker.MarkerTypes & (TextMarkerTypes.ScrollBarLeftTriangle)) != 0) {
						var scaleTransform = new ScaleTransform(-1, 1);
						scaleTransform.Freeze();
						drawingContext.PushTransform(scaleTransform);
						drawingContext.DrawGeometry(brush, null, triangleGeometry);
						drawingContext.Pop();
					}
					if ((marker.MarkerTypes & (TextMarkerTypes.ScrollBarRightTriangle)) != 0) {
						drawingContext.DrawGeometry(brush, null, triangleGeometry);
					}
					drawingContext.Pop();
				}
			}
			
			bool IsVisibleInAdorner(ITextMarker marker)
			{
				return (marker.MarkerTypes & (TextMarkerTypes.ScrollBarLeftTriangle | TextMarkerTypes.ScrollBarRightTriangle)) != 0;
			}
			
			protected override void OnMouseDown(MouseButtonEventArgs e)
			{
				base.OnMouseDown(e);
				var marker = FindNextMarker(e.GetPosition(this));
				if (marker != null) {
					var location = editor.Document.GetLocation(marker.StartOffset);
					// Use JumpTo() if possible
					var textEditor = editor.TextArea.GetService(typeof(ITextEditor)) as ITextEditor;
					if (textEditor != null)
						textEditor.JumpTo(location.Line, location.Column);
					else
						editor.ScrollTo(location.Line, location.Column);
					e.Handled = true;
				}
			}
			
			ITextMarker FindNextMarker(Point mousePos)
			{
				var renderSize = this.RenderSize;
				var document = editor.Document;
				var textView = editor.TextArea.TextView;
				double documentHeight = textView.DocumentHeight;
				
				ITextMarker bestMarker = null;
				double bestDistance = double.PositiveInfinity;
				foreach (var marker in textMarkerService.TextMarkers) {
					if (!IsVisibleInAdorner(marker))
						continue;
					var location = document.GetLocation(marker.StartOffset);
					double visualTop = textView.GetVisualTopByDocumentLine(location.Line);
					double renderPos = visualTop / documentHeight * renderSize.Height;
					
					double distance = Math.Abs(renderPos - mousePos.Y);
					if (distance < bestDistance) {
						bestDistance = distance;
						bestMarker = marker;
					}
				}
				return bestMarker;
			}
			
			protected override void OnToolTipOpening(ToolTipEventArgs e)
			{
				base.OnToolTipOpening(e);
				var marker = FindNextMarker(Mouse.GetPosition(this));
				if (marker != null && marker.ToolTip != null) {
					this.ToolTip = marker.ToolTip;
				} else {
					// prevent tooltip from opening
					e.Handled = true;
				}
			}
		}
		#endregion
	}
}
