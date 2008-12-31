using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using SharpDevelop.XamlDesigner.Controls;
using System.Diagnostics;
using SharpDevelop.XamlDesigner.Dom;

namespace SharpDevelop.XamlDesigner.Placement
{
	class SnapEngine
	{
		public SnapEngine(DesignItem containerItem)
		{
			this.containerItem = containerItem;
		}

		DesignItem containerItem;
		List<Snapline> map = new List<Snapline>();

		public const double Accuracy = 7;
		public const double GlobalMargin = 11;
		public const double ControlMargin = 7;

		public static double? GetBaseline(DependencyObject obj)
		{
			return (double?)obj.GetValue(BaselineProperty);
		}

		public static void SetBaseline(DependencyObject obj, double? value)
		{
			obj.SetValue(BaselineProperty, value);
		}

		public static void UpdateBaseline(DesignItem item)
		{
			SetBaseline(item.View, CalculateBaseline(item.View));
		}

		public static readonly DependencyProperty BaselineProperty =
			DependencyProperty.RegisterAttached("Baseline", typeof(double?), typeof(SnapEngine));

		public Canvas FeedbackLayer
		{
			get { return containerItem.Context.DesignView.FeedbackLayer; }
		}

		public void BuildMap(MoveOperation op)
		{
			var containerRect = new Rect(containerItem.View.RenderSize);
			AddLinesToMap(containerRect, 0, SnaplineKind.Bounds, false);
			AddLinesToMap(containerRect, -GlobalMargin, SnaplineKind.Bounds, false);

			var list = new List<DesignItem>();

			foreach (var item in containerItem.Children().Except(op.Items)) {
				if (item.View.RenderTransform == Transform.Identity &&
					item.View.LayoutTransform == Transform.Identity) {
					list.Add(item);
				}
			}

			foreach (var item in list) {
				var bounds = item.GetBounds();
				AddLinesToMap(bounds, 0, SnaplineKind.Bounds, false);
				AddLinesToMap(bounds, ControlMargin, SnaplineKind.Margin, true);
				TryAddBaseline(map, bounds, CalculateBaseline(item.View));
			}
		}

		public void SnapResize(ResizeOperation op)
		{
		}

		public void SnapMove(MoveOperation op)
		{
			HideSnaplines();

			var input = BuildInput(op);

			Vector delta;
			if (TrySnap(input, out delta)) {
				foreach (var info in op.PlacementInfos) {
					var snappedBounds = info.NewBoundsInContainer;
					snappedBounds.Offset(delta);
					info.NewBoundsInContainer = snappedBounds;
				}

				var snappedInput = BuildInput(op);
				ShowSnaplines(snappedInput, Orientation.Horizontal);
				ShowSnaplines(snappedInput, Orientation.Vertical);
			}
		}

		List<Snapline> BuildInput(MoveOperation op)
		{
			var result = new List<Snapline>();
			var info = op.PlacementInfos[0];

			AddLinesToInput(result, op.PlacementInfos[0].NewBoundsInContainer);
			TryAddBaseline(result, info.NewBoundsInContainer, CalculateBaseline(info.Item.View));

			return result;
		}

		public void HideSnaplines()
		{
			FeedbackLayer.Children.Clear();
		}

		void ShowSnaplines(IEnumerable<Snapline> snappedInput, Orientation orient)
		{
			var offsetDict = new Dictionary<double, Snapline>();

			foreach (var inputLine in snappedInput) {
				if (inputLine.Orientation == orient) {
					foreach (var mapLine in map) {
						if (mapLine.Orientation == orient &&
							mapLine.Offset == inputLine.Offset) {

							var offset = mapLine.Offset;
							Snapline drawLine;

							if (!offsetDict.TryGetValue(offset, out drawLine)) {
								drawLine = new Snapline();
								drawLine.Orientation = mapLine.Orientation;
								drawLine.Offset = offset;
								drawLine.Start = double.MaxValue;
								drawLine.End = double.MinValue;
								offsetDict[offset] = drawLine;
							}
							drawLine.Start = Math.Min(drawLine.Start, Math.Min(inputLine.Start, mapLine.Start));
							drawLine.End = Math.Max(drawLine.End, Math.Max(inputLine.End, mapLine.End));
						}
					}
				}
			}

			var tr = containerItem.View.TransformToVisual(FeedbackLayer);
			var viewStyle = DesignResources.SnaplineStyle;
			var drawLines = offsetDict.Values.ToList();

			foreach (var line in drawLines) {
				var view = new DashedLine();
				if (line.Orientation == Orientation.Horizontal) {
					view.Point1 = new Point(line.Start, line.Offset);
					view.Point2 = new Point(line.End, line.Offset);
				}
				else {
					view.Point1 = new Point(line.Offset, line.Start);
					view.Point2 = new Point(line.Offset, line.End);
				}
				view.Point1 = tr.Transform(view.Point1);
				view.Point2 = tr.Transform(view.Point2);
				view.Style = viewStyle;

				FeedbackLayer.Children.Add(view);
			}
		}

