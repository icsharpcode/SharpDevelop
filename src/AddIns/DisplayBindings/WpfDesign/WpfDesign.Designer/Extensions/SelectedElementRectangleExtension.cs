// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2413 $</version>
// </file>

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

using ICSharpCode.WpfDesign.Adorners;
using ICSharpCode.WpfDesign.Extensions;

namespace ICSharpCode.WpfDesign.Designer.Extensions
{
	/// <summary>
	/// Draws a dotted line around selected UIElements.
	/// </summary>
	[ExtensionFor(typeof(UIElement))]
	public sealed class SelectedElementRectangleExtension : SelectionAdornerProvider
	{
		/// <summary>
		/// Creates a new SelectedElementRectangleExtension instance.
		/// </summary>
		public SelectedElementRectangleExtension()
		{
			Rectangle selectionRect = new Rectangle();
			selectionRect.SnapsToDevicePixels = true;
			selectionRect.Stroke = Brushes.White;
			selectionRect.IsHitTestVisible = false;
			
			Rectangle dottedRect = new Rectangle();
			dottedRect.SnapsToDevicePixels = true;
			dottedRect.Stroke = Brushes.Black;
			dottedRect.StrokeDashCap = PenLineCap.Square;
			dottedRect.StrokeDashArray = new DoubleCollection(new double[] { 0, 2 });
			dottedRect.IsHitTestVisible = false;
			
			RelativePlacement placement = new RelativePlacement(HorizontalAlignment.Stretch, VerticalAlignment.Stretch);
			placement.XOffset = -1;
			placement.YOffset = -1;
			placement.WidthOffset = 2;
			placement.HeightOffset = 2;
			
			this.AddAdorners(placement, selectionRect, dottedRect);
		}
	}
}
