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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ICSharpCode.SharpDevelop.Widgets
{
	/// <summary>
	/// Makes it easier to bind data bind a set of radio buttons to a single value.
	/// By default, the value associated with a radio button is expected to be stored in
	/// RadioButton.Tag; although this can be changed using the SelectedValuePath property.
	/// </summary>
	public class RadioButtonGroup : Selector
	{
		static RadioButtonGroup()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(RadioButtonGroup), new FrameworkPropertyMetadata(typeof(RadioButtonGroup)));
			
			SelectedValuePathProperty.OverrideMetadata(typeof(RadioButtonGroup), new FrameworkPropertyMetadata("Tag"));
		}
		
		public RadioButtonGroup()
		{
			AddHandler(RadioButton.CheckedEvent, new RoutedEventHandler(radio_Checked));
		}
		
		protected override void OnSelectionChanged(SelectionChangedEventArgs e)
		{
			base.OnSelectionChanged(e);
			foreach (object item in e.RemovedItems) {
				RadioButton radio = item as RadioButton;
				if (radio != null)
					radio.IsChecked = false;
			}
			foreach (object item in e.AddedItems) {
				RadioButton radio = item as RadioButton;
				if (radio != null)
					radio.IsChecked = true;
			}
		}
		
		void radio_Checked(object sender, RoutedEventArgs e)
		{
			if (Items.Contains(e.Source)) {
				this.SelectedItem = e.Source;
				e.Handled = true;
			}
		}
	}
}
