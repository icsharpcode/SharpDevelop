// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
