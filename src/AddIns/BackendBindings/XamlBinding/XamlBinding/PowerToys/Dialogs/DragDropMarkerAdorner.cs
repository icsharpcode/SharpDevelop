// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace ICSharpCode.XamlBinding.PowerToys.Dialogs
{
	public class DragDropMarkerAdorner : Adorner
	{
		DragDropMarkerAdorner(UIElement adornedElement)
			: base(adornedElement)
		{
		}
		
		Point start, end;
		
		protected override void OnRender(DrawingContext drawingContext)
		{
			base.OnRender(drawingContext);
			
			drawingContext.DrawLine(new Pen(Brushes.Black, 1), start, end);
		}
		
		protected override Size MeasureOverride(Size constraint)
		{
			return new Size(1, 1); // dummy values
		}
		
		protected override Size ArrangeOverride(Size finalSize)
		{
			return new Size(1, 1); // dummy values
		}
		
		public static DragDropMarkerAdorner CreateAdornerContentMove(StackPanel panel, FrameworkElement aboveElement)
		{
			DragDropMarkerAdorner adorner;
			
			if (aboveElement is StackPanel) {
				aboveElement = (panel.Children.Count > 0 ? panel.Children[panel.Children.Count - 1] : panel) as FrameworkElement;
				adorner = new DragDropMarkerAdorner(aboveElement);
				adorner.start = new Point(5, 5 + aboveElement.DesiredSize.Height);
				adorner.end = new Point(panel.ActualWidth - 10, 5 + aboveElement.DesiredSize.Height);
			} else {
				aboveElement = aboveElement.TemplatedParent as FrameworkElement;
				adorner = new DragDropMarkerAdorner(aboveElement);
				adorner.start = new Point(5, 0);
				adorner.end = new Point(panel.ActualWidth - 10, 0);
			}
			
			AdornerLayer.GetAdornerLayer(aboveElement).Add(adorner);
			
			return adorner;
		}
		
		public static DragDropMarkerAdorner CreateAdornerCellMove(FrameworkElement element)
		{
			DragDropMarkerAdorner adorner = new DragDropMarkerAdorner(element);
			
			if (element is GridLengthEditor) {
				GridLengthEditor editor = element as GridLengthEditor;
				if (editor.Orientation == Orientation.Horizontal) {
					adorner.start = new Point(5,5);
					adorner.end = new Point(5, editor.ActualWidth + 50);
				} else {
					adorner.start = new Point(5,5);
					adorner.end = new Point(editor.ActualHeight + 250, 5);
				}
			} else if (element is StackPanel) {
				StackPanel panel = element as StackPanel;
				Dock dockState = (Dock)panel.GetValue(DockPanel.DockProperty);
				if (dockState == Dock.Right) {
					adorner.start = new Point(5,5);
					adorner.end = new Point(5, panel.ActualHeight);
				} else if (dockState == Dock.Bottom) {
					adorner.start = new Point(5,5);
					adorner.end = new Point(panel.ActualWidth - 10, 5);
				}
			}
			
			AdornerLayer.GetAdornerLayer(element).Add(adorner);
			
			return adorner;
		}
	}
}
