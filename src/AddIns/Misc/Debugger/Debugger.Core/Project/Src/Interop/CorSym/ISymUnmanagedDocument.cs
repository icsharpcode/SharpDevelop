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

    [ComImport, Guid("40DE4037-7C81-3E1E-B022-AE1ABFF2CA08"), ComConversionLoss, InterfaceType((short) 1)]
    public interface ISymUnmanagedDocument
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetURL([In] uint cchUrl, out uint pcchUrl, [Out] IntPtr szUrl);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        Guid GetDocumentType();
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        Guid GetLanguage();
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        Guid GetLanguageVendor();
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        Guid GetCheckSumAlgorithmId();
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetCheckSum([In] uint cData, out uint pcData, [Out] IntPtr data);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        uint FindClosestLine([In] uint line);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        int HasEmbeddedSource();
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        uint GetSourceLength();
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetSourceRange([In] uint startLine, [In] uint startColumn, [In] uint endLine, [In] uint endColumn, [In] uint cSourceBytes, out uint pcSourceBytes, [Out] IntPtr source);
    }
}

#pragma warning restore 108, 1591