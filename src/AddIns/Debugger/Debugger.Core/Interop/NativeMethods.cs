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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

using Debugger.Interop.CorDebug;

namespace Debugger.Interop
{
	public static class NativeMethods
	{
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool CloseHandle(IntPtr handle);
		
		[DllImport("mscoree.dll", CharSet=CharSet.Unicode, PreserveSig=false)]
		public static extern Debugger.Interop.CorDebug.ICorDebug CreateDebuggingInterfaceFromVersion(int debuggerVersion, string debuggeeVersion);
		
		[DllImport("mscoree.dll", CharSet=CharSet.Unicode)]
		public static extern int GetCORVersion([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder szName, Int32 cchBuffer, out Int32 dwLength);
		
		[DllImport("mscoree.dll", CharSet=CharSet.Unicode)]
		public static extern int GetRequestedRuntimeVersion(string exeFilename, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pVersion, Int32 cchBuffer, out Int32 dwLength);
	}
}

#pragma warning restore 1591
