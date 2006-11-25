// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

#pragma warning disable 108, 1591 

namespace Debugger.Interop.CorSym
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [ComImport, InterfaceType((short) 1), Guid("AA544D42-28CB-11D3-BD22-0000F80849BD")]
    public interface ISymUnmanagedBinder
    {
        [return: MarshalAs(UnmanagedType.Interface)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        ISymUnmanagedReader GetReaderForFile([In, MarshalAs(UnmanagedType.IUnknown)] object importer, [In] IntPtr filename, [In] IntPtr searchPath);
        [return: MarshalAs(UnmanagedType.Interface)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        ISymUnmanagedReader GetReaderFromStream([In, MarshalAs(UnmanagedType.IUnknown)] object importer, [In, MarshalAs(UnmanagedType.Interface)] IStream pstream);
    }
}

#pragma warning restore 108, 1591