// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

using ICSharpCode.WpfDesign.Adorners;
using ICSharpCode.WpfDesign.Designer.Controls;

namespace ICSharpCode.WpfDesign.Designer
{
	sealed class DesignPanel : Decorator, IDesignPanel
	{
		#region Hit Testing
		/// <summary>
		/// this element is always hit (unless HitTestVisible is set to false)
		/// </summary>
		sealed class EatAllHitTestRequests : UIElement
		{
			protected override GeometryHitTestResult HitTestCore(GeometryHitTestParameters hitTestParameters)
			{
				return new GeometryHitTestResult(this, IntersectionDetail.FullyContains);
			}
			
			protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
			{
				return new PointHitTestResult(this, hitTestParameters.HitPoint);
			}
		}
		
		// Like VisualTreeHelper.HitTest(Visual,Point); but allows using a filter callback.
		static PointHitTestResult RunHitTest(Visual reference, Point point, HitTestFilterCallback filterCallback)
		{
			HitTestResult result = null;
			VisualTreeHelper.HitTest(reference, filterCallback,
			                         delegate (HitTestResult resultParameter) {
			                         	result = resultParameter;
			                         	return HitTestResultBehavior.Stop;
			                         },
			                         new PointHitTestParameters(point));
			return result as PointHitTestResult;
		}
		
		static HitTestFilterBehavior FilterHitTestInvisibleElements(DependencyObject potentialHitTestTarget)
		{
			UIElement element = potentialHitTestTarget as UIElement;
			if (element != null) {
				if (!(element.IsHitTestVisible && element.Visibility == Visibility.Visible)) {
					return HitTestFilterBehavior.ContinueSkipSelfAndChildren;
				}
			}
			return HitTestFilterBehavior.Continue;
		}
		
		DesignPanelHitTestResult _lastHitTestResult;
		
		/// <summary>
		/// Performs a custom hit testing lookup for the specified mouse event args.
		/// </summary>
		public DesignPanelHitTestResult HitTest(MouseEventArgs e, bool testAdorners, bool testDesignSurface)
		{
			_lastHitTestResult = CustomHitTestInternal(e.GetPosition(this), testAdorners, testDesignSurface);
			return _lastHitTestResult;
		}
		
		DesignPanelHitTestResult CustomHitTestInternal(Point mousePosition, bool testAdorners, bool testDesignSurface)
		{
			if (mousePosition.X < 0 || mousePosition.Y < 0 || mousePosition.X > this.RenderSize.Width || mousePosition.Y > this.RenderSize.Height) {
				return DesignPanelHitTestResult.NoHit;
			}
			// First try hit-testing on the adorner layer.
			
			PointHitTestResult result;
			DesignPanelHitTestResult customResult;
			
			if (testAdorners) {
				result = RunHitTest(_adornerLayer, mousePosition, FilterHitTestInvisibleElements);
				if (result != null && result.VisualHit != null) {
					if (result.VisualHit == _lastHitTestResult.VisualHit)
						return _lastHitTestResult;
					customResult = new DesignPanelHitTestResult(result.VisualHit);
					DependencyObject obj = result.VisualHit;
					while (obj != null && obj != _adornerLayer) {
						AdornerPanel adorner = obj as AdornerPanel;
						if (adorner != null) {
							customResult.AdornerHit = adorner;
						}
						obj = VisualTreeHelper.GetParent(obj);
					}
					return customResult;
				}
			}
			
			if (testDesignSurface) {
				result = RunHitTest(this.Child, mousePosition, delegate { return HitTestFilterBehavior.Continue; });
				if (result != null && result.VisualHit != null) {
					customResult = new DesignPanelHitTestResult(result.VisualHit);
					
					ViewService viewService = _context.Services.View;
					DependencyObject obj = result.VisualHit;
					while (obj != null) {
						if ((customResult.ModelHit = viewService.GetModel(obj)) != null)
							break;
						obj = VisualTreeHelper.GetParent(obj);
					}
					if (customResult.ModelHit == null)
					{
						customResult.ModelHit = _context.RootItem;
					}
					return customResult;
				}
			}
			return DesignPanelHitTestResult.NoHit;
		}
		#endregion
		
