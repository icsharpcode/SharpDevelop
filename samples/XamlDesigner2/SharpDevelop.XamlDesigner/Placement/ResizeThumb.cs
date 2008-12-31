using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using SharpDevelop.XamlDesigner.Controls;
using System.Windows.Documents;
using System.Diagnostics;
using System.Windows.Controls.Primitives;
using SharpDevelop.XamlDesigner.Placement;
using SharpDevelop.XamlDesigner.Dom;

namespace SharpDevelop.XamlDesigner
{
	class ResizeThumb : AdvancedThumb
	{
		static ResizeThumb()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ResizeThumb),
				new FrameworkPropertyMetadata(typeof(ResizeThumb)));
		}

		public ResizeThumb()
		{
			UseDelay = false;
		}

		ResizeDirection dir;
		ResizeOperation op;
		ResizeInfo info;
		DesignView designView;

		public ResizeDirection ResizeDirection
		{
			get { return dir; }
			set
			{
				dir = value;
				Cursor = GetCursor(dir);
				HorizontalAlignment = GetHorizontalAlignment(dir);
				VerticalAlignment = GetVerticalAlignmentt(dir);
			}
		}

		protected override void OnDragStarted()
		{
			var item = DesignItem.GetAttachedItem(this.FindAncestor<Adorner>().AdornedElement);
			op = new ResizeOperation(item, dir);

			designView = item.Context.DesignView;
			info = new ResizeInfo(designView.FeedbackLayer);
			info.Update(op.PlacementInfo.OriginalBounds.Size);
		}

		protected override void OnDragDelta()
		{
			op.Resize(Delta);
			info.Update(op.PlacementInfo.NewBoundsInContainer.Size);
			BringIntoView();
		}

		protected override void OnDragCompleted()
		{
			op.Commit();
			info.Remove();
		}

		static Cursor GetCursor(ResizeDirection dir)
		{
			switch (dir) {
				case ResizeDirection.Down: return Cursors.SizeNS;
				case ResizeDirection.Left: return Cursors.SizeWE;
				case ResizeDirection.LeftDown: return Cursors.SizeNESW;
				case ResizeDirection.LeftUp: return Cursors.SizeNWSE;
				case ResizeDirection.Right: return Cursors.SizeWE;
				case ResizeDirection.RightDown: return Cursors.SizeNWSE;
				case ResizeDirection.RightUp: return Cursors.SizeNESW;
				default: return Cursors.SizeNS;
			}
		}

		public static HorizontalAlignment GetHorizontalAlignment(ResizeDirection dir)
		{
			switch (dir) {
				case ResizeDirection.Down: return HorizontalAlignment.Center;
				case ResizeDirection.Left: return HorizontalAlignment.Left;
				case ResizeDirection.LeftDown: return HorizontalAlignment.Left;
				case ResizeDirection.LeftUp: return HorizontalAlignment.Left;
				case ResizeDirection.Right: return HorizontalAlignment.Right;
				case ResizeDirection.RightDown: return HorizontalAlignment.Right;
				case ResizeDirection.RightUp: return HorizontalAlignment.Right;
				default: return HorizontalAlignment.Center;
			}
		}

		public static VerticalAlignment GetVerticalAlignmentt(ResizeDirection dir)
		{
			switch (dir) {
				case ResizeDirection.Down: return VerticalAlignment.Bottom;
				case ResizeDirection.Left: return VerticalAlignment.Center;
				case ResizeDirection.LeftDown: return VerticalAlignment.Bottom;
				case ResizeDirection.LeftUp: return VerticalAlignment.Top;
				case ResizeDirection.Right: return VerticalAlignment.Center;
				case ResizeDirection.RightDown: return VerticalAlignment.Bottom;
				case ResizeDirection.RightUp: return VerticalAlignment.Top;
				default: return VerticalAlignment.Top;
			}
		}
	}
}
