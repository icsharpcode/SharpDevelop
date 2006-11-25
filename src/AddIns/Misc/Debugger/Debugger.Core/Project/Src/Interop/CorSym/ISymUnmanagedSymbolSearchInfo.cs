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

    [ComImport, ComConversionLoss, InterfaceType((short) 1), Guid("F8B3534A-A46B-4980-B520-BEC4ACEABA8F")]
    public interface ISymUnmanagedSymbolSearchInfo
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetSearchPathLength(out uint pcchPath);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetSearchPath([In] uint cchPath, out uint pcchPath, [Out] IntPtr szPath);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetHRESULT([MarshalAs(UnmanagedType.Error)] out int phr);
    }
}

#pragma warning restore 108, 1591