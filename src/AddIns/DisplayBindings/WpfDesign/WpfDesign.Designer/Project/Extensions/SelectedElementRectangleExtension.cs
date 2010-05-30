// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
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
            selectionRect.Stroke = new SolidColorBrush(Color.FromRgb(51, 51, 51));
            selectionRect.StrokeThickness = 1.5;
            selectionRect.IsHitTestVisible = false;

            RelativePlacement placement = new RelativePlacement(HorizontalAlignment.Stretch, VerticalAlignment.Stretch);
            placement.XOffset = -1;
            placement.YOffset = -1;
            placement.WidthOffset = 2;
            placement.HeightOffset = 2;

            this.AddAdorners(placement, selectionRect);
		}
	}
}
