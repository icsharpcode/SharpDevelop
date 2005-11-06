// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Interop.CorDebug
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [ComImport, Guid("096E81D5-ECDA-4202-83F5-C65980A9EF75"), InterfaceType((short) 1)]
    public interface ICorDebugAppDomain2
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetArrayOrPointerType([In] uint elementType, [In] uint nRank, [In, MarshalAs(UnmanagedType.Interface)] ICorDebugType pTypeArg, [MarshalAs(UnmanagedType.Interface)] out ICorDebugType ppType);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetFunctionPointerType([In] uint nTypeArgs, [In, MarshalAs(UnmanagedType.Interface)] ref ICorDebugType ppTypeArgs, [MarshalAs(UnmanagedType.Interface)] out ICorDebugType ppType);
    }
}
