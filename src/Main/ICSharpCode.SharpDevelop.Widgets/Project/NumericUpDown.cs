// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Globalization;
using System.Diagnostics;

namespace ICSharpCode.SharpDevelop.Widgets
{
	public class NumericUpDown : Control
	{
		static NumericUpDown()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericUpDown),
				new FrameworkPropertyMetadata(typeof(NumericUpDown)));
		}
		
		TextBox textBox;
		DragRepeatButton upButton;
		DragRepeatButton downButton;

		public static readonly DependencyProperty DecimalPlacesProperty =
			DependencyProperty.Register("DecimalPlaces", typeof(int), typeof(NumericUpDown));

		public int DecimalPlaces {
			get { return (int)GetValue(DecimalPlacesProperty); }
			set { SetValue(DecimalPlacesProperty, value); }
		}

		public static readonly DependencyProperty MinimumProperty =
			DependencyProperty.Register("Minimum", typeof(double), typeof(NumericUpDown));

		public double Minimum {
			get { return (double)GetValue(MinimumProperty); }
			set { SetValue(MinimumProperty, value); }
		}

		public static readonly DependencyProperty MaximumProperty =
			DependencyProperty.Register("Maximum", typeof(double), typeof(NumericUpDown),
			new FrameworkPropertyMetadata(100.0));

		public double Maximum {
			get { return (double)GetValue(MaximumProperty); }
			set { SetValue(MaximumProperty, value); }
		}

		public static readonly DependencyProperty ValueProperty =
			DependencyProperty.Register("Value", typeof(double), typeof(NumericUpDown),
			new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public double Value {
			get { return (double)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}

		public static readonly DependencyProperty SmallChangeProperty =
			DependencyProperty.Register("SmallChange", typeof(double), typeof(NumericUpDown),
			new FrameworkPropertyMetadata(1.0));

		public double SmallChange {
			get { return (double)GetValue(SmallChangeProperty); }
			set { SetValue(SmallChangeProperty, value); }
		}

		public static readonly DependencyProperty LargeChangeProperty =
			DependencyProperty.Register("LargeChange", typeof(double), typeof(NumericUpDown),
			new FrameworkPropertyMetadata(10.0));

		public double LargeChange {
			get { return (double)GetValue(LargeChangeProperty); }
			set { SetValue(LargeChangeProperty, value); }
		}

		bool IsDragging {
			get {
				return upButton.IsDragging;
			}
			set {
				upButton.IsDragging = value; downButton.IsDragging = value;
			}
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			upButton = (DragRepeatButton)Template.FindName("PART_UpButton", this);
			downButton = (DragRepeatButton)Template.FindName("PART_DownButton", this);
			textBox = (TextBox)Template.FindName("PART_TextBox", this);

			upButton.Click += new RoutedEventHandler(upButton_Click);
			downButton.Click += new RoutedEventHandler(downButton_Click);

			var upDrag = new DragListener(upButton);
			var downDrag = new DragListener(downButton);

			upDrag.Started += drag_Started;
			upDrag.Changed += drag_Changed;
			upDrag.Completed += drag_Completed;

			downDrag.Started += drag_Started;
			downDrag.Changed += drag_Changed;			
			downDrag.Completed += drag_Completed;

			Print();
		}

		void drag_Started(DragListener drag)
		{
			OnDragStarted();
		}

		void drag_Changed(DragListener drag)
		{
			IsDragging = true;
			MoveValue(-drag.DeltaDelta.Y * SmallChange);
		}

		void drag_Completed(DragListener drag)
		{
			IsDragging = false;
			OnDragCompleted();
		}

		void downButton_Click(object sender, RoutedEventArgs e)
		{
			if (!IsDragging) SmallDown();
		}

		void upButton_Click(object sender, RoutedEventArgs e)
		{
			if (!IsDragging) SmallUp();
		}

		protected virtual void OnDragStarted()
		{
		}

		protected virtual void OnDragCompleted()
		{
		}

		public void SmallUp()
		{
			MoveValue(SmallChange);
		}

		public void SmallDown()
		{
			MoveValue(-SmallChange);
		}

		public void LargeUp()
		{
			MoveValue(LargeChange);
		}

		public void LargeDown()
		{
			MoveValue(-LargeChange);
		}

		void MoveValue(double delta)
		{
			double result;
			if (double.IsNaN(Value) || double.IsInfinity(Value)) {
				SetValue(delta);
			}
			else if (double.TryParse(textBox.Text, out result)) {
				SetValue(result + delta);
			}
			else {
				SetValue(Value + delta);
			}
		}

		void Print()
		{
			if (textBox != null) {
				textBox.Text = Value.ToString("F" + DecimalPlaces);
				textBox.CaretIndex = int.MaxValue;
			}
		}

		//wpf bug?: Value = -1 updates bindings without coercing, workaround
		//update: not derived from RangeBase - no problem
		void SetValue(double newValue)
		{
			newValue = CoerceValue(newValue);
			if (Value != newValue) {
				Value = newValue;
			}
		}

		double CoerceValue(double newValue)
		{
			return Math.Max(Minimum, Math.Min(newValue, Maximum));
		}

		protected override void OnPreviewKeyDown(KeyEventArgs e)
		{
			base.OnPreviewKeyDown(e);
			if (e.Key == Key.Enter) {
				double result;
				if (double.TryParse(textBox.Text, out result)) {
					SetValue(result);
				}
				else {
					Print();
				}
				textBox.SelectAll();
				e.Handled = true;
			}
			else if (e.Key == Key.Up) {
				SmallUp();
				e.Handled = true;
			}
			else if (e.Key == Key.Down) {
				SmallDown();
				e.Handled = true;
			}
			else if (e.Key == Key.PageUp) {
				LargeUp();
				e.Handled = true;
			}
			else if (e.Key == Key.PageDown) {
				LargeDown();
				e.Handled = true;
			}
			//else if (e.Key == Key.Home) {
			//    Maximize();
			//    e.Handled = true;
			//}
			//else if (e.Key == Key.End) {
			//    Minimize();
			//    e.Handled = true;
			//}
		}

		//protected override void OnMouseWheel(MouseWheelEventArgs e)
		//{
		//    if (e.Delta > 0)
		//    {
		//        if (Keyboard.IsKeyDown(Key.LeftShift))
		//        {
		//            LargeUp();
		//        }
		//        else
		//        {
		//            SmallUp();
		//        }
		//    }
		//    else
		//    {
		//        if (Keyboard.IsKeyDown(Key.LeftShift))
		//        {
		//            LargeDown();
		//        }
		//        else
		//        {
		//            SmallDown();
		//        }
		//    }
		//    e.Handled = true;
		//}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == ValueProperty) {
				Value = CoerceValue((double)e.NewValue);
				Print();
			}
			else if (e.Property == SmallChangeProperty &&
				ReadLocalValue(LargeChangeProperty) == DependencyProperty.UnsetValue) {
				LargeChange = SmallChange * 10;
			}
		}
	}

	public class DragRepeatButton : RepeatButton
	{
		public static readonly DependencyProperty IsDraggingProperty =
			DependencyProperty.Register("IsDragging", typeof(bool), typeof(DragRepeatButton));

		public bool IsDragging {
			get { return (bool)GetValue(IsDraggingProperty); }
			set { SetValue(IsDraggingProperty, value); }
		}
	}
}
