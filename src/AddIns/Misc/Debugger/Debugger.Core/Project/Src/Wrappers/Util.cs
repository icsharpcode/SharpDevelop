// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Debugger.Wrappers
{
	public delegate void UnmanagedStringGetter(uint bufferSize, out uint returnedSize, System.IntPtr pString);
	
	public static class Util
	{
		public static string GetString(UnmanagedStringGetter getter)
		{
			uint pStringLenght = 0;
			IntPtr pString = IntPtr.Zero;
			getter(pStringLenght, out pStringLenght, pString);
			// Allocate string buffer
			pString = Marshal.AllocHGlobal((int)pStringLenght * 2);
			getter(pStringLenght, out pStringLenght, pString);
			string str = Marshal.PtrToStringUni(pString);
			// Release buffer
			Marshal.FreeHGlobal(pString);
			return str;
		}
	}
}
