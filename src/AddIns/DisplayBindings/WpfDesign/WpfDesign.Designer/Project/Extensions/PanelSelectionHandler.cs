// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.WpfDesign.Adorners;
using ICSharpCode.WpfDesign.Designer.Controls;
using ICSharpCode.WpfDesign.Designer.Services;
using ICSharpCode.WpfDesign.Extensions;

namespace ICSharpCode.WpfDesign.Designer.Extensions
{
	/// <summary>
	/// Handles selection multiple controls inside a Panel.
	/// </summary>
	[ExtensionFor(typeof(Panel))]
	public class PanelSelectionHandler : BehaviorExtension, IHandlePointerToolMouseDown
	{
		protected override void OnInitialized()
		{
			base.OnInitialized();
			this.ExtendedItem.AddBehavior(typeof(IHandlePointerToolMouseDown), this);
		}
		
		public void HandleSelectionMouseDown(IDesignPanel designPanel, MouseButtonEventArgs e, DesignPanelHitTestResult result)
		{
			if (e.ChangedButton == MouseButton.Left && MouseGestureBase.IsOnlyButtonPressed(e, MouseButton.Left)) {
				e.Handled = true;
				new RangeSelectionGesture(result.ModelHit).Start(designPanel, e);
			}
		}
	}
	
	sealed class RangeSelectionGesture : ClickOrDragMouseGesture
	{
		DesignItem container;
		AdornerPanel adornerPanel;
		SelectionFrame selectionFrame;
		
		GrayOutDesignerExceptActiveArea grayOut;
		
		public RangeSelectionGesture(DesignItem container)
		{
			this.container = container;
			this.positionRelativeTo = container.View;
		}
		
		protected override void OnDragStarted(MouseEventArgs e)
		{
			adornerPanel = new AdornerPanel();
			adornerPanel.SetAdornedElement(container.View, container);
			
			selectionFrame = new SelectionFrame();
			adornerPanel.Children.Add(selectionFrame);
			
			designPanel.Adorners.Add(adornerPanel);
			
			GrayOutDesignerExceptActiveArea.Start(ref grayOut, services, container.View);
		}
		
		protected override void OnMouseMove(object sender, MouseEventArgs e)
		{
			base.OnMouseMove(sender, e);
			if (hasDragStarted) {
				SetPlacement(e.GetPosition(positionRelativeTo));
			}
		}
		
		protected override void OnMouseUp(object sender, MouseButtonEventArgs e)
		{
			if (hasDragStarted == false) {
				services.Selection.SetSelectedComponents(new DesignItem [] { container }, SelectionTypes.Auto);
			} else {
				Point endPoint = e.GetPosition(positionRelativeTo);
				Rect frameRect = new Rect(
					Math.Min(startPoint.X, endPoint.X),
					Math.Min(startPoint.Y, endPoint.Y),
					Math.Abs(startPoint.X - endPoint.X),
					Math.Abs(startPoint.Y - endPoint.Y)
				);
				
				ICollection<DesignItem> items = GetChildDesignItemsInContainer(container, new RectangleGeometry(frameRect));
				if (items.Count == 0) {
					items.Add(container);
				}
				services.Selection.SetSelectedComponents(items, SelectionTypes.Auto);
			}
			Stop();
		}
		
		static ICollection<DesignItem> GetChildDesignItemsInContainer(
			DesignItem container, Geometry geometry)
		{
			HashSet<DesignItem> resultItems = new HashSet<DesignItem>();
			ViewService viewService = container.Services.View;
			
			HitTestFilterCallback filterCallback = delegate(DependencyObject potentialHitTestTarget) {
				FrameworkElement element = potentialHitTestTarget as FrameworkElement;
				if (element != null) {
					// ensure we are able to select elements with width/height=0
					if (element.ActualWidth == 0 || element.ActualHeight == 0) {
						DependencyObject tmp = element;
						DesignItem model = null;
						while (tmp != null) {
							model = viewService.GetModel(tmp);
							if (model != null) break;
							tmp = VisualTreeHelper.GetParent(tmp);
						}
						if (model != container) {
							resultItems.Add(model);
							return HitTestFilterBehavior.ContinueSkipChildren;
						}
					}
				}
				return HitTestFilterBehavior.Continue;
			};
			
			HitTestResultCallback resultCallback = delegate(HitTestResult result) {
				if (((GeometryHitTestResult) result).IntersectionDetail == IntersectionDetail.FullyInside) {
					// find the model for the visual contained in the selection area
					DependencyObject tmp = result.VisualHit;
					DesignItem model = null;
					while (tmp != null) {
						model = viewService.GetModel(tmp);
						if (model != null) break;
						tmp = VisualTreeHelper.GetParent(tmp);
					}
					if (model != container) {
						resultItems.Add(model);
					}
				}
				return HitTestResultBehavior.Continue;
			};
			
			VisualTreeHelper.HitTest(container.View, filterCallback, resultCallback, new GeometryHitTestParameters(geometry));
			return resultItems;
		}
		
		void SetPlacement(Point endPoint)
		{
			RelativePlacement p = new RelativePlacement();
			p.XOffset = Math.Min(startPoint.X, endPoint.X);
			p.YOffset = Math.Min(startPoint.Y, endPoint.Y);
			p.WidthOffset = Math.Max(startPoint.X, endPoint.X) - p.XOffset;
			p.HeightOffset = Math.Max(startPoint.Y, endPoint.Y) - p.YOffset;
			AdornerPanel.SetPlacement(selectionFrame, p);
		}
		
		protected override void OnStopped()
		{
			if (adornerPanel != null) {
				designPanel.Adorners.Remove(adornerPanel);
				adornerPanel = null;
			}
			GrayOutDesignerExceptActiveArea.Stop(ref grayOut);
			selectionFrame = null;
			base.OnStopped();
		}
	}
}
