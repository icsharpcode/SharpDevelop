// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace ICSharpCode.Profiler.Controls
{
	public struct TimeLineInfo {
		public double value;
		public bool displayMarker;
	}
	
	public class TimeLineControl : FrameworkElement
	{
		ObservableCollection<TimeLineInfo> valuesList;
		int selectedStartIndex, selectedEndIndex;
		double pieceWidth;

		public event EventHandler<RangeEventArgs> RangeChanged;

		protected virtual void OnRangeChanged(RangeEventArgs e)
		{
			if (RangeChanged != null)
			{
				RangeChanged(this, e);
			}
		}

		public int SelectedEndIndex
		{
			get { return selectedEndIndex; }
			set {
				selectedEndIndex = value;
				this.InvalidateMeasure();
				this.InvalidateVisual();
				OnRangeChanged(new RangeEventArgs(selectedStartIndex, selectedEndIndex));
			}
		}

		public int SelectedStartIndex
		{
			get { return selectedStartIndex; }
			set {
				selectedStartIndex = value;
				this.InvalidateMeasure();
				this.InvalidateVisual();
				OnRangeChanged(new RangeEventArgs(selectedStartIndex, selectedEndIndex));
			}
		}

		public ObservableCollection<TimeLineInfo> ValuesList
		{
			get { return valuesList; }
		}

		public TimeLineControl()
		{
			this.valuesList = new ObservableCollection<TimeLineInfo>();
			this.valuesList.CollectionChanged += delegate { this.InvalidateMeasure(); this.InvalidateVisual(); };
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			return new Size(1, base.MeasureOverride(availableSize).Height + 10);
		}
		
		const int offset = 15;
		
		protected override void OnRender(DrawingContext drawingContext)
		{
			base.OnRender(drawingContext);

			if (this.valuesList.Count == 0)
				return;

			this.pieceWidth = (this.RenderSize.Width - offset) / (double)this.valuesList.Count;

			double oldX = 0, oldY = this.RenderSize.Height - offset;

			StreamGeometry geometry = new StreamGeometry();

			using (StreamGeometryContext ctx = geometry.Open())
			{
				ctx.BeginFigure(new Point(offset, this.RenderSize.Height - offset), true, true);

				List<Point> points = new List<Point>();

				for (int i = 0; i < this.valuesList.Count; i++)
				{
					double x = this.pieceWidth / 2.0 + this.pieceWidth * i + offset;
					double y = this.RenderSize.Height - this.valuesList[i].value * (this.RenderSize.Height - offset) - offset;

					points.Add(new Point(x, y));

					oldX = x;
					oldY = y;
				}

				points.Add(new Point(oldX, this.RenderSize.Height - offset));

				ctx.PolyLineTo(points, true, true);
			}

			geometry.Freeze();

			Brush b = new LinearGradientBrush(Colors.Red, Colors.Orange, 90);

			drawingContext.DrawRectangle(Brushes.White, new Pen(Brushes.White, 1), new Rect(new Point(0, 0), this.RenderSize));
			drawingContext.DrawLine(new Pen(Brushes.Black, 1), new Point(offset / 2, this.RenderSize.Height - offset / 2), new Point(this.RenderSize.Width - offset, this.RenderSize.Height - offset / 2));
			drawingContext.DrawLine(new Pen(Brushes.Black, 1), new Point(offset / 2, 0), new Point(offset / 2, this.RenderSize.Height - offset / 2));
			
			var p = new Pen(Brushes.DarkRed, 2);
			
			for (int i = 0; i < this.valuesList.Count; i++) {
				drawingContext.DrawLine(new Pen(Brushes.Black, 1),
				                        new Point(offset + pieceWidth / 2 + pieceWidth * i, this.RenderSize.Height - offset / 4),
				                        new Point(offset + pieceWidth / 2 + pieceWidth * i, this.RenderSize.Height - (offset / 4 * 3 + 3)));
				
				if (this.valuesList[i].displayMarker) {
					drawingContext.DrawLine(p, new Point(offset + pieceWidth * i, 0),
					                        new Point(offset + pieceWidth * i, this.RenderSize.Height - offset));
				}

			}
			
			drawingContext.DrawGeometry(b, new Pen(b, 3), geometry);
			
			for (int i = 0; i < this.valuesList.Count; i++) {
				if (this.valuesList[i].displayMarker)
					drawingContext.DrawLine(p, new Point(offset + pieceWidth * i, 0),
					                        new Point(offset + pieceWidth * i, this.RenderSize.Height - offset));
			}
			
			drawingContext.DrawRectangle(
				new SolidColorBrush(Color.FromArgb(64, Colors.CornflowerBlue.R,
				                                   Colors.CornflowerBlue.G, Colors.CornflowerBlue.B)),
				new Pen(Brushes.Blue, 1),
				new Rect(
					new Point(this.selectedStartIndex * this.pieceWidth + offset, 0),
					new Point(this.selectedEndIndex * this.pieceWidth + offset, this.RenderSize.Height - offset)
				)
			);
		}

		protected override void OnMouseUp(System.Windows.Input.MouseButtonEventArgs e)
		{
			int index = TransformToIndex(e.GetPosition(this));
			index = (index < 0) ? 0 : index;
			index = (index > this.valuesList.Count) ? this.valuesList.Count : index;
			
			this.selectedEndIndex = index;
			
			this.InvalidateMeasure();
			this.InvalidateVisual();
			this.ReleaseMouseCapture();
		}

		protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e)
		{
			this.CaptureMouse();
			
			int index = TransformToIndex(e.GetPosition(this));
			index = (index < 0) ? 0 : index;
			index = (index > this.valuesList.Count) ? this.valuesList.Count : index;
			
			this.selectedStartIndex = this.selectedEndIndex = index;
			this.InvalidateMeasure();
			this.InvalidateVisual();
		}

		protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
		{
			int index = TransformToIndex(e.GetPosition(this));
			
			index = (index < 0) ? 0 : index;
			index = (index > this.valuesList.Count) ? this.valuesList.Count : index;
			
			if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
			{
				this.selectedEndIndex = index;
				this.InvalidateMeasure();
				this.InvalidateVisual();
			}
		}

		protected override void OnLostMouseCapture(System.Windows.Input.MouseEventArgs e)
		{
			base.OnLostMouseCapture(e);
			if (this.selectedEndIndex == this.selectedStartIndex)
				this.selectedEndIndex++;
			OnRangeChanged(new RangeEventArgs(this.selectedStartIndex, this.selectedEndIndex));
			Debug.Print("lost capture");
		}

		private int TransformToIndex(Point point)
		{
			return (int)Math.Round(point.X / this.pieceWidth);
		}
	}
}
