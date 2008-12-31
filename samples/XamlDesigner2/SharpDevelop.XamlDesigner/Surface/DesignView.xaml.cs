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
using SharpDevelop.XamlDesigner.Controls;
using System.Diagnostics;
using SharpDevelop.XamlDesigner.Placement;
using SharpDevelop.XamlDesigner.Dom;

namespace SharpDevelop.XamlDesigner
{
	public partial class DesignView : UserControl, IHasContext
	{
		public DesignView(DesignContext context)
		{
			this.Context = context;
			InitializeComponent();
			this.SeparatedContainer = new SeparatedContainer(this, uxSeparatedItemsLayer);
			ResetActiveTool();
			WireEvents();
			//Loaded += new RoutedEventHandler(DesignView_Loaded);
		}

		//void DesignView_Loaded(object sender, RoutedEventArgs e)
		//{
		//    //AdornerLayer.Update();
		//}

		bool updatingZoom;

		public DesignContext Context { get; private set; }
		internal SeparatedContainer SeparatedContainer { get; private set; }

		//public object Root
		//{
		//    get { return uxRootContainer.Content; }
		//    set { uxRootContainer.Content = value; }
		//}

		public Canvas FeedbackLayer
		{
			get { return uxFeedbackLayer; }
		}

		public AdornerLayer AdornerLayer
		{
			get { return uxAdornerDecorator.AdornerLayer; }
		}

		public FrameworkElement ScrolledLayer
		{
			get { return uxScrolledLayer; }
		}

		public FrameworkElement ZoomedLayer
		{
			get { return uxZoomedLayer; }
		}

		public GeneralTransform TransformScrolledToZoomed
		{
			get { return ScrolledLayer.TransformToDescendant(ZoomedLayer); }
		}

		Exception exception;

		public Exception Exception
		{
			get { return exception; }
			set
			{
				exception = value;
				if (exception == null) {
					uxScrollViewer.Visibility = Visibility.Visible;
					uxExceptionInfo.Visibility = Visibility.Collapsed;
				}
				else {
					uxScrollViewer.Visibility = Visibility.Collapsed;
					uxExceptionInfo.Visibility = Visibility.Visible;
					uxExceptionMessage.Text = exception.ToString();					
				}
			}
		}

		public static readonly DependencyProperty ActiveToolProperty =
			DependencyProperty.Register("ActiveTool", typeof(Tool), typeof(DesignView));

		public Tool ActiveTool
		{
			get { return (Tool)GetValue(ActiveToolProperty); }
			set { SetValue(ActiveToolProperty, value); }
		}

		public static readonly DependencyProperty ZoomProperty =
			DependencyProperty.Register("Zoom", typeof(double), typeof(DesignView),
			new PropertyMetadata(1.0));

		public double Zoom
		{
			get { return (double)GetValue(ZoomProperty); }
			set { SetValue(ZoomProperty, value); }
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);
			if (e.Property == ZoomProperty) {
				UpdateZoom((double)e.OldValue);
			}
			else if (e.Property == ActiveToolProperty) {
				if (ActiveTool != null) {
					ActiveTool.DesignView = this;
				}
				else {
					ResetActiveTool();
				}
			}
		}

		void UpdateZoom(double oldZoom)
		{
			updatingZoom = true;
			uxZoomedLayer.LayoutTransform = new ScaleTransform(Zoom, Zoom);
		}

		public IEnumerable<DesignItem> HitTest(Point p)
		{
			return HitTestCore(new PointHitTestParameters(p));
		}

		public IEnumerable<DesignItem> HitTest(Rect r)
		{
			return HitTestCore(new GeometryHitTestParameters(new RectangleGeometry(r)));
		}

