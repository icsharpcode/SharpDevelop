// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// Description of ExtendedPopup.
	/// </summary>
	public class ExtendedPopup : Popup
	{
		RoutedEventHandler lostFocus;
		RoutedEventHandler gotFocus;
		
//		protected override void OnOpened(EventArgs e)
//		{
//			IntPtr hwnd = ((HwndSource)PresentationSource.FromVisual(Child)).Handle;
//			SetWindowPos(hwnd, HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
//			lostFocus = new RoutedEventHandler(MainWindowLostFocus);
//			gotFocus = new RoutedEventHandler(MainWindowGotFocus);
//			AddHandler(Window., lostFocus);
//			AddHandler(Window.GotFocusEvent, gotFocus);
//			
//		}
//		
//		protected override void OnClosed(EventArgs e)
//		{
//			RemoveHandler(Window.LostFocusEvent, lostFocus);
//			RemoveHandler(Window.GotFocusEvent, gotFocus);
//		}
//		
//		void MainWindowLostFocus(object sender, RoutedEventArgs e)
//		{
//			Visibility = Visibility.Collapsed;
//		}
//		
//		void MainWindowGotFocus(object sender, RoutedEventArgs e)
//		{
//			Visibility = Visibility.Visible;
//		}
		
		const int SWP_NOMOVE = 0x002;
		const int SWP_NOSIZE = 0x001;
		static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
		
		[DllImport("user32", EntryPoint="SetWindowPos")]
		static extern int SetWindowPos(IntPtr hWnd, IntPtr hwndInsertAfter, int x, int y, int cx, int cy, uint uFlags);
	}
}
