// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// A user control for choosing a color.
	/// </summary>
	public partial class ColorPicker : UserControl
	{
		public static readonly DependencyProperty ValueProperty =
			DependencyProperty.Register("Value", typeof(Color), typeof(ColorPicker), new FrameworkPropertyMetadata(new Color(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));
		
		public Color Value {
			get { return (Color)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}
		
		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register("Text", typeof(string), typeof(ColorPicker),
			                            new FrameworkPropertyMetadata(OnValueChanged));
		
		public string Text {
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
		
		static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			ColorPicker picker = obj as ColorPicker;
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
		
		public ColorPicker()
		{
			InitializeComponent();
		}
		
		void ButtonClick(object sender, RoutedEventArgs e)
		{
			e.Handled = true;
			using (SharpDevelopColorDialog dlg = new SharpDevelopColorDialog()) {
				dlg.WpfColor = this.Value;
				if (dlg.ShowWpfDialog() == true) {
					// use SetCurrentValue instead of SetValue so that two-way data binding can be used
					SetCurrentValue(ValueProperty, dlg.WpfColor);
				}
			}
		}
	}
}
