// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Media;
using System.Windows.Shapes;
using ICSharpCode.WpfDesign.Adorners;
using ICSharpCode.WpfDesign.Extensions;
using ICSharpCode.WpfDesign.Designer.Controls;
using System.Windows;

namespace ICSharpCode.WpfDesign.Designer.Extensions
{
	/// <summary>
	/// Draws a dotted line around selected UIElements.
	/// </summary>
	[ExtensionFor(typeof(UIElement))]
	public class SelectedElementRectangleExtension : SelectionAdornerProvider
	{
		/// <summary>
		/// Creates a new SelectedElementRectangleExtension instance.
		/// </summary>
		public SelectedElementRectangleExtension()
		{
			Rectangle r = new Rectangle();
			r.SnapsToDevicePixels = true;
			r.Stroke = Brushes.Black;
			r.StrokeDashCap = PenLineCap.Square;
			r.StrokeDashArray = new DoubleCollection(new double[] { 0, 2 });
			r.IsHitTestVisible = false;
			
			RelativePlacement placement = new RelativePlacement();
			placement.WidthRelativeToContentWidth = 1;
			placement.HeightRelativeToContentHeight = 1;
			placement.WidthOffset = 2;
			placement.HeightOffset = 2;
			
			placement.XOffset = -1;
			placement.YOffset = -1;
			
			this.AddAdorner(r, placement);
		}
	}
}
