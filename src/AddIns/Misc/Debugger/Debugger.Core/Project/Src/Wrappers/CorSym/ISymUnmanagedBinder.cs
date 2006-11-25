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
	
	public partial class ISymUnmanagedBinder
	{
		public ISymUnmanagedReader GetReaderForFile(object importer, string filename, string searchPath)
		{
			IntPtr pfilename = Marshal.StringToCoTaskMemUni(filename);
			IntPtr psearchPath = Marshal.StringToCoTaskMemUni(searchPath);
			ISymUnmanagedReader res = GetReaderForFile(importer, pfilename, psearchPath);
			Marshal.FreeCoTaskMem(pfilename);
			Marshal.FreeCoTaskMem(psearchPath);
			return res;
		}
	}
}

#pragma warning restore 1591
