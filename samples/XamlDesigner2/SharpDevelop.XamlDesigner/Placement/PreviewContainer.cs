using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Diagnostics;

namespace SharpDevelop.XamlDesigner.Placement
{
	class PreviewContainer : PlacementContainer
	{
		public override void OnMove(MoveOperation op)
		{
			var panel = ContainerItem.View as Panel;
			var orient = GetOrientation(panel);
			int? afterIndex = null;
			var p = TransformToContainer().Transform(op.EndPoint);

			FrameworkElement element = null;
			Dock side = Dock.Left;

			if (panel.Children.Count == 0) {
				side = orient == Orientation.Horizontal ? Dock.Left : Dock.Top;
			}
			else {
				for (int i = 0; i < panel.Children.Count; i++) {
					var child = panel.Children[i] as FrameworkElement;
					var bounds = LayoutInformation.GetLayoutSlot(child);
					if (bounds.Contains(p)) {
						if (orient == Orientation.Horizontal) {
							if (p.X > bounds.X + bounds.Width / 2) {
								afterIndex = i;
								break;
							}
						}
						else {
							if (p.Y > bounds.Y + bounds.Height / 2) {
								afterIndex = i;
								break;
							}
						}
						afterIndex = i - 1;
						break;
					}
				}

				if (!afterIndex.HasValue && panel.Children.Count > 0) {
					afterIndex = panel.Children.Count - 1;
					//info.IsDefault = true;
				}

				if (afterIndex.HasValue) {
					if (afterIndex.Value >= 0) {
						element = panel.Children[afterIndex.Value] as FrameworkElement;
						side = orient == Orientation.Horizontal ? Dock.Right : Dock.Bottom;
						//info.InsertAfter = DesignContext.GetAttachedItem(info.MarkElement);
					}
					else {
						element = panel.Children[0] as FrameworkElement;
						side = orient == Orientation.Horizontal ? Dock.Left : Dock.Top;
					}
				}

				var area = element == null ? new Rect(ContainerItem.View.RenderSize) :
					LayoutInformation.GetLayoutSlot(element);

				ContainerItem.Context.AdornerManager.ShowInsertAdorner(ContainerItem.View, area, side);
			}
		}

		public override void Leave(MoveOperation op)
		{
			ContainerItem.Context.AdornerManager.HideInsertAdorner();
		}

		public virtual Orientation GetOrientation(FrameworkElement parentView)
		{
			return Orientation.Horizontal;
		}
	}
}
