// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace ICSharpCode.AvalonEdit.Rendering
{
	sealed class CurrentLineHighlightRenderer : IBackgroundRenderer
	{
		#region Fields
		
		int line;
		TextView textView;
		
		public static readonly Color DefaultBackground = Color.FromArgb(22, 20, 220, 224);
		public static readonly Color DefaultBorder = Color.FromArgb(52, 0, 255, 110);
		
		#endregion

		#region Properties
		
		public int Line {
			get { return this.Line; } 
			set {
				if (this.line != value) {
					this.line = value;
					this.textView.InvalidateLayer(this.Layer);
				}
			}
		}
		
		public KnownLayer Layer
		{
			get { return KnownLayer.Selection; }
		}
		
		public Brush BackgroundBrush {
			get; set;
		}
		
		public Pen BorderPen {
			get; set;
		}
		
		#endregion		
		
		public CurrentLineHighlightRenderer(TextView textView)
		{
			if (textView == null)
				throw new ArgumentNullException("textView");
			
			this.BorderPen = new Pen(new SolidColorBrush(DefaultBorder), 1);
			this.BorderPen.Freeze();
			
			this.BackgroundBrush = new SolidColorBrush(DefaultBackground);
			this.BackgroundBrush.Freeze();
			
			this.textView = textView;
			this.textView.BackgroundRenderers.Add(this);
			
			this.line = 0;
		}
		
		public void Draw(TextView textView, DrawingContext drawingContext)
		{
			if(!this.textView.Options.HighlightCurrentLine)
				return;
			
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
			var linePosX = position.X;
			var linePosY = position.Y - this.textView.ScrollOffset.Y;
			
			builder.AddRectangle(textView, new Rect(linePosX, linePosY, lineWidth, lineHeigth));
			
			Geometry geometry = builder.CreateGeometry();
			if (geometry != null) {
				drawingContext.DrawGeometry(this.BackgroundBrush, this.BorderPen, geometry);
			}
		}
	}
}
