// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;

using ICSharpCode.AvalonEdit.Editing;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// Description of CaretHighlightAdorner.
	/// </summary>
	public class CaretHighlightAdorner : Adorner
	{
		const double SizeFactor = 0.5;
		
		Rect min, max;
		Pen blackPen;
		
		public CaretHighlightAdorner(TextArea textArea)
			: base(textArea.TextView)
		{
			Rect caret = textArea.Caret.CalculateCaretRectangle();
			
			this.min = caret;
			this.max = new Rect(caret.Location, new Size(caret.Width + Math.Max(caret.Width, caret.Height) * SizeFactor, caret.Height + Math.Max(caret.Width, caret.Height) * SizeFactor));
			
			Vector centerOffset = new Vector(caret.Width / 2, caret.Height / 2);
			
			min.Offset(-textArea.TextView.ScrollOffset);
			max.Offset(-textArea.TextView.ScrollOffset);
			
			max.Offset(-centerOffset);
			
			blackPen = new Pen(TextBlock.GetForeground(textArea.TextView).Clone(), 1);
		}
		
		protected override void OnRender(DrawingContext drawingContext)
		{
			var geometry = new RectangleGeometry(max, 2, 2);
			
			geometry.BeginAnimation(RectangleGeometry.RectProperty, new RectAnimation(max, min, new Duration(TimeSpan.FromMilliseconds(400))) { AutoReverse = true });
			blackPen.Brush.BeginAnimation(Brush.OpacityProperty, new DoubleAnimation(1, 0, new Duration(TimeSpan.FromMilliseconds(600))) { BeginTime = TimeSpan.FromMilliseconds(200), AutoReverse = true });
			
			drawingContext.DrawGeometry(null, blackPen, geometry);
		}
	}
}
