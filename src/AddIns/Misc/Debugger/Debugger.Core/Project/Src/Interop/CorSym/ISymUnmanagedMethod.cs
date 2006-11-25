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

    [ComImport, InterfaceType((short) 1), ComConversionLoss, Guid("B62B923C-B500-3158-A543-24F307A8B7E1")]
    public interface ISymUnmanagedMethod
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        uint GetToken();
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        uint GetSequencePointCount();
        [return: MarshalAs(UnmanagedType.Interface)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        ISymUnmanagedScope GetRootScope();
        [return: MarshalAs(UnmanagedType.Interface)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        ISymUnmanagedScope GetScopeFromOffset([In] uint offset);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        uint GetOffset([In, MarshalAs(UnmanagedType.Interface)] ISymUnmanagedDocument document, [In] uint line, [In] uint column);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetRanges([In, MarshalAs(UnmanagedType.Interface)] ISymUnmanagedDocument document, [In] uint line, [In] uint column, [In] uint cRanges, out uint pcRanges, [Out] IntPtr ranges);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetParameters([In] uint cParams, out uint pcParams, [Out] IntPtr @params);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetNamespace([MarshalAs(UnmanagedType.Interface)] out ISymUnmanagedNamespace pRetVal);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetSourceStartEnd([In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=0, SizeConst=2)] ISymUnmanagedDocument[] docs, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=0, SizeConst=2)] uint[] lines, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=0, SizeConst=2)] uint[] columns, out int pRetVal);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetSequencePoints([In] uint cPoints, out uint pcPoints, [Out, MarshalAs(UnmanagedType.LPArray)] uint[] offsets, [Out, MarshalAs(UnmanagedType.LPArray)] ISymUnmanagedDocument[] documents, [Out, MarshalAs(UnmanagedType.LPArray)] uint[] lines, [Out, MarshalAs(UnmanagedType.LPArray)] uint[] columns, [Out, MarshalAs(UnmanagedType.LPArray)] uint[] endLines, [Out, MarshalAs(UnmanagedType.LPArray)] uint[] endColumns);
    }
}

#pragma warning restore 108, 1591