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

namespace ICSharpCode.WpfDesign.Designer.Controls.TypeEditors.BrushEditor
{
	public class NumericUpDown : RangeBase
	{
		static NumericUpDown()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericUpDown),
			                                         new FrameworkPropertyMetadata(typeof(NumericUpDown)));
		}

		TextBox textBox;
		DragRepeatButton upButton;
		DragRepeatButton downButton;

		bool IsDragging
		{
			get
			{
				return upButton.IsDragging;
			}
			set
			{
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

			upDrag.Changed += drag_Changed;
			downDrag.Changed += drag_Changed;
			upDrag.Completed += drag_Completed;
			downDrag.Completed += drag_Completed;

			Print();
		}

		void drag_Changed(DragListener drag)
		{
			IsDragging = true;
			UpdateValue(-drag.DeltaDelta.Y);
		}

		void drag_Completed(DragListener drag)
		{
			IsDragging = false;
		}

		void downButton_Click(object sender, RoutedEventArgs e)
		{
			if (!IsDragging) SmallDown();
		}

		void upButton_Click(object sender, RoutedEventArgs e)
		{
			if (!IsDragging) SmallUp();
		}

		public void SmallUp()
		{
			UpdateValue(SmallChange);
		}

		public void SmallDown()
		{
			UpdateValue(-SmallChange);
		}

		public void LargeUp()
		{
			UpdateValue(LargeChange);
		}

		public void LargeDown()
		{
			UpdateValue(-LargeChange);
		}

		public void Minimize()
		{
			Parse();
			Value = Minimum;
		}

		public void Maximize()
		{
			Parse();
			Value = Maximum;
		}

		void UpdateValue(double delta)
		{
			Parse();
			SetValue(Value + delta);
		}

		void Parse()
		{
			double result;
			if (double.TryParse(textBox.Text, out result))
			{
				SetValue(result);
			}
			else
			{
				Print();
			}
		}

		void Print()
		{
			if (textBox != null)
			{
				textBox.Text = Value.ToString();
				textBox.CaretIndex = int.MaxValue;
			}
		}

		//wpf bug?: Value = -1 updates bindings without coercing
		//workaround
		void SetValue(double newValue)
		{
			newValue = (double)Math.Max(Minimum, Math.Min(newValue, Maximum));
			if (Value != newValue)
			{
				Value = newValue;
			}
		}

		protected override void OnValueChanged(double oldValue, double newValue)
		{
			base.OnValueChanged(oldValue, newValue);
			Print();
		}

		protected override void OnPreviewKeyDown(KeyEventArgs e)
		{
			base.OnPreviewKeyDown(e);
			if (e.Key == Key.Enter)
			{
				Parse();
				textBox.SelectAll();
				e.Handled = true;
			}
			else if (e.Key == Key.Up)
			{
				SmallUp();
				e.Handled = true;
			}
			else if (e.Key == Key.Down)
			{
				SmallDown();
				e.Handled = true;
			}
			else if (e.Key == Key.PageUp)
			{
				LargeUp();
				e.Handled = true;
			}
			else if (e.Key == Key.PageDown)
			{
				LargeDown();
				e.Handled = true;
			}
			else if (e.Key == Key.Home)
			{
				Maximize();
				e.Handled = true;
			}
			else if (e.Key == Key.End)
			{
				Minimize();
				e.Handled = true;
			}
		}

		protected override void OnMouseWheel(MouseWheelEventArgs e)
		{
			if (e.Delta > 0)
			{
				if (Keyboard.IsKeyDown(Key.LeftShift))
				{
					LargeUp();
				}
				else
				{
					SmallUp();
				}
			}
			else
			{
				if (Keyboard.IsKeyDown(Key.LeftShift))
				{
					LargeDown();
				}
				else
				{
					SmallDown();
				}
			}
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);
			if (e.Property == SmallChangeProperty &&
			    ReadLocalValue(LargeChangeProperty) == DependencyProperty.UnsetValue)
			{
				LargeChange = SmallChange * 10;
			}
		}
	}

	public class DragRepeatButton : RepeatButton
	{
		public static readonly DependencyProperty IsDraggingProperty =
			DependencyProperty.Register("IsDragging", typeof(bool), typeof(DragRepeatButton));

		public bool IsDragging
		{
			get { return (bool)GetValue(IsDraggingProperty); }
			set { SetValue(IsDraggingProperty, value); }
		}
	}
}
