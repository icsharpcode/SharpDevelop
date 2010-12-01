// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

using Microsoft.Windows.Themes;

namespace Debugger.AddIn.Pads.ParallelPad
{
	public partial class DrawSurface : UserControl
	{
		private Point dragStartedPoint;		
		private ScaleTransform zoom = new ScaleTransform();
		
		public DrawSurface()
		{
			InitializeComponent();
		
			ContentControl.LayoutTransform = zoom;
			
			this.PreviewMouseLeftButtonDown += DrawSurface_PreviewMouseLeftButtonDown;
			this.PreviewMouseLeftButtonUp += DrawSurface_MouseLeftButtonUp;
		}
		
		public void SetGraph(ParallelStacksGraph graph)
		{
			this.ParallelStacksLayout.Graph = graph;
		
			if (graph == null)
				this.ParallelStacksLayout.CancelLayout();
			else {
				this.ParallelStacksLayout.Relayout();
				this.ParallelStacksLayout.Relayout();
			}
		}
		
		public bool IsZoomControlVisible {
			get { return ZoomControl.Visibility == Visibility.Visible; }
			set {
				if (value)
					ZoomControl.Visibility = Visibility.Visible;
				else
					ZoomControl.Visibility = Visibility.Hidden;
			}
		}

		#region Pan
		
		void DrawSurface_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (e.OriginalSource is TextBlock || e.OriginalSource is Shape || e.OriginalSource is ScrollChrome)
				return;
			
			dragStartedPoint = e.GetPosition(drawingSurface);
			drawingSurface.CaptureMouse();
			this.PreviewMouseMove += DrawSurface_PreviewMouseMove;
			e.Handled = true;
		}

		void DrawSurface_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			drawingSurface.ReleaseMouseCapture();
			if (e.OriginalSource is Shape || e.OriginalSource is TextBlock)
				return;
			
			this.PreviewMouseMove -= DrawSurface_PreviewMouseMove;
			Cursor = Cursors.Arrow;
		}

		void DrawSurface_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			if (e.OriginalSource is TextBlock || e.OriginalSource is Shape || e.OriginalSource is ScrollChrome)
				return;
			
			if (!drawingSurface.IsMouseCaptured) return;
			
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				Cursor = Cursors.SizeAll;
				var point = e.GetPosition(drawingSurface);
				Vector v = point - dragStartedPoint;
				ContentControl.Margin = new Thickness(
					ContentControl.Margin.Left + v.X / 80,
					ContentControl.Margin.Top + v.Y / 80, 0, 0);
				
				e.Handled = true;
			}
		}
		
		#endregion
		
		#region Zoom
		
		void SliderControl_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (e.OldValue == 0)
				return;
			
			double value = (e.NewValue / 5d) * 100;
			
			this.PercentText.Text = string.Format("{0}%", value);
			
			// zoom canvas
			zoom.ScaleX = e.NewValue / 5d;
			zoom.ScaleY = e.NewValue / 5d;
			zoom.CenterX = drawingSurface.ActualWidth / 2d;
			zoom.CenterY = drawingSurface.ActualHeight / 2d;
		}
		
		void Reset_Click(object sender, RoutedEventArgs e)
		{
			this.SliderControl.Value = 5;
		}
		
		#endregion
	}
}