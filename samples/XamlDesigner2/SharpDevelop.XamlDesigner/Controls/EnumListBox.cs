using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using SharpDevelop.XamlDesigner.Converters;
using System.Globalization;

namespace SharpDevelop.XamlDesigner.Controls
{
	public class EnumListBox : ListBox
	{
		static EnumListBox()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(EnumListBox),
				new FrameworkPropertyMetadata(typeof(EnumListBox)));
		}

		public static readonly DependencyProperty EnumValueProperty =
			DependencyProperty.Register("EnumValue", typeof(object), typeof(EnumListBox),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public object EnumValue
		{
			get { return (object)GetValue(EnumValueProperty); }
			set { SetValue(EnumValueProperty, value); }
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == EnumValueProperty) {
				if (EnumValue != null) {
					//TODO: flags
					SelectedIndex = Enum.GetValues(EnumValue.GetType()).Cast<object>().ToList().IndexOf(EnumValue);
				}
				else {
					UnselectAll();
				}
			}
			else if (e.Property == SelectedIndexProperty) {
				if (EnumValue != null) {
					EnumValue = Enum.ToObject(EnumValue.GetType(), SelectedIndex);
				}
			}
		}
	}
}
