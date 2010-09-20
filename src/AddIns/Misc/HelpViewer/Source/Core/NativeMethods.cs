// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
