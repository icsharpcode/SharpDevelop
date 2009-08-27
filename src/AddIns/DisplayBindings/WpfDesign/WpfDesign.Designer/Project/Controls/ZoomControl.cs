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
using System.IO;
using System.Reflection;
using System.Resources;

namespace ICSharpCode.WpfDesign.Designer.Controls
{
	public class ZoomControl : ContentControl
	{
		static ZoomControl()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ZoomControl),
			                                         new FrameworkPropertyMetadata(typeof(ZoomControl)));
			
			PanToolCursor = new Cursor(GetStream("Images/PanToolCursor.cur"));
			PanToolCursorMouseDown = new Cursor(GetStream("Images/PanToolCursorMouseDown.cur"));
		}

		static Stream GetStream(string path)
		{
			var a = Assembly.GetExecutingAssembly();
			var m = new ResourceManager(a.GetName().Name + ".g", a);
			var s = m.GetStream(path.ToLower());
			return s;
		}

		static Cursor PanToolCursor;
		static Cursor PanToolCursorMouseDown;
		public static double ZoomFactor = 1.1;
		public static double Minimum = 0.1;
		public static double Maximum = 10;

		double startHorizontalOffset;
		double startVericalOffset;

		internal ScrollViewer ScrollViewer;
		FrameworkElement container;
		ScaleTransform transform;
		Point startPoint;
		bool isMouseDown;
		bool pan;

		public static readonly DependencyProperty ZoomProperty =
			DependencyProperty.Register("Zoom", typeof(double), typeof(ZoomControl),
			                            new PropertyMetadata(1.0, null, CoerceZoom));

		public double Zoom
		{
			get { return (double)GetValue(ZoomProperty); }
			set { SetValue(ZoomProperty, value); }
		}

		static object CoerceZoom(DependencyObject d, object baseValue)
		{
			var zoom = (double)baseValue;
			return Math.Max(Minimum, Math.Min(Maximum, zoom));
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			ScrollViewer = (ScrollViewer)Template.FindName("scrollViewer", this);
			container = (FrameworkElement)Template.FindName("container", this);
			transform = new ScaleTransform();
			container.LayoutTransform = transform;

			var uxPlus = (ButtonBase)Template.FindName("uxPlus", this);
			var uxMinus = (ButtonBase)Template.FindName("uxMinus", this);
			var uxReset = (ButtonBase)Template.FindName("uxReset", this);

			uxPlus.Click += delegate { ZoomIn(); };
			uxMinus.Click += delegate { ZoomOut(); };
			uxReset.Click += delegate { Reset(); };
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);
			if (e.Property == ZoomProperty)
			{
				transform.ScaleX = Zoom;
				transform.ScaleY = Zoom;
				CenterViewport((double)e.OldValue);
			}
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (!pan && e.Key == Key.Space)
			{
				Cursor = PanToolCursor;
				pan = true;
			}
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			if (e.Key == Key.Space)
			{
				ClearValue(CursorProperty);
				pan = false;
			}
		}

		protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
		{
			if (pan)
			{
				Cursor = PanToolCursorMouseDown;
				Mouse.Capture(this); // will call move
				isMouseDown = true;
				startPoint = e.GetPosition(this);
				PanStart();
			}
		}

		protected override void OnPreviewMouseMove(MouseEventArgs e)
		{
			if (isMouseDown && pan)
			{
				var endPoint = e.GetPosition(this);
				PanContinue(endPoint - startPoint);
			}
		}

		protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
		{
			if (isMouseDown)
			{
				Cursor = PanToolCursor;
				isMouseDown = false;
				Mouse.Capture(null);
			}
		}

		protected override void OnMouseEnter(MouseEventArgs e)
		{
			Focus();
		}

		public void Fit()
		{
			Zoom = Math.Min(
				ScrollViewer.ActualWidth / container.ActualWidth,
				ScrollViewer.ActualHeight / container.ActualHeight);
		}

		public void ZoomIn()
		{
			Zoom *= ZoomFactor;
		}

		public void ZoomOut()
		{
			Zoom /= ZoomFactor;
		}

		public void Reset()
		{
			Zoom = 1;
			ScrollViewer.ScrollToHorizontalOffset(0);
			ScrollViewer.ScrollToVerticalOffset(0);
		}

		void PanStart()
		{
			startHorizontalOffset = ScrollViewer.HorizontalOffset;
			startVericalOffset = ScrollViewer.VerticalOffset;
		}

		void PanContinue(Vector delta)
		{
			ScrollViewer.ScrollToHorizontalOffset(startHorizontalOffset - delta.X);
			ScrollViewer.ScrollToVerticalOffset(startVericalOffset - delta.Y);
		}

		void CenterViewport(double oldZoom)
		{
			var k = Zoom / oldZoom;
			var add = (k * ScrollViewer.ViewportWidth - ScrollViewer.ViewportWidth) / 2;
			ScrollViewer.ScrollToHorizontalOffset(k * ScrollViewer.HorizontalOffset + add);
			add = (k * ScrollViewer.ViewportHeight - ScrollViewer.ViewportHeight) / 2;
			ScrollViewer.ScrollToVerticalOffset(k * ScrollViewer.VerticalOffset + add);
		}
	}
}
