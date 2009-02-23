// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>
using System;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// A WPF control that aligns its content on pixel boundaries.
	/// </summary>
	public class PixelSnapper : UIElement
	{
		public PixelSnapper()
		{
		}
		
		public PixelSnapper(UIElement visualChild)
			: this()
		{
			this.Child = visualChild;
		}
		
		UIElement _visualChild;
		
		/// <summary>
		/// Gets/sets the visual child.
		/// </summary>
		public UIElement Child {
			get { return _visualChild; }
			set {
				RemoveVisualChild(_visualChild);
				_visualChild = value;
				AddVisualChild(_visualChild);
				InvalidateMeasure();
			}
		}
		
		/// <summary>
		/// Gets the visual child.
		/// </summary>
		protected override Visual GetVisualChild(int index)
		{
			if (index == 0 && _visualChild != null)
				return _visualChild;
			else
				throw new ArgumentOutOfRangeException("index");
		}
		
		/// <summary>
		/// Gets the number of visual children.
		/// </summary>
		protected override int VisualChildrenCount {
			get { return _visualChild != null ? 1 : 0; }
		}
		
		/// <summary>
		/// Measure the visual child.
		/// </summary>
		protected override Size MeasureCore(Size availableSize)
		{
			if (_visualChild != null) {
				_visualChild.Measure(availableSize);
				return _visualChild.DesiredSize;
			} else {
				return base.MeasureCore(availableSize);
			}
		}
		
		protected override void ArrangeCore(Rect finalRect)
		{
			base.ArrangeCore(finalRect);
			if (_visualChild != null) {
				_pixelOffset = GetPixelOffset();
				//LoggingService.Debug("Arrange, Pixel Offset=" + _pixelOffset);
				_visualChild.Arrange(new Rect(new Point(_pixelOffset.X, _pixelOffset.Y), finalRect.Size));
				
				// check again after the whole layout pass has finished, maybe we need to move
				Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new ThreadStart(CheckLayout));
			}
		}
		
		private void CheckLayout()
		{
			Point pixelOffset = GetPixelOffset();
			if (!AreClose(pixelOffset, _pixelOffset)) {
				InvalidateArrange();
			}
		}
		
		// Gets the matrix that will convert a point from "above" the
		// coordinate space of a visual into the the coordinate space
		// "below" the visual.
		static Matrix GetVisualTransform(Visual v)
		{
			if (v != null) {
				Matrix m = Matrix.Identity;
				
				Transform transform = VisualTreeHelper.GetTransform(v);
				if (transform != null) {
					m *= transform.Value;
				}
				
				Vector offset = VisualTreeHelper.GetOffset(v);
				m.Translate(offset.X, offset.Y);
				
				return m;
			}
			
			return Matrix.Identity;
		}
		
		static Point ApplyVisualTransform(Point point, Visual v, bool inverse)
		{
			if (v != null) {
				Matrix visualTransform = GetVisualTransform(v);
				if (inverse)
					visualTransform.Invert();
				point = visualTransform.Transform(point);
			}
			return point;
		}
		
		private Point GetPixelOffset()
		{
			Point pixelOffset = new Point();
			
			PresentationSource ps = PresentationSource.FromVisual(this);
			if (ps != null) {
				Visual rootVisual = ps.RootVisual;
				
				// Transform (0,0) from this element up to pixels.
				pixelOffset = this.TransformToAncestor(rootVisual).Transform(pixelOffset);
				pixelOffset = ApplyVisualTransform(pixelOffset, rootVisual, false);
				pixelOffset = ps.CompositionTarget.TransformToDevice.Transform(pixelOffset);
				
				// Round the origin to the nearest whole pixel.
				pixelOffset.X = Math.Round(pixelOffset.X);
				pixelOffset.Y = Math.Round(pixelOffset.Y);
				
				// Transform the whole-pixel back to this element.
				pixelOffset = ps.CompositionTarget.TransformFromDevice.Transform(pixelOffset);
				pixelOffset = ApplyVisualTransform(pixelOffset, rootVisual, true);
				pixelOffset = rootVisual.TransformToDescendant(this).Transform(pixelOffset);
			}
			
			return pixelOffset;
		}
		
		static bool AreClose(Point point1, Point point2)
		{
			return AreClose(point1.X, point2.X) && AreClose(point1.Y, point2.Y);
		}
		
		static bool AreClose(double value1, double value2)
		{
			if (value1 == value2)
			{
				return true;
			}
			double delta = value1 - value2;
			return ((delta < 1.53E-06) && (delta > -1.53E-06));
		}
		
		private Point _pixelOffset;
	}
}
