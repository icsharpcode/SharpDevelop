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

    [ComImport, Guid("20D9645D-03CD-4E34-9C11-9848A5B084F1"), InterfaceType((short) 1)]
    public interface ISymUnmanagedReaderSymbolSearchInfo
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetSymbolSearchInfoCount(out uint pcSearchInfo);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetSymbolSearchInfo([In] uint cSearchInfo, out uint pcSearchInfo, [MarshalAs(UnmanagedType.Interface)] out ISymUnmanagedSymbolSearchInfo rgpSearchInfo);
    }
}

#pragma warning restore 108, 1591