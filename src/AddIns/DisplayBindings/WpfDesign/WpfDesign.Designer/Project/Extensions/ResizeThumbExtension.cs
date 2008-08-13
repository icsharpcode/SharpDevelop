// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

using ICSharpCode.WpfDesign.Adorners;
using ICSharpCode.WpfDesign.Designer.Controls;
using ICSharpCode.WpfDesign.Extensions;
using System.Collections.Generic;

namespace ICSharpCode.WpfDesign.Designer.Extensions
{
	/// <summary>
	/// The resize thumb around a component.
	/// </summary>
	[ExtensionFor(typeof(FrameworkElement))]
	public sealed class ResizeThumbExtension : PrimarySelectionAdornerProvider
	{
		readonly AdornerPanel adornerPanel;
		readonly ResizeThumb[] resizeThumbs;
		/// <summary>An array containing this.ExtendedItem as only element</summary>
		readonly DesignItem[] extendedItemArray = new DesignItem[1];
		IPlacementBehavior resizeBehavior;
		PlacementOperation operation;
		ChangeGroup changeGroup;
		
		public ResizeThumbExtension()
		{
			adornerPanel = new AdornerPanel();
			adornerPanel.Order = AdornerOrder.Foreground;
			this.Adorners.Add(adornerPanel);
			
			resizeThumbs = new ResizeThumb[] {
				CreateThumb(PlacementAlignment.TopLeft, Cursors.SizeNWSE),
				CreateThumb(PlacementAlignment.Top, Cursors.SizeNS),
				CreateThumb(PlacementAlignment.TopRight, Cursors.SizeNESW),
				CreateThumb(PlacementAlignment.Left, Cursors.SizeWE),
				CreateThumb(PlacementAlignment.Right, Cursors.SizeWE),
				CreateThumb(PlacementAlignment.BottomLeft, Cursors.SizeNESW),
				CreateThumb(PlacementAlignment.Bottom, Cursors.SizeNS),
				CreateThumb(PlacementAlignment.BottomRight, Cursors.SizeNWSE)
			};
		}
		
		ResizeThumb CreateThumb(PlacementAlignment alignment, Cursor cursor)
		{
			ResizeThumb resizeThumb = new ResizeThumbImpl( cursor == Cursors.SizeNS, cursor == Cursors.SizeWE );
			resizeThumb.Cursor = cursor;
			resizeThumb.Alignment = alignment;
			AdornerPanel.SetPlacement(resizeThumb, new RelativePlacement(alignment.Horizontal, alignment.Vertical));
			adornerPanel.Children.Add(resizeThumb);
			
			DragListener drag = new DragListener(resizeThumb);
			drag.Started += new DragHandler(drag_Started);
			drag.Changed += new DragHandler(drag_Changed);
			drag.Completed += new DragHandler(drag_Completed);
			return resizeThumb;
		}

		Size oldSize;		

		void drag_Started(DragListener drag)
		{
			oldSize = new Size(ModelTools.GetWidth(ExtendedItem.View), ModelTools.GetHeight(ExtendedItem.View));
			if (resizeBehavior != null) 
				operation = PlacementOperation.Start(extendedItemArray, PlacementType.Resize);
			else {				
				changeGroup = this.ExtendedItem.Context.OpenGroup("Resize", extendedItemArray);
			}
		}

		void drag_Changed(DragListener drag)
		{
			double dx = 0;
			double dy = 0;
			var alignment = (drag.Target as ResizeThumb).Alignment;
			
			if (alignment.Horizontal == HorizontalAlignment.Left) dx = -drag.Delta.X;
			if (alignment.Horizontal == HorizontalAlignment.Right) dx = drag.Delta.X;
			if (alignment.Vertical == VerticalAlignment.Top) dy = -drag.Delta.Y;
			if (alignment.Vertical == VerticalAlignment.Bottom) dy = drag.Delta.Y;
			
			var newWidth = Math.Max(0, oldSize.Width + dx);
			var newHeight = Math.Max(0, oldSize.Height + dy);

			ModelTools.Resize(ExtendedItem, newWidth, newHeight);

			if (operation != null) {
				var info = operation.PlacedItems[0];
				var result = info.OriginalBounds;				
				
				if (alignment.Horizontal == HorizontalAlignment.Left)
					result.X = Math.Min(result.Right, result.X - dx);
				if (alignment.Vertical == VerticalAlignment.Top)
					result.Y = Math.Min(result.Bottom, result.Y - dy);
				result.Width = newWidth;
				result.Height = newHeight;
				
				info.Bounds = result;
				operation.CurrentContainerBehavior.SetPosition(info);
	        }
		}

		void drag_Completed(DragListener drag)
		{
			if (operation != null) {
				if (drag.IsCanceled) operation.Abort();
				else operation.Commit();
				operation = null;
			} else {
				if (drag.IsCanceled) changeGroup.Abort();
				else changeGroup.Commit();
				changeGroup = null;
			}
		}
		
		protected override void OnInitialized()
		{
			base.OnInitialized();
			extendedItemArray[0] = this.ExtendedItem;
			this.Services.Selection.PrimarySelectionChanged += OnPrimarySelectionChanged;			
			resizeBehavior = PlacementOperation.GetPlacementBehavior(extendedItemArray);			
			OnPrimarySelectionChanged(null, null);
		}
		
		protected override void OnRemove()
		{
			this.Services.Selection.PrimarySelectionChanged -= OnPrimarySelectionChanged;
			base.OnRemove();
		}
		
		void OnPrimarySelectionChanged(object sender, EventArgs e)
		{
			bool isPrimarySelection = this.Services.Selection.PrimarySelection == this.ExtendedItem;
			foreach (ResizeThumb g in adornerPanel.Children) {
				g.IsPrimarySelection = isPrimarySelection;
			}
		}
	}
}
