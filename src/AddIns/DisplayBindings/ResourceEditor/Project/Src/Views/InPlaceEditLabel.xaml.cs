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
using System.Windows.Documents;
using System.Windows.Input;

namespace ResourceEditor.Views
{
	/// <summary>
	/// Interaction logic for InPlaceEditLabel.xaml
	/// </summary>
	public partial class InPlaceEditLabel : UserControl
	{
		string textBeforeEditing;
		bool selectAllText;
		
		public InPlaceEditLabel()
		{
			InitializeComponent();
			editingTextBox.Visibility = Visibility.Collapsed;
			readOnlyTextBlock.Visibility = Visibility.Visible;
			editingTextBox.DataContext = this;
		}
		
		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register("Text", typeof(string), typeof(InPlaceEditLabel),
				new FrameworkPropertyMetadata());
		
		public string Text {
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
		
		public static readonly DependencyProperty DisplayTextProperty =
			DependencyProperty.Register("DisplayText", typeof(object), typeof(InPlaceEditLabel),
			                            new FrameworkPropertyMetadata(OnDisplayTextChanged));
		
		static void OnDisplayTextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			var textBlock = ((InPlaceEditLabel)obj).readOnlyTextBlock;
			if (e.NewValue is Span) {
				textBlock.Inlines.Clear();
				textBlock.Inlines.Add((Span)e.NewValue);
			} else {
				textBlock.Text = e.NewValue.ToString();
			}
		}
		
		public object DisplayText {
			get { return (object)GetValue(DisplayTextProperty); }
			set { SetValue(DisplayTextProperty, value); }
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
			
			if (e.Property.Name == IsEditingProperty.Name) {
				if ((bool)e.NewValue) {
					editingTextBox.Visibility = Visibility.Visible;
					readOnlyTextBlock.Visibility = Visibility.Collapsed;
					textBeforeEditing = this.Text;
					
					if (editingTextBox.Text == this.Text) {
						SelectAllText();
					} else {
						// This is a workaround for the case when TextBox.Text property is updated after this code,
						// so we can still select everything as soon as Text property is updated.
						selectAllText = true;
					}
				} else {
					editingTextBox.Visibility = Visibility.Collapsed;
					readOnlyTextBlock.Visibility = Visibility.Visible;
				}
			}
		}
		
		void EditingTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (selectAllText) {
				SelectAllText();
				selectAllText = false;
			}
		}
		
		void SelectAllText()
		{
			editingTextBox.Focus();
			editingTextBox.SelectAll();
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
		
		void EditingTextBox_LostFocus(object sender, RoutedEventArgs e)
		{
			// When losing focus, also stop editing
			IsEditing = false;
		}
	}
}