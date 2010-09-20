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

namespace ICSharpCode.WpfDesign.Designer.Controls
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

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
		public object Value {
			get { return (object)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}

		public static readonly DependencyProperty ContainerProperty =
			DependencyProperty.Register("Container", typeof(Panel), typeof(EnumBar));

		public Panel Container {
			get { return (Panel)GetValue(ContainerProperty); }
			set { SetValue(ContainerProperty, value); }
		}

		public static readonly DependencyProperty ButtonStyleProperty =
			DependencyProperty.Register("ButtonStyle", typeof(Style), typeof(EnumBar));

		public Style ButtonStyle {
			get { return (Style)GetValue(ButtonStyleProperty); }
			set { SetValue(ButtonStyleProperty, value); }
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
						b.SetBinding(StyleProperty, new Binding("ButtonStyle") { Source = this });
						b.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(button_PreviewMouseLeftButtonDown);
						uxPanel.Children.Add(b);
					}
				}

				UpdateButtons();
				UpdateContainer();

			} else if (e.Property == ContainerProperty) {
				UpdateContainer();
			}
		}

		void UpdateButtons()
		{
			foreach (EnumButton c in uxPanel.Children) {
				if (c.Value.Equals(Value)) {
					c.IsChecked = true;
				}
				else {
					c.IsChecked = false;
				}
			}
		}

		void UpdateContainer()
		{
			if (Container != null) {
				for (int i = 0; i < uxPanel.Children.Count; i++) {
					var c = uxPanel.Children[i] as EnumButton;
					if (c.IsChecked.Value) 
						Container.Children[i].Visibility = Visibility.Visible;
					else 
						Container.Children[i].Visibility = Visibility.Collapsed;
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
