// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using System.Windows.Input;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// Custom focus scope implementation.
	/// See http://www.codeproject.com/KB/WPF/EnhancedFocusScope.aspx for a description of the problems with the normal WPF FocusScope.
	/// </summary>
	public static class CustomFocusManager
	{
		// DP for attached behavior, toggles remembering on or off
		public static readonly DependencyProperty RememberFocusedChildProperty =
			DependencyProperty.RegisterAttached("RememberFocusedChild", typeof(bool), typeof(CustomFocusManager),
			                                    new FrameworkPropertyMetadata(false, OnRememberFocusedChildChanged));
		
		// This property is used to remember the focused child.
		// We are using WeakReferences because a visual tree may change while it is not visible, and we don't
		// want to keep parts of the old visual tree alive.
		static readonly DependencyProperty FocusedChildProperty =
			DependencyProperty.RegisterAttached("FocusedChild", typeof(WeakReference), typeof(CustomFocusManager));
		
		public static bool GetRememberFocusedChild(UIElement element)
		{
			if (element == null)
				throw new ArgumentNullException("element");
			return (bool)element.GetValue(RememberFocusedChildProperty);
		}
		
		public static void SetRememberFocusedChild(UIElement element, bool value)
		{
			if (element == null)
				throw new ArgumentNullException("element");
			element.SetValue(RememberFocusedChildProperty, value);
		}
		
		public static IInputElement GetFocusedChild(UIElement element)
		{
			if (element == null)
				throw new ArgumentNullException("element");
			WeakReference r = (WeakReference)element.GetValue(FocusedChildProperty);
			if (r != null)
				return (IInputElement)r.Target;
			else
				return null;
		}
		
		public static void SetFocusToRememberedChild(UIElement element)
		{
			IInputElement focusedChild = GetFocusedChild(element);
			LoggingService.Debug("Restoring focus for " + element + " to " + focusedChild);
			if (focusedChild != null)
				Keyboard.Focus(focusedChild);
		}
		
		static void OnRememberFocusedChildChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			UIElement element = d as UIElement;
			if (element != null) {
				if ((bool)e.OldValue)
					element.RemoveHandler(UIElement.GotFocusEvent, onGotFocusEventHandler);
				if ((bool)e.NewValue)
					element.AddHandler(UIElement.GotFocusEvent, onGotFocusEventHandler, true);
			}
		}
		
		static readonly RoutedEventHandler onGotFocusEventHandler = OnGotFocus;
		
		static void OnGotFocus(object sender, RoutedEventArgs e)
		{
			UIElement element = (UIElement)sender;
			IInputElement focusedElement = e.OriginalSource as IInputElement;
			WeakReference r = (WeakReference)element.GetValue(FocusedChildProperty);
			if (r != null) {
				r.Target = focusedElement;
			} else {
				element.SetValue(FocusedChildProperty, new WeakReference(focusedElement));
			}
		}
	}
}
