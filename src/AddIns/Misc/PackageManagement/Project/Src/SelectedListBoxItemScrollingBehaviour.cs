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
using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace ICSharpCode.PackageManagement
{
	public class SelectedListBoxItemScrollingBehaviour
	{
		ListBox listBox;
		DependencyPropertyChangedEventArgs propertyChangedEventArgs;
		
		public SelectedListBoxItemScrollingBehaviour(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			this.listBox = dependencyObject as ListBox;
			this.propertyChangedEventArgs = e;
		}
		
		public void Update()
		{
			if (ShouldUpdateSelectionChangedHandlerRegistration()) {
				UpdateSelectionChangedHandlerRegistration();
			}
		}
		
		bool ShouldUpdateSelectionChangedHandlerRegistration()
		{
			return DependencyObjectIsListBox() && NewPropertyValueIsBoolean();
		}
		
		bool DependencyObjectIsListBox()
		{
			return (listBox != null);
		}
		
		bool NewPropertyValueIsBoolean()
		{
			return (propertyChangedEventArgs.NewValue is bool);
		}
		
		void UpdateSelectionChangedHandlerRegistration()
		{
			if ((bool)propertyChangedEventArgs.NewValue) {
				RegisterSelectionChangedHandler(listBox);
			} else {
				UnregisterSelectionChangedHandler(listBox);
			}
		}
		
		protected virtual void RegisterSelectionChangedHandler(ListBox listBox)
		{
			listBox.SelectionChanged += OnListBoxSelectionChanged;
		}
		
		protected virtual void UnregisterSelectionChangedHandler(ListBox listBox)
		{
			listBox.SelectionChanged -= OnListBoxSelectionChanged;
		}
		
		static void OnListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			OnListBoxSelectionChanged(sender, e.OriginalSource, e.AddedItems, ScrollListBoxItemIntoView);
		}
		
		static void ScrollListBoxItemIntoView(ListBox listBox, object item)
		{
			listBox.ScrollIntoView(item);
		}
		
		protected static void OnListBoxSelectionChanged(object sender,
			object originalSource,
			IList addedItems,
			Action<ListBox, object> executeScrollListBoxItemIntoView)
		{
			if (!Object.ReferenceEquals(sender, originalSource)) {
				return;
			}

			ListBox listBox = originalSource as ListBox;
			if (listBox != null) {
				if (addedItems.Count > 0) {
					executeScrollListBoxItemIntoView(listBox, addedItems[0]);
				}
			}
		}
	}
}
