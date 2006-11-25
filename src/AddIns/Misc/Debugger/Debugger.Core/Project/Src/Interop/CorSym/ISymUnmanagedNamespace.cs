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

    [ComImport, InterfaceType((short) 1), ComConversionLoss, Guid("0DFF7289-54F8-11D3-BD28-0000F80849BD")]
    public interface ISymUnmanagedNamespace
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetName([In] uint cchName, out uint pcchName, [Out] IntPtr szName);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetNamespaces([In] uint cNameSpaces, out uint pcNameSpaces, [Out] IntPtr namespaces);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetVariables([In] uint cVars, out uint pcVars, [Out] IntPtr pVars);
    }
}

#pragma warning restore 108, 1591