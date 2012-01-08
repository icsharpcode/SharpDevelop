// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace ICSharpCode.SharpDevelop
{
	public static class MimeTypeDetection
	{
		const int BUFFER_SIZE = 4 * 1024;
		
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
		
		static unsafe string FindMimeType(byte[] buffer, int offset, int length)
		{
			if (buffer.Length == 0)
				return "text/plain";
			
			fixed (byte *b = &buffer[offset]) {
				const int FMFD_ENABLEMIMESNIFFING = 0x00000002;
				IntPtr mimeout;
				int result = FindMimeFromData(IntPtr.Zero, null, b, length, null, FMFD_ENABLEMIMESNIFFING, out mimeout, 0);
				
				if (result != 0)
					throw Marshal.GetExceptionForHR(result);
				string mime = Marshal.PtrToStringUni(mimeout);
				Marshal.FreeCoTaskMem(mimeout);
				return mime;
			}
		}

		public static string FindMimeType(byte[] buffer)
		{
			if (buffer == null)
				throw new ArgumentNullException("buffer");
			return FindMimeType(buffer, 0, buffer.Length);
		}

		public static string FindMimeType(Stream stream)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");
			byte[] buffer = new byte[BUFFER_SIZE];
			stream.Position = 0;
			return FindMimeType(buffer, 0, stream.Read(buffer, 0, buffer.Length));
		}
		
		public static string FindMimeType(ITextBuffer buffer)
		{
			if (buffer == null)
				throw new ArgumentNullException("buffer");
			// TODO USE PROPER ENCODING!
			// Maybe use Encoding detection from AvalonEdit?
			byte[] bytes = Encoding.Default.GetBytes(buffer.TextLength > BUFFER_SIZE ? buffer.GetText(0, BUFFER_SIZE) : buffer.Text);
			return FindMimeType(bytes, 0, bytes.Length);
		}
	}
}
