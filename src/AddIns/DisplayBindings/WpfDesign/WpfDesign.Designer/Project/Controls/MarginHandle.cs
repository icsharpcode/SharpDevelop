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

using ICSharpCode.WpfDesign.Adorners;

namespace ICSharpCode.WpfDesign.Designer.Controls
{
	/// <summary>
	/// Adorner that displays the margin of a control in a Grid.
	/// </summary>
	public class MarginHandle : Control
	{
		/// <summary>
		/// Places the Handle with a certain offset so the Handle does not interfere with selection outline.
		/// </summary>
		public static double HandleLengthOffset;
		
		static MarginHandle()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(MarginHandle), new FrameworkPropertyMetadata(typeof(MarginHandle)));
			HandleLengthOffset=2;
		}

		/// <summary>
		/// Dependency property for <see cref="HandleLength"/>.
		/// </summary>
		public static readonly DependencyProperty HandleLengthProperty
			= DependencyProperty.Register("HandleLength", typeof(double), typeof(MarginHandle), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnHandleLengthChanged)));

		/// <summary>
		/// Gets/Sets the length of Margin Handle.
		/// </summary>
		public double HandleLength{
			get { return (double)GetValue(HandleLengthProperty); }
			set { SetValue(HandleLengthProperty, value); }
		}
		
		readonly Grid grid;
		readonly DesignItem adornedControlItem;
		readonly AdornerPanel adornerPanel;
		readonly HandleOrientation orientation;
		readonly FrameworkElement adornedControl;

		/// <summary> This grid contains the handle line and the endarrow.</summary>
		Grid lineArrow;
		
		/// <summary>
		/// Gets the Stub for this handle
		/// </summary>
		public MarginStub Stub {get; protected set;}
		
		/// <summary>
		/// Gets/Sets the angle by which handle rotates.
		/// </summary>
		public double Angle { get; set; }
		
		/// <summary>
		/// Gets/Sets the angle by which the Margin display has to be rotated
		/// </summary>
		public virtual double TextTransform{
			get{
				if((double)orientation==90 || (double)orientation == 180)
					return 180;
				if ((double)orientation == 270)
					return 0;
				return (double)orientation;
			}
			set{ }
		}
		
		/// <summary>
		/// Decides the visiblity of handle/stub when <see cref="HandleLength"/> changes
		/// </summary>
		/// <param name="d"></param>
		/// <param name="e"></param>
		public static void OnHandleLengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			MarginHandle mar=(MarginHandle)d;
			mar.DecideVisiblity((double)e.NewValue);
		}
		
		/// <summary>
		/// Decides whether to permanently display the handle or not.
		/// </summary>
		public bool ShouldBeVisible { get; set; }
		
		/// <summary>
		/// Decides whether stub has to be only displayed.
		/// </summary>
		public bool DisplayOnlyStub { get; set; }
		
		/// <summary>
		/// Gets the orientation of the handle.
		/// </summary>
		public HandleOrientation Orientation {
			get { return orientation; }
		}

		protected MarginHandle()
		{ }
		 
		public MarginHandle(DesignItem adornedControlItem, AdornerPanel adornerPanel, HandleOrientation orientation)
		{
			Debug.Assert(adornedControlItem!=null);
			this.adornedControlItem = adornedControlItem;
			this.adornerPanel = adornerPanel;
			this.orientation = orientation;
			Angle = (double)orientation;
			grid=(Grid)adornedControlItem.Parent.Component;
			adornedControl=(FrameworkElement)adornedControlItem.Component;
			Stub = new MarginStub(this);
			ShouldBeVisible=true;
			BindAndPlaceHandle();
			
			adornedControlItem.PropertyChanged += OnPropertyChanged;
			OnPropertyChanged(this.adornedControlItem, new PropertyChangedEventArgs("HorizontalAlignment"));
			OnPropertyChanged(this.adornedControlItem, new PropertyChangedEventArgs("VerticalAlignment"));
		}

		/// <summary>
		/// Binds the <see cref="HandleLength"/> to the margin and place the handles.
		/// </summary>
		void BindAndPlaceHandle()
		{
			if (!adornerPanel.Children.Contains(this))
				adornerPanel.Children.Add(this);
			if (!adornerPanel.Children.Contains(Stub))
				adornerPanel.Children.Add(Stub);
			RelativePlacement placement=new RelativePlacement();
			Binding binding = new Binding();
			binding.Source = adornedControl;
			switch (orientation)
			{
				case HandleOrientation.Left:
					binding.Path = new PropertyPath("Margin.Left");
					placement = new RelativePlacement(HorizontalAlignment.Left, VerticalAlignment.Center);
					placement.XOffset=-HandleLengthOffset;
					break;
				case HandleOrientation.Top:
					binding.Path = new PropertyPath("Margin.Top");
					placement = new RelativePlacement(HorizontalAlignment.Center, VerticalAlignment.Top);
					placement.YOffset=-HandleLengthOffset;
					break;
				case HandleOrientation.Right:
					binding.Path = new PropertyPath("Margin.Right");
					placement = new RelativePlacement(HorizontalAlignment.Right, VerticalAlignment.Center);
					placement.XOffset=HandleLengthOffset;
					break;
				case HandleOrientation.Bottom:
					binding.Path = new PropertyPath("Margin.Bottom");
					placement = new RelativePlacement(HorizontalAlignment.Center, VerticalAlignment.Bottom);
					placement.YOffset=HandleLengthOffset;
					break;
			}

			binding.Mode = BindingMode.TwoWay;
			SetBinding(HandleLengthProperty, binding);

			AdornerPanel.SetPlacement(this, placement);
			AdornerPanel.SetPlacement(Stub, placement);
			
			DecideVisiblity(this.HandleLength);
		}

		/// <summary>
		/// Decides the visibllity of Handle or stub,whichever is set and hides the line-endarrow if the control is near the Grid or goes out of it.
		/// </summary>		
		public void DecideVisiblity(double handleLength)
		{
			if (ShouldBeVisible)
			{
				if (!DisplayOnlyStub)
				{
					this.Visibility = handleLength != 0.0 ? Visibility.Visible : Visibility.Hidden;
					if (this.lineArrow != null)
					{
						lineArrow.Visibility = handleLength < 25 ? Visibility.Hidden : Visibility.Visible;
					}
					Stub.Visibility = this.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
				}
				else
				{
					this.Visibility = Visibility.Hidden;
					Stub.Visibility = Visibility.Visible;
				}
			}
			else {
				this.Visibility = Visibility.Hidden;
				Stub.Visibility = Visibility.Hidden;
			}
		}
		
		void OnPropertyChanged(object sender,PropertyChangedEventArgs e)
		{
			if(e.PropertyName=="HorizontalAlignment" && (orientation==HandleOrientation.Left || orientation==HandleOrientation.Right)) {
				var ha = (HorizontalAlignment) adornedControlItem.Properties[FrameworkElement.HorizontalAlignmentProperty].ValueOnInstance;
				if(ha==HorizontalAlignment.Stretch) {
					DisplayOnlyStub = false;
				}else if(ha==HorizontalAlignment.Center) {
					DisplayOnlyStub = true;
				} else
					DisplayOnlyStub = ha.ToString() != orientation.ToString();
			}

			if(e.PropertyName=="VerticalAlignment" && (orientation==HandleOrientation.Top || orientation==HandleOrientation.Bottom)) {
				var va = (VerticalAlignment)adornedControlItem.Properties[FrameworkElement.VerticalAlignmentProperty].ValueOnInstance;

				if(va==VerticalAlignment.Stretch) {
					DisplayOnlyStub = false;
				} else if(va==VerticalAlignment.Center) {
					DisplayOnlyStub = true;
				} else
					DisplayOnlyStub = va.ToString() != orientation.ToString();
			}
			DecideVisiblity(this.HandleLength);
		}
		
		protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e)
		{
			base.OnMouseEnter(e);
			this.Cursor = Cursors.Hand;
		}

		protected override void OnMouseLeave(MouseEventArgs e)
		{
			base.OnMouseLeave(e);
			this.Cursor = Cursors.Arrow;
		}
		
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			lineArrow = new Grid();
			lineArrow = (Grid)Template.FindName("lineArrow", this) as Grid;
			Debug.Assert(lineArrow != null);
		}

	}

	/// <summary>
	/// Display a stub indicating that the margin is not set.
	/// </summary>
	public class MarginStub : Control
	{
		MarginHandle marginHandle;
		
		/// <summary>
		/// Gets the margin handle using this stub.
		/// </summary>
		public MarginHandle Handle{
			get { return marginHandle; }
		}
		
		static MarginStub()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(MarginStub), new FrameworkPropertyMetadata(typeof(MarginStub)));
		}

		public MarginStub(MarginHandle marginHandle)
		{
			this.marginHandle = marginHandle;
		}

		protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
		{
			base.OnMouseLeftButtonDown(e);
			marginHandle.DecideVisiblity(marginHandle.HandleLength);
		}
		
		protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e)
		{
			base.OnMouseEnter(e);
			this.Cursor = Cursors.Hand;
		}
		
		protected override void OnMouseLeave(MouseEventArgs e)
		{
			base.OnMouseLeave(e);
			this.Cursor = Cursors.Arrow;
		}
	}

	/// <summary>
	/// Specifies the Handle orientation
	/// </summary>
	public enum HandleOrientation
	{
		/*  Rotation of the handle is done with respect to right orientation and in clockwise direction*/

		/// <summary>
		/// Indicates that the margin handle is left-oriented and rotated 180 degrees with respect to <see cref="Right"/>.
		/// </summary>
		Left = 180,
		/// <summary>
		/// Indicates that the margin handle is top-oriented and rotated 270 degrees with respect to <see cref="Right"/>.
		/// </summary>
		Top = 270,
		/// <summary>
		/// Indicates that the margin handle is right.
		/// </summary>
		Right = 0,
		/// <summary>
		/// Indicates that the margin handle is left-oriented and rotated 180 degrees with respect to <see cref="Right"/>.
		/// </summary>
		Bottom = 90
	}
}

namespace ICSharpCode.WpfDesign.Designer.Controls.Converters
{
	/// <summary>
	/// Offset the Handle Length with MarginHandle.HandleLengthOffset
	/// </summary>
	public class HandleLengthWithOffset : IValueConverter
	{
		public static HandleLengthWithOffset Instance = new HandleLengthWithOffset();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if(value is double) {
				return Math.Max((double)value - MarginHandle.HandleLengthOffset,0);
			}
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}
