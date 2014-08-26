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
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace ResourceEditor.Views
{
	/// <summary>
	/// Interaction logic for InPlaceEditLabel.xaml
	/// </summary>
	public partial class InPlaceEditLabel : UserControl
	{
		string textBeforeEditing;
		
		public InPlaceEditLabel()
		{
			InitializeComponent();
			editingTextBox.Visibility = Visibility.Collapsed;
			readOnlyTextBlock.Visibility = Visibility.Visible;
			readOnlyTextBlock.DataContext = this;
			editingTextBox.DataContext = this;
		}
		
		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register("Text", typeof(string), typeof(InPlaceEditLabel),
				new FrameworkPropertyMetadata());
		
		public string Text {
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
		
		public static readonly DependencyProperty IsEditingProperty =
			DependencyProperty.Register("IsEditing", typeof(bool), typeof(InPlaceEditLabel),
				new FrameworkPropertyMetadata());
		
		public bool IsEditing {
			get { return (bool)GetValue(IsEditingProperty); }
			set { SetValue(IsEditingProperty, value); }
		}
		
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);
			
			if (e.Property == IsEditingProperty) {
				if ((bool)e.NewValue) {
					editingTextBox.Visibility = Visibility.Visible;
					readOnlyTextBlock.Visibility = Visibility.Collapsed;
					editingTextBox.Focus();
					textBeforeEditing = this.Text;
					editingTextBox.SelectAll();
				} else {
					editingTextBox.Visibility = Visibility.Collapsed;
					readOnlyTextBlock.Visibility = Visibility.Visible;
				}
			}
		}
		
		protected override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp(e);
			
			if (e.Key == Key.Enter) {
				IsEditing = false;
			} else if (e.Key == Key.Escape) {
				// Cancel editing and restore original text
				this.Text = textBeforeEditing;
				IsEditing = false;
			}
		}
		
		void EditingTextBox_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if ((bool)e.NewValue) {
				// Auto-select whole text as soon as TextBox becomes visible
//				editingTextBox.Focus();
//				editingTextBox.SelectAll();
//				textBeforeEditing = this.Text;
			}
		}
		
		void EditingTextBox_LostFocus(object sender, RoutedEventArgs e)
		{
			// When losing focus, also stop editing
			IsEditing = false;
		}
	}
}