		void AddLinesToMap(Rect r, double inflate, SnaplineKind kind, bool opposite)
		{
			var r2 = r;
			r2.Inflate(0, inflate);
			AddLine(map, r2, ResizeDirection.Up, kind, opposite ? SnaplineSide.Bottom : SnaplineSide.Top);
			AddLine(map, r2, ResizeDirection.Down, kind, opposite ? SnaplineSide.Top : SnaplineSide.Bottom);

			r2 = r;
			r2.Inflate(inflate, 0);
			AddLine(map, r2, ResizeDirection.Left, kind, opposite ? SnaplineSide.Right : SnaplineSide.Left);
			AddLine(map, r2, ResizeDirection.Right, kind, opposite ? SnaplineSide.Left : SnaplineSide.Right);
		}

		void AddLinesToInput(List<Snapline> list, Rect r)
		{
			AddLine(list, r, ResizeDirection.Up, SnaplineKind.Bounds, null);
			AddLine(list, r, ResizeDirection.Down, SnaplineKind.Bounds, null);
			AddLine(list, r, ResizeDirection.Left, SnaplineKind.Bounds, null);
			AddLine(list, r, ResizeDirection.Right, SnaplineKind.Bounds, null);
		}

		static void AddLine(List<Snapline> list, Rect r, ResizeDirection dir,
			SnaplineKind kind, SnaplineSide? side)
		{
			switch (dir) {
				case ResizeDirection.Up:
					list.Add(new Snapline() {
						Kind = kind,
						Side = side ?? SnaplineSide.Top,
						Orientation = Orientation.Horizontal,
						Start = r.Left,
						End = r.Right,
						Offset = r.Top
					});
					break;
				case ResizeDirection.Down:
					list.Add(new Snapline() {
						Kind = kind,
						Side = side ?? SnaplineSide.Bottom,
						Orientation = Orientation.Horizontal,
						Start = r.Left,
						End = r.Right,
						Offset = r.Bottom
					});
					break;
				case ResizeDirection.Left:
					list.Add(new Snapline() {
						Kind = kind,
						Side = side ?? SnaplineSide.Left,
						Orientation = Orientation.Vertical,
						Start = r.Top,
						End = r.Bottom,
						Offset = r.Left
					});
					break;
				case ResizeDirection.Right:
					list.Add(new Snapline() {
						Kind = kind,
						Side = side ?? SnaplineSide.Right,
						Orientation = Orientation.Vertical,
						Start = r.Top,
						End = r.Bottom,
						Offset = r.Right
					});
					break;
			}
		}

		void TryAddBaseline(List<Snapline> list, Rect bounds, double? baseline)
		{
			if (baseline.HasValue) {
				list.Add(new Snapline() {
					Kind = SnaplineKind.Baseline,
					Side = SnaplineSide.Any,
					Orientation = Orientation.Horizontal,
					Start = bounds.Left,
					End = bounds.Right,
					Offset = bounds.Top + baseline.Value
				});
			}
		}

		bool TrySnap(List<Snapline> input, out Vector delta)
		{
			double deltaX = 0;
			double deltaY = 0;
			var result = false;
			delta = new Vector();

			if (TrySnap(input, Orientation.Horizontal, out deltaY)) {
				delta.Y = deltaY;
				result = true;
			}
			if (TrySnap(input, Orientation.Vertical, out deltaX)) {
				delta.X = deltaX;
				result = true;
			}

			return result;
		}

		bool TrySnap(IEnumerable<Snapline> input, Orientation orient, out double delta)
		{
			delta = double.MaxValue;

			foreach (var inputLine in input) {
				if (inputLine.Orientation == orient) {
					foreach (var mapLine in map) {
						if (mapLine.Orientation == orient && 
							(mapLine.Kind == inputLine.Kind || 
							mapLine.Kind == SnaplineKind.Margin && inputLine.Kind == SnaplineKind.Bounds) &&
							(mapLine.Side == SnaplineSide.Any || mapLine.Side == inputLine.Side)) {

							if (Math.Abs(mapLine.Offset - inputLine.Offset) <= Accuracy &&
								
								(mapLine.Kind != SnaplineKind.Margin ||
								Math.Max(mapLine.Start, inputLine.Start) < Math.Min(mapLine.End, inputLine.End))) {

								var newDelta = mapLine.Offset - inputLine.Offset;
								if (Math.Abs(newDelta) < Math.Abs(delta)) {
									delta = newDelta;
								}
							}
						}
					}
				}
			}

			return delta != double.MaxValue;
		}

		//TODO: GlyphRun must be used
		static double? CalculateBaseline(FrameworkElement element)
		{
			var textBox = element.FindChild<TextBox>();
			if (textBox != null) {
				var r = textBox.GetRectFromCharacterIndex(0).Bottom;
				return textBox.TranslatePoint(new Point(0, r), element).Y;
			}
			var textBlock = element.FindChild<TextBlock>();
			if (textBlock != null)
				return textBlock.TranslatePoint(new Point(0, textBlock.BaselineOffset), element).Y;
			
			return null;
		}

		class Snapline
		{
			public double Start;
			public double End;
			public double Offset;
			public SnaplineKind Kind;
			public SnaplineSide Side;
			public Orientation Orientation;
		}

		enum SnaplineKind
		{
			Bounds,
			Margin,
			Baseline
		}

		enum SnaplineSide
		{
			Left,
			Right,
			Top,
			Bottom,
			Any
		}
	}
}
