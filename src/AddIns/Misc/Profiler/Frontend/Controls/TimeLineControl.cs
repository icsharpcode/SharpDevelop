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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace ICSharpCode.Profiler.Controls
{
	public class TimeLineSegment {
		public float Value { get; set; }
		public bool DisplayMarker { get; set; }
		public long TimeOffset { get; set; }
	}
	
	public class TimeLineControl : FrameworkElement
	{
		ObservableCollection<TimeLineSegment> valuesList;
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
		
		public int SelectedPerformanceCounter { get; set; }
		
		public float MaxValue { get; set; }

		public string Unit { get; set; }
		
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

		public ObservableCollection<TimeLineSegment> ValuesList
		{
			get { return valuesList; }
		}

		public TimeLineControl()
		{
			this.valuesList = new ObservableCollection<TimeLineSegment>();
			this.valuesList.CollectionChanged += delegate { this.InvalidateMeasure(); this.InvalidateVisual(); };
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			this.pieceWidth = 25;
			Size calculatedSize = base.MeasureOverride(availableSize);
			return new Size(Math.Max(this.pieceWidth * this.valuesList.Count + 1, calculatedSize.Width), calculatedSize.Height + 10);
		}
		
		protected override void OnToolTipOpening(ToolTipEventArgs e)
		{
			Console.WriteLine("tooltip");
			base.OnToolTipOpening(e);
		}
		
		const int offset = 0;
		const int offsetFromTop = 40;
		
		protected override void OnRender(DrawingContext drawingContext)
		{
			base.OnRender(drawingContext);

			if (this.valuesList.Count == 0)
				return;
			
			double oldX = offsetFromTop, oldY = this.RenderSize.Height;

			StreamGeometry geometry = new StreamGeometry();

			using (StreamGeometryContext ctx = geometry.Open())
			{
				ctx.BeginFigure(new Point(0, this.RenderSize.Height), true, true);

				List<Point> points = new List<Point>();
				
				double maxHeight = this.RenderSize.Height - offsetFromTop;

				for (int i = 0; i < this.valuesList.Count; i++)
				{
					double x = this.pieceWidth / 2.0 + this.pieceWidth * i;
					double y = offsetFromTop + (maxHeight - maxHeight * (this.valuesList[i].Value / this.MaxValue));

					points.Add(new Point(x, y));

					oldX = x;
					oldY = y;
				}

				points.Add(new Point(oldX, offsetFromTop + this.RenderSize.Height));

				ctx.PolyLineTo(points, true, true);
			}

			geometry.Freeze();

			Brush b = new LinearGradientBrush(Colors.Red, Colors.Orange, 90);

			drawingContext.DrawRectangle(Brushes.White, new Pen(Brushes.White, 1), new Rect(new Point(0, offsetFromTop), this.RenderSize));
			
			var p = new Pen(Brushes.DarkRed, 2);
			
			drawingContext.DrawGeometry(b, new Pen(b, 3), geometry);
			
			for (int i = 0; i < this.valuesList.Count; i++) {
				if (this.valuesList[i].DisplayMarker)
					drawingContext.DrawLine(p, new Point(pieceWidth * i, offsetFromTop),
					                        new Point(pieceWidth * i, this.RenderSize.Height));
			}
			
			drawingContext.DrawRectangle(
				new SolidColorBrush(Color.FromArgb(64, Colors.Blue.R, Colors.Blue.G, Colors.Blue.B)),
				new Pen(Brushes.Blue, 1),
				new Rect(
					new Point(Math.Min(this.selectedStartIndex, this.selectedEndIndex) * pieceWidth + offset, 0),
					new Point(Math.Max(this.selectedStartIndex, this.selectedEndIndex) * pieceWidth + offset + pieceWidth, this.RenderSize.Height - offset)
				)
			);
		}

		protected override void OnMouseUp(System.Windows.Input.MouseButtonEventArgs e)
		{
			bool valid;
			int index = TransformToIndex(e.GetPosition(this), out valid);
			
			if (index < this.selectedStartIndex) {
				this.selectedEndIndex = this.selectedStartIndex;
				this.selectedStartIndex = index;
			} else
				this.selectedEndIndex = index;
			
			Console.WriteLine("start: {0} end: {1} count: {2}", SelectedStartIndex, SelectedEndIndex, valuesList.Count);
			
			this.InvalidateMeasure();
			this.InvalidateVisual();
			this.ReleaseMouseCapture();
		}

		protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e)
		{
			this.CaptureMouse();
			bool valid;
			int index = TransformToIndex(e.GetPosition(this), out valid);
			
			this.selectedStartIndex = this.selectedEndIndex = index;
			
			this.InvalidateMeasure();
			this.InvalidateVisual();
		}

		ToolTip tooltip;
		
		void HandleMovement(MouseEventArgs e)
		{
			bool valid;
			int index = TransformToIndex(e.GetPosition(this), out valid);
			
			if (e.LeftButton == MouseButtonState.Pressed) {
				this.selectedEndIndex = index;
				this.InvalidateMeasure();
				this.InvalidateVisual();
			} else if (tooltip == null && valid) {
				tooltip = new ToolTip();
				tooltip.Content = "Value: " + this.valuesList[index].Value.ToString("0.00") + " " + this.Unit;
				tooltip	.IsOpen = true;
			}
		}

		protected override void OnLostMouseCapture(MouseEventArgs e)
		{
			base.OnLostMouseCapture(e);
			OnRangeChanged(new RangeEventArgs(this.selectedStartIndex, this.selectedEndIndex));
		}

		int TransformToIndex(Point point, out bool valid)
		{
			int value = (int)Math.Floor(point.X / this.pieceWidth);
			valid = (0 <= value && value <= this.valuesList.Count - 1);
			return Math.Min(Math.Max(0, value), this.valuesList.Count - 1);
		}
		
		#region MouseHover
		/// <summary>
		/// The MouseHover event.
		/// </summary>
		public static readonly RoutedEvent MouseHoverEvent =
			EventManager.RegisterRoutedEvent("MouseHover", RoutingStrategy.Bubble,
			                                 typeof(MouseEventHandler), typeof(TimeLineControl));
		
		/// <summary>
		/// The MouseHoverStopped event.
		/// </summary>
		public static readonly RoutedEvent MouseHoverStoppedEvent =
			EventManager.RegisterRoutedEvent("MouseHoverStopped", RoutingStrategy.Bubble,
			                                 typeof(MouseEventHandler), typeof(TimeLineControl));
		
		/// <summary>
		/// Occurs when the mouse has hovered over a fixed location for some time.
		/// </summary>
		public event MouseEventHandler MouseHover {
			add { AddHandler(MouseHoverEvent, value); }
			remove { RemoveHandler(MouseHoverEvent, value); }
		}
		
		/// <summary>
		/// Occurs when the mouse had previously hovered but now started moving again.
		/// </summary>
		public event MouseEventHandler MouseHoverStopped {
			add { AddHandler(MouseHoverStoppedEvent, value); }
			remove { RemoveHandler(MouseHoverStoppedEvent, value); }
		}
		
		DispatcherTimer mouseHoverTimer;
		Point mouseHoverStartPoint;
		MouseEventArgs mouseHoverLastEventArgs;
		
		/// <inheritdoc/>
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			Point newPosition = e.GetPosition(this);
			Vector mouseMovement = mouseHoverStartPoint - newPosition;
			if (Math.Abs(mouseMovement.X) > SystemParameters.MouseHoverWidth
			    || Math.Abs(mouseMovement.Y) > SystemParameters.MouseHoverHeight)
			{
				StopHovering();
				mouseHoverStartPoint = newPosition;
				mouseHoverLastEventArgs = e;
				mouseHoverTimer = new DispatcherTimer(SystemParameters.MouseHoverTime, DispatcherPriority.Background,
				                                      OnMouseHoverTimerElapsed, this.Dispatcher);
				mouseHoverTimer.Start();
			}
			
			HandleMovement(e);
			// do not set e.Handled - allow others to also handle MouseMove
		}
		
		/// <inheritdoc/>
		protected override void OnMouseLeave(MouseEventArgs e)
		{
			base.OnMouseLeave(e);
			StopHovering();
			// do not set e.Handled - allow others to also handle MouseLeave
		}
		
		void StopHovering()
		{
			if (mouseHoverTimer != null) {
				mouseHoverTimer.Stop();
				mouseHoverTimer = null;
			}
			
			if (this.tooltip != null) {
				this.tooltip.IsOpen = false;
				this.tooltip = null;
			}
		}
		
		void OnMouseHoverTimerElapsed(object sender, EventArgs e)
		{
			mouseHoverTimer.Stop();
			mouseHoverTimer = null;
		}
		#endregion
	}
}
