using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace SharpDevelop.XamlDesigner.Controls
{
	class TreeBoxItem : ListBoxItem
	{
		public TreeBoxItem()
		{
			SetResourceReference(StyleProperty, typeof(ListBoxItem));
		}

		Point startPoint;
		//bool click;

		public TreeBoxItemCore Core
		{
			get { return Content as TreeBoxItemCore; }
		}

		//protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		//{
		//    base.OnMouseLeftButtonDown(e);
		//    if (IsSelected) {
		//        //click = true;
		//        startPoint = e.GetPosition(null);
		//        CaptureMouse();
		//        e.Handled = true;
		//    }
		//}

		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			base.OnMouseLeftButtonDown(e);
			startPoint = e.GetPosition(null);
			CaptureMouse();
		}

		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			ReleaseMouseCapture();
			//if (click) {
			//	OnMouseLeftButtonDown(e);
			//}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (IsMouseCaptured) {
				var currentPoint = e.GetPosition(null);
				if (Math.Abs(currentPoint.X - startPoint.X) >= SystemParameters.MinimumHorizontalDragDistance ||
					Math.Abs(currentPoint.Y - startPoint.Y) >= SystemParameters.MinimumVerticalDragDistance) {

					//click = true;
					this.FindAncestor<TreeBox>().TryStartDrag();
				}
			}
		}
	}
}
