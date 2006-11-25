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

    [ComImport, Guid("68005D0F-B8E0-3B01-84D5-A11A94154942"), ComConversionLoss, InterfaceType((short) 1)]
    public interface ISymUnmanagedScope
    {
        [return: MarshalAs(UnmanagedType.Interface)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        ISymUnmanagedMethod GetMethod();
        [return: MarshalAs(UnmanagedType.Interface)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        ISymUnmanagedScope GetParent();
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetChildren([In] uint cChildren, out uint pcChildren, [Out, MarshalAs(UnmanagedType.LPArray)] ISymUnmanagedScope[] children);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        uint GetStartOffset();
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        uint GetEndOffset();
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        uint GetLocalCount();
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetLocals([In] uint cLocals, out uint pcLocals, [Out, MarshalAs(UnmanagedType.LPArray)] ISymUnmanagedVariable[] locals);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetNamespaces([In] uint cNameSpaces, out uint pcNameSpaces, [Out, MarshalAs(UnmanagedType.LPArray)] ISymUnmanagedNamespace[] namespaces);
    }
}

#pragma warning restore 108, 1591