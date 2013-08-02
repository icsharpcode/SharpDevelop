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
	/// Extends WPF popup to hide the popup while SharpDevelop isn't the active application.
	/// </summary>
	public class ExtendedPopup : Popup
	{
		IntPtr hwnd;
		
		protected override void OnOpened(EventArgs e)
		{
			hwnd = ((HwndSource)PresentationSource.FromVisual(Child)).Handle;
			SetWindowPos(hwnd, HWND_TOP, 0, 0, 0, 0, NoChangeFlags);
			
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
			LoggingService.Debug("Application activated!");
			SetWindowPos(hwnd, HWND_TOP, 0, 0, 0, 0, NoChangeFlags | SWP_SHOWWINDOW);
		}
		
		void ApplicationDeactivated(object sender, EventArgs e)
		{
			LoggingService.Debug("Application deactivated!");
			SetWindowPos(hwnd, HWND_NOTOPMOST, 0, 0, 0, 0, NoChangeFlags | SWP_HIDEWINDOW);
		}
		
		#region Win32 API
		// for all possible flags see http://msdn.microsoft.com/en-us/library/ms633545%28VS.85%29.aspx
		
		const int SWP_NOSIZE     = 0x0001;
		const int SWP_NOMOVE     = 0x0002;
		const int SWP_NOACTIVATE = 0x0010;
		const int SWP_SHOWWINDOW = 0x0040;
		const int SWP_HIDEWINDOW = 0x0080;
		
		// SWP_NOACTIVATE fixes SD-1728 - http://bugtracker.sharpdevelop.net/Default.aspx?p=4&i=1728
		static readonly uint NoChangeFlags = SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE;
		
		static readonly IntPtr HWND_TOP       = new IntPtr(0);
		static readonly IntPtr HWND_TOPMOST   = new IntPtr(-1);
		static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
		
		[DllImport("user32.dll", SetLastError=true, EntryPoint="SetWindowPos")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
		#endregion
	}
}