		#region Fields + Constructor
		DesignContext _context;
		readonly EatAllHitTestRequests _eatAllHitTestRequests;
		readonly AdornerLayer _adornerLayer;
		readonly Canvas _markerCanvas;
		
		public DesignPanel()
		{
			this.Focusable = true;
			this.VerticalAlignment = VerticalAlignment.Top;
			this.HorizontalAlignment = HorizontalAlignment.Left;
			
			_eatAllHitTestRequests = new EatAllHitTestRequests();
			_adornerLayer = new AdornerLayer(this);
			_markerCanvas = new Canvas();
			_markerCanvas.IsHitTestVisible = false;
		}
		#endregion
		
		#region Properties
		
		/// <summary>
		/// Gets/Sets the design context.
		/// </summary>
		public DesignContext Context {
			get { return _context; }
			set { _context = value; }
		}
		
		public ICollection<AdornerPanel> Adorners {
			get {
				return _adornerLayer.Adorners;
			}
		}
		
		public Canvas MarkerCanvas {
			get { return _markerCanvas; }
		}
		
		/// <summary>
		/// Gets/Sets if the design content is visible for hit-testing purposes.
		/// </summary>
		public bool IsContentHitTestVisible {
			get { return !_eatAllHitTestRequests.IsHitTestVisible; }
			set { _eatAllHitTestRequests.IsHitTestVisible = !value; }
		}
		
		/// <summary>
		/// Gets/Sets if the adorner layer is visible for hit-testing purposes.
		/// </summary>
		public bool IsAdornerLayerHitTestVisible {
			get { return _adornerLayer.IsHitTestVisible; }
			set { _adornerLayer.IsHitTestVisible = value; }
		}
		
		#endregion
		
		#region Visual Child Management
		public override UIElement Child {
			get { return base.Child; }
			set {
				if (base.Child == value)
					return;
				if (value == null) {
					// Child is being set from some value to null
					
					// remove _adornerLayer and _eatAllHitTestRequests
					RemoveVisualChild(_adornerLayer);
					RemoveVisualChild(_eatAllHitTestRequests);
					RemoveVisualChild(_markerCanvas);
				} else if (base.Child == null) {
					// Child is being set from null to some value
					AddVisualChild(_adornerLayer);
					AddVisualChild(_eatAllHitTestRequests);
					AddVisualChild(_markerCanvas);
				}
				base.Child = value;
			}
		}
		
		protected override Visual GetVisualChild(int index)
		{
			if (base.Child != null) {
				if (index == 0)
					return base.Child;
				else if (index == 1)
					return _eatAllHitTestRequests;
				else if (index == 2)
					return _adornerLayer;
				else if (index == 3)
					return _markerCanvas;
			}
			return base.GetVisualChild(index);
		}
		
		protected override int VisualChildrenCount {
			get {
				if (base.Child != null)
					return 4;
				else
					return base.VisualChildrenCount;
			}
		}
		
		protected override Size MeasureOverride(Size constraint)
		{
			Size result = base.MeasureOverride(constraint);
			if (this.Child != null) {
				_adornerLayer.Measure(constraint);
				_eatAllHitTestRequests.Measure(constraint);
				_markerCanvas.Measure(constraint);
			}
			return result;
		}
		
		protected override Size ArrangeOverride(Size arrangeSize)
		{
			Size result = base.ArrangeOverride(arrangeSize);
			if (this.Child != null) {
				Rect r = new Rect(new Point(0, 0), arrangeSize);
				_adornerLayer.Arrange(r);
				_eatAllHitTestRequests.Arrange(r);
				_markerCanvas.Arrange(r);
			}
			return result;
		}
		#endregion
		
		protected override void OnQueryCursor(QueryCursorEventArgs e)
		{
			base.OnQueryCursor(e);
			if (_context != null) {
				Cursor cursor = _context.Services.Tool.CurrentTool.Cursor;
				if (cursor != null) {
					e.Cursor = cursor;
					e.Handled = true;
				}
			}
		}
	}
}
