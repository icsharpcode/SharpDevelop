// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Runtime.InteropServices;

namespace ICSharpCode.SharpDevelop
{
	public static class MimeTypeDetection
	{
		[DllImport("urlmon.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = false)]
		static extern unsafe int FindMimeFromData(
			IntPtr pBC,
			[MarshalAs(UnmanagedType.LPWStr)] string pwzUrl,
			byte* pBuffer,
			int cbSize,
			[MarshalAs(UnmanagedType.LPWStr)] string pwzMimeProposed,
			int dwMimeFlags,
			out IntPtr ppwzMimeOut,
			int dwReserved);
		
		public static unsafe string FindMimeType(byte[] buffer)
		{
			fixed (byte *b = buffer) {
				const int FMFD_ENABLEMIMESNIFFING = 0x00000002;
				IntPtr mimeout;
				int result = FindMimeFromData(IntPtr.Zero, null, b, buffer.Length, null, FMFD_ENABLEMIMESNIFFING, out mimeout, 0);
				
				if (result != 0)
					throw Marshal.GetExceptionForHR(result);
				string mime = Marshal.PtrToStringUni(mimeout);
				Marshal.FreeCoTaskMem(mimeout);
				return mime;
			}
		}
	}
}
