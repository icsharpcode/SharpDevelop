// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;

namespace ICSharpCode.SharpDevelop
{
	public static class MimeTypeDetection
	{
		const int BUFFER_SIZE = 4 * 1024;
		
		public const string Binary = "application/octet-stream";
		public const string Text = "text/plain";
		public const string Xml = "text/xml";
		
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
		
		public static string FindMimeType(Stream stream)
		{
			StreamReader reader;
			if (stream.Length >= 2) {
				int firstByte = stream.ReadByte();
				int secondByte = stream.ReadByte();
				switch ((firstByte << 8) | secondByte) {
					case 0xfffe: // UTF-16 LE BOM / UTF-32 LE BOM
					case 0xfeff: // UTF-16 BE BOM
						stream.Position -= 2;
						reader = new StreamReader(stream, detectEncodingFromByteOrderMarks: true);
						break;
					case 0xefbb: // start of UTF-8 BOM
						if (stream.ReadByte() == 0xbf) {
							reader = new StreamReader(stream, Encoding.UTF8);
							break;
						} else {
							return Binary;
						}
					default:
						if (IsUTF8(stream, (byte)firstByte, (byte)secondByte)) {
							stream.Position = 0;
							reader = new StreamReader(stream, Encoding.UTF8);
							break;
						} else {
							byte[] buffer = new byte[BUFFER_SIZE];
							int length = stream.Read(buffer, 0, BUFFER_SIZE);
							return FindMimeType(buffer, 0, length);
						}
				}
			} else {
				return Text;
			}
			// Now we got a StreamReader with the correct encoding
			// Check for XML now
			try {
				XmlTextReader xmlReader = new XmlTextReader(reader);
				xmlReader.XmlResolver = null;
				xmlReader.MoveToContent();
				return Xml;
			} catch (XmlException) {
				return Text;
			}
		}
		
		static bool IsUTF8(Stream fs, byte firstByte, byte secondByte)
		{
			int max = (int)Math.Min(fs.Length, 500000); // look at max. 500 KB
			const int ASCII = 0;
			const int Error = 1;
			const int UTF8  = 2;
			const int UTF8Sequence = 3;
			int state = ASCII;
			int sequenceLength = 0;
			byte b;
			for (int i = 0; i < max; i++) {
				if (i == 0) {
					b = firstByte;
				} else if (i == 1) {
					b = secondByte;
				} else {
					b = (byte)fs.ReadByte();
				}
				if (b < 0x80) {
					// normal ASCII character
					if (state == UTF8Sequence) {
						state = Error;
						break;
					}
				} else if (b < 0xc0) {
					// 10xxxxxx : continues UTF8 byte sequence
					if (state == UTF8Sequence) {
						--sequenceLength;
						if (sequenceLength < 0) {
							state = Error;
							break;
						} else if (sequenceLength == 0) {
							state = UTF8;
						}
					} else {
						state = Error;
						break;
					}
				} else if (b >= 0xc2 && b < 0xf5) {
					// beginning of byte sequence
					if (state == UTF8 || state == ASCII) {
						state = UTF8Sequence;
						if (b < 0xe0) {
							sequenceLength = 1; // one more byte following
						} else if (b < 0xf0) {
							sequenceLength = 2; // two more bytes following
						} else {
							sequenceLength = 3; // three more bytes following
						}
					} else {
						state = Error;
						break;
					}
				} else {
					// 0xc0, 0xc1, 0xf5 to 0xff are invalid in UTF-8 (see RFC 3629)
					state = Error;
					break;
				}
			}
			return state != Error;
		}
		
		static unsafe string FindMimeType(byte[] buffer, int offset, int length)
		{
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
			using (MemoryStream stream = new MemoryStream(buffer))
				return FindMimeType(stream);
		}
	}
}
