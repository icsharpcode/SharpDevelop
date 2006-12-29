// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ICSharpCode.WpfDesign.Designer.Controls
{
	/// <summary>
	/// Base class for elements that have a single visual child.
	/// </summary>
	public class SingleVisualChildElement : FrameworkElement
	{
		UIElement _visualChild;
		
		/// <summary>
		/// Gets/sets the visual child.
		/// </summary>
		protected UIElement VisualChild {
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
		protected override Size MeasureOverride(Size availableSize)
		{
			if (_visualChild != null) {
				_visualChild.Measure(availableSize);
				return _visualChild.DesiredSize;
			} else {
				return base.MeasureOverride(availableSize);
			}
		}
		
		/// <summary>
		/// Arrange the visual child.
		/// </summary>
		protected override Size ArrangeOverride(Size finalSize)
		{
			if (_visualChild != null) {
				_visualChild.Arrange(new Rect(new Point(0, 0), finalSize));
				return finalSize;
			} else {
				return base.ArrangeOverride(finalSize);
			}
		}
	}
}
