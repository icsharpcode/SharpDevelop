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

    [ComImport, Guid("B01FAFEB-C450-3A4D-BEEC-B4CEEC01E006"), InterfaceType((short) 1)]
    public interface ISymUnmanagedDocumentWriter
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void SetSource([In] uint sourceSize, [In] ref byte source);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void SetCheckSum([In] Guid algorithmId, [In] uint checkSumSize, [In] ref byte checkSum);
    }
}

#pragma warning restore 108, 1591