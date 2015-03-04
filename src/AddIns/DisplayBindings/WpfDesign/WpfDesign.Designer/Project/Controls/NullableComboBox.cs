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
using System.Windows.Media;

namespace ICSharpCode.WpfDesign.Designer.Controls
{
	/// <summary>
	/// A ComboBox wich is Nullable
	/// </summary>
	public class NullableComboBox : ComboBox
	{
		static NullableComboBox()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(NullableComboBox), new FrameworkPropertyMetadata(typeof(NullableComboBox)));
		}
		
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			var btn = GetTemplateChild("PART_ClearButton") as Button;

			btn.Click += btn_Click;
		}

		void btn_Click(object sender, RoutedEventArgs e)
		{
			var clearButton = (Button)sender;
			var parent = VisualTreeHelper.GetParent(clearButton);

			while (!(parent is ComboBox))
			{
				parent = VisualTreeHelper.GetParent(parent);
			}

			var comboBox = (ComboBox)parent;
			comboBox.SelectedIndex = -1;
		}

		public bool IsNullable
		{
			get { return (bool)GetValue(IsNullableProperty); }
			set { SetValue(IsNullableProperty, value); }
		}

		public static readonly DependencyProperty IsNullableProperty =
			DependencyProperty.Register("IsNullable", typeof(bool), typeof(NullableComboBox), new PropertyMetadata(true));
	}
}
