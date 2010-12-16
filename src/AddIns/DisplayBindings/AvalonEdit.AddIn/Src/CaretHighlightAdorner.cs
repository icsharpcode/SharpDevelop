// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// Description of CaretHighlightAdorner.
	/// </summary>
	public class CaretHighlightAdorner : Adorner
	{
		CaretHighlight highlight;
		
		public CaretHighlightAdorner(UIElement adornedElement, Point origin)
			: base(adornedElement)
		{
			this.Highlight = new CaretHighlight() {
				RenderTransform = new TranslateTransform(origin.X, origin.Y),
			};
		}
		
		/// <summary>
		/// Gets/sets the visual child.
		/// </summary>
		protected CaretHighlight Highlight {
			get { return highlight; }
			set {
				RemoveVisualChild(highlight);
				highlight = value;
				AddVisualChild(highlight);
				InvalidateMeasure();
			}
		}

		/// <summary>
		/// Gets the visual child.
		/// </summary>
		protected override Visual GetVisualChild(int index)
		{
			if (index == 0 && highlight != null)
				return highlight;
			else
				throw new ArgumentOutOfRangeException("index");
		}

		/// <summary>
		/// Gets the number of visual children.
		/// </summary>
		protected override int VisualChildrenCount {
			get { return highlight != null ? 1 : 0; }
		}

		/// <summary>
		/// Measure the visual child.
		/// </summary>
		protected override Size MeasureOverride(Size availableSize)
		{
			if (highlight != null) {
				highlight.Measure(availableSize);
				return availableSize;
			} else {
				return base.MeasureOverride(availableSize);
			}
		}

		/// <summary>
		/// Arrange the visual child.
		/// </summary>
		protected override Size ArrangeOverride(Size finalSize)
		{
			if (highlight != null) {
				highlight.Arrange(new Rect(new Point(0, 0), finalSize));
				return finalSize;
			} else {
				return base.ArrangeOverride(finalSize);
			}
		}
	}
}
