// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

#pragma warning disable 1591

namespace Debugger.Wrappers.CorSym
{
	using System;
	using System.Runtime.InteropServices;

	public partial class ISymUnmanagedDocument
	{
		public string URL {
			get {
				return Util.GetString(GetURL, 256, true);
			}
		}
		
		public byte[] CheckSum {
			get {
				uint checkSumLength = 0;
				GetCheckSum(checkSumLength, out checkSumLength, IntPtr.Zero);
				IntPtr checkSumPtr = Marshal.AllocHGlobal((int)checkSumLength);
				GetCheckSum(checkSumLength, out checkSumLength, checkSumPtr);
				byte[] checkSumBytes = new byte[checkSumLength];
				Marshal.Copy(checkSumPtr, checkSumBytes, 0, (int)checkSumLength);
				Marshal.FreeHGlobal(checkSumPtr);
				return checkSumBytes;
			}
		}
	}
}

#pragma warning restore 1591
