// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ICSharpCode.WpfDesign.Adorners
{
	/// <summary>
	/// Manages display of adorners on the design surface.
	/// </summary>
	public sealed class AdornerPanel : Panel
	{
		#region Attached Property Placement
		/// <summary>
		/// The dependency property used to store the placement of adorner visuals.
		/// </summary>
		public static readonly DependencyProperty PlacementProperty = DependencyProperty.RegisterAttached(
			"Placement", typeof(Placement), typeof(AdornerPanel),
			new FrameworkPropertyMetadata(new Placement(), FrameworkPropertyMetadataOptions.AffectsParentMeasure)
		);
		
		/// <summary>
		/// Gets the placement of the specified adorner visual.
		/// </summary>
		public static Placement GetPlacement(Visual visual)
		{
			if (visual == null)
				throw new ArgumentNullException("visual");
			return (Placement)visual.GetValue(PlacementProperty);
		}
		
		/// <summary>
		/// Sets the placement of the specified adorner visual.
		/// </summary>
		public static void SetPlacement(Visual visual, Placement placement)
		{
			if (visual == null)
				throw new ArgumentNullException("visual");
			if (placement == null)
				throw new ArgumentNullException("placement");
			visual.SetValue(PlacementProperty, placement);
		}
		#endregion
		
		UIElement _adornedElement;
		AdornerOrder _Order = AdornerOrder.Content;
		
		public UIElement AdornedElement {
			get { return _adornedElement; }
			set { _adornedElement = value; }
		}
		
		public AdornerOrder Order {
			get { return _Order; }
			set { _Order = value; }
		}
		
		protected override Size MeasureOverride(Size availableSize)
		{
			if (this.AdornedElement != null) {
				foreach (DependencyObject v in this.VisualChildren) {
					UIElement e = v as UIElement;
					if (e != null) {
						e.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
					}
				}
				return this.AdornedElement.RenderSize;
			} else {
				return base.MeasureOverride(availableSize);
			}
		}
		
		protected override Size ArrangeOverride(Size finalSize)
		{
			foreach (UIElement element in base.InternalChildren) {
				element.Arrange(new Rect(finalSize));
			}
			return finalSize;
		}
		
		private IEnumerable<DependencyObject> VisualChildren {
			get {
				int count = VisualTreeHelper.GetChildrenCount(this);
				for (int i = 0; i < count; i++) {
					yield return VisualTreeHelper.GetChild(this, i);
				}
			}
		}
	}
	
	public struct AdornerOrder : IComparable<AdornerOrder>
	{
		public static readonly AdornerOrder Background = new AdornerOrder(100);
		public static readonly AdornerOrder Content = new AdornerOrder(200);
		public static readonly AdornerOrder Foreground = new AdornerOrder(300);
		
		int i;
		
		public AdornerOrder(int i)
		{
			this.i = i;
		}
		
		public int CompareTo(AdornerOrder other)
		{
			return i.CompareTo(other.i);
		}
	}
}
