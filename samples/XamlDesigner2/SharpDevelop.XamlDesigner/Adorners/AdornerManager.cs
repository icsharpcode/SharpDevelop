using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using SharpDevelop.XamlDesigner.Controls;
using SharpDevelop.XamlDesigner.Placement;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Media;
using SharpDevelop.XamlDesigner.Dom;

namespace SharpDevelop.XamlDesigner
{
	public class AdornerManager
	{
		public AdornerManager(DesignContext context)
		{
			this.context = context;
			context.Selection.Changed += new DesignSelectionChangedHandler(Selection_Changed);
		}

		DesignContext context;
		GeneralAdorner insertAdorner;

		AdornerLayer AdornerLayer
		{
			get { return context.DesignView.AdornerLayer; }
		}

		void Selection_Changed(object sender, DesignSelectionChangedEventArgs e)
		{
			foreach (var item in e.OldItems) {
				if (item.View != null) {
					OnUnselect(item);
				}
			}
			foreach (var item in e.NewItems) {
				if (item.View != null) {
					OnSelect(item);
				}
			}
		}

		void AddGeneralAdorner(DesignItem item, FrameworkElement adorner)
		{
			var generalAdorner = new GeneralAdorner(item.View);
			generalAdorner.Child = adorner;
			AdornerLayer.Add(generalAdorner);
		}

		void ClearAdorners(DesignItem item)
		{
			if (item.View != null) {
				var adorners = AdornerLayer.GetAdorners(item.View);
				if (adorners != null) {
					foreach (var adorner in adorners) {
						AdornerLayer.Remove(adorner);
					}
				}
			}
		}

		public void OnSelect(DesignItem item)
		{
			AddGeneralAdorner(item, new ResizeAdorner());
		}

		public void OnUnselect(DesignItem item)
		{
			ClearAdorners(item);
		}

		public void OnBeginMove(IEnumerable<DesignItem> items)
		{
			foreach (var item in context.Selection) {
				ClearAdorners(item);
			}
			foreach (var item in items) {
				AddGeneralAdorner(item, new MoveAdorner());
			}
		}

		public void OnEndMove(IEnumerable<DesignItem> items)
		{
			foreach (var item in items) {
				ClearAdorners(item);
			}
			foreach (var item in context.Selection) {
				OnSelect(item);
			}
		}

		public void ShowInsertAdorner(FrameworkElement target, Rect area, Dock side)
		{
			if (insertAdorner == null) {
				insertAdorner = new GeneralAdorner(target);
				AdornerLayer.Add(insertAdorner);
			}

			Point location;
			double length;
			Orientation orientation;

			switch (side) {
				case Dock.Left:
					location = area.TopLeft;
					length = area.Height;
					orientation = Orientation.Vertical;
					break;
				case Dock.Right:
					location = area.TopRight;
					length = area.Height;
					orientation = Orientation.Vertical;
					break;
				case Dock.Top:
					location = area.TopLeft;
					length = area.Width;
					orientation = Orientation.Horizontal;
					break;
				default:
					location = area.BottomLeft;
					length = area.Width;
					orientation = Orientation.Horizontal;
					break;
			}

			var insertLine = new InsertLine();
			insertAdorner.Child = insertLine;
			insertAdorner.ChildSize = new Size(length, double.NaN);
			insertAdorner.ChildLocation = location;

			if (orientation == Orientation.Vertical) {
				insertLine.LayoutTransform = new RotateTransform(90);
			}
		}

		public void HideInsertAdorner()
		{
			if (insertAdorner != null) {
				AdornerLayer.Remove(insertAdorner);
				insertAdorner = null;
			}
		}
	}
}
