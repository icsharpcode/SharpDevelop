// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace ICSharpCode.Core.Presentation
{
	public class ZoomScrollViewer : ScrollViewer
	{
		static ZoomScrollViewer()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ZoomScrollViewer),
			                                         new FrameworkPropertyMetadata(typeof(ZoomScrollViewer)));
		}
		
		public static readonly DependencyProperty CurrentZoomProperty =
			DependencyProperty.Register("CurrentZoom", typeof(double), typeof(ZoomScrollViewer),
			                            new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, null, CoerceZoom));
		
		public double CurrentZoom {
			get { return (double)GetValue(CurrentZoomProperty); }
			set { SetValue(CurrentZoomProperty, value); }
		}
		
		static object CoerceZoom(DependencyObject d, object baseValue)
		{
			var zoom = (double)baseValue;
			ZoomScrollViewer sv = (ZoomScrollViewer)d;
			return Math.Max(sv.MinimumZoom, Math.Min(sv.MaximumZoom, zoom));
		}
		
		public static readonly DependencyProperty MinimumZoomProperty =
			DependencyProperty.Register("MinimumZoom", typeof(double), typeof(ZoomScrollViewer),
			                            new FrameworkPropertyMetadata(0.2));
		
		public double MinimumZoom {
			get { return (double)GetValue(MinimumZoomProperty); }
			set { SetValue(MinimumZoomProperty, value); }
		}
		
		public static readonly DependencyProperty MaximumZoomProperty =
			DependencyProperty.Register("MaximumZoom", typeof(double), typeof(ZoomScrollViewer),
			                            new FrameworkPropertyMetadata(5.0));
		
		public double MaximumZoom {
			get { return (double)GetValue(MaximumZoomProperty); }
			set { SetValue(MaximumZoomProperty, value); }
		}
		
		public static readonly DependencyProperty MouseWheelZoomProperty =
			DependencyProperty.Register("MouseWheelZoom", typeof(bool), typeof(ZoomScrollViewer),
			                            new FrameworkPropertyMetadata(true));
		
		public bool MouseWheelZoom {
			get { return (bool)GetValue(MouseWheelZoomProperty); }
			set { SetValue(MouseWheelZoomProperty, value); }
		}
		
		protected override void OnMouseWheel(MouseWheelEventArgs e)
		{
			if (!e.Handled && Keyboard.Modifiers == ModifierKeys.Control && MouseWheelZoom) {
				SetCurrentValue(CurrentZoomProperty, RoundToOneIfClose(CurrentZoom * Math.Pow(1.001, e.Delta)));
				e.Handled = true;
			}
			base.OnMouseWheel(e);
		}
		
		internal static double RoundToOneIfClose(double val)
		{
			if (Math.Abs(val - 1.0) < 0.0001)
				return 1.0;
			else
				return val;
		}
	}
	
	sealed class IsNormalZoomConverter : IValueConverter
	{
		public static readonly IsNormalZoomConverter Instance = new IsNormalZoomConverter();
		
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return ((double)value) == 1.0;
		}
		
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}