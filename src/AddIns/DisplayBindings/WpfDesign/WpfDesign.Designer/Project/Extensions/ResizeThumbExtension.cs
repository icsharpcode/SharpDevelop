// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
	public sealed class ResizeThumbExtension : SelectionAdornerProvider
	{
		readonly AdornerPanel adornerPanel;
		readonly ResizeThumb[] resizeThumbs;
		/// <summary>An array containing this.ExtendedItem as only element</summary>
		readonly DesignItem[] extendedItemArray = new DesignItem[1];
		IPlacementBehavior resizeBehavior;
		PlacementOperation operation;
		ChangeGroup changeGroup;		
				
		bool _isResizing;
		
		/// <summary>
		/// Gets whether this extension is resizing any element.
		/// </summary>
		public bool IsResizing{
			get { return _isResizing; }
		}
		
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
			AdornerPanel.SetPlacement(resizeThumb, Place(ref resizeThumb, alignment));
			adornerPanel.Children.Add(resizeThumb);
			
			DragListener drag = new DragListener(resizeThumb);
			drag.Started += new DragHandler(drag_Started);
			drag.Changed += new DragHandler(drag_Changed);
			drag.Completed += new DragHandler(drag_Completed);
			return resizeThumb;
		}
		
		/// <summary>
		/// Places resize thumbs at their respective positions
		/// and streches out thumbs which are at the center of outline to extend resizability across the whole outline
		/// </summary>
		/// <param name="resizeThumb"></param>
		/// <param name="alignment"></param>
		/// <returns></returns>
		private RelativePlacement Place(ref ResizeThumb resizeThumb,PlacementAlignment alignment)
        {
            RelativePlacement placement = new RelativePlacement(alignment.Horizontal,alignment.Vertical);
            
            if (alignment.Horizontal == HorizontalAlignment.Center)
            {
                placement.WidthRelativeToContentWidth = 1;
                placement.HeightOffset = 6;
                resizeThumb.Opacity = 0;
                return placement;
            }
            if (alignment.Vertical == VerticalAlignment.Center)
            {
                placement.HeightRelativeToContentHeight = 1;
                placement.WidthOffset = 6;
                resizeThumb.Opacity = 0;
                return placement;
            }
            
            placement.WidthOffset = 6;
            placement.HeightOffset = 6;
            return placement;
        }

		Size oldSize;
		
		// TODO : Remove all hide/show extensions from here.
		void drag_Started(DragListener drag)
		{
			/* Abort editing Text if it was editing, because it interferes with the undo stack. */
			foreach(var extension in this.ExtendedItem.Extensions){
				if(extension is InPlaceEditorExtension){
					((InPlaceEditorExtension)extension).AbortEdit();
				}
			}
				
			oldSize = new Size(ModelTools.GetWidth(ExtendedItem.View), ModelTools.GetHeight(ExtendedItem.View));
			if (resizeBehavior != null)
				operation = PlacementOperation.Start(extendedItemArray, PlacementType.Resize);
			else {
				changeGroup = this.ExtendedItem.Context.OpenGroup("Resize", extendedItemArray);
			}
			_isResizing=true;
			ShowSizeAndHideHandles();
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
				
				info.Bounds = result.Round();
				info.ResizeThumbAlignment = alignment;
				operation.CurrentContainerBehavior.BeforeSetPosition(operation);
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
			_isResizing=false;
			HideSizeAndShowHandles();
		}
		
		protected override void OnInitialized()
		{
			base.OnInitialized();
			extendedItemArray[0] = this.ExtendedItem;
			this.ExtendedItem.PropertyChanged += OnPropertyChanged;
			this.Services.Selection.PrimarySelectionChanged += OnPrimarySelectionChanged;
			resizeBehavior = PlacementOperation.GetPlacementBehavior(extendedItemArray);
			UpdateAdornerVisibility();
			OnPrimarySelectionChanged(null, null);
		}
		
		void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			UpdateAdornerVisibility();
		}
		
		protected override void OnRemove()
		{
			this.ExtendedItem.PropertyChanged -= OnPropertyChanged;
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
		
		void UpdateAdornerVisibility()
		{
			FrameworkElement fe = this.ExtendedItem.View as FrameworkElement;
			foreach (ResizeThumb r in resizeThumbs) {
				bool isVisible = resizeBehavior != null && resizeBehavior.CanPlace(extendedItemArray, PlacementType.Resize, r.Alignment);
				r.Visibility = isVisible ? Visibility.Visible : Visibility.Hidden;
			}
		}
		
		void ShowSizeAndHideHandles()
    	{
            SizeDisplayExtension sizeDisplay=null;
            MarginHandleExtension marginDisplay=null;
            foreach(var extension in ExtendedItem.Extensions) {
                if (extension is SizeDisplayExtension)
                    sizeDisplay = extension as SizeDisplayExtension;
                if (extension is MarginHandleExtension)
            		 marginDisplay = extension as MarginHandleExtension;
            }

            if(sizeDisplay!=null) {
                sizeDisplay.HeightDisplay.Visibility = Visibility.Visible;
                sizeDisplay.WidthDisplay.Visibility = Visibility.Visible;
            }
            
            if(marginDisplay!=null)
            	marginDisplay.HideHandles();
        }

        void HideSizeAndShowHandles()
        {
            SizeDisplayExtension sizeDisplay = null;
            MarginHandleExtension marginDisplay=null;
            foreach (var extension in ExtendedItem.Extensions){
                if (extension is SizeDisplayExtension)
                    sizeDisplay = extension as SizeDisplayExtension;
                if (extension is MarginHandleExtension)
            		 marginDisplay = extension as MarginHandleExtension;
            }

            if (sizeDisplay != null) {
                sizeDisplay.HeightDisplay.Visibility = Visibility.Hidden;
                sizeDisplay.WidthDisplay.Visibility = Visibility.Hidden;
            }
            if (marginDisplay !=null ) {
            	marginDisplay.ShowHandles();
            }
        }		
	}
}
