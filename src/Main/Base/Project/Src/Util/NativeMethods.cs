// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Security;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Contains P/Invoke methods for functions in the Windows API.
	/// </summary>
	static class NativeMethods
	{
		static readonly IntPtr FALSE = new IntPtr(0);
		static readonly IntPtr TRUE = new IntPtr(1);
		
		public const int WM_SETREDRAW = 0x00B;
		public const int WM_USER = 0x400;
		
		[DllImport("user32.dll")]
		public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
		
		[DllImport("user32.dll")]
		public static extern IntPtr SetForegroundWindow(IntPtr hWnd);
		
		public static void SetWindowRedraw(IntPtr hWnd, bool allowRedraw)
		{
			SendMessage(hWnd, WM_SETREDRAW, allowRedraw ? TRUE : FALSE, IntPtr.Zero);
		}
		
		[DllImport("user32.dll", ExactSpelling=true)]
		static extern short GetKeyState(int vKey);
		
		public static bool IsKeyPressed(Keys key)
		{
			return GetKeyState((int)key) < 0;
		}
	}
}
