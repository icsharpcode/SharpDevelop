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
			new FrameworkPropertyMetadata(Placement.FillContent, FrameworkPropertyMetadataOptions.AffectsParentMeasure)
		);
		
		/// <summary>
		/// Gets the placement of the specified adorner.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
		public static Placement GetPlacement(UIElement adorner)
		{
			if (adorner == null)
				throw new ArgumentNullException("adorner");
			return (Placement)adorner.GetValue(PlacementProperty);
		}
		
		/// <summary>
		/// Sets the placement of the specified adorner.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
		public static void SetPlacement(UIElement adorner, Placement placement)
		{
			if (adorner == null)
				throw new ArgumentNullException("adorner");
			if (placement == null)
				throw new ArgumentNullException("placement");
			adorner.SetValue(PlacementProperty, placement);
		}
		#endregion
		
		UIElement _adornedElement;
		AdornerOrder _Order = AdornerOrder.Content;
		
		/// <summary>
		/// Gets/Sets the element adorned by this AdornerPanel.
		/// Do not change this property after the panel was added to an AdornerLayer!
		/// </summary>
		public UIElement AdornedElement {
			get { return _adornedElement; }
			set { _adornedElement = value; }
		}
		
		/// <summary>
		/// Gets/Sets the order used to display the AdornerPanel relative to other AdornerPanels.
		/// Do not change this property after the panel was added to an AdornerLayer!
		/// </summary>
		public AdornerOrder Order {
			get { return _Order; }
			set { _Order = value; }
		}
		
		/// <summary/>
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
		
		/// <summary/>
		protected override Size ArrangeOverride(Size finalSize)
		{
			foreach (UIElement element in base.InternalChildren) {
				GetPlacement(element).Arrange(this, element, finalSize);
			}
			return finalSize;
		}
		
		private DependencyObject[] VisualChildren {
			get {
				int count = VisualTreeHelper.GetChildrenCount(this);
				DependencyObject[] children = new DependencyObject[count];
				for (int i = 0; i < children.Length; i++) {
					children[i] = VisualTreeHelper.GetChild(this, i);
				}
				return children;
			}
		}
	}
	
	/// <summary>
	/// Describes where an Adorner is positioned on the Z-Layer.
	/// </summary>
	public struct AdornerOrder : IComparable<AdornerOrder>
	{
		/// <summary>
		/// The adorner is in the background layer.
		/// </summary>
		public static readonly AdornerOrder Background = new AdornerOrder(100);
		
		/// <summary>
		/// The adorner is in the content layer.
		/// </summary>
		public static readonly AdornerOrder Content = new AdornerOrder(200);
		
		/// <summary>
		/// The adorner is in the foreground layer.
		/// </summary>
		public static readonly AdornerOrder Foreground = new AdornerOrder(300);
		
		int i;
		
		internal AdornerOrder(int i)
		{
			this.i = i;
		}
		
		/// <summary>
		/// Compares the <see cref="AdornerOrder"/> to another AdornerOrder.
		/// </summary>
		public int CompareTo(AdornerOrder other)
		{
			return i.CompareTo(other.i);
		}
	}
}
