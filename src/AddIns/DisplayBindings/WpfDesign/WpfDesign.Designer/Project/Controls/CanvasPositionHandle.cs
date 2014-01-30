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
using System.Windows.Input;
using System.Globalization;
using System.ComponentModel;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using ICSharpCode.WpfDesign.Adorners;

namespace ICSharpCode.WpfDesign.Designer.Controls
{
	/// <summary>
	/// Adorner that displays the margin of a control in a Grid.
	/// </summary>
	public class CanvasPositionHandle : MarginHandle
	{
		static CanvasPositionHandle()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(CanvasPositionHandle), new FrameworkPropertyMetadata(typeof(CanvasPositionHandle)));
			HandleLengthOffset=2;
		}

		private Path line1;
		private Path line2;
		public override void OnApplyTemplate()
		{
			line1 = GetTemplateChild("line1") as Path;
			line2 = GetTemplateChild("line2") as Path;

			base.OnApplyTemplate();
		}

		readonly Canvas canvas;
		readonly DesignItem adornedControlItem;
		readonly AdornerPanel adornerPanel;
		readonly HandleOrientation orientation;
		readonly FrameworkElement adornedControl;

		/// <summary> This grid contains the handle line and the endarrow.</summary>
//		Grid lineArrow;

		private DependencyPropertyDescriptor leftDescriptor;
		private DependencyPropertyDescriptor rightDescriptor;
		private DependencyPropertyDescriptor topDescriptor;
		private DependencyPropertyDescriptor bottomDescriptor;
		private DependencyPropertyDescriptor widthDescriptor;
		private DependencyPropertyDescriptor heightDescriptor;

		public CanvasPositionHandle(DesignItem adornedControlItem, AdornerPanel adornerPanel, HandleOrientation orientation)
		{
			Debug.Assert(adornedControlItem != null);
			this.adornedControlItem = adornedControlItem;
			this.adornerPanel = adornerPanel;
			this.orientation = orientation;

			Angle = (double) orientation;
			
			canvas = (Canvas) adornedControlItem.Parent.Component;
			adornedControl = (FrameworkElement) adornedControlItem.Component;
			Stub = new MarginStub(this);
			ShouldBeVisible = true;

			leftDescriptor = DependencyPropertyDescriptor.FromProperty(Canvas.LeftProperty,
			                                                           adornedControlItem.Component.GetType());
			leftDescriptor.AddValueChanged(adornedControl, OnPropertyChanged);
			rightDescriptor = DependencyPropertyDescriptor.FromProperty(Canvas.RightProperty,
			                                                            adornedControlItem.Component.GetType());
			rightDescriptor.AddValueChanged(adornedControl, OnPropertyChanged);
			topDescriptor = DependencyPropertyDescriptor.FromProperty(Canvas.TopProperty,
			                                                          adornedControlItem.Component.GetType());
			topDescriptor.AddValueChanged(adornedControl, OnPropertyChanged);
			bottomDescriptor = DependencyPropertyDescriptor.FromProperty(Canvas.BottomProperty,
			                                                             adornedControlItem.Component.GetType());
			bottomDescriptor.AddValueChanged(adornedControl, OnPropertyChanged);
			widthDescriptor = DependencyPropertyDescriptor.FromProperty(Control.WidthProperty,
			                                                            adornedControlItem.Component.GetType());
			widthDescriptor.AddValueChanged(adornedControl, OnPropertyChanged);
			heightDescriptor = DependencyPropertyDescriptor.FromProperty(Control.WidthProperty,
			                                                             adornedControlItem.Component.GetType());
			heightDescriptor.AddValueChanged(adornedControl, OnPropertyChanged);
			BindAndPlaceHandle();
		}


		void OnPropertyChanged(object sender, EventArgs e)
		{
			BindAndPlaceHandle();
		}

		/// <summary>
		/// Gets/Sets the angle by which the Canvas display has to be rotated
		/// </summary>
		public override double TextTransform
		{
			get
			{
				if ((double)orientation == 90 || (double)orientation == 180)
					return 180;
				if ((double)orientation == 270)
					return 0;
				return (double)orientation;
			}
			set { }
		}
		
		/// <summary>
		/// Binds the <see cref="MarginHandle.HandleLength"/> to the margin and place the handles.
		/// </summary>
		void BindAndPlaceHandle()
		{
			if (!adornerPanel.Children.Contains(this))
				adornerPanel.Children.Add(this);
			if (!adornerPanel.Children.Contains(Stub))
				adornerPanel.Children.Add(Stub);
			RelativePlacement placement=new RelativePlacement();
			switch (orientation)
			{
				case HandleOrientation.Left:
					{
						var wr = (double) leftDescriptor.GetValue(adornedControl);
						if (double.IsNaN(wr))
						{
							wr = (double) rightDescriptor.GetValue(adornedControl);
							wr = canvas.ActualWidth - (adornedControl.ActualWidth + wr);
						}
						else
						{
							if (line1 != null)
							{
								line1.StrokeDashArray.Clear();
								line2.StrokeDashArray.Clear();
							}
						}
						this.HandleLength = wr;
						placement = new RelativePlacement(HorizontalAlignment.Left, VerticalAlignment.Center);
						placement.XOffset = -HandleLengthOffset;
						break;
					}
				case HandleOrientation.Top:
					{
						var wr = (double)topDescriptor.GetValue(adornedControl);
						if (double.IsNaN(wr))
						{
							wr = (double)bottomDescriptor.GetValue(adornedControl);
							wr = canvas.ActualHeight - (adornedControl.ActualHeight + wr);
						}
						else
						{
							if (line1 != null)
							{
								line1.StrokeDashArray.Clear();
								line2.StrokeDashArray.Clear();
							}
						}
						this.HandleLength = wr;
						placement = new RelativePlacement(HorizontalAlignment.Center, VerticalAlignment.Top);
						placement.YOffset = -HandleLengthOffset;
						break;
					}
				case HandleOrientation.Right:
					{
						var wr = (double) rightDescriptor.GetValue(adornedControl);
						if (double.IsNaN(wr))
						{
							wr = (double) leftDescriptor.GetValue(adornedControl);
							wr = canvas.ActualWidth - (adornedControl.ActualWidth + wr);
						}
						else
						{
							if (line1 != null)
							{
								line1.StrokeDashArray.Clear();
								line2.StrokeDashArray.Clear();
							}
						}
						this.HandleLength = wr;
						placement = new RelativePlacement(HorizontalAlignment.Right, VerticalAlignment.Center);
						placement.XOffset = HandleLengthOffset;
						break;
					}
				case HandleOrientation.Bottom:
					{
						var wr = (double)bottomDescriptor.GetValue(adornedControl);
						if (double.IsNaN(wr))
						{
							wr = (double)topDescriptor.GetValue(adornedControl);
							wr = canvas.ActualHeight - (adornedControl.ActualHeight + wr);
						}
						else
						{
							if (line1 != null)
							{
								line1.StrokeDashArray.Clear();
								line2.StrokeDashArray.Clear();
							}
						}
						this.HandleLength = wr;
						placement = new RelativePlacement(HorizontalAlignment.Center, VerticalAlignment.Bottom);
						placement.YOffset = HandleLengthOffset;
						break;
					}
			}

			AdornerPanel.SetPlacement(this, placement);
			this.Visibility = Visibility.Visible;
		}
	}
}
