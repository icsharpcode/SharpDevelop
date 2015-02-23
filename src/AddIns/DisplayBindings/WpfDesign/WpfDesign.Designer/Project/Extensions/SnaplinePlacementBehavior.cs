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
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ICSharpCode.WpfDesign.Adorners;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Windows.Input;

namespace ICSharpCode.WpfDesign.Designer.Extensions
{
	public class SnaplinePlacementBehavior : RasterPlacementBehavior
	{
		public static bool GetDisableSnaplines(DependencyObject obj)
		{
			return (bool)obj.GetValue(DisableSnaplinesProperty);
		}

		public static void SetDisableSnaplines(DependencyObject obj, bool value)
		{
			obj.SetValue(DisableSnaplinesProperty, value);
		}

		public static readonly DependencyProperty DisableSnaplinesProperty =
			DependencyProperty.RegisterAttached("DisableSnaplines", typeof(bool), typeof(SnaplinePlacementBehavior), new PropertyMetadata(false));

		
		AdornerPanel adornerPanel;
		Canvas surface;
		List<Snapline> horizontalMap;
		List<Snapline> verticalMap;
		double? baseline;
		
		public const double Accuracy = 5;
		public const double Margin = 8;
		
		public override void BeginPlacement(PlacementOperation operation)
		{
			base.BeginPlacement(operation);
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

		public override Point PlacePoint(Point point)
		{
			if (surface == null)
				return base.PlacePoint(point);

			DesignPanel designPanel = ExtendedItem.Services.DesignPanel as DesignPanel;
			if (designPanel == null || !designPanel.UseSnaplinePlacement)
				return base.PlacePoint(point); ;

			surface.Children.Clear();
			if (Keyboard.IsKeyDown(Key.LeftCtrl))
				return base.PlacePoint(point); ;

			Rect bounds = new Rect(point.X, point.Y, 0, 0);
			
			var horizontalInput = new List<Snapline>();
			var verticalInput = new List<Snapline>();
			
			AddLines(bounds, 0, false, horizontalInput, verticalInput, null);
			if (baseline.HasValue)
			{
				var textOffset = bounds.Top + baseline.Value;
				horizontalInput.Add(new Snapline() { Group = 1, Offset = textOffset, Start = bounds.Left, End = bounds.Right });
			}
			
			List<Snapline> drawLines;
			double delta;

			var newPoint = base.PlacePoint(point);
			if (Snap(horizontalInput, horizontalMap, Accuracy, out drawLines, out delta))
			{
				foreach (var d in drawLines)
				{
					DrawLine(d.Start, d.Offset + d.DrawOffset, d.End, d.Offset + d.DrawOffset);
				}

				point.Y += delta;
			}
			else
				point.Y = newPoint.Y;

			if (Snap(verticalInput, verticalMap, Accuracy, out drawLines, out delta))
			{
				foreach (var d in drawLines)
				{
					DrawLine(d.Offset + d.DrawOffset, d.Start, d.Offset + d.DrawOffset, d.End);
				}

				point.X += delta;
			}
			else
				point.X = newPoint.X;


			return point;
		}

		public override void BeforeSetPosition(PlacementOperation operation)
		{
			base.BeforeSetPosition(operation);
			if (surface == null) return;
			
			DesignPanel designPanel = ExtendedItem.Services.DesignPanel as DesignPanel;
			if (designPanel == null || !designPanel.UseSnaplinePlacement)
				return;
			
			surface.Children.Clear();
			if (Keyboard.IsKeyDown(Key.LeftCtrl)) return;
			
			Rect bounds = Rect.Empty;
			foreach (var item in operation.PlacedItems) {
				bounds.Union(item.Bounds);
			}

			var horizontalInput = new List<Snapline>();
			var verticalInput = new List<Snapline>();
			var info = operation.PlacedItems[0];

			if (operation.Type == PlacementType.Resize)
			{
				AddLines(bounds, 0, false, horizontalInput, verticalInput, info.ResizeThumbAlignment);
			}
			else
			{
				AddLines(bounds, 0, false, horizontalInput, verticalInput, null);
				if (baseline.HasValue)
				{
					var textOffset = bounds.Top + baseline.Value;
					horizontalInput.Add(new Snapline() { Group = 1, Offset = textOffset, Start = bounds.Left, End = bounds.Right });
				}
			}

			// debug
			//foreach (var t in horizontalMap.Concat(horizontalInput)) {
			//    surface.Children.Add(new Line() { X1 = t.Start, X2 = t.End, Y1 = t.Offset, Y2 = t.Offset, Stroke = Brushes.Black });
			//}
			//foreach (var t in verticalMap.Concat(verticalInput)) {
			//    surface.Children.Add(new Line() { X1 = t.Offset, X2 = t.Offset, Y1 = t.Start , Y2 = t.End, Stroke = Brushes.Black });
			//}
			//return;

			List<Snapline> drawLines;
			double delta;

			if (Snap(horizontalInput, horizontalMap, Accuracy, out drawLines, out delta))
			{

				if (operation.Type == PlacementType.Resize)
				{
					if (info.ResizeThumbAlignment != null && info.ResizeThumbAlignment.Value.Vertical == VerticalAlignment.Top)
					{
						bounds.Y += delta;
						bounds.Height = Math.Max(0, bounds.Height - delta);
					}
					else
					{
						bounds.Height = Math.Max(0, bounds.Height + delta);
					}
					info.Bounds = bounds;
				}
				else
				{
					foreach (var item in operation.PlacedItems)
					{
						var r = item.Bounds;
						r.Y += delta;
						item.Bounds = r;
					}
				}

				foreach (var d in drawLines)
				{
					DrawLine(d.Start, d.Offset + d.DrawOffset, d.End, d.Offset + d.DrawOffset);
				}
			}

			if (Snap(verticalInput, verticalMap, Accuracy, out drawLines, out delta))
			{

				if (operation.Type == PlacementType.Resize)
				{
					if (info.ResizeThumbAlignment != null && info.ResizeThumbAlignment.Value.Horizontal == HorizontalAlignment.Left)
					{
						bounds.X += delta;
						bounds.Width = Math.Max(0, bounds.Width - delta);
					}
					else
					{
						bounds.Width = Math.Max(0, bounds.Width + delta);
					}
					info.Bounds = bounds;
				}
				else
				{
					foreach (var item in operation.PlacedItems)
					{
						var r = item.Bounds;
						r.X += delta;
						item.Bounds = r;
					}
				}

				foreach (var d in drawLines)
				{
					DrawLine(d.Offset + d.DrawOffset, d.Start, d.Offset + d.DrawOffset, d.End);
				}
			}
		}

		void CreateSurface(PlacementOperation operation)
		{
			if (ExtendedItem.Services.GetService<IDesignPanel>() != null) {
				
				surface = new Canvas();
				adornerPanel = new AdornerPanel();
				adornerPanel.SetAdornedElement(ExtendedItem.View, ExtendedItem);
				AdornerPanel.SetPlacement(surface, AdornerPlacement.FillContent);
				adornerPanel.Children.Add(surface);
				ExtendedItem.Services.DesignPanel.Adorners.Add(adornerPanel);
				
				BuildMaps(operation);
				
				if (operation.Type != PlacementType.Resize && operation.PlacedItems.Count == 1) {
					baseline = GetBaseline(operation.PlacedItems[0].Item.View);
				}
			}
		}
		
		private IEnumerable<DesignItem> AllDesignItems(DesignItem designItem = null)
		{
			if (designItem == null && this.ExtendedItem.Services.DesignPanel is DesignPanel)
			{
				designItem = this.ExtendedItem.Services.DesignPanel.Context.RootItem;
				if (designItem != null) {
					yield return designItem;
					if (designItem.ContentProperty.Value != null) {
						yield return designItem.ContentProperty.Value;
						designItem = ExtendedItem; //set designitem back to current control after yield
					}
				}
			}
			
			if (designItem != null && designItem.ContentProperty != null && designItem.ContentProperty.IsCollection)
				foreach (var collectionElement in designItem.ContentProperty.CollectionElements)
			{
				if (collectionElement != null)
					yield return collectionElement;
				
				foreach (var el in AllDesignItems(collectionElement))
				{
					if (el != null)
						yield return el;
				}
			}
		}
		
		void BuildMaps(PlacementOperation operation)
		{
			horizontalMap = new List<Snapline>();
			verticalMap = new List<Snapline>();
			
			var containerRect = new Rect(0, 0, ModelTools.GetWidth(ExtendedItem.View), ModelTools.GetHeight(ExtendedItem.View));
			AddLines(containerRect, -Margin, false);
			
			AddLines(containerRect, 0, false);

			if (!CanPlace(operation.PlacedItems.Select(x => x.Item), operation.Type, PlacementAlignment.Center))
				return;

			foreach (var item in AllDesignItems() /* ExtendedItem.ContentProperty.CollectionElements */
			         .Except(operation.PlacedItems.Select(f => f.Item))
			         .Where(x=> x.View != null && !GetDisableSnaplines(x.View))) {
				if (item != null) {
					var bounds = GetPosition(operation, item);
					
					AddLines(bounds, 0, false);
					AddLines(bounds, Margin, true);
					AddBaseline(item, bounds, horizontalMap);
				}
			}
		}
		
		void AddLines(Rect r, double inflate, bool requireOverlap)
		{
			AddLines(r, inflate, requireOverlap, horizontalMap, verticalMap, null);
		}
		
		void AddLines(Rect r, double inflate, bool requireOverlap, List<Snapline> h, List<Snapline> v, PlacementAlignment? filter)
		{
			if (r != Rect.Empty)
			{
				Rect r2 = r;
				r2.Inflate(inflate, inflate);
				
				if (filter == null || filter.Value.Vertical == VerticalAlignment.Top)
					h.Add(new Snapline() { RequireOverlap = requireOverlap, Offset = r2.Top - 1, Start = r.Left, End = r.Right });
				if (filter == null || filter.Value.Vertical == VerticalAlignment.Bottom)
					h.Add(new Snapline() { RequireOverlap = requireOverlap, Offset = r2.Bottom - 1, Start = r.Left, End = r.Right });
				if (filter == null || filter.Value.Horizontal == HorizontalAlignment.Left)
					v.Add(new Snapline() { RequireOverlap = requireOverlap, Offset = r2.Left - 1, Start = r.Top, End = r.Bottom });
				if (filter == null || filter.Value.Horizontal == HorizontalAlignment.Right)
					v.Add(new Snapline() { RequireOverlap = requireOverlap, Offset = r2.Right - 1, Start = r.Top, End = r.Bottom });
				
				if (filter == null)
				{
					h.Add(new Snapline() { RequireOverlap = requireOverlap, Offset = r2.Top + Math.Abs((r2.Top - r2.Bottom) / 2) - 1, DrawOffset = 1, Start = r.Left, End = r.Right });
					v.Add(new Snapline() { RequireOverlap = requireOverlap, Offset = r2.Left + Math.Abs((r2.Left - r2.Right) / 2) - 1, DrawOffset = 1, Start = r.Top, End = r.Bottom });
				}
			}
		}
		
		void AddBaseline(DesignItem item, Rect bounds, List<Snapline> list)
		{
			var baseline = GetBaseline(item.View);
			if (baseline.HasValue) {
				var textOffset = item.View.TranslatePoint(new Point(0, baseline.Value), ExtendedItem.View).Y;
				list.Add(new Snapline() { Group = 1, Offset = textOffset, Start = bounds.Left, End = bounds.Right });
			}
		}
		
		void DeleteSurface()
		{
			if (surface != null) {
				ExtendedItem.Services.DesignPanel.Adorners.Remove(adornerPanel);
				adornerPanel = null;
				surface = null;
				horizontalMap = null;
				verticalMap = null;
			}
		}
		
		void DrawLine(double x1, double y1, double x2, double y2)
		{
			if (double.IsInfinity(x1) || double.IsNaN(x1) || double.IsInfinity(y1) || double.IsNaN(y1) ||
			    double.IsInfinity(x2) || double.IsNaN(x2) || double.IsInfinity(y2) || double.IsNaN(y2))
				return;

			var line1 = new Line() {
				X1 = x1,
				Y1 = y1,
				X2 = x2,
				Y2 = y2,
				StrokeThickness = 1,
				Stroke = Brushes.White
			};
			surface.Children.Add(line1);
			
			var line2 = new Line() {
				X1 = x1,
				Y1 = y1,
				X2 = x2,
				Y2 = y2,
				StrokeThickness = 1,
				Stroke = Brushes.Orange,
				StrokeDashArray = new DoubleCollection(new double[] { 5, 2 }),
				StrokeDashOffset = x1 + y1  // fix dashes
			};
			surface.Children.Add(line2);
		}
		
		//TODO: GlyphRun must be used
		static double? GetBaseline(UIElement element) {
			var textBox = element as TextBox;
			if (textBox != null) {
				var r = textBox.GetRectFromCharacterIndex(0).Bottom;
				return textBox.TranslatePoint(new Point(0, r), element).Y;
			}
			var textBlock = element as TextBlock;
			if (textBlock != null)
				return textBlock.TranslatePoint(new Point(0, textBlock.ActualHeight), element).Y;
			
			return null;
		}
		
		static bool Snap(List<Snapline> input, List<Snapline> map, double accuracy,
		                 out List<Snapline> drawLines, out double delta)
		{
			delta = double.MaxValue;
			drawLines = null;
			
			foreach (var inputLine in input) {
				foreach (var mapLine in map) {
					if (Math.Abs(mapLine.Offset - inputLine.Offset) <= accuracy) {
						if (!inputLine.RequireOverlap && !mapLine.RequireOverlap ||
						    Math.Max(inputLine.Start, mapLine.Start) < Math.Min(inputLine.End, mapLine.End))
						{
							if (mapLine.Group == inputLine.Group)
								delta = mapLine.Offset - inputLine.Offset;
						}
					}
				}
			}
			
			if (delta == double.MaxValue) return false;
			var offsetDict = new Dictionary<double, Snapline>();
			
			foreach (var inputLine in input) {
				inputLine.Offset += delta;
				foreach (var mapLine in map) {
					if (inputLine.Offset == mapLine.Offset) {
						var offset = mapLine.Offset;
						Snapline drawLine;
						if (!offsetDict.TryGetValue(offset, out drawLine)) {
							drawLine = new Snapline();
							drawLine.Start = double.MaxValue;
							drawLine.End = double.MinValue;
							drawLine.DrawOffset = mapLine.DrawOffset;
							offsetDict[offset] = drawLine;
						}
						drawLine.Offset = offset;
						drawLine.Start = Math.Min(drawLine.Start, Math.Min(inputLine.Start, mapLine.Start));
						drawLine.End = Math.Max(drawLine.End, Math.Max(inputLine.End, mapLine.End));
					}
				}
			}
			
			drawLines = offsetDict.Values.ToList();
			return true;
		}
		
		[DebuggerDisplay("Snapline: {Offset}")]
		class Snapline
		{
			public double Offset;
			public double Start;
			public double End;
			public bool RequireOverlap;
			public int Group;
			
			public double DrawOffset = 0;
		}
	}
}
