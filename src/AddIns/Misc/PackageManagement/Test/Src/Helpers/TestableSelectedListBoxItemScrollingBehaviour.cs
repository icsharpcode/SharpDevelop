// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;

using ICSharpCode.PackageManagement;

namespace PackageManagement.Tests.Helpers
{
	public class TestableSelectedListBoxItemScrollingBehaviour : SelectedListBoxItemScrollingBehaviour
	{
		public bool IsRegisterSelectionChangedHandlerCalled;
		public ListBox ListBoxPassedToRegisterSelectionChangedHandler;
		
		public bool IsUnregisterSelectionChangedHandlerCalled;
		public ListBox ListBoxPassedToUnregisterSelectionChangedHandler;
		
		public bool IsScrollListBoxItemIntoViewCalled;
		public ListBox ListBoxPassedToScrollListBoxItemIntoView;
		public object ItemPassedToScrollListBoxItemIntoView;
		
		public TestableSelectedListBoxItemScrollingBehaviour(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
			: base(dependencyObject, e)
		{		
		}
		
		protected override void RegisterSelectionChangedHandler(ListBox listBox)
		{
			IsRegisterSelectionChangedHandlerCalled = true;
			ListBoxPassedToRegisterSelectionChangedHandler = listBox;
		}
		
		protected override void UnregisterSelectionChangedHandler(ListBox listBox)
		{
			IsUnregisterSelectionChangedHandlerCalled = true;
			ListBoxPassedToUnregisterSelectionChangedHandler = listBox;
		}
		
		public void CallOnListBoxSelectionChanged(ListBox listBox)
		{
			CallOnListBoxSelectionChanged(listBox, "selectedItem");
		}
		
		public void CallOnListBoxSelectionChanged(ListBox listBox, string selectedItem)
		{
			var addedItems = new string[] { selectedItem };
			CallOnListBoxSelectionChanged(listBox, listBox, addedItems);
		}
		
		public void CallOnListBoxSelectionChanged(object sender, object originalSource, IList addedItems)
		{
			OnListBoxSelectionChanged(sender, originalSource, addedItems, ScrollListBoxItemIntoView);
		}
		
		void ScrollListBoxItemIntoView(ListBox listBox, object item)
		{
			IsScrollListBoxItemIntoViewCalled = true;
			ListBoxPassedToScrollListBoxItemIntoView = listBox;
			ItemPassedToScrollListBoxItemIntoView = item;
		}
	}
}
