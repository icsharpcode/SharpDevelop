using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using SharpDevelop.XamlDesigner.Dom;

namespace SharpDevelop.XamlDesigner.Placement
{
	class GridContainer : PositionalContainer
	{
		public override void SetPosition(DesignItem item, Rect bounds)
		{
			item.View.Width = bounds.Width;
			item.View.Height = bounds.Height;
			item.View.Margin = new Thickness(bounds.X, bounds.Y, 0, 0);
			item.View.HorizontalAlignment = HorizontalAlignment.Left;
			item.View.VerticalAlignment = VerticalAlignment.Top;
		}
	}
}
