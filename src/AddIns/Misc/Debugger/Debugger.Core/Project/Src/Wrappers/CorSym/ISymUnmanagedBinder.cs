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
		public static ISymUnmanagedReader GetReaderForFile(this ISymUnmanagedBinder symBinder, object importer, string filename, string searchPath)
		{
			IntPtr pfilename = Marshal.StringToCoTaskMemUni(filename);
			IntPtr psearchPath = Marshal.StringToCoTaskMemUni(searchPath);
			ISymUnmanagedReader res = symBinder.GetReaderForFile(importer, pfilename, psearchPath);
			Marshal.FreeCoTaskMem(pfilename);
			Marshal.FreeCoTaskMem(psearchPath);
			return res;
		}
	}
}

#pragma warning restore 1591
