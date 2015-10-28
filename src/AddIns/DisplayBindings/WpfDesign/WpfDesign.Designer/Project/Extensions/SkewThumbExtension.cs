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
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using ICSharpCode.WpfDesign.Adorners;
using ICSharpCode.WpfDesign.Designer.Controls;
using ICSharpCode.WpfDesign.Extensions;
using System.Collections.Generic;
using ICSharpCode.WpfDesign.UIExtensions;

namespace ICSharpCode.WpfDesign.Designer.Extensions
{
	[ExtensionServer(typeof(OnlyOneItemSelectedExtensionServer))]
	[ExtensionFor(typeof(FrameworkElement))]
	public sealed class SkewThumbExtension : SelectionAdornerProvider
	{
		readonly AdornerPanel adornerPanel;
		readonly DesignItem[] extendedItemArray = new DesignItem[1];
		
		private AdornerLayer _adornerLayer;
		
		public SkewThumbExtension()
		{
			adornerPanel = new AdornerPanel();
			adornerPanel.Order = AdornerOrder.BeforeForeground;
			this.Adorners.Add(adornerPanel);
		}
		
		#region Skew
		
		private Point startPoint;
		private UIElement parent;
		private SkewTransform skewTransform;
		private double skewX;
		private double skewY;
		private DesignItem rtTransform;
		private Thumb thumb1;
		private Thumb thumb2;
		PlacementOperation operation;
		
		private void dragX_Started(DragListener drag)
		{
			_adornerLayer = this.adornerPanel.TryFindParent<AdornerLayer>();
			
			var designerItem = this.ExtendedItem.Component as FrameworkElement;
			this.parent = VisualTreeHelper.GetParent(designerItem) as UIElement;
			
			startPoint = Mouse.GetPosition(this.parent);
			
			if (this.skewTransform == null)
			{
				this.skewX = 0;
				this.skewY = 0;
			}
			else
			{
				this.skewX = this.skewTransform.AngleX;
				this.skewY = this.skewTransform.AngleY;
			}
			
			rtTransform = this.ExtendedItem.Properties[FrameworkElement.RenderTransformProperty].Value;
			
			operation = PlacementOperation.Start(extendedItemArray, PlacementType.Resize);
		}
		
		private void dragX_Changed(DragListener drag)
		{
			Point currentPoint = Mouse.GetPosition(this.parent);
			Vector deltaVector = Point.Subtract(currentPoint, this.startPoint);
			
			var destAngle = (-0.5*deltaVector.X) + skewX;
			
			if (destAngle == 0 && skewY == 0)
			{
				this.ExtendedItem.Properties.GetProperty(FrameworkElement.RenderTransformProperty).Reset();
				rtTransform = null;
				skewTransform = null;
			}
			else
			{
				if ((rtTransform == null) || !(rtTransform.Component is SkewTransform))
				{
					if (!this.ExtendedItem.Properties.GetProperty(FrameworkElement.RenderTransformOriginProperty).IsSet) {
						this.ExtendedItem.Properties.GetProperty(FrameworkElement.RenderTransformOriginProperty).SetValue(new Point(0.5,0.5));
					}
					
					if (this.skewTransform == null)
						this.skewTransform = new SkewTransform(0, 0);
					this.ExtendedItem.Properties.GetProperty(FrameworkElement.RenderTransformProperty).SetValue(skewTransform);
					rtTransform = this.ExtendedItem.Properties[FrameworkElement.RenderTransformProperty].Value;
				}
				rtTransform.Properties["AngleX"].SetValue(destAngle);
			}
			
			_adornerLayer.UpdateAdornersForElement(this.ExtendedItem.View, true);
		}
		
		void dragX_Completed(ICSharpCode.WpfDesign.Designer.Controls.DragListener drag)
		{
			operation.Commit();
		}
		
		private void dragY_Started(DragListener drag)
		{
			_adornerLayer = this.adornerPanel.TryFindParent<AdornerLayer>();
			
			var designerItem = this.ExtendedItem.Component as FrameworkElement;
			this.parent = VisualTreeHelper.GetParent(designerItem) as UIElement;
			
			startPoint = Mouse.GetPosition(this.parent);
			
			if (this.skewTransform == null)
			{
				this.skewX = 0;
				this.skewY = 0;
			}
			else
			{
				this.skewX = this.skewTransform.AngleX;
				this.skewY = this.skewTransform.AngleY;
			}
			
			rtTransform = this.ExtendedItem.Properties[FrameworkElement.RenderTransformProperty].Value;
			
			operation = PlacementOperation.Start(extendedItemArray, PlacementType.Resize);
		}
		
