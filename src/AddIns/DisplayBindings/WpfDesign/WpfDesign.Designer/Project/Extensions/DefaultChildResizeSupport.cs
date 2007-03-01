// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows;
using ICSharpCode.WpfDesign.Adorners;
using ICSharpCode.WpfDesign.Extensions;

namespace ICSharpCode.WpfDesign.Designer.Extensions
{
	/// <summary>
	/// Implements IChildResizeSupport supporting size changes using the
	/// Width, Height, Margin properties.
	/// </summary>
	[ExtensionFor(typeof(FrameworkElement))]
	public sealed class DefaultChildResizeSupport : BehaviorExtension, IChildResizeSupport
	{
		internal static readonly DefaultChildResizeSupport Instance = new DefaultChildResizeSupport();
		
		/// <inherits/>
		protected override void OnInitialized()
		{
			base.OnInitialized();
			this.ExtendedItem.AddBehavior(typeof(IChildResizeSupport), this);
		}
		
		/// <inherits/>
		public bool CanResizeChild(DesignItem childItem)
		{
			FrameworkElement child = (FrameworkElement)childItem.Component;
			return !double.IsNaN(child.Width)
				|| !double.IsNaN(child.Height)
				|| childItem.Properties[FrameworkElement.MarginProperty].IsSet;
		}
		
		/// <inherits/>
		public Placement GetPlacement(DesignItem childItem, double horizontalChange, double verticalChange, HorizontalAlignment horizontal, VerticalAlignment vertical)
		{
			FrameworkElement child = (FrameworkElement)childItem.Component;
			if (childItem.Properties[FrameworkElement.MarginProperty].IsSet == false) {
				if (child.HorizontalAlignment == HorizontalAlignment.Left)
					horizontal = HorizontalAlignment.Right;
				else if (child.HorizontalAlignment == HorizontalAlignment.Right)
					horizontal = HorizontalAlignment.Left;
				else
					horizontal = HorizontalAlignment.Center;
				if (child.VerticalAlignment == VerticalAlignment.Top)
					horizontal = HorizontalAlignment.Right;
				else if (child.VerticalAlignment == VerticalAlignment.Bottom)
					vertical = VerticalAlignment.Bottom;
				else
					vertical = VerticalAlignment.Center;
			}
			return RootElementResizeSupport.Instance.GetPlacement(childItem, horizontalChange, verticalChange, horizontal, vertical);
		}
		
		/// <inherits/>
		public void Resize(DesignItem childItem, double horizontalChange, double verticalChange, HorizontalAlignment horizontal, VerticalAlignment vertical)
		{
			RelativePlacement p = (RelativePlacement)GetPlacement(childItem, horizontalChange, verticalChange, horizontal, vertical);
			Resize(childItem, p);
		}
		
		internal static void Resize(DesignItem childItem, RelativePlacement p)
		{
			DesignItemProperty margin = childItem.Properties[FrameworkElement.MarginProperty];
			if (margin.IsSet) {
				Thickness t = (Thickness)margin.ValueOnInstance;
				t.Left += p.XOffset;
				t.Top += p.YOffset;
				t.Right -= p.XOffset + p.WidthOffset;
				t.Bottom -= p.YOffset + p.HeightOffset;
				margin.SetValue(t);
			}
			
			double horizontalChange = p.WidthOffset;
			double verticalChange = p.HeightOffset;
			
			FrameworkElement child = (FrameworkElement)childItem.Component;
			double width = child.Width;
			double height = child.Height;
			if (margin.IsSet) {
				// when margin is used, only set width/height if it is not Auto
				if (!double.IsNaN(width)) {
					childItem.Properties[FrameworkElement.WidthProperty].SetValue( Math.Max(0, horizontalChange + width) );
				}
				if (!double.IsNaN(height)) {
					childItem.Properties[FrameworkElement.HeightProperty].SetValue( Math.Max(0, verticalChange + height) );
				}
			} else {
				// when margin is not used, we'll have to set width+height
				// but don't create width if we don't have any horizontal change
				if (double.IsNaN(width) && horizontalChange != 0) {
					width = child.ActualWidth;
				}
				if (double.IsNaN(height) && verticalChange != 0) {
					height = child.ActualHeight;
				}
				if (!double.IsNaN(width)) {
					childItem.Properties[FrameworkElement.WidthProperty].SetValue( Math.Max(0, horizontalChange + width) );
				}
				if (!double.IsNaN(height)) {
					childItem.Properties[FrameworkElement.HeightProperty].SetValue( Math.Max(0, verticalChange + height) );
				}
			}
		}
	}
	
	sealed class RootElementResizeSupport : IChildResizeSupport
	{
		public static readonly RootElementResizeSupport Instance = new RootElementResizeSupport();
		
		public bool CanResizeChild(DesignItem child)
		{
			return true;
		}
		
		public Placement GetPlacement(DesignItem child, double horizontalChange, double verticalChange, HorizontalAlignment horizontal, VerticalAlignment vertical)
		{
			RelativePlacement rp = new RelativePlacement(HorizontalAlignment.Stretch, VerticalAlignment.Stretch);
			
			if (horizontal == HorizontalAlignment.Right) {
				rp.WidthOffset += horizontalChange;
			} else if (horizontal == HorizontalAlignment.Left) {
				rp.XOffset -= horizontalChange;
				rp.WidthOffset += horizontalChange;
			} else {
				rp.XOffset -= horizontalChange;
				rp.WidthOffset += horizontalChange * 2;
			}
			
			if (vertical == VerticalAlignment.Bottom) {
				rp.HeightOffset += verticalChange;
			} else if (vertical == VerticalAlignment.Top) {
				rp.YOffset -= verticalChange;
				rp.HeightOffset += verticalChange;
			} else {
				rp.YOffset -= verticalChange;
				rp.HeightOffset += verticalChange * 2;
			}
			
			return rp;
		}
		
		public void Resize(DesignItem childItem, double horizontalChange, double verticalChange, HorizontalAlignment horizontal, VerticalAlignment vertical)
		{
			FrameworkElement child = (FrameworkElement)childItem.Component;
			double width = child.Width;
			double height = child.Height;
			if (double.IsNaN(width) && horizontalChange != 0) {
				width = child.ActualWidth;
			}
			if (double.IsNaN(height) && verticalChange != 0) {
				height = child.ActualHeight;
			}
			if (!double.IsNaN(width)) {
				childItem.Properties[FrameworkElement.WidthProperty].SetValue( Math.Max(0, horizontalChange + width) );
			}
			if (!double.IsNaN(height)) {
				childItem.Properties[FrameworkElement.HeightProperty].SetValue( Math.Max(0, verticalChange + height) );
			}
		}
	}
}
