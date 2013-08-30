// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using ICSharpCode.WpfDesign.Adorners;
using ICSharpCode.WpfDesign.Extensions;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Input;
using ICSharpCode.WpfDesign.Designer.Controls;
using System.Windows.Media;

namespace ICSharpCode.WpfDesign.Designer.Extensions
{
	[ExtensionFor(typeof(FrameworkElement))]
	public class RenderTransformOriginExtension : SelectionAdornerProvider
	{
		readonly AdornerPanel adornerPanel;
		RenderTransformOriginThumb renderTransformOriginThumb;
		/// <summary>An array containing this.ExtendedItem as only element</summary>
		readonly DesignItem[] extendedItemArray = new DesignItem[1];
		IPlacementBehavior resizeBehavior;
		PlacementOperation operation;
		ChangeGroup changeGroup;
		
		public RenderTransformOriginExtension()
		{
			adornerPanel = new AdornerPanel();
			adornerPanel.Order = AdornerOrder.Foreground;
			this.Adorners.Add(adornerPanel);
			
			CreateRenderTransformOriginThumb();
		}
		
		void CreateRenderTransformOriginThumb()
		{
			renderTransformOriginThumb = new RenderTransformOriginThumb();
			renderTransformOriginThumb.Cursor = Cursors.Hand;
			
			AdornerPanel.SetPlacement(renderTransformOriginThumb,
			                          new RelativePlacement(HorizontalAlignment.Left, VerticalAlignment.Top){XRelativeToContentWidth = renderTransformOrigin.X, YRelativeToContentHeight = renderTransformOrigin.Y});
			adornerPanel.Children.Add(renderTransformOriginThumb);

			renderTransformOriginThumb.DragDelta += new DragDeltaEventHandler(renderTransformOriginThumb_DragDelta);
			renderTransformOriginThumb.DragCompleted += new DragCompletedEventHandler(renderTransformOriginThumb_DragCompleted);
		}

		void renderTransformOriginThumb_DragCompleted(object sender, DragCompletedEventArgs e)
		{
			this.ExtendedItem.Properties.GetProperty(FrameworkElement.RenderTransformOriginProperty).SetValue(new Point(Math.Round(renderTransformOrigin.X, 4), Math.Round(renderTransformOrigin.Y, 4)));
		}
		
		void renderTransformOriginThumb_DragDelta(object sender, DragDeltaEventArgs e)
		{
			var p = AdornerPanel.GetPlacement(renderTransformOriginThumb) as RelativePlacement;
			var pointAbs = adornerPanel.RelativeToAbsolute(new Vector(p.XRelativeToContentWidth, p.YRelativeToContentHeight));
			var pointAbsNew = pointAbs + new Vector(e.HorizontalChange, e.VerticalChange);
			var pRel = adornerPanel.AbsoluteToRelative(pointAbsNew);
			renderTransformOrigin = new Point(pRel.X, pRel.Y);
			
			this.ExtendedItem.View.SetValue(FrameworkElement.RenderTransformOriginProperty, renderTransformOrigin);
			//this.ExtendedItem.Properties.GetProperty(FrameworkElement.RenderTransformOriginProperty).SetValue(new Point(Math.Round(pRel.X, 4), Math.Round(pRel.Y, 4)));
		}
		
		Point renderTransformOrigin = new Point(0.5, 0.5);
		
		DependencyPropertyDescriptor renderTransformOriginPropertyDescriptor;
		
		protected override void OnInitialized()
		{
			base.OnInitialized();
			extendedItemArray[0] = this.ExtendedItem;
			this.ExtendedItem.PropertyChanged += OnPropertyChanged;
			
			if (this.ExtendedItem.Properties.GetProperty(FrameworkElement.RenderTransformOriginProperty).IsSet) {
				renderTransformOrigin = (Point)this.ExtendedItem.Properties.GetProperty(FrameworkElement.RenderTransformOriginProperty).ValueOnInstance;
			}
			
			AdornerPanel.SetPlacement(renderTransformOriginThumb,
			                          new RelativePlacement(HorizontalAlignment.Left, VerticalAlignment.Top){XRelativeToContentWidth = renderTransformOrigin.X, YRelativeToContentHeight = renderTransformOrigin.Y});
			
			renderTransformOriginPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(FrameworkElement.RenderTransformOriginProperty, typeof(FrameworkElement));
			renderTransformOriginPropertyDescriptor.AddValueChanged(this.ExtendedItem.Component, OnRenderTransformOriginPropertyChanged);
		}
		
		private void OnRenderTransformOriginPropertyChanged(object sender, EventArgs e)
		{
			var pRel = renderTransformOrigin;
			if (this.ExtendedItem.Properties.GetProperty(FrameworkElement.RenderTransformOriginProperty).IsSet)
				pRel = (Point)this.ExtendedItem.Properties.GetProperty(FrameworkElement.RenderTransformOriginProperty).ValueOnInstance;
						
			AdornerPanel.SetPlacement(renderTransformOriginThumb,
			                          new RelativePlacement(HorizontalAlignment.Left, VerticalAlignment.Top){ XRelativeToContentWidth = pRel.X, YRelativeToContentHeight = pRel.Y });
			
		}
		
		void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{ }
		
		protected override void OnRemove()
		{
			renderTransformOriginPropertyDescriptor.RemoveValueChanged(this.ExtendedItem.Component, OnRenderTransformOriginPropertyChanged);
			
			base.OnRemove();
		}
	}
}
