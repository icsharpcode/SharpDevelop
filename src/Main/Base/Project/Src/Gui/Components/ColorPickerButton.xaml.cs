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
