// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit
{
	/// <summary>
	/// Redners a ruler at a certain colum
	/// </summary>
	public class ColumnRulerRenderer : IBackgroundRenderer
	{
		
		Pen pen;
		int column;
		TextView textView;
		
		public ColumnRulerRenderer(TextView textView)
		{
			if (textView == null)
				throw new ArgumentNullException("textView");
			
			this.pen = new Pen(Brushes.LightGray, 1);
			this.pen.Freeze();
			this.textView = textView;
			this.textView.BackgroundRenderers.Add(this);
		}
		
		public KnownLayer Layer {
			get {
				return KnownLayer.Background;
			}
		}
		
		public void SetRuler(int column, Brush brush)
		{
			if (this.column != column) {
				this.column = column;
				textView.InvalidateLayer(this.Layer);
			}
			if (pen.Brush != brush) {
				this.pen = new Pen(brush, 1);
				this.pen.Freeze();
				textView.InvalidateLayer(this.Layer);
			}
		}
		
		public void Draw(TextView textView, System.Windows.Media.DrawingContext drawingContext)
		{
			if(column < 1)
				return;
			
			double offset = textView.WideSpaceWidth * column;
			System.Windows.Size pixelSize = PixelSnapHelpers.GetPixelSize(textView);
			double markerXPos = PixelSnapHelpers.PixelAlign(offset, pixelSize.Width);
			Point start = new Point(markerXPos, 0);
			Point end = new Point(markerXPos, Math.Max(textView.DocumentHeight, textView.ActualHeight));
			
			drawingContext.DrawLine(pen, start, end);
		}
	}
}
