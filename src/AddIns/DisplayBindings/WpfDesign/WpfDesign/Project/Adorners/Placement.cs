// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace ICSharpCode.WpfDesign.Adorners
{
	// We have to support the different coordinate spaces as explained in
	// http://myfun.spaces.live.com/blog/cns!AC1291870308F748!242.entry
	
	/// <summary>
	/// Defines how a design-time adorner is placed.
	/// </summary>
	public class Placement
	{
		PlacementSpace space = PlacementSpace.Render;
		
		/// <summary>
		/// Gets/Sets the space in which the adorner is placed.
		/// </summary>
		public PlacementSpace Space {
			get { return space; }
			set { space = value; }
		}
		
		double widthRelativeToDesiredWidth, heightRelativeToDesiredHeight;
		
		/// <summary>
		/// Gets/Sets the width of the adorner relative to the desired adorner width.
		/// </summary>
		public double WidthRelativeToDesiredWidth {
			get { return widthRelativeToDesiredWidth; }
			set { widthRelativeToDesiredWidth = value; }
		}
		
		/// <summary>
		/// Gets/Sets the height of the adorner relative to the desired adorner height.
		/// </summary>
		public double HeightRelativeToDesiredHeight {
			get { return heightRelativeToDesiredHeight; }
			set { heightRelativeToDesiredHeight = value; }
		}
		
		double widthRelativeToContentWidth, heightRelativeToContentHeight;
		
		/// <summary>
		/// Gets/Sets the width of the adorner relative to the width of the adorned item.
		/// </summary>
		public double WidthRelativeToContentWidth {
			get { return widthRelativeToContentWidth; }
			set { widthRelativeToContentWidth = value; }
		}
		
		/// <summary>
		/// Gets/Sets the height of the adorner relative to the height of the adorned item.
		/// </summary>
		public double HeightRelativeToContentHeight {
			get { return heightRelativeToContentHeight; }
			set { heightRelativeToContentHeight = value; }
		}
		
		double widthOffset, heightOffset;
		
		/// <summary>
		/// Gets/Sets an offset that is added to the adorner width for the size calculation.
		/// </summary>
		public double WidthOffset {
			get { return widthOffset; }
			set { widthOffset = value; }
		}
		
		/// <summary>
		/// Gets/Sets an offset that is added to the adorner height for the size calculation.
		/// </summary>
		public double HeightOffset {
			get { return heightOffset; }
			set { heightOffset = value; }
		}
		
		Size CalculateSize(Visual adornerVisual, UIElement adornedElement)
		{
			Size size = new Size(widthOffset, heightOffset);
			if (widthRelativeToDesiredWidth != 0 || heightRelativeToDesiredHeight != 0) {
				UIElement adornerElement = adornerVisual as UIElement;
				if (adornerElement == null) {
					throw new DesignerException("Cannot calculate the size relative to the adorner's desired size if the adorner is not an UIElement.");
				}
				size.Width += widthRelativeToDesiredWidth * adornerElement.DesiredSize.Width;
				size.Height += heightRelativeToDesiredHeight * adornerElement.DesiredSize.Height;
			}
			size.Width += widthRelativeToContentWidth * adornedElement.RenderSize.Width;
			size.Height += heightRelativeToContentHeight * adornedElement.RenderSize.Height;
			return size;
		}
	}
	
	/// <summary>
	/// Describes the space in which an adorner is placed.
	/// </summary>
	public enum PlacementSpace
	{
		/// <summary>
		/// The adorner is affected by the render transform of the adorned element.
		/// </summary>
		Render,
		/// <summary>
		/// The adorner is affected by the layout transform of the adorned element.
		/// </summary>
		Layout,
		/// <summary>
		/// The adorner is not affected by transforms of designed controls.
		/// </summary>
		Designer
	}
	
	/// <summary>
	/// The possible layers where adorners can be placed.
	/// </summary>
	public enum AdornerZLayer
	{
		/// <summary>
		/// This layer is below the other adorner layers.
		/// </summary>
		Low,
		/// <summary>
		/// This layer is for normal background adorners.
		/// </summary>
		Normal,
		/// <summary>
		/// This layer is for selection adorners
		/// </summary>
		Selection,
		/// <summary>
		/// This layer is for primary selection adorners
		/// </summary>
		PrimarySelection,
		/// <summary>
		/// This layer is above the other layers.
		/// It is used for temporary drawings, e.g. the selection frame while selecting multiple controls with the mouse.
		/// </summary>
		High
	}
}
