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

namespace ICSharpCode.XamlDesigner
{
	public partial class EnumBar
	{
		public EnumBar()
		{
			InitializeComponent();
		}

		Type currentEnumType;

		public static readonly DependencyProperty ValueProperty =
			DependencyProperty.Register("Value", typeof(object), typeof(EnumBar),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public object Value {
			get { return (object)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == ValueProperty) {
				var type = e.NewValue.GetType();

				if (currentEnumType != type) {
					currentEnumType = type;
					uxPanel.Children.Clear();
					foreach (var v in Enum.GetValues(type)) {
						var b = new EnumButton();
						b.Value = v;
						b.Content = Enum.GetName(type, v);
						b.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(button_PreviewMouseLeftButtonDown);
						uxPanel.Children.Add(b);
					}
				}

				foreach (EnumButton c in uxPanel.Children) {
					if (c.Value.Equals(Value)) {
						c.IsChecked = true;
					}
					else {
						c.IsChecked = false;
					}
				}
			}
		}

		void button_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			Value = (sender as EnumButton).Value;
			e.Handled = true;
		}
	}
}
