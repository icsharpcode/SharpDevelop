using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using SharpDevelop.XamlDesigner.Dom;

namespace SharpDevelop.XamlDesigner.Placement
{
	class CanvasContainer : PositionalContainer
	{
		public override void SetPosition(DesignItem item, Rect bounds)
		{
			Canvas.SetLeft(item.View, bounds.X);
			Canvas.SetTop(item.View, bounds.Y);
			item.View.ClearValue(FrameworkElement.MarginProperty);
		}
	}
}
