// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

#pragma warning disable 1591

using System;
using System.Runtime.InteropServices;

namespace Debugger.Interop
{
	public delegate void UnmanagedStringGetter(uint pStringLength, out uint stringLength, System.IntPtr pString);
	
	public static class Util
	{
		public static string GetString(UnmanagedStringGetter getter)
		{
			return GetString(getter, 64, true);
		}
		
		public static unsafe string GetString(UnmanagedStringGetter getter, uint defaultLength, bool trim)
		{
			// 8M characters ought to be enough for everyone...
			// (we need some limit to avoid OutOfMemoryExceptions when trying to load extremely large
			// strings - see SD2-1470).
			const uint MAX_LENGTH = 8 * 1024 * 1024;
			string managedString;
			uint exactLength;

			if (defaultLength > MAX_LENGTH)
				defaultLength = MAX_LENGTH;
			// First attempt
			// TODO: Consider removing "+ 2" for the zero
			byte[] buffer = new byte[(int)defaultLength * 2 + 2];  // + 2 for terminating zero
			fixed(byte* pBuffer = buffer) {
				getter((uint)buffer.Length, out exactLength, defaultLength > 0 ? new IntPtr(pBuffer) : IntPtr.Zero);
				
				exactLength = (exactLength > MAX_LENGTH) ? MAX_LENGTH : exactLength;
				
				if(exactLength > defaultLength) {
					// Second attempt
					byte[] buffer2 = new byte[(int)exactLength * 2 + 2];  // + 2 for terminating zero
					fixed(byte* pBuffer2 = buffer2) {
						getter((uint)buffer2.Length, out exactLength, new IntPtr(pBuffer2));
						managedString = Marshal.PtrToStringUni(new IntPtr(pBuffer2), (int)exactLength);
					}
				} else {
					managedString = Marshal.PtrToStringUni(new IntPtr(pBuffer), (int)exactLength);
				}
				
				// TODO:  Check how the trimming and the last 0 charater works
				// The API might or might not include terminating null at the end
			}
			if (trim)
				managedString = managedString.TrimEnd('\0');
			return managedString;
		}
	}
}

#pragma warning restore 1591
