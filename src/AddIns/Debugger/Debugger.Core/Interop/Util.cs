// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

#pragma warning disable 1591

using System;
using System.Runtime.InteropServices;

namespace Debugger.Interop
{
	public delegate void UnmanagedStringGetter(uint pStringLength, out uint stringLength, System.IntPtr pString);
	
	public static class Util
	{
		const uint DefaultBufferSize = 16;
		
		public static unsafe string GetCorSymString(UnmanagedStringGetter getter)
		{
			// CorSym does not support truncated result - it thorws exception and does not set actualLength (even with preserve sig)
			// ICorDebugStringValue does not support 0 as buffer size
			
			uint actualLength;
			getter(0, out actualLength, IntPtr.Zero);
			char[] buffer = new char[(int)actualLength];
			string managedString;
			fixed(char* pBuffer = buffer) {
				getter(actualLength, out actualLength, new IntPtr(pBuffer));
				managedString = Marshal.PtrToStringUni(new IntPtr(pBuffer), (int)actualLength);
			}
			managedString = managedString.TrimEnd('\0');
			return managedString;
		}
		
		public static string GetString(UnmanagedStringGetter getter)
		{
			return GetString(getter, DefaultBufferSize, true);
		}
		
		public static unsafe string GetString(UnmanagedStringGetter getter, uint defaultBufferSize, bool trimNull)
		{
			string managedString;
			
			// DebugStringValue does not like buffer size of 0
			defaultBufferSize = Math.Max(defaultBufferSize, 1);
			
			char[] buffer = new char[(int)defaultBufferSize];
			fixed(char* pBuffer = buffer) {
				uint actualLength = 0;
				getter(defaultBufferSize, out actualLength, new IntPtr(pBuffer));
				
				if(actualLength > defaultBufferSize) {
					char[] buffer2 = new char[(int)actualLength];
					fixed(char* pBuffer2 = buffer2) {
						getter(actualLength, out actualLength, new IntPtr(pBuffer2));
						managedString = Marshal.PtrToStringUni(new IntPtr(pBuffer2), (int)actualLength);
					}
				} else {
					managedString = Marshal.PtrToStringUni(new IntPtr(pBuffer), (int)actualLength);
				}
			}
			if (trimNull)
				managedString = managedString.TrimEnd('\0');
			return managedString;
		}
	}
}

#pragma warning restore 1591
