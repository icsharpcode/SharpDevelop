// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
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
	public sealed class RotateThumbExtension : SelectionAdornerProvider
	{
		readonly AdornerPanel adornerPanel;
		readonly Thumb thumb;
		/// <summary>An array containing this.ExtendedItem as only element</summary>
		readonly DesignItem[] extendedItemArray = new DesignItem[1];
		IPlacementBehavior resizeBehavior;
		PlacementOperation operation;
		
		public RotateThumbExtension()
		{
			adornerPanel = new AdornerPanel();
			adornerPanel.Order = AdornerOrder.Foreground;
			this.Adorners.Add(adornerPanel);
			
			thumb = CreateRotateThumb();
		}
		
		ResizeThumb CreateRotateThumb()
		{
			ResizeThumb rotateThumb = new RotateThumb();
			rotateThumb.Cursor = Cursors.Hand;
			rotateThumb.Cursor = ZoomControl.GetCursor("Images/rotate.cur");
			rotateThumb.Alignment = PlacementAlignment.Top;
			AdornerPanel.SetPlacement(rotateThumb,
			                          new RelativePlacement(HorizontalAlignment.Center, VerticalAlignment.Top) { WidthRelativeToContentWidth = 1, HeightOffset = 0 });
			adornerPanel.Children.Add(rotateThumb);

			DragListener drag = new DragListener(rotateThumb);
			drag.Started += drag_Rotate_Started;
			drag.Changed += drag_Rotate_Changed;
			return rotateThumb;
		}
		
		#region Rotate
		
		private Point centerPoint;
		private UIElement parent;
		private Vector startVector;
		private RotateTransform rotateTransform;
		private double initialAngle;
		private DesignItem rtTransform;
		private double angle;

		private void drag_Rotate_Started(DragListener drag)
		{
			var designerItem = this.ExtendedItem.Component as FrameworkElement;
			this.parent = VisualTreeHelper.GetParent(designerItem) as UIElement;
			this.centerPoint = designerItem.TranslatePoint(
				new Point(designerItem.ActualWidth*designerItem.RenderTransformOrigin.X,
				          designerItem.ActualHeight*designerItem.RenderTransformOrigin.Y),
				this.parent);

			Point startPoint = Mouse.GetPosition(this.parent);
			this.startVector = Point.Subtract(startPoint, this.centerPoint);

			if (this.rotateTransform == null)
			{
				this.initialAngle = 0;
			}
			else
			{
				this.initialAngle = this.rotateTransform.Angle;
			}

			rtTransform = this.ExtendedItem.Properties[FrameworkElement.RenderTransformProperty].Value;

			operation = PlacementOperation.Start(extendedItemArray, PlacementType.Resize);
		}

		private void drag_Rotate_Changed(DragListener drag)
		{
			Point currentPoint = Mouse.GetPosition(this.parent);
			Vector deltaVector = Point.Subtract(currentPoint, this.centerPoint);

			double angle = Vector.AngleBetween(this.startVector, deltaVector);

			var destAngle = this.initialAngle + Math.Round(angle, 0);

			if (!Keyboard.IsKeyDown(Key.LeftCtrl))
				destAngle = ((int)destAngle / 15) * 15;

			if (destAngle == 0)
			{
				this.ExtendedItem.Properties.GetProperty(FrameworkElement.RenderTransformProperty).Reset();
				rtTransform = null;
				rotateTransform = null;
			}
			else
			{
				if (rtTransform == null)
				{
					if (!this.ExtendedItem.Properties.GetProperty(FrameworkElement.RenderTransformOriginProperty).IsSet) {
						this.ExtendedItem.Properties.GetProperty(FrameworkElement.RenderTransformOriginProperty).SetValue(new Point(0.5,0.5));
					}
					
					if (this.rotateTransform == null)
						this.rotateTransform = new RotateTransform(0);
					this.ExtendedItem.Properties.GetProperty(FrameworkElement.RenderTransformProperty).SetValue(rotateTransform);
					rtTransform = this.ExtendedItem.Properties[FrameworkElement.RenderTransformProperty].Value;
				}
				rtTransform.Properties["Angle"].SetValue(destAngle);
				this.angle = destAngle * Math.PI / 180.0;
			}
		}

		#endregion
		
		protected override void OnInitialized()
		{
			if (this.ExtendedItem.Component is WindowClone)
				return;
			base.OnInitialized();
			extendedItemArray[0] = this.ExtendedItem;
			this.ExtendedItem.PropertyChanged += OnPropertyChanged;
			this.Services.Selection.PrimarySelectionChanged += OnPrimarySelectionChanged;
			resizeBehavior = PlacementOperation.GetPlacementBehavior(extendedItemArray);
			OnPrimarySelectionChanged(null, null);
			
			var designerItem = this.ExtendedItem.Component as FrameworkElement;
			this.rotateTransform = designerItem.RenderTransform as RotateTransform;
			
			if (rotateTransform != null)
				angle = rotateTransform.Angle;
		}
		
		void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{ }
		
		protected override void OnRemove()
		{
			this.ExtendedItem.PropertyChanged -= OnPropertyChanged;
			this.Services.Selection.PrimarySelectionChanged -= OnPrimarySelectionChanged;
			base.OnRemove();
		}
		
		void OnPrimarySelectionChanged(object sender, EventArgs e)
		{
			bool isPrimarySelection = this.Services.Selection.PrimarySelection == this.ExtendedItem;
			foreach (RotateThumb g in adornerPanel.Children) {
				g.IsPrimarySelection = isPrimarySelection;
			}
		}
	}
}
