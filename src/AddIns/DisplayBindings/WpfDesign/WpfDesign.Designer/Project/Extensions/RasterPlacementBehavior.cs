// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using ICSharpCode.WpfDesign.Adorners;
using ICSharpCode.WpfDesign.Designer.Controls;
using System.Windows;

namespace ICSharpCode.WpfDesign.Designer.Extensions
{
	public class RasterPlacementBehavior : DefaultPlacementBehavior
	{
		Canvas surface;
		AdornerPanel adornerPanel;
		bool rasterDrawn = false;
		int raster = 5;

		public override void BeginPlacement(PlacementOperation operation)
		{
			base.BeginPlacement(operation);
			
			DesignPanel designPanel = ExtendedItem.Services.DesignPanel as DesignPanel;
			if (designPanel != null)
				raster = designPanel.RasterWidth;
			
			CreateSurface(operation);
		}

		public override void EndPlacement(PlacementOperation operation)
		{
			base.EndPlacement(operation);
			DeleteSurface();
		}

		public override void EnterContainer(PlacementOperation operation)
		{
			base.EnterContainer(operation);
			CreateSurface(operation);
		}

		public override void LeaveContainer(PlacementOperation operation)
		{
			base.LeaveContainer(operation);
			DeleteSurface();
		}

		void CreateSurface(PlacementOperation operation)
		{
			if (ExtendedItem.Services.GetService<IDesignPanel>() != null)
			{
				surface = new Canvas();
				adornerPanel = new AdornerPanel();
				adornerPanel.SetAdornedElement(ExtendedItem.View, ExtendedItem);
				AdornerPanel.SetPlacement(surface, AdornerPlacement.FillContent);
				adornerPanel.Children.Add(surface);
				ExtendedItem.Services.DesignPanel.Adorners.Add(adornerPanel);
			}
		}

		void DeleteSurface()
		{
			rasterDrawn = false;
			if (surface != null)
			{
				ExtendedItem.Services.DesignPanel.Adorners.Remove(adornerPanel);
				adornerPanel = null;
				surface = null;
			}
		}

		public override void BeforeSetPosition(PlacementOperation operation)
		{
			base.BeforeSetPosition(operation);
			if (surface == null) return;

			DesignPanel designPanel = ExtendedItem.Services.DesignPanel as DesignPanel;
			if (designPanel == null || !designPanel.UseRasterPlacement)
				return;

			if (Keyboard.IsKeyDown(Key.LeftCtrl))
			{
				surface.Children.Clear();
				rasterDrawn = false;
				return;
			}
			
			drawRaster();

			var bounds = operation.PlacedItems[0].Bounds;
			bounds.Y = ((int)bounds.Y/raster)*raster;
			bounds.X = ((int)bounds.X/raster)*raster;
			bounds.Width = Convert.ToInt32((bounds.Width/raster))*raster;
			bounds.Height = Convert.ToInt32((bounds.Height/raster))*raster;
			operation.PlacedItems[0].Bounds = bounds;
		}

		public override Point PlacePoint(Point point)
		{
			if (surface == null)
				return base.PlacePoint(point);

			DesignPanel designPanel = ExtendedItem.Services.DesignPanel as DesignPanel;
			if (designPanel == null || !designPanel.UseRasterPlacement)
				return base.PlacePoint(point);

			if (Keyboard.IsKeyDown(Key.LeftCtrl))
			{
				surface.Children.Clear();
				rasterDrawn = false;
				return base.PlacePoint(point);
			}

			drawRaster();

			point.Y = ((int)point.Y / raster) * raster;
			point.X = ((int)point.X / raster) * raster;

			return point;
		}

		private void drawRaster()
		{
			if (!rasterDrawn)
			{
				rasterDrawn = true;

				var w = ModelTools.GetWidth(ExtendedItem.View);
				var h = ModelTools.GetHeight(ExtendedItem.View);
				var dash = new DoubleCollection() {1, raster - 1};
				for (int i = 0; i <= h; i += raster)
				{
					var line = new Line()
					{
						X1 = 0,
						Y1 = i,
						X2 = w,
						Y2 = i,
						StrokeThickness = 1,
						Stroke = Brushes.Black,
						StrokeDashArray = dash,
					};
					surface.Children.Add(line);
				}
			}
		}
	}
}
