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
		Visibility oldState;
		IntPtr hwnd;
		
		protected override void OnOpened(EventArgs e)
		{
			hwnd = ((HwndSource)PresentationSource.FromVisual(Child)).Handle;
			SetWindowPos(hwnd, HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
			
			Application.Current.Activated += ApplicationActivated;
			Application.Current.Deactivated += ApplicationDeactivated;
		}

		protected override void OnClosed(EventArgs e)
		{
			Application.Current.Activated -= ApplicationActivated;
			Application.Current.Deactivated -= ApplicationDeactivated;
		}
		
		void ApplicationActivated(object sender, EventArgs e)
		{
			Visibility = oldState;
			SetWindowPos(hwnd, HWND_TOP, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
		}
		
		void ApplicationDeactivated(object sender, EventArgs e)
		{
			oldState = Visibility;
			Visibility = Visibility.Hidden;
		}
		
		#region Win32 API
		const int SWP_NOMOVE = 0x002;
		const int SWP_NOSIZE = 0x001;
		static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
		static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
		static readonly IntPtr HWND_TOP = new IntPtr(0);
		
		[DllImport("user32", EntryPoint="SetWindowPos")]
		static extern int SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
		#endregion
	}
}
