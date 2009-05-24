// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.AvalonEdit.Rendering;

namespace ICSharpCode.AvalonEdit.Editing
{
	sealed class FoldingMarginMarker : UIElement
	{
		internal VisualLine VisualLine;
		internal FoldingSection FoldingSection;
		
		bool isExpanded;
		
		public bool IsExpanded {
			get { return isExpanded; }
			set {
				if (isExpanded != value) {
					isExpanded = value;
					InvalidateVisual();
				}
				if (FoldingSection != null)
					FoldingSection.IsFolded = !value;
			}
		}
		
		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			base.OnMouseDown(e);
			if (!e.Handled) {
				if (e.ChangedButton == MouseButton.Left) {
					IsExpanded = !IsExpanded;
				e.Handled = true;
				}
			}
		}
		
		const double MarginSizeFactor = 0.7;
		
		protected override Size MeasureCore(Size availableSize)
		{
			double size = MarginSizeFactor * FoldingMargin.SizeFactor * (double)GetValue(TextBlock.FontSizeProperty);
			size = Math.Round(size);
			if (Math.Abs((size % 2) - 1) < 0.001) {
				size -= 1;
			}
			return new Size(size, size);
		}
		
		protected override void OnRender(DrawingContext drawingContext)
		{
			Pen blackPen = new Pen(Brushes.Black, 1);
			Rect rect = new Rect(new Point(0.5, 0.5), this.RenderSize);
			drawingContext.DrawRectangle(Brushes.White,
			                             IsMouseDirectlyOver ? blackPen : new Pen(Brushes.Gray, 1),
			                             rect);
			double middleX = rect.Left + rect.Width / 2;
			double middleY = rect.Top + rect.Height / 2;
			double space = Math.Round(rect.Width / 8) + 1;
			drawingContext.DrawLine(blackPen,
			                        new Point(rect.Left + space, middleY),
			                        new Point(rect.Right - space, middleY));
			if (!isExpanded) {
				drawingContext.DrawLine(blackPen,
				                        new Point(middleX, rect.Top + space),
				                        new Point(middleX, rect.Bottom - space));
			}
		}
		
		protected override void OnIsMouseDirectlyOverChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnIsMouseDirectlyOverChanged(e);
			InvalidateVisual();
		}
	}
}
