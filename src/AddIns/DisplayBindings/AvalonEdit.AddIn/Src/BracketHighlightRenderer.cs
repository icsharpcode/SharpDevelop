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
using System.Diagnostics;
using System.Windows.Media;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.AvalonEdit.AddIn
{
	public class BracketHighlightRenderer : IBackgroundRenderer
	{
		BracketSearchResult result;
		Pen borderPen;
		Brush backgroundBrush;
		TextView textView;
		
		public static readonly Color DefaultBackground = Color.FromArgb(22, 0, 0, 255);
		public static readonly Color DefaultBorder = Color.FromArgb(52, 0, 0, 255);
		
		public const string BracketHighlight = "Bracket highlight";
		
		public void SetHighlight(BracketSearchResult result)
		{
			if (this.result != result) {
				this.result = result;
				textView.InvalidateLayer(this.Layer);
			}
		}
		
		public BracketHighlightRenderer(TextView textView)
		{
			if (textView == null)
				throw new ArgumentNullException("textView");
			
			this.textView = textView;
			
			this.textView.BackgroundRenderers.Add(this);
		}

		void UpdateColors(Color background, Color foreground)
		{
			this.borderPen = new Pen(new SolidColorBrush(foreground), 1);
			this.borderPen.Freeze();

			this.backgroundBrush = new SolidColorBrush(background);
			this.backgroundBrush.Freeze();
		}
		
		public KnownLayer Layer {
			get {
				return KnownLayer.Selection;
			}
		}
		
		public void Draw(TextView textView, DrawingContext drawingContext)
		{
			if (this.result == null)
				return;
			
			BackgroundGeometryBuilder builder = new BackgroundGeometryBuilder();
			
			builder.CornerRadius = 1;
			builder.AlignToMiddleOfPixels = true;
			
			builder.AddSegment(textView, new TextSegment() { StartOffset = result.OpeningBracketOffset, Length = result.OpeningBracketLength });
			builder.CloseFigure(); // prevent connecting the two segments
			builder.AddSegment(textView, new TextSegment() { StartOffset = result.ClosingBracketOffset, Length = result.ClosingBracketLength });
			
			Geometry geometry = builder.CreateGeometry();
			if (geometry != null) {
				drawingContext.DrawGeometry(backgroundBrush, borderPen, geometry);
			}
		}
		
		public static void ApplyCustomizationsToRendering(BracketHighlightRenderer renderer, IEnumerable<CustomizedHighlightingColor> customizations)
		{
			renderer.UpdateColors(DefaultBackground, DefaultBorder);
			foreach (CustomizedHighlightingColor color in customizations) {
				if (color.Name == BracketHighlight) {
					renderer.UpdateColors(color.Background ?? Colors.Blue, color.Foreground ?? Colors.Blue);
					// 'break;' is necessary because more specific customizations come first in the list
					// (language-specific customizations are first, followed by 'all languages' customizations)
					break;
				}
			}
		}
	}
}
