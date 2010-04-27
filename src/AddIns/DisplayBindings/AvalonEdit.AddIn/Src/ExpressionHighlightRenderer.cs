// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Konicek" email="martin.konicek@gmail.com"/>
//     <version>$Revision: $</version>
// </file>
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
			//this.borderPen = new Pen(new SolidColorBrush(Color.FromRgb(70, 230, 70)), 1);
			this.borderPen = new Pen(Brushes.Transparent, 1);
			this.borderPen.Freeze();
			this.backgroundBrush = new SolidColorBrush(Color.FromArgb(120, 60, 255, 60));
			this.backgroundBrush.Freeze();
			this.textView = textView;
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
			builder.CornerRadius = 1;
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
