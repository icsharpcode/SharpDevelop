// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

#pragma warning disable 1591

namespace Debugger.Interop.CorSym
{
	using System;
	using System.Runtime.InteropServices;

	public static partial class CorSymExtensionMethods
	{
		public static string GetURL(this ISymUnmanagedDocument symDoc)
		{
			return Util.GetString(symDoc.GetURL, 256, true);
		}
		
		public static byte[] GetCheckSum(this ISymUnmanagedDocument symDoc)
		{
			uint checkSumLength = 0;
			symDoc.GetCheckSum(checkSumLength, out checkSumLength, IntPtr.Zero);
			IntPtr checkSumPtr = Marshal.AllocHGlobal((int)checkSumLength);
			symDoc.GetCheckSum(checkSumLength, out checkSumLength, checkSumPtr);
			byte[] checkSumBytes = new byte[checkSumLength];
			Marshal.Copy(checkSumPtr, checkSumBytes, 0, (int)checkSumLength);
			Marshal.FreeHGlobal(checkSumPtr);
			return checkSumBytes;
		}
	}
}

#pragma warning restore 1591
