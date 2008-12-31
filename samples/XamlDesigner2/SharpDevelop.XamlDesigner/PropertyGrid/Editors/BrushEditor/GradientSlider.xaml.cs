using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.ComponentModel;

namespace SharpDevelop.XamlDesigner.PropertyGrid.Editors.BrushEditor
{
	public partial class GradientSlider : UserControl
	{
		public GradientSlider()
		{
			InitializeComponent();

			BindingOperations.SetBinding(this, SelectedStopProperty, new Binding("SelectedItem") {
				Source = itemsControl,
				Mode = BindingMode.TwoWay
			});

			strip.DragStarted += new DragStartedEventHandler(strip_DragStarted);
			strip.DragDelta += new DragDeltaEventHandler(strip_DragDelta);
		}

		static GradientSlider()
		{
			EventManager.RegisterClassHandler(typeof(GradientSlider),
				Thumb.DragDeltaEvent, new DragDeltaEventHandler(ClassDragDelta));
		}

		GradientStop newStop;
		double startOffset;

		public static readonly DependencyProperty GradientBrushProperty =
			DependencyProperty.Register("GradientBrush", typeof(GradientBrush), typeof(GradientSlider),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public GradientBrush GradientBrush
		{
			get { return (GradientBrush)GetValue(GradientBrushProperty); }
			set { SetValue(GradientBrushProperty, value); }
		}

		public static readonly DependencyProperty SelectedStopProperty =
			DependencyProperty.Register("SelectedStop", typeof(GradientStop), typeof(GradientSlider));

		public GradientStop SelectedStop
		{
			get { return (GradientStop)GetValue(SelectedStopProperty); }
			set { SetValue(SelectedStopProperty, value); }
		}

		public static readonly DependencyProperty GradientStopsProperty =
			DependencyProperty.Register("GradientStops", typeof(BindingList<GradientStop>), typeof(GradientSlider));

		public BindingList<GradientStop> GradientStops
		{
			get { return (BindingList<GradientStop>)GetValue(GradientStopsProperty); }
			set { SetValue(GradientStopsProperty, value); }
		}

		public static Color GetColorAtOffset(IList<GradientStop> stops, double offset)
		{
			var array = stops.OrderBy(s => s.Offset).ToArray();
			if (array.Length < 2) {
				return Colors.White;
			}

			GradientStop s1 = array[0];
			GradientStop s2 = array.Last();

			foreach (var item in array) {
				if (item.Offset < offset && item.Offset > s1.Offset) s1 = item;
				if (item.Offset > offset && item.Offset < s2.Offset) s2 = item;
			}

			return Color.FromArgb(
				Interpolate(offset, s1.Color.A, s1.Offset, s2.Color.A, s2.Offset),
				Interpolate(offset, s1.Color.R, s1.Offset, s2.Color.R, s2.Offset),
				Interpolate(offset, s1.Color.G, s1.Offset, s2.Color.G, s2.Offset),
				Interpolate(offset, s1.Color.B, s1.Offset, s2.Color.B, s2.Offset)
			);
		}

		static byte Interpolate(double offset, byte v1, double offset1, byte v2, double offset2)
		{
			if (v1 == v2) return v1;
			return (byte)(v1 + (offset - offset1) * (v2 - v1) / (offset2 - offset1));
		}

		static void ClassDragDelta(object sender, DragDeltaEventArgs e)
		{
			(sender as GradientSlider).thumb_DragDelta(sender, e);
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == GradientBrushProperty) {
				if (GradientBrush != null) {
					GradientStops = new BindingList<GradientStop>(GradientBrush.GradientStops);
					SelectedStop = GradientStops.FirstOrDefault();
				}
				else {
					GradientStops = null;
				}
			}
		}

		void strip_DragStarted(object sender, DragStartedEventArgs e)
		{
			startOffset = e.HorizontalOffset / strip.ActualWidth;
			newStop = new GradientStop(GetColorAtOffset(GradientStops, startOffset), startOffset);
			GradientStops.Add(newStop);
			SelectedStop = newStop;
			e.Handled = true;
		}

		void strip_DragDelta(object sender, DragDeltaEventArgs e)
		{
			MoveStop(newStop, startOffset, e);
			e.Handled = true;
		}

		void thumb_DragDelta(object sender, DragDeltaEventArgs e)
		{
			var stop = (e.OriginalSource as GradientThumb).GradientStop;
			MoveStop(stop, stop.Offset, e);
		}

		void MoveStop(GradientStop stop, double oldOffset, DragDeltaEventArgs e)
		{
			if (e.VerticalChange > 50 && GradientStops.Count > 2) {
				GradientStops.Remove(stop);
				SelectedStop = GradientStops.FirstOrDefault();
				return;
			}
			stop.Offset = (oldOffset + e.HorizontalChange / strip.ActualWidth).Coerce(0, 1);
		}
	}

	public class GradientItemsControl : Selector
	{
		protected override DependencyObject GetContainerForItemOverride()
		{
			return new GradientThumb();
		}
	}

	public class GradientThumb : Thumb
	{
		public GradientStop GradientStop
		{
			get { return DataContext as GradientStop; }
		}

		protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
		{
			base.OnPreviewMouseDown(e);
			var itemsControl = ItemsControl.ItemsControlFromItemContainer(this) as GradientItemsControl;
			itemsControl.SelectedItem = GradientStop;
		}
	}
}
