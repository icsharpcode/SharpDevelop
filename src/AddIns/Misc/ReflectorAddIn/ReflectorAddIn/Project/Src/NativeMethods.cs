/*
 * Created by SharpDevelop.
 * User: Daniel
 * Date: 04.06.2009
 * Time: 14:28
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ReflectorAddIn
{
	static class NativeMethods
	{
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);
		
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		static extern int GetWindowText(IntPtr hWnd, [Out] StringBuilder title, int size);
		
		public static string GetWindowText(IntPtr hWnd, int maxLength)
		{
			StringBuilder b = new StringBuilder(maxLength + 1);
			if (GetWindowText(hWnd, b, b.Capacity) != 0)
				return b.ToString();
			else
				return string.Empty;
		}
		
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		internal static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, ref CopyDataStruct lParam);
		
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool SetForegroundWindow(IntPtr hWnd);
	}
	
	[return: MarshalAs(UnmanagedType.Bool)]
	delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
	
	[StructLayout(LayoutKind.Sequential)]
	struct CopyDataStruct
	{
		public IntPtr Padding;
		public int Size;
		public IntPtr Buffer;

		public CopyDataStruct(IntPtr padding, int size, IntPtr buffer)
		{
			this.Padding = padding;
			this.Size = size;
			this.Buffer = buffer;
		}
	}
}
