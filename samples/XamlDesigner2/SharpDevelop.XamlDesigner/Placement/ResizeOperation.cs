using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using SharpDevelop.XamlDesigner.Dom;

namespace SharpDevelop.XamlDesigner.Placement
{
	class ResizeOperation
	{
		public ResizeOperation(DesignItem item, ResizeDirection dir)
		{
			this.dir = dir;

			PlacementInfo = new PlacementInfo();
			PlacementInfo.Item = item;
			PlacementInfo.OriginalBounds = item.GetBounds();
		}

		public PlacementInfo PlacementInfo;
		ResizeDirection dir;

		public void Resize(Vector delta)
		{
			double dx = 0;
			double dy = 0;

			var ha = ResizeThumb.GetHorizontalAlignment(dir);
			var va = ResizeThumb.GetVerticalAlignmentt(dir);

			if (ha == HorizontalAlignment.Left) dx = -delta.X;
			if (ha == HorizontalAlignment.Right) dx = delta.X;
			if (va == VerticalAlignment.Top) dy = -delta.Y;
			if (va == VerticalAlignment.Bottom) dy = delta.Y;

			var newWidth = Math.Round(Math.Max(0, PlacementInfo.OriginalBounds.Width + dx));
			var newHeight = Math.Round(Math.Max(0, PlacementInfo.OriginalBounds.Height + dy));

			//item.Properties.GetProperty(FrameworkElement.WidthProperty).SetValue(newWidth);
			//item.Properties.GetProperty(FrameworkElement.HeightProperty).SetValue(newHeight);

			PlacementInfo.Item.View.Width = newWidth;
			PlacementInfo.Item.View.Height = newHeight;
			PlacementInfo.NewBoundsInContainer = PlacementInfo.Item.GetBounds();
		}

		public void Abort()
		{
		}

		public void Commit()
		{
		}
	}
}
