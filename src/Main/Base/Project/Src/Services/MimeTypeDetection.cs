// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ICSharpCode.SharpDevelop
{
	public static class MimeTypeDetection
	{
		const int BUFFER_SIZE = 4 * 1024;
		
		// Known BOMs
		public static readonly byte[] UTF8 = new byte[] { 0xEF, 0xBB, 0xBF };
		public static readonly byte[] UTF16BE = new byte[] { 0xFE, 0xFF };
		public static readonly byte[] UTF16LE = new byte[] { 0xFF, 0xFE };
		public static readonly byte[] UTF32BE = new byte[] { 0x00, 0x00, 0xFE, 0xFF };
		public static readonly byte[] UTF32LE = new byte[] { 0xFF, 0xFE, 0x00, 0x00 };
		
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
		
		
		static byte[] DetectAndRemoveBOM(byte[] buffer, out int len)
		{
			len = UTF8.Length;
			if (buffer.StartsWith(UTF8))
				return buffer.Skip(UTF8.Length).ToArray();
			len = UTF32BE.Length;
			if (buffer.StartsWith(UTF32BE))
				return buffer.Skip(UTF32BE.Length).ToArray();
			len = UTF32LE.Length;
			if (buffer.StartsWith(UTF32LE))
				return buffer.Skip(UTF32LE.Length).ToArray();
			len = UTF16LE.Length;
			if (buffer.StartsWith(UTF16LE))
				return buffer.Skip(UTF16LE.Length).ToArray();
			len = UTF16BE.Length;
			if (buffer.StartsWith(UTF16BE))
				return buffer.Skip(UTF16BE.Length).ToArray();
			len = 0;
			return buffer;
		}
		
		static bool StartsWith(this byte[] buffer, byte[] start)
		{
			if (buffer.Length < start.Length)
				return false;
			int i = 0;
			while (i < start.Length && buffer[i] == start[i])
				i++;
			return i >= start.Length;
		}
		
		static unsafe string FindMimeType(byte[] buffer, int offset, int length)
		{
			int len;
			buffer = DetectAndRemoveBOM(buffer, out len);
			length -= len;
			offset = (offset < len) ? 0 : offset - len;
			if (length == 0)
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
	}
}
