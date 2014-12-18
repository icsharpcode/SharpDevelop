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
using ICSharpCode.WpfDesign.Designer.themes;

namespace ICSharpCode.WpfDesign.Designer.Controls
{
	public partial class EnumBar
	{
		public EnumBar()
		{
			SpecialInitializeComponent();
		}
		
		/// <summary>
		/// Fixes InitializeComponent with multiple Versions of same Assembly loaded
		/// </summary>
		public void SpecialInitializeComponent()
		{
			if (!this._contentLoaded) {
				this._contentLoaded = true;
				Uri resourceLocator = new Uri(VersionedAssemblyResourceDictionary.GetXamlNameForType(this.GetType()), UriKind.Relative);
				Application.LoadComponent(this, resourceLocator);
			}
			
			this.InitializeComponent();
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
