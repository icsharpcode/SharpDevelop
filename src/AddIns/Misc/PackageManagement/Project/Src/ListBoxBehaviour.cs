// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using System.Windows.Controls;

namespace ICSharpCode.PackageManagement
{
	public static class ListBoxBehaviour
	{
		public static readonly DependencyProperty IsSelectedItemScrolledIntoViewProperty =
			DependencyProperty.RegisterAttached(
				"IsSelectedItemScrolledIntoView",
				typeof(bool),
				typeof(ListBoxBehaviour),
				new UIPropertyMetadata(false, OnIsSelectedItemScrolledIntoViewChanged));
				
		public static bool GetIsSelectedItemScrolledIntoView(ListBox listBox)
		{
			return (bool)listBox.GetValue(IsSelectedItemScrolledIntoViewProperty);
		}
		
		public static void SetIsSelectedItemScrolledIntoView(ListBox listBox, bool value)
		{
			listBox.SetValue(IsSelectedItemScrolledIntoViewProperty, value);
		}
		
		static void OnIsSelectedItemScrolledIntoViewChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			var scrollingBehaviour = new SelectedListBoxItemScrollingBehaviour(dependencyObject, e);
			scrollingBehaviour.Update();
		}
	}
}
