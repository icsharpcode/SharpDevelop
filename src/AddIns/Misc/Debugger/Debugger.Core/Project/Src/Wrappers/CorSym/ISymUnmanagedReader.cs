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
	
	public partial class ISymUnmanagedReader
	{
		public ISymUnmanagedDocument GetDocument(string url, System.Guid language, System.Guid languageVendor, System.Guid documentType)
		{
			IntPtr p = Marshal.StringToCoTaskMemUni(url);
			ISymUnmanagedDocument res = GetDocument(p, language, languageVendor, documentType);
			Marshal.FreeCoTaskMem(p);
			return res;
		}
	}
}

#pragma warning restore 1591
