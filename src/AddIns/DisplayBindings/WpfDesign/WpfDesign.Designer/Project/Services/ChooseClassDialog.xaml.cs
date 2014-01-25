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
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace ICSharpCode.WpfDesign.Designer.Services
{
	public partial class ChooseClassDialog
	{
		public ChooseClassDialog(ChooseClass core)
		{
			DataContext = core;
			InitializeComponent();
			
			uxFilter.Focus();
			uxList.MouseDoubleClick += uxList_MouseDoubleClick;
			uxOk.Click += delegate { Ok(); };
			
			AddHandler(Keyboard.GotKeyboardFocusEvent,
			           new KeyboardFocusChangedEventHandler(
			           	(sender, e) => uxList.SetValue(IsSelectionActivePropertyKey, true)
			           ),
			           true);
		}
		
		//HACK: listbox is always highlighted
		public static DependencyPropertyKey IsSelectionActivePropertyKey =
			(DependencyPropertyKey)typeof(Selector).GetField("IsSelectionActivePropertyKey",
			                                                 BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
		
		protected override void OnPreviewKeyDown(KeyEventArgs e)
		{
			if (e.Key == Key.Enter) {
				Ok();
				e.Handled = true;
			} else if (e.Key == Key.Up) {
				uxList.SelectedIndex = Math.Max(0, uxList.SelectedIndex - 1);
				e.Handled = true;
			} else if (e.Key == Key.Down) {
				uxList.SelectedIndex++;
				e.Handled = true;
			}
		}
		
		void uxList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			var f = e.OriginalSource as FrameworkElement;
			if (f != null && f.DataContext is Type) {
				Ok();
			}
		}
		
		void Ok()
		{
			DialogResult = true;
			Close();
		}
	}
	
	class ClassListBox : ListBox
	{
		protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
		{
			base.OnItemsChanged(e);
			SelectedIndex = 0;
			ScrollIntoView(SelectedItem);
		}
		
		protected override void OnSelectionChanged(SelectionChangedEventArgs e)
		{
			base.OnSelectionChanged(e);
			ScrollIntoView(SelectedItem);
		}
	}
	
	public class ClassNameConverter : IValueConverter
	{
		public static ClassNameConverter Instance = new ClassNameConverter();
		
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var c = value as Type;
			if (c == null) return value;
			return c.Name + " (" + c.Namespace + ")";
		}
		
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
	
	public class NullToBoolConverter : IValueConverter
	{
		public static NullToBoolConverter Instance = new NullToBoolConverter();
		
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value == null ? false : true;
		}
		
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
