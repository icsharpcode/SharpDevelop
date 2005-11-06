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

    [ComImport, Guid("7FCC5FB5-49C0-41DE-9938-3B88B5B9ADD7"), InterfaceType((short) 1)]
    public interface ICorDebugModule2
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void SetJMCStatus([In] int bIsJustMyCode, [In] uint cTokens, [In] ref uint pTokens);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void ApplyChanges([In] uint cbMetadata, [In, MarshalAs(UnmanagedType.LPArray)] byte[] pbMetadata, [In] uint cbIL, [In, MarshalAs(UnmanagedType.LPArray)] byte[] pbIL);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void SetJITCompilerFlags([In] uint dwFlags);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetJITCompilerFlags(out uint pdwFlags);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void ResolveAssembly([In] uint tkAssemblyRef, [In, MarshalAs(UnmanagedType.Interface)] ref ICorDebugAssembly ppAssembly);
    }
}
