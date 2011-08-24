// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

using Widgets = ICSharpCode.SharpDevelop.Widgets;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// A user control for choosing a color.
	/// </summary>
	public partial class ColorPickerButton : UserControl
	{
		public static readonly DependencyProperty ValueProperty =
			DependencyProperty.Register("Value", typeof(Color), typeof(ColorPickerButton), new FrameworkPropertyMetadata(new Color(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));
		
		public Color Value {
			get { return (Color)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}
		
		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register("Text", typeof(string), typeof(ColorPickerButton),
			                            new FrameworkPropertyMetadata(OnValueChanged));
		
		public string Text {
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
		
		static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			ColorPickerButton picker = obj as ColorPickerButton;
			if (picker != null) {
				Color color = picker.Value;
				picker.border.Background = new SolidColorBrush(color);
				// find a foreground color that is visible on the background
				if (color.A < 128)
					picker.textBlock.Foreground = SystemColors.WindowTextBrush;
				else if (0.299f * color.R + 0.587f * color.G + 0.114f * color.B > 128)
					picker.textBlock.Foreground = Brushes.Black;
				else
					picker.textBlock.Foreground = Brushes.WhiteSmoke;
				
				picker.textBlock.Text = picker.Text ?? color.ToString();
			}
		}
		
		public ColorPickerButton()
		{
			InitializeComponent();
		}
		
		void ButtonClick(object sender, RoutedEventArgs e)
		{
			var control = new Widgets.ColorPicker() {
				Background = SystemColors.ControlBrush
			};
			Popup popup = new Popup() {
				Child = control,
				Placement = PlacementMode.Bottom,
				PlacementTarget = this,
				IsOpen = true,
				StaysOpen = false
			};
			
			control.SetBinding(
				Widgets.ColorPicker.ColorProperty,
				new Binding("Value") {
					Source = this,
					Mode = BindingMode.TwoWay
				}
			);
		}
	}
}
