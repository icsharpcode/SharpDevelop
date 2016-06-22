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
