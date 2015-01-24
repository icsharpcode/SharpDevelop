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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Diagnostics;
using ICSharpCode.WpfDesign.Designer.Controls;
using ICSharpCode.WpfDesign.UIExtensions;

namespace ICSharpCode.WpfDesign.Designer.ThumbnailView
{
	public class ThumbnailView : Control, INotifyPropertyChanged
	{
		public DesignSurface DesignSurface
		{
			get { return (DesignSurface)GetValue(DesignSurfaceProperty); }
			set { SetValue(DesignSurfaceProperty, value); }
		}

		public static readonly DependencyProperty DesignSurfaceProperty =
			DependencyProperty.Register("DesignSurface", typeof(DesignSurface), typeof(ThumbnailView), new PropertyMetadata(OnDesignSurfaceChanged));

		private static void OnDesignSurfaceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var ctl = d as ThumbnailView;
			
			
			if (ctl.oldSurface != null)
				ctl.oldSurface.LayoutUpdated -= ctl.DesignSurface_LayoutUpdated;
			
			ctl.oldSurface = ctl.DesignSurface;
			ctl.scrollViewer = null;

			if (ctl.DesignSurface != null)
			{
				ctl.DesignSurface.LayoutUpdated += ctl.DesignSurface_LayoutUpdated;
			}

			ctl.OnPropertyChanged("ScrollViewer");
		}
		
		static ThumbnailView()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ThumbnailView), new FrameworkPropertyMetadata(typeof(ThumbnailView)));
		}

		public ScrollViewer ScrollViewer
		{
			get
			{
				if (DesignSurface != null && scrollViewer == null)
					scrollViewer = DesignSurface.TryFindChild<ZoomControl>();

				return scrollViewer;
			}
		}


		void DesignSurface_LayoutUpdated(object sender, EventArgs e)
		{
			if (this.scrollViewer == null)
				OnPropertyChanged("ScrollViewer");

			if (this.scrollViewer != null)
			{
				double scale, xOffset, yOffset;
				this.InvalidateScale(out scale, out xOffset, out yOffset);

				this.zoomThumb.Width = scrollViewer.ViewportWidth * scale;
				this.zoomThumb.Height = scrollViewer.ViewportHeight * scale;
				
				Canvas.SetLeft(this.zoomThumb, xOffset + this.ScrollViewer.HorizontalOffset*scale);
				Canvas.SetTop(this.zoomThumb, yOffset + this.ScrollViewer.VerticalOffset*scale);
			}
		}

		private DesignSurface oldSurface;
		private ZoomControl scrollViewer;
		private Canvas zoomCanvas;
		private Thumb zoomThumb;

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			this.zoomThumb = Template.FindName("PART_ZoomThumb", this) as Thumb;
			this.zoomCanvas = Template.FindName("PART_ZoomCanvas", this) as Canvas;

			this.zoomThumb.DragDelta += this.Thumb_DragDelta;

			
		}

		private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
		{
			if (DesignSurface != null)
			{
				if (scrollViewer != null)
				{
					double scale, xOffset, yOffset;
					this.InvalidateScale(out scale, out xOffset, out yOffset);

					scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset + e.HorizontalChange / scale);
					scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + e.VerticalChange / scale);
				}
			}
		}

		private void InvalidateScale(out double scale, out double xOffset, out double yOffset)
		{
			scale = 1;
			xOffset = 0;
			yOffset = 0;
			
			if (this.DesignSurface.DesignContext != null && this.DesignSurface.DesignContext.RootItem != null)
			{
				var designedElement = this.DesignSurface.DesignContext.RootItem.Component as FrameworkElement;
				
				
				var fac1 = designedElement.ActualWidth / zoomCanvas.ActualWidth;
				var fac2 = designedElement.ActualHeight / zoomCanvas.ActualHeight;
				
				// zoom canvas size
				double x = this.zoomCanvas.ActualWidth;
				double y = this.zoomCanvas.ActualHeight;
				
				if (fac1 < fac2)
				{
					x = designedElement.ActualWidth / fac2;
					xOffset = (zoomCanvas.ActualWidth - x) / 2;
					yOffset = 0;
				}
				else
				{
					y =  designedElement.ActualHeight / fac1;
					xOffset = 0;
					yOffset = (zoomCanvas.ActualHeight - y) / 2;
				}
				
				double w = designedElement.ActualWidth;
				double h = designedElement.ActualHeight;
				
				double scaleX = x / w;
				double scaleY = y / h;

				scale = (scaleX < scaleY) ? scaleX : scaleY;
				
				xOffset += (x - scale * w) / 2;
				yOffset += (y - scale * h) / 2;
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