		private void dragY_Changed(DragListener drag)
		{
			Point currentPoint = Mouse.GetPosition(this.parent);
			Vector deltaVector = Point.Subtract(currentPoint, this.startPoint);
			
			var destAngle = (-0.5*deltaVector.Y) + skewY;
			
			if (destAngle == 0 && skewX == 0)
			{
				this.ExtendedItem.Properties.GetProperty(FrameworkElement.RenderTransformProperty).Reset();
				rtTransform = null;
				skewTransform = null;
			}
			else
			{
				if (rtTransform == null)
				{
					if (!this.ExtendedItem.Properties.GetProperty(FrameworkElement.RenderTransformOriginProperty).IsSet)
					{
						this.ExtendedItem.Properties.GetProperty(FrameworkElement.RenderTransformOriginProperty).SetValue(new Point(0.5, 0.5));
					}
					
					if (this.skewTransform == null)
						this.skewTransform = new SkewTransform(0, 0);
					this.ExtendedItem.Properties.GetProperty(FrameworkElement.RenderTransformProperty).SetValue(skewTransform);
					rtTransform = this.ExtendedItem.Properties[FrameworkElement.RenderTransformProperty].Value;
				}
				rtTransform.Properties["AngleY"].SetValue(destAngle);
			}
			
			_adornerLayer.UpdateAdornersForElement(this.ExtendedItem.View, true);
		}
		
		void dragY_Completed(ICSharpCode.WpfDesign.Designer.Controls.DragListener drag)
		{
			operation.Commit();
		}
		
		#endregion
		
		protected override void OnInitialized()
		{
			
			if (this.ExtendedItem.Component is WindowClone)
				return;
			base.OnInitialized();
			
			extendedItemArray[0] = this.ExtendedItem;
			this.ExtendedItem.PropertyChanged += OnPropertyChanged;
			
			var designerItem = this.ExtendedItem.Component as FrameworkElement;
			this.skewTransform = designerItem.RenderTransform as SkewTransform;
			
			if (skewTransform != null)
			{
				skewX = skewTransform.AngleX;
				skewY = skewTransform.AngleY;
			}
			
			thumb1 = new Thumb() { Cursor = Cursors.ScrollWE, Height = 14, Width = 4, Opacity = 1 };
			thumb2 = new Thumb() { Cursor = Cursors.ScrollNS, Width = 14, Height = 4, Opacity = 1 };
			
			OnPropertyChanged(null, null);
			
			adornerPanel.Children.Add(thumb1);
			adornerPanel.Children.Add(thumb2);
			
			DragListener drag1 = new DragListener(thumb1);
			drag1.Started += dragX_Started;
			drag1.Changed += dragX_Changed;
			drag1.Completed += dragX_Completed;
			DragListener drag2 = new DragListener(thumb2);
			drag2.Started += dragY_Started;
			drag2.Changed += dragY_Changed;
			drag2.Completed += dragY_Completed;
		}
		
		void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (sender == null || e.PropertyName == "Width" || e.PropertyName == "Height") {
				AdornerPanel.SetPlacement(thumb1,
				                          new RelativePlacement(HorizontalAlignment.Center, VerticalAlignment.Top) {
				                          	YOffset = 0,
				                          	XOffset = -1 * PlacementOperation.GetRealElementSize(ExtendedItem.View).Width / 4
				                          });
				
				AdornerPanel.SetPlacement(thumb2,
				                          new RelativePlacement(HorizontalAlignment.Left, VerticalAlignment.Center) {
				                          	YOffset = -1 * PlacementOperation.GetRealElementSize(ExtendedItem.View).Height / 4,
				                          	XOffset = 0
				                          });

				var designPanel = this.ExtendedItem.Services.DesignPanel as DesignPanel;
				if (designPanel != null)
					designPanel.AdornerLayer.UpdateAdornersForElement(this.ExtendedItem.View, true);
			}
		}
		
		protected override void OnRemove()
		{
			this.ExtendedItem.PropertyChanged -= OnPropertyChanged;
			base.OnRemove();
		}
	}
}
