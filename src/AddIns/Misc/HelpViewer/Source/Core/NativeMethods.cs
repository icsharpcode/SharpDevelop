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

using System;
using System.Runtime.InteropServices;

namespace MSHelpSystem.Core.Native
{
	internal sealed class NativeMethods
	{
		NativeMethods()
		{
		}

		[DllImport("Wtsapi32.dll")]
		internal static extern bool WTSQuerySessionInformation(IntPtr hServer, int sessionId, WTSInfoClass wtsInfoClass, out IntPtr ppBuffer, out uint pBytesReturned);

		[DllImport("Wtsapi32.dll", ExactSpelling = true, SetLastError = false)]
		public static extern void WTSFreeMemory(IntPtr memory);

		internal enum WTSInfoClass
		{
	        WTSInitialProgram,
	        WTSApplicationName,
	        WTSWorkingDirectory,
	        WTSOEMId,
	        WTSSessionId,
	        WTSUserName,
	        WTSWinStationName,
	        WTSDomainName,
	        WTSConnectState,
	        WTSClientBuildNumber,
	        WTSClientName,
	        WTSClientDirectory,
	        WTSClientProductId,
	        WTSClientHardwareId,
	        WTSClientAddress,
	        WTSClientDisplay,
	        WTSClientProtocolType
		}

		internal static IntPtr WTS_CURRENT_SERVER_HANDLE = IntPtr.Zero;
		internal static int WTS_CURRENT_SESSION = -1;

		public static int GetSessionId()
		{
			IntPtr pSessionId = IntPtr.Zero;
			Int32 sessionId = 0;
			uint bytesReturned;
			
			try {
				bool returnValue = WTSQuerySessionInformation(WTS_CURRENT_SERVER_HANDLE, WTS_CURRENT_SESSION, WTSInfoClass.WTSSessionId, out pSessionId, out bytesReturned);
				if  (returnValue) sessionId = Marshal.ReadInt32(pSessionId);
			}
			finally {
				if (pSessionId != IntPtr.Zero) WTSFreeMemory(pSessionId);
			}
			return sessionId;
		}
	}		
}