		IEnumerable<DesignItem> HitTestCore(HitTestParameters parameters)
		{
			List<DependencyObject> hits = new List<DependencyObject>();

			VisualTreeHelper.HitTest(uxScrolledLayer,
				delegate(DependencyObject potentialHitTestTarget) {
					return HitTestFilterBehavior.Continue;
				},
				delegate(HitTestResult hitResult) {
					var geometryHitResult = hitResult as GeometryHitTestResult;
					if (geometryHitResult == null ||
						geometryHitResult.IntersectionDetail == IntersectionDetail.FullyContains ||
						geometryHitResult.IntersectionDetail == IntersectionDetail.FullyInside ||
						geometryHitResult.IntersectionDetail == IntersectionDetail.Intersects) {
						hits.Add(hitResult.VisualHit);
					}
					return HitTestResultBehavior.Continue;
				},
				parameters);

			List<DesignItem> result = new List<DesignItem>();

			foreach (var hit in hits) {
				var item = GetDesignItem(hit);
				if (item != null && !result.Contains(item)) {
					result.Add(item);
				}
			}

			return result;
		}

		DesignItem GetDesignItem(DependencyObject view)
		{
			while (view != null) {
				var item = DesignItem.GetAttachedItem(view);
				if (item != null) {
					return item;
				}
				view = VisualTreeHelper.GetParent(view);
			}
			return null;
		}

		public void ResetActiveTool()
		{
			ActiveTool = PointerTool.Instance;
		}

		void WireEvents()
		{
			uxScrollViewer.ScrollChanged += new ScrollChangedEventHandler(uxScrollViewer_ScrollChanged);
			
			uxMouseLayer.RelativeTo = uxMouseLayer;
			uxMouseLayer.DragStarted += new AdvancedDragEventHandler(uxMouseLayer_DragStarted);
			uxMouseLayer.DragDelta += new AdvancedDragEventHandler(uxMouseLayer_DragDelta);
			uxMouseLayer.DragCompleted += new AdvancedDragEventHandler(uxMouseLayer_DragCompleted);
			uxMouseLayer.Click += new AdvancedDragEventHandler(uxMouseLayer_Click);
			uxMouseLayer.MouseEnter += new MouseEventHandler(uxMouseLayer_MouseEnter);
			uxMouseLayer.MouseMove += new MouseEventHandler(uxMouseLayer_MouseMove);
			uxMouseLayer.MouseLeave += new MouseEventHandler(uxMouseLayer_MouseLeave);
		}

		void uxScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			if (updatingZoom) {
				updatingZoom = false;

				var originHorizontalOffset = e.HorizontalOffset - e.HorizontalChange;
				var originExtentWidth = e.ExtentWidth - e.ExtentWidthChange;
				var cx = originExtentWidth > e.ViewportWidth ?
					(originHorizontalOffset + e.ViewportWidth / 2) / originExtentWidth : 0.5;

				var originVerticalOffset = e.VerticalOffset - e.VerticalChange;
				var originExtentHeight = e.ExtentHeight - e.ExtentHeightChange;
				var cy = originExtentHeight > e.ViewportHeight ?
					(originVerticalOffset + e.ViewportHeight / 2) / originExtentHeight : 0.5;

				uxScrollViewer.ScrollToHorizontalOffset(cx * e.ExtentWidth - e.ViewportWidth / 2);
				uxScrollViewer.ScrollToVerticalOffset(cy * e.ExtentHeight - e.ViewportHeight / 2);
			}
		}

		void uxMouseLayer_Click(object sender, AdvancedDragEventArgs e)
		{
			ActiveTool.OnMouseClick(e);
		}

		void uxMouseLayer_DragStarted(object sender, AdvancedDragEventArgs e)
		{
			ActiveTool.OnDragStarted(e);
		}

		void uxMouseLayer_DragDelta(object sender, AdvancedDragEventArgs e)
		{
			ActiveTool.OnDragDelta(e);
		}

		void uxMouseLayer_DragCompleted(object sender, AdvancedDragEventArgs e)
		{
			ActiveTool.OnDragCompleted(e);
		}

		void uxMouseLayer_MouseLeave(object sender, MouseEventArgs e)
		{
			ActiveTool.OnMouseLeave();
		}

		void uxMouseLayer_MouseMove(object sender, MouseEventArgs e)
		{
			ActiveTool.OnMouseMove();
		}

		void uxMouseLayer_MouseEnter(object sender, MouseEventArgs e)
		{
			ActiveTool.OnMouseEnter();
		}
	}
}
