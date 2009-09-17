// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com" />
//     <version>$Revision$</version>
// </file>

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
	/// Interaction logic for ColorPicker.xaml
	/// </summary>
	public partial class ColorPicker : UserControl
	{
		SharpDevelopColorDialog dialog;
		
		public static readonly DependencyProperty ValueProperty =
			DependencyProperty.Register("Value", typeof(Color), typeof(ColorPicker));
		
		public Color Value {
			get { return (Color)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}
		
		public ColorPicker()
		{
			InitializeComponent();
			dialog = new SharpDevelopColorDialog();
		}
		
		void ButtonClick(object sender, RoutedEventArgs e)
		{
			if (dialog.ShowWpfDialog() ?? false) {
				this.Value = dialog.WpfColor;
			}
		}
	}
	
	public class ColorToBrushConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is Color)
				return new SolidColorBrush((Color)value);
			
			throw new NotSupportedException();
		}
		
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is SolidColorBrush)
				return (value as SolidColorBrush).Color;
			
			throw new NotSupportedException();		
		}
	}
}