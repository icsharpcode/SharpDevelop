using System;

namespace System.Runtime.InteropServices.APIs
{
	/// <summary>
	/// Summary description for Clipboard.
	/// </summary>
	public class APIsClipboard
	{
		public static bool ClearClipboard(IntPtr hWnd)
		{
			if(!OpenClipboard(hWnd)) return false;
			bool res = EmptyClipboard();
			CloseClipboard();
			return res;
		}

		[DllImport("user32.dll")]
		protected static extern bool EmptyClipboard();

		[DllImport("user32.dll")]
		protected static extern bool OpenClipboard(IntPtr hWnd);

		[DllImport("user32.dll")]
		protected static extern bool CloseClipboard();
	}
}
