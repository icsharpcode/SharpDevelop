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
using System.Globalization;
using SharpDevelop.XamlDesigner.Converters;
using System.Reflection;
using System.Windows.Threading;

namespace SharpDevelop.XamlDesigner.Controls
{
	public class EditSlider : Control
	{
		static EditSlider()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(EditSlider),
				new FrameworkPropertyMetadata(typeof(EditSlider)));
		}

		public EditSlider()
		{
			SizeChanged += new SizeChangedEventHandler(EditSlider_SizeChanged);
		}

		AdvancedThumb thumb;
		FrameworkElement bar;
		TextBox textBox;
		Vector startDelta;
		double startValue;

		BindingExpression currentValueExpression;

		static PropertyInfo sourceValueProperty =
			typeof(BindingExpression).GetProperty("SourceValue", BindingFlags.NonPublic | BindingFlags.Instance);

		public AdvancedThumb Thumb
		{
			get { return thumb; }
		}

		public static readonly DependencyProperty ValueProperty =
			DependencyProperty.Register("Value", typeof(double), typeof(EditSlider),
			new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public double Value
		{
			get { return (double)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}

		public static readonly DependencyProperty MinimumProperty =
			DependencyProperty.Register("Minimum", typeof(double), typeof(EditSlider),
			new FrameworkPropertyMetadata(double.NegativeInfinity));

		public double Minimum
		{
			get { return (double)GetValue(MinimumProperty); }
			set { SetValue(MinimumProperty, value); }
		}

		public static readonly DependencyProperty MaximumProperty =
			DependencyProperty.Register("Maximum", typeof(double), typeof(EditSlider),
			new FrameworkPropertyMetadata(double.PositiveInfinity));

		public double Maximum
		{
			get { return (double)GetValue(MaximumProperty); }
			set { SetValue(MaximumProperty, value); }
		}

		public static readonly DependencyProperty StepProperty =
			DependencyProperty.Register("Step", typeof(double), typeof(EditSlider),
			new FrameworkPropertyMetadata(0.25));

		public double Step
		{
			get { return (double)GetValue(StepProperty); }
			set { SetValue(StepProperty, value); }
		}

		public static readonly DependencyProperty DecimalsProperty =
			DependencyProperty.Register("Decimals", typeof(int), typeof(EditSlider),
			new FrameworkPropertyMetadata(0));

		public int Decimals
		{
			get { return (int)GetValue(DecimalsProperty); }
			set { SetValue(DecimalsProperty, value); }
		}

		public static readonly DependencyProperty InPercentsProperty =
			DependencyProperty.Register("InPercents", typeof(bool), typeof(EditSlider));

		public bool InPercents
		{
			get { return (bool)GetValue(InPercentsProperty); }
			set { SetValue(InPercentsProperty, value); }
		}

		public static readonly DependencyProperty InvisibleValueProperty =
			DependencyProperty.Register("InvisibleValue", typeof(bool), typeof(EditSlider));

		public bool InvisibleValue
		{
			get { return (bool)GetValue(InvisibleValueProperty); }
			set { SetValue(InvisibleValueProperty, value); }
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == MinimumProperty ||
				e.Property == MaximumProperty) {
				SetNewValue(Value);
				UpdateBar();
			}
			else if (e.Property == InPercentsProperty) {
				PrintCurrentValue();
			}
			else if (e.Property == ValueProperty) {
				UpdateCurrentValueExpression();
				UpdateBar();
			}
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			bar = Template.FindName("PART_Bar", this) as FrameworkElement;

			textBox = Template.FindName("PART_TextBox", this) as TextBox;
			textBox.KeyDown += new KeyEventHandler(textBox_KeyDown);
			textBox.GotKeyboardFocus += new KeyboardFocusChangedEventHandler(textBox_GotKeyboardFocus);
			textBox.SetBinding(TextBox.TextProperty, new Binding("Value") {
				Source = this,
				Converter = new TextConverter() { EditSlider = this }
			});

			thumb = Template.FindName("PART_Thumb", this) as AdvancedThumb;
			thumb.Click += new AdvancedDragEventHandler(thumb_Click);
			thumb.DragStarted += new AdvancedDragEventHandler(thumb_DragStarted);
			thumb.DragDelta += new AdvancedDragEventHandler(thumb_DragDelta);
			thumb.DragCompleted += new AdvancedDragEventHandler(thumb_DragCompleted);

			UpdateBar();
		}

		void textBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter) {
				Focusable = true;
				Focus();
				ClearValue(FocusableProperty);
			}
		}

		void textBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			textBox.SelectAll();
		}

		void thumb_Click(object sender, AdvancedDragEventArgs e)
		{
			textBox.Focus();
		}

		void thumb_DragStarted(object sender, AdvancedDragEventArgs e)
		{
			startDelta = e.Delta;
			startValue = Value;
		}

		void thumb_DragDelta(object sender, AdvancedDragEventArgs e)
		{
			var finalDelta = e.Delta - startDelta;
			var newValue = startValue + (finalDelta.X - finalDelta.Y) * Step;
			SetNewValue(Math.Round(newValue, Decimals + (InPercents ? 2 : 0)));
		}

		void thumb_DragCompleted(object sender, AdvancedDragEventArgs e)
		{
			if (e.IsCancel) {
				SetNewValue(startValue);
			}
		}

		void EditSlider_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			UpdateBar();
		}

		void PrintCurrentValue()
		{
			if (textBox != null) {
				var expr = BindingOperations.GetBindingExpression(textBox, TextBox.TextProperty);
				expr.UpdateTarget();
			}
		}

		void UpdateBar()
		{
			if (bar != null) {
				var range = Maximum - Minimum;
				if (range > 0 && range < double.MaxValue) {
					var parentWidth = (bar.Parent as FrameworkElement).ActualWidth;
					bar.Width = parentWidth * (Value - Minimum) / range;
				}
			}
		}

		void UpdateCurrentValueExpression()
		{
			var newExpression = GetBindingExpression(ValueProperty);

			if (currentValueExpression != newExpression) {
				currentValueExpression = newExpression;
				if (currentValueExpression != null) {
					// HACK
					var sourceValue = sourceValueProperty.GetValue(currentValueExpression, null);
					var sourceType = sourceValue != null ? sourceValue.GetType() : null;

					AutoRange(sourceType);
				}
			}

			// if there was exception...
			if (currentValueExpression != null) {
				currentValueExpression.UpdateTarget();
			}
		}

		void AutoRange(Type type)
		{
			if (type == typeof(Byte)) {
				Minimum = Byte.MinValue;
				Maximum = Byte.MaxValue;
			}
			else if (type == typeof(SByte)) {
				Minimum = SByte.MinValue;
				Maximum = SByte.MaxValue;
			}
		}

		void SetNewValue(double newValue)
		{
			newValue = Math.Max(Minimum, Math.Min(Maximum, newValue));
			// NaN also
			if (!object.Equals(Value, newValue)) {
				Value = newValue;
			}
		}

		class TextConverter : IValueConverter
		{
			public EditSlider EditSlider;

			public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
			{
				if (EditSlider.InvisibleValue) return null;

				var d = (double)value;
				if (EditSlider.InPercents) {
					d = d * 100;
					return Utils.DoubleToInvariantString(d) + "%";
				}
				return Utils.DoubleToInvariantString(d);
			}

			public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			{
				EditSlider.PrintCurrentValue();

				var s = (string)value;
				if (EditSlider.InPercents) {
					s = s.Replace("%", "");
				}
				double result;
				if (Utils.TryParseDouble(s, out result)) {
					return EditSlider.InPercents ? result / 100 : result;
				}

				return null;
			}
		}
	}
}