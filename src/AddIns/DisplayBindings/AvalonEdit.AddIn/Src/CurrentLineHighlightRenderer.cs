// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.AvalonEdit.AddIn
{
	public class CurrentLineHighlightRenderer : IBackgroundRenderer
	{
		int line;
		Pen borderPen;
		Brush backgroundBrush;
		TextView textView;
		
		public static readonly Color DefaultBackground = Color.FromArgb(22, 20, 220, 224);
		public static readonly Color DefaultBorder = Color.FromArgb(52, 0, 255, 110);
		
		public const string CurrentLineHighlight = "Current line highlight";
		
		public CurrentLineHighlightRenderer(TextView textView)
		{
			if (textView == null)
				throw new ArgumentNullException("textView");
			
			this.textView = textView;
			this.textView.BackgroundRenderers.Add(this);
			this.line = -1;
		}
		
		public void SetHighlight(int caretLine)
		{
			this.line = caretLine;
			this.textView.InvalidateLayer(this.Layer);
		}
		
		void UpdateColors(Color background, Color foreground)
		{
			this.borderPen = new Pen(new SolidColorBrush(foreground), 1);
			this.borderPen.Freeze();

			this.backgroundBrush = new SolidColorBrush(background);
			this.backgroundBrush.Freeze();
		}
		
		public KnownLayer Layer 
		{
			get 
			{
				return KnownLayer.Selection;
			}
		}
		
		public void Draw(TextView textView, DrawingContext drawingContext)
		{
			BackgroundGeometryBuilder builder = new BackgroundGeometryBuilder();
			
			builder.CornerRadius = 1;
			builder.AlignToMiddleOfPixels = true;
	
			var visualLine = this.textView.GetVisualLine(line);
			if(visualLine == null) return;
			
			var textViewPos = visualLine.GetTextViewPosition(0);
			if(textViewPos == null) return;
			
			var position = this.textView.GetVisualPosition(textViewPos, VisualYPosition.LineTop);
			if(position == null) return;
			
			var lineWidth = this.textView.ActualWidth;
			var lineHeigth = visualLine.Height;
			var linePosX = position.X - this.textView.ScrollOffset.X;
			var linePosY = position.Y - this.textView.ScrollOffset.Y;
			
			builder.AddRectangle(textView, new Rect(linePosX, linePosY, lineWidth, lineHeigth));
			
			Geometry geometry = builder.CreateGeometry();
			if (geometry != null) {
				drawingContext.DrawGeometry(backgroundBrush, borderPen, geometry);
			}
		}
		
		public static void ApplyCustomizationsToRendering(CurrentLineHighlightRenderer renderer, IEnumerable<CustomizedHighlightingColor> customizations)
		{
			renderer.UpdateColors(DefaultBackground, DefaultBorder);
			foreach (CustomizedHighlightingColor color in customizations) {
				if (color.Name == CurrentLineHighlight) {
					renderer.UpdateColors(color.Background ?? Colors.Pink, color.Foreground ?? Colors.Pink);
					break;
				}
			}
		}
	}
}
