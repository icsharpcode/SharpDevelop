// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

using ICSharpCode.WpfDesign.Adorners;
using ICSharpCode.WpfDesign.Designer.Controls;
using ICSharpCode.WpfDesign.Extensions;

namespace ICSharpCode.WpfDesign.Designer.Extensions
{
	/// <summary>
	/// The resize thumb at the top left edge of a component.
	/// </summary>
	[ExtensionFor(typeof(FrameworkElement))]
	public sealed class ResizeThumbExtension : PrimarySelectionAdornerProvider
	{
		AdornerPanel adornerPanel;
		DragFrame dragFrame;
		IChildResizeSupport resizeBehavior;
		GrayOutDesignerExceptActiveArea grayOut;
		
		/// <summary></summary>
		public ResizeThumbExtension()
		{
			adornerPanel = new AdornerPanel();
			adornerPanel.Order = AdornerOrder.Foreground;
			
			CreateThumb(HorizontalAlignment.Left, VerticalAlignment.Top);
			CreateThumb(HorizontalAlignment.Right, VerticalAlignment.Top);
			CreateThumb(HorizontalAlignment.Left, VerticalAlignment.Bottom);
			CreateThumb(HorizontalAlignment.Right, VerticalAlignment.Bottom);
		}
		
		void CreateThumb(HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment)
		{
			ResizeThumb resizeThumb = new ResizeThumb();
			AdornerPanel.SetPlacement(resizeThumb, new RelativePlacement(horizontalAlignment, verticalAlignment));
			adornerPanel.Children.Add(resizeThumb);
			
			resizeThumb.DragStarted   += OnDragStarted;
			resizeThumb.DragDelta     += OnDragDelta(horizontalAlignment, verticalAlignment);
			resizeThumb.DragCompleted += OnDragCompleted(horizontalAlignment, verticalAlignment);
		}
		
		void OnDragStarted(object sender, DragStartedEventArgs e)
		{
			if (dragFrame == null)
				dragFrame = new DragFrame();
			
			if (this.ExtendedItem.Parent != null) {
				GrayOutDesignerExceptActiveArea.Start(ref grayOut, this.Services.GetService<IDesignPanel>(), this.ExtendedItem.Parent.View);
			}
			
			AdornerPanel.SetPlacement(dragFrame, Placement.FillContent);
			adornerPanel.Children.Add(dragFrame);
			adornerPanel.Cursor = Cursors.SizeNWSE;
//			newSize.Width = double.IsNaN(component.Width) ? component.ActualWidth : component.Width;
//			newSize.Height = double.IsNaN(component.Height) ? component.ActualHeight : component.Height;
		}
		
		DragDeltaEventHandler OnDragDelta(HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment)
		{
			return delegate(object sender, DragDeltaEventArgs e) {
				if (resizeBehavior != null) {
					Placement p = resizeBehavior.GetPlacement(this.ExtendedItem,
					                                          FixChange(e.HorizontalChange, horizontalAlignment),
					                                          FixChange(e.VerticalChange, verticalAlignment),
					                                          horizontalAlignment, verticalAlignment);
					if (p != null) {
						AdornerPanel.SetPlacement(dragFrame, p);
					}
				}
			};
		}
		
		DragCompletedEventHandler OnDragCompleted(HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment)
		{
			return delegate (object sender, DragCompletedEventArgs e) {
				adornerPanel.Children.Remove(dragFrame);
				adornerPanel.ClearValue(AdornerPanel.CursorProperty);
				GrayOutDesignerExceptActiveArea.Stop(ref grayOut, this.Services.GetService<IDesignPanel>());
				
				if (e.Canceled == false && resizeBehavior != null) {
					using (ChangeGroup group = this.ExtendedItem.OpenGroup("Resize")) {
						resizeBehavior.Resize(this.ExtendedItem,
						                      FixChange(e.HorizontalChange, horizontalAlignment),
						                      FixChange(e.VerticalChange, verticalAlignment),
						                      horizontalAlignment, verticalAlignment);
						group.Commit();
					}
				}
			};
		}
		
		double FixChange(double val, VerticalAlignment va)
		{
			if (va == VerticalAlignment.Bottom)
				return val;
			else if (va == VerticalAlignment.Top)
				return -val;
			else
				return 0;
		}
		
		double FixChange(double val, HorizontalAlignment ha)
		{
			if (ha == HorizontalAlignment.Right)
				return val;
			else if (ha == HorizontalAlignment.Left)
				return -val;
			else
				return 0;
		}
		
		/// <summary/>
		protected override void OnInitialized()
		{
			base.OnInitialized();
			this.ExtendedItem.PropertyChanged += OnPropertyChanged;
			this.Services.Selection.PrimarySelectionChanged += OnPrimarySelectionChanged;
			
			DesignItem parentItem = this.ExtendedItem.Parent;
			if (parentItem == null) // resizing the root element
				resizeBehavior = RootElementResizeSupport.Instance;
			else
				resizeBehavior = parentItem.GetBehavior<IChildResizeSupport>();
			
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
			if (resizeBehavior == null || !resizeBehavior.CanResizeChild(this.ExtendedItem)) {
				if (this.Adorners.Count == 1) {
					this.Adorners.Clear();
				}
			} else {
				if (this.Adorners.Count == 0) {
					this.Adorners.Add(adornerPanel);
				}
			}
		}
	}
}
