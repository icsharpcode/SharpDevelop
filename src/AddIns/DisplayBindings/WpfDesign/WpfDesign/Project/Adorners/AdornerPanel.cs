// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
			"Placement", typeof(AdornerPlacement), typeof(AdornerPanel),
			new FrameworkPropertyMetadata(AdornerPlacement.FillContent, FrameworkPropertyMetadataOptions.AffectsParentMeasure)
		);
		
		/// <summary>
		/// Gets the placement of the specified adorner.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
		public static AdornerPlacement GetPlacement(UIElement adorner)
		{
			if (adorner == null)
				throw new ArgumentNullException("adorner");
			return (AdornerPlacement)adorner.GetValue(PlacementProperty);
		}
		
		/// <summary>
		/// Sets the placement of the specified adorner.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
		public static void SetPlacement(UIElement adorner, AdornerPlacement placement)
		{
			if (adorner == null)
				throw new ArgumentNullException("adorner");
			if (placement == null)
				throw new ArgumentNullException("placement");
			adorner.SetValue(PlacementProperty, placement);
		}
		#endregion
		
		UIElement _adornedElement;
		DesignItem _adornedDesignItem;
		AdornerOrder _Order = AdornerOrder.Content;
		
		
		/// <summary>
		/// Gets the element adorned by this AdornerPanel.
		/// </summary>
		public UIElement AdornedElement {
			get { return _adornedElement; }
		}
		
		/// <summary>
		/// Gets the design item adorned by this AdornerPanel.
		/// </summary>
		public DesignItem AdornedDesignItem {
			get { return _adornedDesignItem; }
		}
		
		/// <summary>
		/// Sets the AdornedElement and AdornedDesignItem properties.
		/// This method can be called only once.
		/// </summary>
		public void SetAdornedElement(UIElement adornedElement, DesignItem adornedDesignItem)
		{
			if (adornedElement == null)
				throw new ArgumentNullException("adornedElement");
			
			if (_adornedElement == adornedElement && _adornedDesignItem == adornedDesignItem) {
				return; // ignore calls when nothing was changed
			}
			
			if (_adornedElement != null)
				throw new InvalidOperationException("AdornedElement is already set.");
			
			_adornedElement = adornedElement;
			_adornedDesignItem = adornedDesignItem;
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
				foreach (DependencyObject v in base.InternalChildren) {
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
	}
	
	/// <summary>
	/// Describes where an Adorner is positioned on the Z-Layer.
	/// </summary>
	public struct AdornerOrder : IComparable<AdornerOrder>, IEquatable<AdornerOrder>
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
		/// The adorner is in the layer behind the foreground but above the content. This layer
		/// is used for the gray-out effect.
		/// </summary>
		public static readonly AdornerOrder BehindForeground = new AdornerOrder(280);
		
		/// <summary>
		/// The adorner is in the foreground layer.
		/// </summary>
		public static readonly AdornerOrder Foreground = new AdornerOrder(300);
		
		int i;
		
		internal AdornerOrder(int i)
		{
			this.i = i;
		}
		
		/// <summary/>
		public override int GetHashCode()
		{
			return i.GetHashCode();
		}
		
		/// <summary/>
		public override bool Equals(object obj)
		{
			if (!(obj is AdornerOrder)) return false;
			return this == (AdornerOrder)obj;
		}
		
		/// <summary/>
		public bool Equals(AdornerOrder other)
		{
			return i == other.i;
		}
		
		/// <summary>
		/// Compares the <see cref="AdornerOrder"/> to another AdornerOrder.
		/// </summary>
		public int CompareTo(AdornerOrder other)
		{
			return i.CompareTo(other.i);
		}
		
		/// <summary/>
		public static bool operator ==(AdornerOrder leftHandSide, AdornerOrder rightHandSide)
		{
			return leftHandSide.i == rightHandSide.i;
		}
		
		/// <summary/>
		public static bool operator !=(AdornerOrder leftHandSide, AdornerOrder rightHandSide)
		{
			return leftHandSide.i != rightHandSide.i;
		}
		
		/// <summary/>
		public static bool operator <(AdornerOrder leftHandSide, AdornerOrder rightHandSide)
		{
			return leftHandSide.i < rightHandSide.i;
		}
		
		/// <summary/>
		public static bool operator <=(AdornerOrder leftHandSide, AdornerOrder rightHandSide)
		{
			return leftHandSide.i <= rightHandSide.i;
		}
		
		/// <summary/>
		public static bool operator >(AdornerOrder leftHandSide, AdornerOrder rightHandSide)
		{
			return leftHandSide.i > rightHandSide.i;
		}
		
		/// <summary/>
		public static bool operator >=(AdornerOrder leftHandSide, AdornerOrder rightHandSide)
		{
			return leftHandSide.i >= rightHandSide.i;
		}
	}
}
