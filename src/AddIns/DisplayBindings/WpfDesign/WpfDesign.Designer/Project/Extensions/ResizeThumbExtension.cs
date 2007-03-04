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
		ResizeThumb activeResizeThumb;
		
		/// <summary></summary>
		public ResizeThumbExtension()
		{
			adornerPanel = new AdornerPanel();
			adornerPanel.Order = AdornerOrder.Foreground;
			this.Adorners.Add(adornerPanel);
			
			resizeThumbs = new ResizeThumb[] {
				CreateThumb(PlacementAlignments.TopLeft, Cursors.SizeNWSE),
				CreateThumb(PlacementAlignments.Top, Cursors.SizeNS),
				CreateThumb(PlacementAlignments.TopRight, Cursors.SizeNESW),
				CreateThumb(PlacementAlignments.Left, Cursors.SizeWE),
				CreateThumb(PlacementAlignments.Right, Cursors.SizeWE),
				CreateThumb(PlacementAlignments.BottomLeft, Cursors.SizeNESW),
				CreateThumb(PlacementAlignments.Bottom, Cursors.SizeNS),
				CreateThumb(PlacementAlignments.BottomRight, Cursors.SizeNWSE)
			};
		}
		
		ResizeThumb CreateThumb(PlacementAlignment alignment, Cursor cursor)
		{
			ResizeThumb resizeThumb = new ResizeThumbImpl( cursor == Cursors.SizeNS, cursor == Cursors.SizeWE );
			resizeThumb.Cursor = cursor;
			resizeThumb.Alignment = alignment;
			AdornerPanel.SetPlacement(resizeThumb, new RelativePlacement(alignment.Horizontal, alignment.Vertical));
			adornerPanel.Children.Add(resizeThumb);
			
			resizeThumb.DragStarted   += OnDragStarted;
			resizeThumb.DragDelta     += OnDragDelta(alignment);
			resizeThumb.DragCompleted += OnDragCompleted;
			return resizeThumb;
		}
		
		void OnDragStarted(object sender, DragStartedEventArgs e)
		{
			activeResizeThumb = (ResizeThumb)sender;
			operation = PlacementOperation.Start(extendedItemArray, PlacementType.Resize);
			this.ExtendedItem.Services.GetService<IDesignPanel>().KeyDown += OnDesignPanelKeyDown;
		}
		
		DragDeltaEventHandler OnDragDelta(PlacementAlignment alignment)
		{
			return delegate(object sender, DragDeltaEventArgs e) {
				foreach (PlacementInformation info in operation.PlacedItems) {
					double left = info.Bounds.Left;
					double right = info.Bounds.Right;
					double bottom = info.Bounds.Bottom;
					double top = info.Bounds.Top;
					switch (alignment.Horizontal) {
						case HorizontalAlignment.Left:
							left += e.HorizontalChange;
							if (left > right)
								left = right;
							break;
						case HorizontalAlignment.Right:
							right += e.HorizontalChange;
							if (right < left)
								right = left;
							break;
					}
					switch (alignment.Vertical) {
						case VerticalAlignment.Top:
							top += e.VerticalChange;
							if (top > bottom)
								top = bottom;
							break;
						case VerticalAlignment.Bottom:
							bottom += e.VerticalChange;
							if (bottom < top)
								bottom = top;
							break;
					}
					info.Bounds = new Rect(left, top, right - left, bottom - top);
					operation.CurrentContainerBehavior.SetPosition(info);
				}
			};
		}
		
		void OnDragCompleted(object sender, DragCompletedEventArgs e)
		{
			if (operation != null) {
				this.ExtendedItem.Services.GetService<IDesignPanel>().KeyDown -= OnDesignPanelKeyDown;
				if (e.Canceled)
					operation.Abort();
				else
					operation.Commit();
				operation = null;
			}
		}
		
		void OnDesignPanelKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape) {
				activeResizeThumb.CancelDrag();
			}
		}
		
		/// <summary/>
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
		
		/// <summary/>
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
	}
}
