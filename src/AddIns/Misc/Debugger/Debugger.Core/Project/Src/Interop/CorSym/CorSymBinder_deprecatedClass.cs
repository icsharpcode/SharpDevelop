// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Interop.CorSym
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [ComImport, Guid("AA544D41-28CB-11D3-BD22-0000F80849BD"), ClassInterface((short) 0), TypeLibType((short) 2)]
    public class CorSymBinder_deprecatedClass : ISymUnmanagedBinder, CorSymBinder_deprecated
    {
        // Methods
        [return: MarshalAs(UnmanagedType.Interface)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        public virtual extern ISymUnmanagedReader GetReaderForFile([In, MarshalAs(UnmanagedType.IUnknown)] object importer, [In] ref ushort filename, [In] ref ushort searchPath);

        [return: MarshalAs(UnmanagedType.Interface)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        public virtual extern ISymUnmanagedReader GetReaderFromStream([In, MarshalAs(UnmanagedType.IUnknown)] object importer, [In, MarshalAs(UnmanagedType.Interface)] IStream pstream);

    }
}

