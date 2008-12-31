using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Diagnostics;

namespace SharpDevelop.XamlDesigner.Controls
{
	public class GeneralAdorner : Adorner
	{
		public GeneralAdorner(UIElement target)
			: base(target)
		{
			ChildSize = new Size(double.NaN, double.NaN);
		}

		FrameworkElement child;
		Point scale = new Point();
		MatrixTransform desiredTransform;

		public Point ChildLocation { get; set; }
		public Size ChildSize { get; set; }

		public FrameworkElement Child
		{
			get 
			{ 
				return child; 
			}
			set
			{
				if (child != value) {
					RemoveVisualChild(child);
					RemoveLogicalChild(child);
					child = value;
					AddLogicalChild(value);
					AddVisualChild(value);
					InvalidateMeasure();
				}
			}
		}

		protected override int VisualChildrenCount
		{
			get { return child == null ? 0 : 1; }
		}

		protected override Visual GetVisualChild(int index)
		{
			return child;
		}

		protected override Size MeasureOverride(Size constraint)
		{
			var elementMatrix = ((Transform)AdornedElement.TransformToVisual(Parent as Visual)).Value;
			scale.X = elementMatrix.Transform(new Vector(1, 0)).Length;
			scale.Y = elementMatrix.Transform(new Vector(0, 1)).Length;

			elementMatrix.ScalePrepend(1 / scale.X, 1 / scale.Y);
			desiredTransform = new MatrixTransform(elementMatrix);
			
			var result = AdornedElement.RenderSize;
			result.Width *= scale.X;
			result.Height *= scale.Y;
			
			if (child != null) {
				if (!double.IsNaN(ChildSize.Width)) {
					child.Width = ChildSize.Width * scale.X;
				}
				if (!double.IsNaN(ChildSize.Height)) {
					child.Height = ChildSize.Height * scale.Y;
				}
				child.Measure(result);
			}

			return result;
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			if (child != null) {
				var p = new Point(ChildLocation.X * scale.X, ChildLocation.Y * scale.Y);
				child.Arrange(new Rect(p, finalSize));
			}
			return finalSize;
		}

		public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
		{
			return desiredTransform;
		}
	}
}
