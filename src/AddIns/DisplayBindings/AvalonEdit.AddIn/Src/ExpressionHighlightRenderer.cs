// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.Windows.Media;
using System.Collections.Generic;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Refactoring;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// Highlights expressions (references to expression under current caret).
	/// </summary>
	public class ExpressionHighlightRenderer : IBackgroundRenderer
	{
		List<Reference> renderedReferences;
		Pen borderPen;
		Brush backgroundBrush;
		TextView textView;
		public readonly Color DefaultBorderColor = Color.FromArgb(52, 30, 130, 255);	//Color.FromArgb(180, 70, 230, 70))
		public readonly Color DefaultFillColor = Color.FromArgb(22, 30, 130, 255); 	//Color.FromArgb(40, 60, 255, 60)
		readonly int borderThickness = 1;
		readonly int cornerRadius = 1;
		
		public void SetHighlight(List<Reference> renderedReferences)
		{
			if (this.renderedReferences != renderedReferences) {
				this.renderedReferences = renderedReferences;
				textView.InvalidateLayer(this.Layer);
			}
		}
		
		public void ClearHighlight()
		{
			this.SetHighlight(null);
		}
		
		public ExpressionHighlightRenderer(TextView textView)
		{
			if (textView == null)
				throw new ArgumentNullException("textView");
			this.textView = textView;
			this.borderPen = new Pen(new SolidColorBrush(DefaultBorderColor), borderThickness);
			this.backgroundBrush = new SolidColorBrush(DefaultFillColor);
			this.borderPen.Freeze();
			this.backgroundBrush.Freeze();
			this.textView.BackgroundRenderers.Add(this);
		}
		
		public KnownLayer Layer {
			get {
				return KnownLayer.Selection;
			}
		}
		
		public void Draw(TextView textView, DrawingContext drawingContext)
		{
			if (this.renderedReferences == null)
				return;
			BackgroundGeometryBuilder builder = new BackgroundGeometryBuilder();
			builder.CornerRadius = cornerRadius;
			builder.AlignToMiddleOfPixels = true;
			foreach (var reference in this.renderedReferences) {
				builder.AddSegment(textView, new TextSegment() { 
				                   	StartOffset = reference.Offset, 
				                   	Length = reference.Length });
				builder.CloseFigure();
			}
			Geometry geometry = builder.CreateGeometry();
			if (geometry != null) {
				drawingContext.DrawGeometry(backgroundBrush, borderPen, geometry);
			}
		}
	}
}
