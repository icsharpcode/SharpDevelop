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
		
		DesignContext _context;
		readonly EatAllHitTestRequests _eatAllHitTestRequests;
		readonly AdornerLayer _adornerLayer;
		readonly Canvas _markerCanvas;
		
		public DesignPanel()
		{
			this.Focusable = true;
			this.VerticalAlignment = VerticalAlignment.Top;
			this.HorizontalAlignment = HorizontalAlignment.Left;
			this.Margin = new Thickness(10);
			
			_eatAllHitTestRequests = new EatAllHitTestRequests();
			_eatAllHitTestRequests.IsHitTestVisible = false;
			_adornerLayer = new AdornerLayer(this);
			_markerCanvas = new Canvas();
			_markerCanvas.IsHitTestVisible = false;
		}
		
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
		
		/// <summary>
		/// Gets/Sets the design context.
		/// </summary>
		public DesignContext Context {
			get { return _context; }
			set { _context = value; }
		}
		
		private IToolService ToolService {
			[DebuggerStepThrough]
			get { return _context.Services.Tool; }
		}
		
		protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
		{
			base.OnPreviewMouseDown(e);
			if (!_isInInputAction) {
				Debug.WriteLine("DesignPanel.PreviewMouseDown Source=" + e.Source.GetType().Name + " OriginalSource=" + e.OriginalSource.GetType().Name);
				object topMostHitSource = null;
				DesignItem site = FindDesignedElementForOriginalSource(e.OriginalSource, ref topMostHitSource);
				InputHandlingLayer itemLayer = InputHandlingLayer.None;
				if (site != null) {
					Debug.WriteLine(" Found designed element: " + site.Component.GetType().Name);
					IProvideComponentInputHandlingLayer layerProvider = site.GetBehavior<IProvideComponentInputHandlingLayer>();
					if (layerProvider != null) {
						itemLayer = layerProvider.InputLayer;
					}
				} else if (topMostHitSource == _adornerLayer) {
					itemLayer = InputHandlingLayer.Adorners;
				}
				if (ToolService.CurrentTool.InputLayer > itemLayer) {
					ToolService.CurrentTool.OnMouseDown(this, e);
				}
			}
		}
		
		// find a DesignItem for the clicked originalSource. This walks up the visual tree until it finds a designed item
		// or the design surface itself.
		// topMostSource is set to the element directly below the one that caused the tree walk to abort.
		// For hits on the adorner layer, the method will return null and set topMostSource to _adornerLayer.
		DesignItem FindDesignedElementForOriginalSource(object originalSource, ref object topMostSource)
		{
			if (originalSource == null)
				return null;
			DesignItem site = _context.Services.Component.GetDesignItem(originalSource);
			if (site != null)
				return site;
			if (originalSource == this)
				return null;
			topMostSource = originalSource;
			DependencyObject dObj = originalSource as DependencyObject;
			if (dObj == null)
				return null;
			return FindDesignedElementForOriginalSource(VisualTreeHelper.GetParent(dObj), ref topMostSource);
		}
		
		public DesignItem FindDesignedElementForOriginalSource(object originalSource)
		{
			object topMostSource = null;
			return FindDesignedElementForOriginalSource(originalSource, ref topMostSource);
		}
		
		/// <summary>
		/// prevent designed controls from getting the keyboard focus
		/// </summary>
		protected override void OnPreviewGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
		{
			if (e.NewFocus != this) {
				if (e.NewFocus is TabItem) {
					Dispatcher.BeginInvoke(DispatcherPriority.Normal,
					                       new Action(delegate { Focus(); }));
				} else {
					e.Handled = true;
					Focus();
				}
			}
		}
		
		bool _isInInputAction;
		
		void IDesignPanel.StartInputAction()
		{
			if (_isInInputAction) throw new InvalidOperationException();
			_isInInputAction = true;
			_eatAllHitTestRequests.IsHitTestVisible = true;
		}
		
		void IDesignPanel.StopInputAction()
		{
			if (!_isInInputAction) throw new InvalidOperationException();
			_isInInputAction = false;
			_eatAllHitTestRequests.IsHitTestVisible = false;
		}
		
		public ICollection<AdornerPanel> Adorners {
			get {
				return _adornerLayer.Adorners;
			}
		}
		
		public Canvas MarkerCanvas {
			get { return _markerCanvas; }
		}
	}
	
	internal delegate void Action();
}
