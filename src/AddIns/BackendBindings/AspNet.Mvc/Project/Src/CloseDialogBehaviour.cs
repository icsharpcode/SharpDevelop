// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;

namespace ICSharpCode.AspNet.Mvc
{
	public static class CloseWindowBehaviour
	{
		public static readonly DependencyProperty IsClosedProperty =
			DependencyProperty.RegisterAttached(
				"IsClosed",
				typeof(bool),
				typeof(CloseWindowBehaviour),
				new UIPropertyMetadata(false, OnIsClosedChanged));
				
		public static bool GetIsClosed(Window window)
		{
			return (bool)window.GetValue(IsClosedProperty);
		}
		
		public static void SetIsClosed(Window window, bool value)
		{
			window.SetValue(IsClosedProperty, value);
		}
		
		static void OnIsClosedChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			var window = dependencyObject as Window;
			window.DialogResult = (bool)e.NewValue;
		}
	}
}
