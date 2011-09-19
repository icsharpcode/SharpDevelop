// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;

namespace ICSharpCode.AvalonEdit.Search
{
	public class SearchPanel
	{
		TextArea textArea;
		StackPanel searchBox;
		TextBox searchTextBox;
		Border layer;
		SearchResultBackgroundRenderer renderer;
		
		
		public SearchPanel(TextArea textArea)
		{
			if (textArea == null)
				throw new ArgumentNullException("textArea");
			this.textArea = textArea;
			
			searchBox = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(3) };
			searchBox.KeyDown += SearchLayerKeyDown;
			layer = new Border {
				Child = searchBox,
				Background = Brushes.WhiteSmoke,
				VerticalAlignment = VerticalAlignment.Top,
				HorizontalAlignment = HorizontalAlignment.Right,
				Cursor = Cursors.Arrow
			};
			
			var closeButton = new Button {
				Content = "x",
				Margin = new Thickness(3)
			};
			closeButton.Click += delegate { Uninstall(); };
			
			searchTextBox = new TextBox {
				Width = 150, Height = 24
			};
			searchTextBox.TextChanged += SearchTextBoxTextChanged;
			
			searchBox.Children.Add(closeButton);
			searchBox.Children.Add(searchTextBox);
			
			textArea.TextView.Layers.Add(layer);
			
			renderer = new SearchResultBackgroundRenderer();
			textArea.TextView.BackgroundRenderers.Add(renderer);
			
			searchTextBox.Focus();
		}

		void SearchTextBoxTextChanged(object sender, TextChangedEventArgs e)
		{
			renderer.CurrentResults.Clear();
			if (!string.IsNullOrEmpty(searchTextBox.Text)) {
				string text = textArea.Document.Text;
				int lastResult = text.IndexOf(searchTextBox.Text, StringComparison.OrdinalIgnoreCase);
	
				while (lastResult > -1) {
					renderer.CurrentResults.Add(new SearchResult { StartOffset = lastResult, Length = searchTextBox.Text.Length });
					lastResult = text.IndexOf(searchTextBox.Text, lastResult + searchTextBox.Text.Length, StringComparison.OrdinalIgnoreCase);
				}
			}
			textArea.TextView.InvalidateLayer(KnownLayer.Selection);
		}

		void SearchLayerKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape) {
				Uninstall();
			}
		}

		void Uninstall()
		{
			textArea.TextView.Layers.Remove(layer);
			textArea.TextView.BackgroundRenderers.Remove(renderer);
		}
	}
	
	class SearchResult : TextSegment
	{
	}
	
	class SearchResultBackgroundRenderer : IBackgroundRenderer
	{
		TextSegmentCollection<SearchResult> currentResults = new TextSegmentCollection<SearchResult>();
		
		public TextSegmentCollection<SearchResult> CurrentResults {
			get { return currentResults; }
		}
		
		public KnownLayer Layer {
			get {
				// draw behind selection
				return KnownLayer.Selection;
			}
		}
		
		public void Draw(TextView textView, DrawingContext drawingContext)
		{
			if (textView == null)
				throw new ArgumentNullException("textView");
			if (drawingContext == null)
				throw new ArgumentNullException("drawingContext");
			
			if (currentResults == null || !textView.VisualLinesValid)
				return;
			
			var visualLines = textView.VisualLines;
			if (visualLines.Count == 0)
				return;
			
			int viewStart = visualLines.First().FirstDocumentLine.Offset;
			int viewEnd = visualLines.Last().LastDocumentLine.EndOffset;
			
			foreach (SearchResult result in currentResults.FindOverlappingSegments(viewStart, viewEnd - viewStart)) {
				BackgroundGeometryBuilder geoBuilder = new BackgroundGeometryBuilder();
				geoBuilder.AlignToWholePixels = true;
				geoBuilder.CornerRadius = 3;
				geoBuilder.AddSegment(textView, result);
				Geometry geometry = geoBuilder.CreateGeometry();
				if (geometry != null) {
					SolidColorBrush brush = new SolidColorBrush(Colors.LightGreen);
					brush.Freeze();
					drawingContext.DrawGeometry(brush, null, geometry);
				}
			}
		}
	}
}
