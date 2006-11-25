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

    [ComImport, ComConversionLoss, InterfaceType((short) 1), Guid("ED14AA72-78E2-4884-84E2-334293AE5214")]
    public interface ISymUnmanagedWriter
    {
        [return: MarshalAs(UnmanagedType.Interface)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        ISymUnmanagedDocumentWriter DefineDocument([In] IntPtr url, [In] ref Guid language, [In] ref Guid languageVendor, [In] ref Guid documentType);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void SetUserEntryPoint([In] uint entryMethod);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void OpenMethod([In] uint method);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void CloseMethod();
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        uint OpenScope([In] uint startOffset);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void CloseScope([In] uint endOffset);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void SetScopeRange([In] uint scopeID, [In] uint startOffset, [In] uint endOffset);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void DefineLocalVariable([In] IntPtr name, [In] uint attributes, [In] uint cSig, [In] ref byte signature, [In] uint addrKind, [In] uint addr1, [In] uint addr2, [In] uint addr3, [In] uint startOffset, [In] uint endOffset);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void DefineParameter([In] IntPtr name, [In] uint attributes, [In] uint sequence, [In] uint addrKind, [In] uint addr1, [In] uint addr2, [In] uint addr3);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void DefineField([In] uint parent, [In] IntPtr name, [In] uint attributes, [In] uint cSig, [In] ref byte signature, [In] uint addrKind, [In] uint addr1, [In] uint addr2, [In] uint addr3);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void DefineGlobalVariable([In] IntPtr name, [In] uint attributes, [In] uint cSig, [In] ref byte signature, [In] uint addrKind, [In] uint addr1, [In] uint addr2, [In] uint addr3);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void Close();
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void SetSymAttribute([In] uint parent, [In] IntPtr name, [In] uint cData, [In] ref byte data);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void OpenNamespace([In] IntPtr name);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void CloseNamespace();
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void UsingNamespace([In] IntPtr fullName);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void SetMethodSourceRange([In, MarshalAs(UnmanagedType.Interface)] ISymUnmanagedDocumentWriter startDoc, [In] uint startLine, [In] uint startColumn, [In, MarshalAs(UnmanagedType.Interface)] ISymUnmanagedDocumentWriter endDoc, [In] uint endLine, [In] uint endColumn);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void Initialize([In, MarshalAs(UnmanagedType.IUnknown)] object emitter, [In] IntPtr filename, [In, MarshalAs(UnmanagedType.Interface)] IStream pIStream, [In] int fFullBuild);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetDebugInfo([In] ref uint pIDD, [In] uint cData, out uint pcData, [Out] IntPtr data);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void DefineSequencePoints([In, MarshalAs(UnmanagedType.Interface)] ISymUnmanagedDocumentWriter document, [In] uint spCount, [In] ref uint offsets, [In] ref uint lines, [In] ref uint columns, [In] ref uint endLines, [In] ref uint endColumns);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void RemapToken([In] uint oldToken, [In] uint newToken);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void Initialize2([In, MarshalAs(UnmanagedType.IUnknown)] object emitter, [In] IntPtr tempfilename, [In, MarshalAs(UnmanagedType.Interface)] IStream pIStream, [In] int fFullBuild, [In] IntPtr finalfilename);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void DefineConstant([In] IntPtr name, [In, MarshalAs(UnmanagedType.Struct)] object value, [In] uint cSig, [In] ref byte signature);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void Abort();
    }
}

#pragma warning restore 108, 1591