// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
