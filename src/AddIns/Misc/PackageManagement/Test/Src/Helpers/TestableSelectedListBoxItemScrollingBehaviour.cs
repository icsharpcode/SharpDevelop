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
