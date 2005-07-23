// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

// - GetReaderForFile
// - GetSequencePoints
// - GetRanges
// - GetSourceStartEnd
// - GetDocument

//
// ============== CLASS STRUCTURE DECLARATION ==================
//
.namespace DebuggerInterop.Symbols
{
  .class public auto ansi import CorSymWriterClass
         extends [mscorlib]System.Object
         implements DebuggerInterop.Symbols.ISymUnmanagedWriter,
                    DebuggerInterop.Symbols.CorSymWriter
  {
  } // end of class CorSymWriterClass

  .class interface public abstract auto ansi import CorSymWriter
         implements DebuggerInterop.Symbols.ISymUnmanagedWriter
  {
  } // end of class CorSymWriter

  .class interface public abstract auto ansi import ISymUnmanagedWriter
  {
  } // end of class ISymUnmanagedWriter

  .class interface public abstract auto ansi import ISymUnmanagedDocumentWriter
  {
  } // end of class ISymUnmanagedDocumentWriter

  .class interface public abstract auto ansi import IStream
         implements DebuggerInterop.Symbols.ISequentialStream
  {
  } // end of class IStream

  .class interface public abstract auto ansi import ISequentialStream
  {
  } // end of class ISequentialStream

  .class public sequential ansi sealed beforefieldinit _LARGE_INTEGER
         extends [mscorlib]System.ValueType
  {
  } // end of class _LARGE_INTEGER

  .class public sequential ansi sealed beforefieldinit _ULARGE_INTEGER
         extends [mscorlib]System.ValueType
  {
  } // end of class _ULARGE_INTEGER

  .class public sequential ansi sealed beforefieldinit tagSTATSTG
         extends [mscorlib]System.ValueType
  {
  } // end of class tagSTATSTG

  .class public sequential ansi sealed beforefieldinit _FILETIME
         extends [mscorlib]System.ValueType
  {
  } // end of class _FILETIME

  .class public auto ansi import CorSymReaderClass
         extends [mscorlib]System.Object
         implements DebuggerInterop.Symbols.ISymUnmanagedReader,
                    DebuggerInterop.Symbols.CorSymReader
  {
  } // end of class CorSymReaderClass

  .class interface public abstract auto ansi import CorSymReader
         implements DebuggerInterop.Symbols.ISymUnmanagedReader
  {
  } // end of class CorSymReader

  .class interface public abstract auto ansi import ISymUnmanagedReader
  {
  } // end of class ISymUnmanagedReader

  .class interface public abstract auto ansi import ISymUnmanagedDocument
  {
  } // end of class ISymUnmanagedDocument

  .class interface public abstract auto ansi import ISymUnmanagedMethod
  {
  } // end of class ISymUnmanagedMethod

  .class interface public abstract auto ansi import ISymUnmanagedScope
  {
  } // end of class ISymUnmanagedScope

  .class interface public abstract auto ansi import ISymUnmanagedNamespace
  {
  } // end of class ISymUnmanagedNamespace

  .class public auto ansi import CorSymBinderClass
         extends [mscorlib]System.Object
         implements DebuggerInterop.Symbols.ISymUnmanagedBinder,
                    DebuggerInterop.Symbols.CorSymBinder
  {
  } // end of class CorSymBinderClass

  .class interface public abstract auto ansi import CorSymBinder
         implements DebuggerInterop.Symbols.ISymUnmanagedBinder
  {
  } // end of class CorSymBinder

  .class interface public abstract auto ansi import ISymUnmanagedBinder
  {
  } // end of class ISymUnmanagedBinder

  .class public auto ansi import CorSymWriter_SxSClass
         extends [mscorlib]System.Object
         implements DebuggerInterop.Symbols.ISymUnmanagedWriter,
                    DebuggerInterop.Symbols.CorSymWriter_SxS
  {
  } // end of class CorSymWriter_SxSClass

  .class interface public abstract auto ansi import CorSymWriter_SxS
         implements DebuggerInterop.Symbols.ISymUnmanagedWriter
  {
  } // end of class CorSymWriter_SxS

  .class public auto ansi import CorSymReader_SxSClass
         extends [mscorlib]System.Object
         implements DebuggerInterop.Symbols.ISymUnmanagedReader,
                    DebuggerInterop.Symbols.CorSymReader_SxS
  {
  } // end of class CorSymReader_SxSClass

  .class interface public abstract auto ansi import CorSymReader_SxS
         implements DebuggerInterop.Symbols.ISymUnmanagedReader
  {
  } // end of class CorSymReader_SxS

  .class public auto ansi import CorSymBinder_SxSClass
         extends [mscorlib]System.Object
         implements DebuggerInterop.Symbols.ISymUnmanagedBinder,
                    DebuggerInterop.Symbols.CorSymBinder_SxS
  {
  } // end of class CorSymBinder_SxSClass

  .class interface public abstract auto ansi import CorSymBinder_SxS
         implements DebuggerInterop.Symbols.ISymUnmanagedBinder
  {
  } // end of class CorSymBinder_SxS

  .class interface public abstract auto ansi import ISymUnmanagedVariable
  {
  } // end of class ISymUnmanagedVariable

  .class interface public abstract auto ansi import ISymUnmanagedWriter2
         implements DebuggerInterop.Symbols.ISymUnmanagedWriter
  {
  } // end of class ISymUnmanagedWriter2

} // end of namespace Debugger.Symbols


// =============================================================


// =============== GLOBAL FIELDS AND METHODS ===================


// =============================================================


// =============== CLASS MEMBERS DECLARATION ===================
//   note that class flags, 'extends' and 'implements' clauses
//          are provided here for information only

.namespace DebuggerInterop.Symbols
{
  .class public auto ansi import CorSymWriterClass
         extends [mscorlib]System.Object
         implements DebuggerInterop.Symbols.ISymUnmanagedWriter,
                    DebuggerInterop.Symbols.CorSymWriter
  {
    .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 31 30 38 32 39 36 43 31 2D 32 38 31 45   // ..$108296C1-281E
                                                                                                    2D 31 31 44 33 2D 42 44 32 32 2D 30 30 30 30 46   // -11D3-BD22-0000F
                                                                                                    38 30 38 34 39 42 44 00 00 )                      // 80849BD..
    .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.InteropServices.TypeLibTypeAttribute::.ctor(int16) = ( 01 00 02 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.InteropServices.ClassInterfaceAttribute::.ctor(int16) = ( 01 00 00 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor() runtime managed internalcall
    {
    } // end of method CorSymWriterClass::.ctor

    .method public hidebysig newslot virtual 
            instance class DebuggerInterop.Symbols.ISymUnmanagedDocumentWriter 
            marshal( interface) 
            DefineDocument([in] unsigned int16& url,
                           [in] valuetype [mscorlib]System.Guid& language,
                           [in] valuetype [mscorlib]System.Guid& languageVendor,
                           [in] valuetype [mscorlib]System.Guid& documentType) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedWriter::DefineDocument
    } // end of method CorSymWriterClass::DefineDocument

    .method public hidebysig newslot virtual 
            instance void  SetUserEntryPoint([in] unsigned int32 entryMethod) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedWriter::SetUserEntryPoint
    } // end of method CorSymWriterClass::SetUserEntryPoint

    .method public hidebysig newslot virtual 
            instance void  OpenMethod([in] unsigned int32 'method') runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedWriter::OpenMethod
    } // end of method CorSymWriterClass::OpenMethod

    .method public hidebysig newslot virtual 
            instance void  CloseMethod() runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedWriter::CloseMethod
    } // end of method CorSymWriterClass::CloseMethod

    .method public hidebysig newslot virtual 
            instance unsigned int32  OpenScope([in] unsigned int32 startOffset) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedWriter::OpenScope
    } // end of method CorSymWriterClass::OpenScope

    .method public hidebysig newslot virtual 
            instance void  CloseScope([in] unsigned int32 endOffset) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedWriter::CloseScope
    } // end of method CorSymWriterClass::CloseScope

    .method public hidebysig newslot virtual 
            instance void  SetScopeRange([in] unsigned int32 scopeID,
                                         [in] unsigned int32 startOffset,
                                         [in] unsigned int32 endOffset) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedWriter::SetScopeRange
    } // end of method CorSymWriterClass::SetScopeRange

    .method public hidebysig newslot virtual 
            instance void  DefineLocalVariable([in] unsigned int16& name,
                                               [in] unsigned int32 attributes,
                                               [in] unsigned int32 cSig,
                                               [in] unsigned int8& signature,
                                               [in] unsigned int32 addrKind,
                                               [in] unsigned int32 addr1,
                                               [in] unsigned int32 addr2,
                                               [in] unsigned int32 addr3,
                                               [in] unsigned int32 startOffset,
                                               [in] unsigned int32 endOffset) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedWriter::DefineLocalVariable
    } // end of method CorSymWriterClass::DefineLocalVariable

    .method public hidebysig newslot virtual 
            instance void  DefineParameter([in] unsigned int16& name,
                                           [in] unsigned int32 attributes,
                                           [in] unsigned int32 sequence,
                                           [in] unsigned int32 addrKind,
                                           [in] unsigned int32 addr1,
                                           [in] unsigned int32 addr2,
                                           [in] unsigned int32 addr3) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedWriter::DefineParameter
    } // end of method CorSymWriterClass::DefineParameter

    .method public hidebysig newslot virtual 
            instance void  DefineField([in] unsigned int32 parent,
                                       [in] unsigned int16& name,
                                       [in] unsigned int32 attributes,
                                       [in] unsigned int32 cSig,
                                       [in] unsigned int8& signature,
                                       [in] unsigned int32 addrKind,
                                       [in] unsigned int32 addr1,
                                       [in] unsigned int32 addr2,
                                       [in] unsigned int32 addr3) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedWriter::DefineField
    } // end of method CorSymWriterClass::DefineField

    .method public hidebysig newslot virtual 
            instance void  DefineGlobalVariable([in] unsigned int16& name,
                                                [in] unsigned int32 attributes,
                                                [in] unsigned int32 cSig,
                                                [in] unsigned int8& signature,
                                                [in] unsigned int32 addrKind,
                                                [in] unsigned int32 addr1,
                                                [in] unsigned int32 addr2,
                                                [in] unsigned int32 addr3) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedWriter::DefineGlobalVariable
    } // end of method CorSymWriterClass::DefineGlobalVariable

    .method public hidebysig newslot virtual 
            instance void  Close() runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedWriter::Close
    } // end of method CorSymWriterClass::Close

    .method public hidebysig newslot virtual 
            instance void  SetSymAttribute([in] unsigned int32 parent,
                                           [in] unsigned int16& name,
                                           [in] unsigned int32 cData,
                                           [in] unsigned int8& data) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedWriter::SetSymAttribute
    } // end of method CorSymWriterClass::SetSymAttribute

    .method public hidebysig newslot virtual 
            instance void  OpenNamespace([in] unsigned int16& name) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedWriter::OpenNamespace
    } // end of method CorSymWriterClass::OpenNamespace

    .method public hidebysig newslot virtual 
            instance void  CloseNamespace() runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedWriter::CloseNamespace
    } // end of method CorSymWriterClass::CloseNamespace

    .method public hidebysig newslot virtual 
            instance void  UsingNamespace([in] unsigned int16& fullName) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedWriter::UsingNamespace
    } // end of method CorSymWriterClass::UsingNamespace

    .method public hidebysig newslot virtual 
            instance void  SetMethodSourceRange([in] class DebuggerInterop.Symbols.ISymUnmanagedDocumentWriter  marshal( interface) startDoc,
                                                [in] unsigned int32 startLine,
                                                [in] unsigned int32 startColumn,
                                                [in] class DebuggerInterop.Symbols.ISymUnmanagedDocumentWriter  marshal( interface) endDoc,
                                                [in] unsigned int32 endLine,
                                                [in] unsigned int32 endColumn) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedWriter::SetMethodSourceRange
    } // end of method CorSymWriterClass::SetMethodSourceRange

    .method public hidebysig newslot virtual 
            instance void  Initialize([in] object  marshal( iunknown) emitter,
                                      [in] unsigned int16& filename,
                                      [in] class DebuggerInterop.Symbols.IStream  marshal( interface) pIStream,
                                      [in] int32 fFullBuild) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedWriter::Initialize
    } // end of method CorSymWriterClass::Initialize

    .method public hidebysig newslot virtual 
            instance void  GetDebugInfo([in][out] unsigned int32& pIDD,
                                        [in] unsigned int32 cData,
                                        [out] unsigned int32& pcData,
                                        [out] native int data) runtime managed internalcall
    {
      .param [1]
      .custom instance void [mscorlib]System.Runtime.InteropServices.ComAliasNameAttribute::.ctor(string) = ( 01 00 13 43 6F 72 53 79 6D 4C 69 62 2E 55 4C 4F   // ...DebuggerInterop.Symbols.ULO
                                                                                                              4E 47 5F 50 54 52 00 00 )                         // NG_PTR..
      .override DebuggerInterop.Symbols.ISymUnmanagedWriter::GetDebugInfo
    } // end of method CorSymWriterClass::GetDebugInfo

    .method public hidebysig newslot virtual 
            instance void  DefineSequencePoints([in] class DebuggerInterop.Symbols.ISymUnmanagedDocumentWriter  marshal( interface) document,
                                                [in] unsigned int32 spCount,
                                                [in] unsigned int32& offsets,
                                                [in] unsigned int32& lines,
                                                [in] unsigned int32& columns,
                                                [in] unsigned int32& endLines,
                                                [in] unsigned int32& endColumns) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedWriter::DefineSequencePoints
    } // end of method CorSymWriterClass::DefineSequencePoints

    .method public hidebysig newslot virtual 
            instance void  RemapToken([in] unsigned int32 oldToken,
                                      [in] unsigned int32 newToken) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedWriter::RemapToken
    } // end of method CorSymWriterClass::RemapToken

    .method public hidebysig newslot virtual 
            instance void  Initialize2([in] object  marshal( iunknown) emitter,
                                       [in] unsigned int16& tempfilename,
                                       [in] class DebuggerInterop.Symbols.IStream  marshal( interface) pIStream,
                                       [in] int32 fFullBuild,
                                       [in] unsigned int16& finalfilename) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedWriter::Initialize2
    } // end of method CorSymWriterClass::Initialize2

    .method public hidebysig newslot virtual 
            instance void  DefineConstant([in] unsigned int16& name,
                                          [in] object  marshal( struct) 'value',
                                          [in] unsigned int32 cSig,
                                          [in] unsigned int8& signature) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedWriter::DefineConstant
    } // end of method CorSymWriterClass::DefineConstant

    .method public hidebysig newslot virtual 
            instance void  Abort() runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedWriter::Abort
    } // end of method CorSymWriterClass::Abort

  } // end of class CorSymWriterClass

  .class interface public abstract auto ansi import CorSymWriter
         implements DebuggerInterop.Symbols.ISymUnmanagedWriter
  {
    .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 45 44 31 34 41 41 37 32 2D 37 38 45 32   // ..$ED14AA72-78E2
                                                                                                    2D 34 38 38 34 2D 38 34 45 32 2D 33 33 34 32 39   // -4884-84E2-33429
                                                                                                    33 41 45 35 32 31 34 00 00 )                      // 3AE5214..
    .custom instance void [mscorlib]System.Runtime.InteropServices.CoClassAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 1B 43 6F 72 53 79 6D 4C 69 62 2E 43 6F 72   // ...DebuggerInterop.Symbols.Cor
                                                                                                                            53 79 6D 57 72 69 74 65 72 43 6C 61 73 73 00 00 ) // SymWriterClass..
  } // end of class CorSymWriter

  .class interface public abstract auto ansi import ISymUnmanagedWriter
  {
    .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 45 44 31 34 41 41 37 32 2D 37 38 45 32   // ..$ED14AA72-78E2
                                                                                                    2D 34 38 38 34 2D 38 34 45 32 2D 33 33 34 32 39   // -4884-84E2-33429
                                                                                                    33 41 45 35 32 31 34 00 00 )                      // 3AE5214..
    .custom instance void [mscorlib]System.Runtime.InteropServices.InterfaceTypeAttribute::.ctor(int16) = ( 01 00 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public hidebysig newslot abstract virtual 
            instance class DebuggerInterop.Symbols.ISymUnmanagedDocumentWriter 
            marshal( interface) 
            DefineDocument([in] unsigned int16& url,
                           [in] valuetype [mscorlib]System.Guid& language,
                           [in] valuetype [mscorlib]System.Guid& languageVendor,
                           [in] valuetype [mscorlib]System.Guid& documentType) runtime managed internalcall
    {
    } // end of method ISymUnmanagedWriter::DefineDocument

    .method public hidebysig newslot abstract virtual 
            instance void  SetUserEntryPoint([in] unsigned int32 entryMethod) runtime managed internalcall
    {
    } // end of method ISymUnmanagedWriter::SetUserEntryPoint

    .method public hidebysig newslot abstract virtual 
            instance void  OpenMethod([in] unsigned int32 'method') runtime managed internalcall
    {
    } // end of method ISymUnmanagedWriter::OpenMethod

    .method public hidebysig newslot abstract virtual 
            instance void  CloseMethod() runtime managed internalcall
    {
    } // end of method ISymUnmanagedWriter::CloseMethod

    .method public hidebysig newslot abstract virtual 
            instance unsigned int32  OpenScope([in] unsigned int32 startOffset) runtime managed internalcall
    {
    } // end of method ISymUnmanagedWriter::OpenScope

    .method public hidebysig newslot abstract virtual 
            instance void  CloseScope([in] unsigned int32 endOffset) runtime managed internalcall
    {
    } // end of method ISymUnmanagedWriter::CloseScope

    .method public hidebysig newslot abstract virtual 
            instance void  SetScopeRange([in] unsigned int32 scopeID,
                                         [in] unsigned int32 startOffset,
                                         [in] unsigned int32 endOffset) runtime managed internalcall
    {
    } // end of method ISymUnmanagedWriter::SetScopeRange

    .method public hidebysig newslot abstract virtual 
            instance void  DefineLocalVariable([in] unsigned int16& name,
                                               [in] unsigned int32 attributes,
                                               [in] unsigned int32 cSig,
                                               [in] unsigned int8& signature,
                                               [in] unsigned int32 addrKind,
                                               [in] unsigned int32 addr1,
                                               [in] unsigned int32 addr2,
                                               [in] unsigned int32 addr3,
                                               [in] unsigned int32 startOffset,
                                               [in] unsigned int32 endOffset) runtime managed internalcall
    {
    } // end of method ISymUnmanagedWriter::DefineLocalVariable

    .method public hidebysig newslot abstract virtual 
            instance void  DefineParameter([in] unsigned int16& name,
                                           [in] unsigned int32 attributes,
                                           [in] unsigned int32 sequence,
                                           [in] unsigned int32 addrKind,
                                           [in] unsigned int32 addr1,
                                           [in] unsigned int32 addr2,
                                           [in] unsigned int32 addr3) runtime managed internalcall
    {
    } // end of method ISymUnmanagedWriter::DefineParameter

    .method public hidebysig newslot abstract virtual 
            instance void  DefineField([in] unsigned int32 parent,
                                       [in] unsigned int16& name,
                                       [in] unsigned int32 attributes,
                                       [in] unsigned int32 cSig,
                                       [in] unsigned int8& signature,
                                       [in] unsigned int32 addrKind,
                                       [in] unsigned int32 addr1,
                                       [in] unsigned int32 addr2,
                                       [in] unsigned int32 addr3) runtime managed internalcall
    {
    } // end of method ISymUnmanagedWriter::DefineField

    .method public hidebysig newslot abstract virtual 
            instance void  DefineGlobalVariable([in] unsigned int16& name,
                                                [in] unsigned int32 attributes,
                                                [in] unsigned int32 cSig,
                                                [in] unsigned int8& signature,
                                                [in] unsigned int32 addrKind,
                                                [in] unsigned int32 addr1,
                                                [in] unsigned int32 addr2,
                                                [in] unsigned int32 addr3) runtime managed internalcall
    {
    } // end of method ISymUnmanagedWriter::DefineGlobalVariable

    .method public hidebysig newslot abstract virtual 
            instance void  Close() runtime managed internalcall
    {
    } // end of method ISymUnmanagedWriter::Close

    .method public hidebysig newslot abstract virtual 
            instance void  SetSymAttribute([in] unsigned int32 parent,
                                           [in] unsigned int16& name,
                                           [in] unsigned int32 cData,
                                           [in] unsigned int8& data) runtime managed internalcall
    {
    } // end of method ISymUnmanagedWriter::SetSymAttribute

    .method public hidebysig newslot abstract virtual 
            instance void  OpenNamespace([in] unsigned int16& name) runtime managed internalcall
    {
    } // end of method ISymUnmanagedWriter::OpenNamespace

    .method public hidebysig newslot abstract virtual 
            instance void  CloseNamespace() runtime managed internalcall
    {
    } // end of method ISymUnmanagedWriter::CloseNamespace

    .method public hidebysig newslot abstract virtual 
            instance void  UsingNamespace([in] unsigned int16& fullName) runtime managed internalcall
    {
    } // end of method ISymUnmanagedWriter::UsingNamespace

    .method public hidebysig newslot abstract virtual 
            instance void  SetMethodSourceRange([in] class DebuggerInterop.Symbols.ISymUnmanagedDocumentWriter  marshal( interface) startDoc,
                                                [in] unsigned int32 startLine,
                                                [in] unsigned int32 startColumn,
                                                [in] class DebuggerInterop.Symbols.ISymUnmanagedDocumentWriter  marshal( interface) endDoc,
                                                [in] unsigned int32 endLine,
                                                [in] unsigned int32 endColumn) runtime managed internalcall
    {
    } // end of method ISymUnmanagedWriter::SetMethodSourceRange

    .method public hidebysig newslot abstract virtual 
            instance void  Initialize([in] object  marshal( iunknown) emitter,
                                      [in] unsigned int16& filename,
                                      [in] class DebuggerInterop.Symbols.IStream  marshal( interface) pIStream,
                                      [in] int32 fFullBuild) runtime managed internalcall
    {
    } // end of method ISymUnmanagedWriter::Initialize

    .method public hidebysig newslot abstract virtual 
            instance void  GetDebugInfo([in][out] unsigned int32& pIDD,
                                        [in] unsigned int32 cData,
                                        [out] unsigned int32& pcData,
                                        [out] native int data) runtime managed internalcall
    {
      .param [1]
      .custom instance void [mscorlib]System.Runtime.InteropServices.ComAliasNameAttribute::.ctor(string) = ( 01 00 13 43 6F 72 53 79 6D 4C 69 62 2E 55 4C 4F   // ...DebuggerInterop.Symbols.ULO
                                                                                                              4E 47 5F 50 54 52 00 00 )                         // NG_PTR..
    } // end of method ISymUnmanagedWriter::GetDebugInfo

    .method public hidebysig newslot abstract virtual 
            instance void  DefineSequencePoints([in] class DebuggerInterop.Symbols.ISymUnmanagedDocumentWriter  marshal( interface) document,
                                                [in] unsigned int32 spCount,
                                                [in] unsigned int32& offsets,
                                                [in] unsigned int32& lines,
                                                [in] unsigned int32& columns,
                                                [in] unsigned int32& endLines,
                                                [in] unsigned int32& endColumns) runtime managed internalcall
    {
    } // end of method ISymUnmanagedWriter::DefineSequencePoints

    .method public hidebysig newslot abstract virtual 
            instance void  RemapToken([in] unsigned int32 oldToken,
                                      [in] unsigned int32 newToken) runtime managed internalcall
    {
    } // end of method ISymUnmanagedWriter::RemapToken

    .method public hidebysig newslot abstract virtual 
            instance void  Initialize2([in] object  marshal( iunknown) emitter,
                                       [in] unsigned int16& tempfilename,
                                       [in] class DebuggerInterop.Symbols.IStream  marshal( interface) pIStream,
                                       [in] int32 fFullBuild,
                                       [in] unsigned int16& finalfilename) runtime managed internalcall
    {
    } // end of method ISymUnmanagedWriter::Initialize2

    .method public hidebysig newslot abstract virtual 
            instance void  DefineConstant([in] unsigned int16& name,
                                          [in] object  marshal( struct) 'value',
                                          [in] unsigned int32 cSig,
                                          [in] unsigned int8& signature) runtime managed internalcall
    {
    } // end of method ISymUnmanagedWriter::DefineConstant

    .method public hidebysig newslot abstract virtual 
            instance void  Abort() runtime managed internalcall
    {
    } // end of method ISymUnmanagedWriter::Abort

  } // end of class ISymUnmanagedWriter

  .class interface public abstract auto ansi import ISymUnmanagedDocumentWriter
  {
    .custom instance void [mscorlib]System.Runtime.InteropServices.InterfaceTypeAttribute::.ctor(int16) = ( 01 00 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 42 30 31 46 41 46 45 42 2D 43 34 35 30   // ..$B01FAFEB-C450
                                                                                                    2D 33 41 34 44 2D 42 45 45 43 2D 42 34 43 45 45   // -3A4D-BEEC-B4CEE
                                                                                                    43 30 31 45 30 30 36 00 00 )                      // C01E006..
    .method public hidebysig newslot abstract virtual 
            instance void  SetSource([in] unsigned int32 sourceSize,
                                     [in] unsigned int8& source) runtime managed internalcall
    {
    } // end of method ISymUnmanagedDocumentWriter::SetSource

    .method public hidebysig newslot abstract virtual 
            instance void  SetCheckSum([in] valuetype [mscorlib]System.Guid algorithmId,
                                       [in] unsigned int32 checkSumSize,
                                       [in] unsigned int8& checkSum) runtime managed internalcall
    {
    } // end of method ISymUnmanagedDocumentWriter::SetCheckSum

  } // end of class ISymUnmanagedDocumentWriter

  .class interface public abstract auto ansi import IStream
         implements DebuggerInterop.Symbols.ISequentialStream
  {
    .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 30 30 30 30 30 30 30 43 2D 30 30 30 30   // ..$0000000C-0000
                                                                                                    2D 30 30 30 30 2D 43 30 30 30 2D 30 30 30 30 30   // -0000-C000-00000
                                                                                                    30 30 30 30 30 34 36 00 00 )                      // 0000046..
    .custom instance void [mscorlib]System.Runtime.InteropServices.InterfaceTypeAttribute::.ctor(int16) = ( 01 00 01 00 00 00 ) 
    .method public hidebysig newslot abstract virtual 
            instance void  RemoteRead([out] unsigned int8& pv,
                                      [in] unsigned int32 cb,
                                      [out] unsigned int32& pcbRead) runtime managed internalcall
    {
    } // end of method IStream::RemoteRead

    .method public hidebysig newslot abstract virtual 
            instance void  RemoteWrite([in] unsigned int8& pv,
                                       [in] unsigned int32 cb,
                                       [out] unsigned int32& pcbWritten) runtime managed internalcall
    {
    } // end of method IStream::RemoteWrite

    .method public hidebysig newslot abstract virtual 
            instance void  RemoteSeek([in] valuetype DebuggerInterop.Symbols._LARGE_INTEGER dlibMove,
                                      [in] unsigned int32 dwOrigin,
                                      [out] valuetype DebuggerInterop.Symbols._ULARGE_INTEGER& plibNewPosition) runtime managed internalcall
    {
    } // end of method IStream::RemoteSeek

    .method public hidebysig newslot abstract virtual 
            instance void  SetSize([in] valuetype DebuggerInterop.Symbols._ULARGE_INTEGER libNewSize) runtime managed internalcall
    {
    } // end of method IStream::SetSize

    .method public hidebysig newslot abstract virtual 
            instance void  RemoteCopyTo([in] class DebuggerInterop.Symbols.IStream  marshal( interface) pstm,
                                        [in] valuetype DebuggerInterop.Symbols._ULARGE_INTEGER cb,
                                        [out] valuetype DebuggerInterop.Symbols._ULARGE_INTEGER& pcbRead,
                                        [out] valuetype DebuggerInterop.Symbols._ULARGE_INTEGER& pcbWritten) runtime managed internalcall
    {
    } // end of method IStream::RemoteCopyTo

    .method public hidebysig newslot abstract virtual 
            instance void  Commit([in] unsigned int32 grfCommitFlags) runtime managed internalcall
    {
    } // end of method IStream::Commit

    .method public hidebysig newslot abstract virtual 
            instance void  Revert() runtime managed internalcall
    {
    } // end of method IStream::Revert

    .method public hidebysig newslot abstract virtual 
            instance void  LockRegion([in] valuetype DebuggerInterop.Symbols._ULARGE_INTEGER libOffset,
                                      [in] valuetype DebuggerInterop.Symbols._ULARGE_INTEGER cb,
                                      [in] unsigned int32 dwLockType) runtime managed internalcall
    {
    } // end of method IStream::LockRegion

    .method public hidebysig newslot abstract virtual 
            instance void  UnlockRegion([in] valuetype DebuggerInterop.Symbols._ULARGE_INTEGER libOffset,
                                        [in] valuetype DebuggerInterop.Symbols._ULARGE_INTEGER cb,
                                        [in] unsigned int32 dwLockType) runtime managed internalcall
    {
    } // end of method IStream::UnlockRegion

    .method public hidebysig newslot abstract virtual 
            instance void  Stat([out] valuetype DebuggerInterop.Symbols.tagSTATSTG& pstatstg,
                                [in] unsigned int32 grfStatFlag) runtime managed internalcall
    {
    } // end of method IStream::Stat

    .method public hidebysig newslot abstract virtual 
            instance void  Clone([out] class DebuggerInterop.Symbols.IStream&  marshal( interface) ppstm) runtime managed internalcall
    {
    } // end of method IStream::Clone

  } // end of class IStream

  .class interface public abstract auto ansi import ISequentialStream
  {
    .custom instance void [mscorlib]System.Runtime.InteropServices.InterfaceTypeAttribute::.ctor(int16) = ( 01 00 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 30 43 37 33 33 41 33 30 2D 32 41 31 43   // ..$0C733A30-2A1C
                                                                                                    2D 31 31 43 45 2D 41 44 45 35 2D 30 30 41 41 30   // -11CE-ADE5-00AA0
                                                                                                    30 34 34 37 37 33 44 00 00 )                      // 044773D..
    .method public hidebysig newslot abstract virtual 
            instance void  RemoteRead([out] unsigned int8& pv,
                                      [in] unsigned int32 cb,
                                      [out] unsigned int32& pcbRead) runtime managed internalcall
    {
    } // end of method ISequentialStream::RemoteRead

    .method public hidebysig newslot abstract virtual 
            instance void  RemoteWrite([in] unsigned int8& pv,
                                       [in] unsigned int32 cb,
                                       [out] unsigned int32& pcbWritten) runtime managed internalcall
    {
    } // end of method ISequentialStream::RemoteWrite

  } // end of class ISequentialStream

  .class public sequential ansi sealed beforefieldinit _LARGE_INTEGER
         extends [mscorlib]System.ValueType
  {
    .pack 8
    .size 0
    .field public int64 QuadPart
  } // end of class _LARGE_INTEGER

  .class public sequential ansi sealed beforefieldinit _ULARGE_INTEGER
         extends [mscorlib]System.ValueType
  {
    .pack 8
    .size 0
    .field public unsigned int64 QuadPart
  } // end of class _ULARGE_INTEGER

  .class public sequential ansi sealed beforefieldinit tagSTATSTG
         extends [mscorlib]System.ValueType
  {
    .pack 8
    .size 0
    .field public  marshal( lpwstr) string pwcsName
    .field public unsigned int32 typeID
    .field public valuetype DebuggerInterop.Symbols._ULARGE_INTEGER cbSize
    .field public valuetype DebuggerInterop.Symbols._FILETIME mtime
    .field public valuetype DebuggerInterop.Symbols._FILETIME ctime
    .field public valuetype DebuggerInterop.Symbols._FILETIME atime
    .field public unsigned int32 grfMode
    .field public unsigned int32 grfLocksSupported
    .field public valuetype [mscorlib]System.Guid 'clsid'
    .field public unsigned int32 grfStateBits
    .field public unsigned int32 reserved
  } // end of class tagSTATSTG

  .class public sequential ansi sealed beforefieldinit _FILETIME
         extends [mscorlib]System.ValueType
  {
    .pack 4
    .size 0
    .field public unsigned int32 dwLowDateTime
    .field public unsigned int32 dwHighDateTime
  } // end of class _FILETIME

  .class public auto ansi import CorSymReaderClass
         extends [mscorlib]System.Object
         implements DebuggerInterop.Symbols.ISymUnmanagedReader,
                    DebuggerInterop.Symbols.CorSymReader
  {
    .custom instance void [mscorlib]System.Runtime.InteropServices.TypeLibTypeAttribute::.ctor(int16) = ( 01 00 02 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.InteropServices.ClassInterfaceAttribute::.ctor(int16) = ( 01 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 31 30 38 32 39 36 43 32 2D 32 38 31 45   // ..$108296C2-281E
                                                                                                    2D 31 31 44 33 2D 42 44 32 32 2D 30 30 30 30 46   // -11D3-BD22-0000F
                                                                                                    38 30 38 34 39 42 44 00 00 )                      // 80849BD..
    .method public specialname rtspecialname 
            instance void  .ctor() runtime managed internalcall
    {
    } // end of method CorSymReaderClass::.ctor

    .method public hidebysig newslot virtual 
            instance class DebuggerInterop.Symbols.ISymUnmanagedDocument 
            marshal( interface) 
            GetDocument([in] string  marshal( lpwstr) url,
                        [in] valuetype [mscorlib]System.Guid language,
                        [in] valuetype [mscorlib]System.Guid languageVendor,
                        [in] valuetype [mscorlib]System.Guid documentType) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedReader::GetDocument
    } // end of method CorSymReaderClass::GetDocument

    .method public hidebysig newslot virtual 
            instance void  GetDocuments([in] unsigned int32 cDocs,
                                        [out] unsigned int32& pcDocs,
                                        [out] native int pDocs) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedReader::GetDocuments
    } // end of method CorSymReaderClass::GetDocuments

    .method public hidebysig newslot virtual 
            instance unsigned int32  GetUserEntryPoint() runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedReader::GetUserEntryPoint
    } // end of method CorSymReaderClass::GetUserEntryPoint

    .method public hidebysig newslot virtual 
            instance class DebuggerInterop.Symbols.ISymUnmanagedMethod 
            marshal( interface) 
            GetMethod([in] unsigned int32 token) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedReader::GetMethod
    } // end of method CorSymReaderClass::GetMethod

    .method public hidebysig newslot virtual 
            instance class DebuggerInterop.Symbols.ISymUnmanagedMethod 
            marshal( interface) 
            GetMethodByVersion([in] unsigned int32 token,
                               [in] int32 version) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedReader::GetMethodByVersion
    } // end of method CorSymReaderClass::GetMethodByVersion

    .method public hidebysig newslot virtual 
            instance void  GetVariables([in] unsigned int32 parent,
                                        [in] unsigned int32 cVars,
                                        [out] unsigned int32& pcVars,
                                        [out] native int pVars) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedReader::GetVariables
    } // end of method CorSymReaderClass::GetVariables

    .method public hidebysig newslot virtual 
            instance void  GetGlobalVariables([in] unsigned int32 cVars,
                                              [out] unsigned int32& pcVars,
                                              [out] native int pVars) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedReader::GetGlobalVariables
    } // end of method CorSymReaderClass::GetGlobalVariables

    .method public hidebysig newslot virtual 
            instance class DebuggerInterop.Symbols.ISymUnmanagedMethod 
            marshal( interface) 
            GetMethodFromDocumentPosition([in] class DebuggerInterop.Symbols.ISymUnmanagedDocument  marshal( interface) document,
                                          [in] unsigned int32 line,
                                          [in] unsigned int32 column) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedReader::GetMethodFromDocumentPosition
    } // end of method CorSymReaderClass::GetMethodFromDocumentPosition

    .method public hidebysig newslot virtual 
            instance void  GetSymAttribute([in] unsigned int32 parent,
                                           [in] unsigned int16& name,
                                           [in] unsigned int32 cBuffer,
                                           [out] unsigned int32& pcBuffer,
                                           [out] native int buffer) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedReader::GetSymAttribute
    } // end of method CorSymReaderClass::GetSymAttribute

    .method public hidebysig newslot virtual 
            instance void  GetNamespaces([in] unsigned int32 cNameSpaces,
                                         [out] unsigned int32& pcNameSpaces,
                                         [out] native int namespaces) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedReader::GetNamespaces
    } // end of method CorSymReaderClass::GetNamespaces

    .method public hidebysig newslot virtual 
            instance void  Initialize([in] object  marshal( iunknown) importer,
                                      [in] unsigned int16& filename,
                                      [in] unsigned int16& searchPath,
                                      [in] class DebuggerInterop.Symbols.IStream  marshal( interface) pIStream) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedReader::Initialize
    } // end of method CorSymReaderClass::Initialize

    .method public hidebysig newslot virtual 
            instance void  UpdateSymbolStore([in] unsigned int16& filename,
                                             [in] class DebuggerInterop.Symbols.IStream  marshal( interface) pIStream) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedReader::UpdateSymbolStore
    } // end of method CorSymReaderClass::UpdateSymbolStore

    .method public hidebysig newslot virtual 
            instance void  ReplaceSymbolStore([in] unsigned int16& filename,
                                              [in] class DebuggerInterop.Symbols.IStream  marshal( interface) pIStream) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedReader::ReplaceSymbolStore
    } // end of method CorSymReaderClass::ReplaceSymbolStore

    .method public hidebysig newslot virtual 
            instance void  GetSymbolStoreFileName([in] unsigned int32 cchName,
                                                  [out] unsigned int32& pcchName,
                                                  [out] native int szName) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedReader::GetSymbolStoreFileName
    } // end of method CorSymReaderClass::GetSymbolStoreFileName

    .method public hidebysig newslot virtual 
            instance void  GetMethodsFromDocumentPosition([in] class DebuggerInterop.Symbols.ISymUnmanagedDocument  marshal( interface) document,
                                                          [in] unsigned int32 line,
                                                          [in] unsigned int32 column,
                                                          [in] unsigned int32 cMethod,
                                                          [out] unsigned int32& pcMethod,
                                                          [out] native int pRetVal) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedReader::GetMethodsFromDocumentPosition
    } // end of method CorSymReaderClass::GetMethodsFromDocumentPosition

    .method public hidebysig newslot virtual 
            instance void  GetDocumentVersion([in] class DebuggerInterop.Symbols.ISymUnmanagedDocument  marshal( interface) pDoc,
                                              [out] int32& version,
                                              [out] int32& pbCurrent) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedReader::GetDocumentVersion
    } // end of method CorSymReaderClass::GetDocumentVersion

    .method public hidebysig newslot virtual 
            instance void  GetMethodVersion([in] class DebuggerInterop.Symbols.ISymUnmanagedMethod  marshal( interface) pMethod,
                                            [out] int32& version) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedReader::GetMethodVersion
    } // end of method CorSymReaderClass::GetMethodVersion

  } // end of class CorSymReaderClass

  .class interface public abstract auto ansi import CorSymReader
         implements DebuggerInterop.Symbols.ISymUnmanagedReader
  {
    .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 42 34 43 45 36 32 38 36 2D 32 41 36 42   // ..$B4CE6286-2A6B
                                                                                                    2D 33 37 31 32 2D 41 33 42 37 2D 31 45 45 31 44   // -3712-A3B7-1EE1D
                                                                                                    41 44 34 36 37 42 35 00 00 )                      // AD467B5..
    .custom instance void [mscorlib]System.Runtime.InteropServices.CoClassAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 1B 43 6F 72 53 79 6D 4C 69 62 2E 43 6F 72   // ...DebuggerInterop.Symbols.Cor
                                                                                                                            53 79 6D 52 65 61 64 65 72 43 6C 61 73 73 00 00 ) // SymReaderClass..
  } // end of class CorSymReader

  .class interface public abstract auto ansi import ISymUnmanagedReader
  {
    .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 42 34 43 45 36 32 38 36 2D 32 41 36 42   // ..$B4CE6286-2A6B
                                                                                                    2D 33 37 31 32 2D 41 33 42 37 2D 31 45 45 31 44   // -3712-A3B7-1EE1D
                                                                                                    41 44 34 36 37 42 35 00 00 )                      // AD467B5..
    .custom instance void [mscorlib]System.Runtime.InteropServices.InterfaceTypeAttribute::.ctor(int16) = ( 01 00 01 00 00 00 ) 
    .method public hidebysig newslot abstract virtual 
            instance class DebuggerInterop.Symbols.ISymUnmanagedDocument 
            marshal( interface) 
            GetDocument([in] string  marshal( lpwstr) url,
                        [in] valuetype [mscorlib]System.Guid language,
                        [in] valuetype [mscorlib]System.Guid languageVendor,
                        [in] valuetype [mscorlib]System.Guid documentType) runtime managed internalcall
    {
    } // end of method ISymUnmanagedReader::GetDocument

    .method public hidebysig newslot abstract virtual 
            instance void  GetDocuments([in] unsigned int32 cDocs,
                                        [out] unsigned int32& pcDocs,
                                        [out] native int pDocs) runtime managed internalcall
    {
    } // end of method ISymUnmanagedReader::GetDocuments

    .method public hidebysig newslot abstract virtual 
            instance unsigned int32  GetUserEntryPoint() runtime managed internalcall
    {
    } // end of method ISymUnmanagedReader::GetUserEntryPoint

    .method public hidebysig newslot abstract virtual 
            instance class DebuggerInterop.Symbols.ISymUnmanagedMethod 
            marshal( interface) 
            GetMethod([in] unsigned int32 token) runtime managed internalcall
    {
    } // end of method ISymUnmanagedReader::GetMethod

    .method public hidebysig newslot abstract virtual 
            instance class DebuggerInterop.Symbols.ISymUnmanagedMethod 
            marshal( interface) 
            GetMethodByVersion([in] unsigned int32 token,
                               [in] int32 version) runtime managed internalcall
    {
    } // end of method ISymUnmanagedReader::GetMethodByVersion

    .method public hidebysig newslot abstract virtual 
            instance void  GetVariables([in] unsigned int32 parent,
                                        [in] unsigned int32 cVars,
                                        [out] unsigned int32& pcVars,
                                        [out] native int pVars) runtime managed internalcall
    {
    } // end of method ISymUnmanagedReader::GetVariables

    .method public hidebysig newslot abstract virtual 
            instance void  GetGlobalVariables([in] unsigned int32 cVars,
                                              [out] unsigned int32& pcVars,
                                              [out] native int pVars) runtime managed internalcall
    {
    } // end of method ISymUnmanagedReader::GetGlobalVariables

    .method public hidebysig newslot abstract virtual 
            instance class DebuggerInterop.Symbols.ISymUnmanagedMethod 
            marshal( interface) 
            GetMethodFromDocumentPosition([in] class DebuggerInterop.Symbols.ISymUnmanagedDocument  marshal( interface) document,
                                          [in] unsigned int32 line,
                                          [in] unsigned int32 column) runtime managed internalcall
    {
    } // end of method ISymUnmanagedReader::GetMethodFromDocumentPosition

    .method public hidebysig newslot abstract virtual 
            instance void  GetSymAttribute([in] unsigned int32 parent,
                                           [in] unsigned int16& name,
                                           [in] unsigned int32 cBuffer,
                                           [out] unsigned int32& pcBuffer,
                                           [out] native int buffer) runtime managed internalcall
    {
    } // end of method ISymUnmanagedReader::GetSymAttribute

    .method public hidebysig newslot abstract virtual 
            instance void  GetNamespaces([in] unsigned int32 cNameSpaces,
                                         [out] unsigned int32& pcNameSpaces,
                                         [out] native int namespaces) runtime managed internalcall
    {
    } // end of method ISymUnmanagedReader::GetNamespaces

    .method public hidebysig newslot abstract virtual 
            instance void  Initialize([in] object  marshal( iunknown) importer,
                                      [in] unsigned int16& filename,
                                      [in] unsigned int16& searchPath,
                                      [in] class DebuggerInterop.Symbols.IStream  marshal( interface) pIStream) runtime managed internalcall
    {
    } // end of method ISymUnmanagedReader::Initialize

    .method public hidebysig newslot abstract virtual 
            instance void  UpdateSymbolStore([in] unsigned int16& filename,
                                             [in] class DebuggerInterop.Symbols.IStream  marshal( interface) pIStream) runtime managed internalcall
    {
    } // end of method ISymUnmanagedReader::UpdateSymbolStore

    .method public hidebysig newslot abstract virtual 
            instance void  ReplaceSymbolStore([in] unsigned int16& filename,
                                              [in] class DebuggerInterop.Symbols.IStream  marshal( interface) pIStream) runtime managed internalcall
    {
    } // end of method ISymUnmanagedReader::ReplaceSymbolStore

    .method public hidebysig newslot abstract virtual 
            instance void  GetSymbolStoreFileName([in] unsigned int32 cchName,
                                                  [out] unsigned int32& pcchName,
                                                  [out] native int szName) runtime managed internalcall
    {
    } // end of method ISymUnmanagedReader::GetSymbolStoreFileName

    .method public hidebysig newslot abstract virtual 
            instance void  GetMethodsFromDocumentPosition([in] class DebuggerInterop.Symbols.ISymUnmanagedDocument  marshal( interface) document,
                                                          [in] unsigned int32 line,
                                                          [in] unsigned int32 column,
                                                          [in] unsigned int32 cMethod,
                                                          [out] unsigned int32& pcMethod,
                                                          [out] native int pRetVal) runtime managed internalcall
    {
    } // end of method ISymUnmanagedReader::GetMethodsFromDocumentPosition

    .method public hidebysig newslot abstract virtual 
            instance void  GetDocumentVersion([in] class DebuggerInterop.Symbols.ISymUnmanagedDocument  marshal( interface) pDoc,
                                              [out] int32& version,
                                              [out] int32& pbCurrent) runtime managed internalcall
    {
    } // end of method ISymUnmanagedReader::GetDocumentVersion

    .method public hidebysig newslot abstract virtual 
            instance void  GetMethodVersion([in] class DebuggerInterop.Symbols.ISymUnmanagedMethod  marshal( interface) pMethod,
                                            [out] int32& version) runtime managed internalcall
    {
    } // end of method ISymUnmanagedReader::GetMethodVersion

  } // end of class ISymUnmanagedReader

  .class interface public abstract auto ansi import ISymUnmanagedDocument
  {
    .custom instance void [mscorlib]System.Runtime.InteropServices.InterfaceTypeAttribute::.ctor(int16) = ( 01 00 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 34 30 44 45 34 30 33 37 2D 37 43 38 31   // ..$40DE4037-7C81
                                                                                                    2D 33 45 31 45 2D 42 30 32 32 2D 41 45 31 41 42   // -3E1E-B022-AE1AB
                                                                                                    46 46 32 43 41 30 38 00 00 )                      // FF2CA08..
    .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public hidebysig newslot abstract virtual 
            instance void  GetURL([in] unsigned int32 cchUrl,
                                  [out] unsigned int32& pcchUrl,
                                  [out] native int szUrl) runtime managed internalcall
    {
    } // end of method ISymUnmanagedDocument::GetURL

    .method public hidebysig newslot abstract virtual 
            instance valuetype [mscorlib]System.Guid 
            GetDocumentType() runtime managed internalcall
    {
    } // end of method ISymUnmanagedDocument::GetDocumentType

    .method public hidebysig newslot abstract virtual 
            instance valuetype [mscorlib]System.Guid 
            GetLanguage() runtime managed internalcall
    {
    } // end of method ISymUnmanagedDocument::GetLanguage

    .method public hidebysig newslot abstract virtual 
            instance valuetype [mscorlib]System.Guid 
            GetLanguageVendor() runtime managed internalcall
    {
    } // end of method ISymUnmanagedDocument::GetLanguageVendor

    .method public hidebysig newslot abstract virtual 
            instance valuetype [mscorlib]System.Guid 
            GetCheckSumAlgorithmId() runtime managed internalcall
    {
    } // end of method ISymUnmanagedDocument::GetCheckSumAlgorithmId

    .method public hidebysig newslot abstract virtual 
            instance void  GetCheckSum([in] unsigned int32 cData,
                                       [out] unsigned int32& pcData,
                                       [out] native int data) runtime managed internalcall
    {
    } // end of method ISymUnmanagedDocument::GetCheckSum

    .method public hidebysig newslot abstract virtual 
            instance unsigned int32  FindClosestLine([in] unsigned int32 line) runtime managed internalcall
    {
    } // end of method ISymUnmanagedDocument::FindClosestLine

    .method public hidebysig newslot abstract virtual 
            instance int32  HasEmbeddedSource() runtime managed internalcall
    {
    } // end of method ISymUnmanagedDocument::HasEmbeddedSource

    .method public hidebysig newslot abstract virtual 
            instance unsigned int32  GetSourceLength() runtime managed internalcall
    {
    } // end of method ISymUnmanagedDocument::GetSourceLength

    .method public hidebysig newslot abstract virtual 
            instance void  GetSourceRange([in] unsigned int32 startLine,
                                          [in] unsigned int32 startColumn,
                                          [in] unsigned int32 endLine,
                                          [in] unsigned int32 endColumn,
                                          [in] unsigned int32 cSourceBytes,
                                          [out] unsigned int32& pcSourceBytes,
                                          [out] native int source) runtime managed internalcall
    {
    } // end of method ISymUnmanagedDocument::GetSourceRange

  } // end of class ISymUnmanagedDocument

  .class interface public abstract auto ansi import ISymUnmanagedMethod
  {
    .custom instance void [mscorlib]System.Runtime.InteropServices.InterfaceTypeAttribute::.ctor(int16) = ( 01 00 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 42 36 32 42 39 32 33 43 2D 42 35 30 30   // ..$B62B923C-B500
                                                                                                    2D 33 31 35 38 2D 41 35 34 33 2D 32 34 46 33 30   // -3158-A543-24F30
                                                                                                    37 41 38 42 37 45 31 00 00 )                      // 7A8B7E1..
    .method public hidebysig newslot abstract virtual 
            instance unsigned int32  GetToken() runtime managed internalcall
    {
    } // end of method ISymUnmanagedMethod::GetToken

    .method public hidebysig newslot abstract virtual 
            instance unsigned int32  GetSequencePointCount() runtime managed internalcall
    {
    } // end of method ISymUnmanagedMethod::GetSequencePointCount

    .method public hidebysig newslot abstract virtual 
            instance class DebuggerInterop.Symbols.ISymUnmanagedScope 
            marshal( interface) 
            GetRootScope() runtime managed internalcall
    {
    } // end of method ISymUnmanagedMethod::GetRootScope

    .method public hidebysig newslot abstract virtual 
            instance class DebuggerInterop.Symbols.ISymUnmanagedScope 
            marshal( interface) 
            GetScopeFromOffset([in] unsigned int32 offset) runtime managed internalcall
    {
    } // end of method ISymUnmanagedMethod::GetScopeFromOffset

    .method public hidebysig newslot abstract virtual 
            instance unsigned int32  GetOffset([in] class DebuggerInterop.Symbols.ISymUnmanagedDocument  marshal( interface) document,
                                               [in] unsigned int32 line,
                                               [in] unsigned int32 column) runtime managed internalcall
    {
    } // end of method ISymUnmanagedMethod::GetOffset

    .method public hidebysig newslot abstract virtual 
            instance void  GetRanges([in] class DebuggerInterop.Symbols.ISymUnmanagedDocument marshal(interface) document,
                                     [in] unsigned int32 line,
                                     [in] unsigned int32 column,
                                     [in] unsigned int32 cRanges,
                                     [out] unsigned int32& pcRanges,
                                     [out] native int ranges) runtime managed internalcall
    {
    } // end of method ISymUnmanagedMethod::GetRanges

    .method public hidebysig newslot abstract virtual 
            instance void  GetParameters([in] unsigned int32 cParams,
                                         [out] unsigned int32& pcParams,
                                         [out] native int params) runtime managed internalcall
    {
    } // end of method ISymUnmanagedMethod::GetParameters

    .method public hidebysig newslot abstract virtual 
            instance void  GetNamespace([out] class DebuggerInterop.Symbols.ISymUnmanagedNamespace&  marshal( interface) pRetVal) runtime managed internalcall
    {
    } // end of method ISymUnmanagedMethod::GetNamespace

    .method public hidebysig newslot abstract virtual 
            instance void  GetSourceStartEnd([in][out] native int docs,
                                             [in][out] native int lines,
                                             [in][out] native int columns,
                                             [out] int32& pRetVal) runtime managed internalcall
    {
    } // end of method ISymUnmanagedMethod::GetSourceStartEnd

    .method public hidebysig newslot abstract virtual 
            instance void  GetSequencePoints([in] unsigned int32 cPoints,
                                             [out] unsigned int32& pcPoints,
                                             [out] unsigned int32[]  marshal([2]) offsets,
                                             [out] class DebuggerInterop.Symbols.ISymUnmanagedDocument[] marshal([]) documents,
                                             [out] unsigned int32[]  marshal([2]) lines,
                                             [out] unsigned int32[]  marshal([2]) columns,
                                             [out] unsigned int32[]  marshal([2]) endLines,
                                             [out] unsigned int32[]  marshal([2]) endColumns) runtime managed internalcall
    {
    } // end of method ISymUnmanagedMethod::GetSequencePoints

  } // end of class ISymUnmanagedMethod

  .class interface public abstract auto ansi import ISymUnmanagedScope
  {
    .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 36 38 30 30 35 44 30 46 2D 42 38 45 30   // ..$68005D0F-B8E0
                                                                                                    2D 33 42 30 31 2D 38 34 44 35 2D 41 31 31 41 39   // -3B01-84D5-A11A9
                                                                                                    34 31 35 34 39 34 32 00 00 )                      // 4154942..
    .custom instance void [mscorlib]System.Runtime.InteropServices.InterfaceTypeAttribute::.ctor(int16) = ( 01 00 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public hidebysig newslot abstract virtual 
            instance class DebuggerInterop.Symbols.ISymUnmanagedMethod 
            marshal( interface) 
            GetMethod() runtime managed internalcall
    {
    } // end of method ISymUnmanagedScope::GetMethod

    .method public hidebysig newslot abstract virtual 
            instance class DebuggerInterop.Symbols.ISymUnmanagedScope 
            marshal( interface) 
            GetParent() runtime managed internalcall
    {
    } // end of method ISymUnmanagedScope::GetParent

    .method public hidebysig newslot abstract virtual 
            instance void  GetChildren([in] unsigned int32 cChildren,
                                       [out] unsigned int32& pcChildren,
                                       [out] class DebuggerInterop.Symbols.ISymUnmanagedScope[] marshal([]) children) runtime managed internalcall
    {
    } // end of method ISymUnmanagedScope::GetChildren

    .method public hidebysig newslot abstract virtual 
            instance unsigned int32  GetStartOffset() runtime managed internalcall
    {
    } // end of method ISymUnmanagedScope::GetStartOffset

    .method public hidebysig newslot abstract virtual 
            instance unsigned int32  GetEndOffset() runtime managed internalcall
    {
    } // end of method ISymUnmanagedScope::GetEndOffset

    .method public hidebysig newslot abstract virtual 
            instance unsigned int32  GetLocalCount() runtime managed internalcall
    {
    } // end of method ISymUnmanagedScope::GetLocalCount

    .method public hidebysig newslot abstract virtual 
            instance void  GetLocals([in] unsigned int32 cLocals,
                                     [out] unsigned int32& pcLocals,
                                     [out] class DebuggerInterop.Symbols.ISymUnmanagedVariable[] marshal([]) locals) runtime managed internalcall
    {
    } // end of method ISymUnmanagedScope::GetLocals

    .method public hidebysig newslot abstract virtual 
            instance void  GetNamespaces([in] unsigned int32 cNameSpaces,
                                         [out] unsigned int32& pcNameSpaces,
                                         [out] native int namespaces) runtime managed internalcall
    {
    } // end of method ISymUnmanagedScope::GetNamespaces

  } // end of class ISymUnmanagedScope

  .class interface public abstract auto ansi import ISymUnmanagedNamespace
  {
    .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 30 44 46 46 37 32 38 39 2D 35 34 46 38   // ..$0DFF7289-54F8
                                                                                                    2D 31 31 44 33 2D 42 44 32 38 2D 30 30 30 30 46   // -11D3-BD28-0000F
                                                                                                    38 30 38 34 39 42 44 00 00 )                      // 80849BD..
    .custom instance void [mscorlib]System.Runtime.InteropServices.InterfaceTypeAttribute::.ctor(int16) = ( 01 00 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public hidebysig newslot abstract virtual 
            instance void  GetName([in] unsigned int32 cchName,
                                   [out] unsigned int32& pcchName,
                                   [out] native int szName) runtime managed internalcall
    {
    } // end of method ISymUnmanagedNamespace::GetName

    .method public hidebysig newslot abstract virtual 
            instance void  GetNamespaces([in] unsigned int32 cNameSpaces,
                                         [out] unsigned int32& pcNameSpaces,
                                         [out] native int namespaces) runtime managed internalcall
    {
    } // end of method ISymUnmanagedNamespace::GetNamespaces

    .method public hidebysig newslot abstract virtual 
            instance void  GetVariables([in] unsigned int32 cVars,
                                        [out] unsigned int32& pcVars,
                                        [out] native int pVars) runtime managed internalcall
    {
    } // end of method ISymUnmanagedNamespace::GetVariables

  } // end of class ISymUnmanagedNamespace

  .class public auto ansi import CorSymBinderClass
         extends [mscorlib]System.Object
         implements DebuggerInterop.Symbols.ISymUnmanagedBinder,
                    DebuggerInterop.Symbols.CorSymBinder
  {
    .custom instance void [mscorlib]System.Runtime.InteropServices.TypeLibTypeAttribute::.ctor(int16) = ( 01 00 02 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.InteropServices.ClassInterfaceAttribute::.ctor(int16) = ( 01 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 41 41 35 34 34 44 34 31 2D 32 38 43 42   // ..$AA544D41-28CB
                                                                                                    2D 31 31 44 33 2D 42 44 32 32 2D 30 30 30 30 46   // -11D3-BD22-0000F
                                                                                                    38 30 38 34 39 42 44 00 00 )                      // 80849BD..
    .method public specialname rtspecialname 
            instance void  .ctor() runtime managed internalcall
    {
    } // end of method CorSymBinderClass::.ctor
    
    .method public hidebysig newslot virtual 
            instance int32 
            GetReaderForFile([in] object  marshal( iunknown) importer,
                             [in] native int filename,
                             [in] native int searchPath,
                             [out] class DebuggerInterop.Symbols.ISymUnmanagedReader& marshal( interface) reader) runtime managed internalcall preservesig
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedBinder::GetReaderForFile
    } // end of method CorSymBinderClass::GetReaderForFile

    .method public hidebysig newslot virtual 
            instance class DebuggerInterop.Symbols.ISymUnmanagedReader 
            marshal( interface) 
            GetReaderFromStream([in] object  marshal( iunknown) importer,
                                [in] class DebuggerInterop.Symbols.IStream  marshal( interface) pstream) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedBinder::GetReaderFromStream
    } // end of method CorSymBinderClass::GetReaderFromStream

  } // end of class CorSymBinderClass

  .class interface public abstract auto ansi import CorSymBinder
         implements DebuggerInterop.Symbols.ISymUnmanagedBinder
  {
    .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 41 41 35 34 34 44 34 32 2D 32 38 43 42   // ..$AA544D42-28CB
                                                                                                    2D 31 31 44 33 2D 42 44 32 32 2D 30 30 30 30 46   // -11D3-BD22-0000F
                                                                                                    38 30 38 34 39 42 44 00 00 )                      // 80849BD..
    .custom instance void [mscorlib]System.Runtime.InteropServices.CoClassAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 1B 43 6F 72 53 79 6D 4C 69 62 2E 43 6F 72   // ...DebuggerInterop.Symbols.Cor
                                                                                                                            53 79 6D 42 69 6E 64 65 72 43 6C 61 73 73 00 00 ) // SymBinderClass..
  } // end of class CorSymBinder

  .class interface public abstract auto ansi import ISymUnmanagedBinder
  {
    .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 41 41 35 34 34 44 34 32 2D 32 38 43 42   // ..$AA544D42-28CB
                                                                                                    2D 31 31 44 33 2D 42 44 32 32 2D 30 30 30 30 46   // -11D3-BD22-0000F
                                                                                                    38 30 38 34 39 42 44 00 00 )                      // 80849BD..
    .custom instance void [mscorlib]System.Runtime.InteropServices.InterfaceTypeAttribute::.ctor(int16) = ( 01 00 01 00 00 00 ) 
    .method public hidebysig newslot abstract virtual 
            instance int32 
            GetReaderForFile([in] object  marshal( iunknown) importer,
                             [in] native int filename,
                             [in] native int searchPath,
                             [out] class DebuggerInterop.Symbols.ISymUnmanagedReader& marshal( interface) reader) runtime managed internalcall preservesig
    {
    } // end of method ISymUnmanagedBinder::GetReaderForFile

    .method public hidebysig newslot abstract virtual 
            instance class DebuggerInterop.Symbols.ISymUnmanagedReader 
            marshal( interface) 
            GetReaderFromStream([in] object  marshal( iunknown) importer,
                                [in] class DebuggerInterop.Symbols.IStream  marshal( interface) pstream) runtime managed internalcall
    {
    } // end of method ISymUnmanagedBinder::GetReaderFromStream

  } // end of class ISymUnmanagedBinder

  .class public auto ansi import CorSymWriter_SxSClass
         extends [mscorlib]System.Object
         implements DebuggerInterop.Symbols.ISymUnmanagedWriter,
                    DebuggerInterop.Symbols.CorSymWriter_SxS
  {
    .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.InteropServices.TypeLibTypeAttribute::.ctor(int16) = ( 01 00 02 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 30 41 45 32 44 45 42 30 2D 46 39 30 31   // ..$0AE2DEB0-F901
                                                                                                    2D 34 37 38 42 2D 42 42 39 46 2D 38 38 31 45 45   // -478B-BB9F-881EE
                                                                                                    38 30 36 36 37 38 38 00 00 )                      // 8066788..
    .custom instance void [mscorlib]System.Runtime.InteropServices.ClassInterfaceAttribute::.ctor(int16) = ( 01 00 00 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor() runtime managed internalcall
    {
    } // end of method CorSymWriter_SxSClass::.ctor

    .method public hidebysig newslot virtual 
            instance class DebuggerInterop.Symbols.ISymUnmanagedDocumentWriter 
            marshal( interface) 
            DefineDocument([in] unsigned int16& url,
                           [in] valuetype [mscorlib]System.Guid& language,
                           [in] valuetype [mscorlib]System.Guid& languageVendor,
                           [in] valuetype [mscorlib]System.Guid& documentType) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedWriter::DefineDocument
    } // end of method CorSymWriter_SxSClass::DefineDocument

    .method public hidebysig newslot virtual 
            instance void  SetUserEntryPoint([in] unsigned int32 entryMethod) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedWriter::SetUserEntryPoint
    } // end of method CorSymWriter_SxSClass::SetUserEntryPoint

    .method public hidebysig newslot virtual 
            instance void  OpenMethod([in] unsigned int32 'method') runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedWriter::OpenMethod
    } // end of method CorSymWriter_SxSClass::OpenMethod

    .method public hidebysig newslot virtual 
            instance void  CloseMethod() runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedWriter::CloseMethod
    } // end of method CorSymWriter_SxSClass::CloseMethod

    .method public hidebysig newslot virtual 
            instance unsigned int32  OpenScope([in] unsigned int32 startOffset) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedWriter::OpenScope
    } // end of method CorSymWriter_SxSClass::OpenScope

    .method public hidebysig newslot virtual 
            instance void  CloseScope([in] unsigned int32 endOffset) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedWriter::CloseScope
    } // end of method CorSymWriter_SxSClass::CloseScope

    .method public hidebysig newslot virtual 
            instance void  SetScopeRange([in] unsigned int32 scopeID,
                                         [in] unsigned int32 startOffset,
                                         [in] unsigned int32 endOffset) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedWriter::SetScopeRange
    } // end of method CorSymWriter_SxSClass::SetScopeRange

    .method public hidebysig newslot virtual 
            instance void  DefineLocalVariable([in] unsigned int16& name,
                                               [in] unsigned int32 attributes,
                                               [in] unsigned int32 cSig,
                                               [in] unsigned int8& signature,
                                               [in] unsigned int32 addrKind,
                                               [in] unsigned int32 addr1,
                                               [in] unsigned int32 addr2,
                                               [in] unsigned int32 addr3,
                                               [in] unsigned int32 startOffset,
                                               [in] unsigned int32 endOffset) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedWriter::DefineLocalVariable
    } // end of method CorSymWriter_SxSClass::DefineLocalVariable

    .method public hidebysig newslot virtual 
            instance void  DefineParameter([in] unsigned int16& name,
                                           [in] unsigned int32 attributes,
                                           [in] unsigned int32 sequence,
                                           [in] unsigned int32 addrKind,
                                           [in] unsigned int32 addr1,
                                           [in] unsigned int32 addr2,
                                           [in] unsigned int32 addr3) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedWriter::DefineParameter
    } // end of method CorSymWriter_SxSClass::DefineParameter

    .method public hidebysig newslot virtual 
            instance void  DefineField([in] unsigned int32 parent,
                                       [in] unsigned int16& name,
                                       [in] unsigned int32 attributes,
                                       [in] unsigned int32 cSig,
                                       [in] unsigned int8& signature,
                                       [in] unsigned int32 addrKind,
                                       [in] unsigned int32 addr1,
                                       [in] unsigned int32 addr2,
                                       [in] unsigned int32 addr3) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedWriter::DefineField
    } // end of method CorSymWriter_SxSClass::DefineField

    .method public hidebysig newslot virtual 
            instance void  DefineGlobalVariable([in] unsigned int16& name,
                                                [in] unsigned int32 attributes,
                                                [in] unsigned int32 cSig,
                                                [in] unsigned int8& signature,
                                                [in] unsigned int32 addrKind,
                                                [in] unsigned int32 addr1,
                                                [in] unsigned int32 addr2,
                                                [in] unsigned int32 addr3) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedWriter::DefineGlobalVariable
    } // end of method CorSymWriter_SxSClass::DefineGlobalVariable

    .method public hidebysig newslot virtual 
            instance void  Close() runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedWriter::Close
    } // end of method CorSymWriter_SxSClass::Close

    .method public hidebysig newslot virtual 
            instance void  SetSymAttribute([in] unsigned int32 parent,
                                           [in] unsigned int16& name,
                                           [in] unsigned int32 cData,
                                           [in] unsigned int8& data) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedWriter::SetSymAttribute
    } // end of method CorSymWriter_SxSClass::SetSymAttribute

    .method public hidebysig newslot virtual 
            instance void  OpenNamespace([in] unsigned int16& name) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedWriter::OpenNamespace
    } // end of method CorSymWriter_SxSClass::OpenNamespace

    .method public hidebysig newslot virtual 
            instance void  CloseNamespace() runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedWriter::CloseNamespace
    } // end of method CorSymWriter_SxSClass::CloseNamespace

    .method public hidebysig newslot virtual 
            instance void  UsingNamespace([in] unsigned int16& fullName) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedWriter::UsingNamespace
    } // end of method CorSymWriter_SxSClass::UsingNamespace

    .method public hidebysig newslot virtual 
            instance void  SetMethodSourceRange([in] class DebuggerInterop.Symbols.ISymUnmanagedDocumentWriter  marshal( interface) startDoc,
                                                [in] unsigned int32 startLine,
                                                [in] unsigned int32 startColumn,
                                                [in] class DebuggerInterop.Symbols.ISymUnmanagedDocumentWriter  marshal( interface) endDoc,
                                                [in] unsigned int32 endLine,
                                                [in] unsigned int32 endColumn) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedWriter::SetMethodSourceRange
    } // end of method CorSymWriter_SxSClass::SetMethodSourceRange

    .method public hidebysig newslot virtual 
            instance void  Initialize([in] object  marshal( iunknown) emitter,
                                      [in] unsigned int16& filename,
                                      [in] class DebuggerInterop.Symbols.IStream  marshal( interface) pIStream,
                                      [in] int32 fFullBuild) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedWriter::Initialize
    } // end of method CorSymWriter_SxSClass::Initialize

    .method public hidebysig newslot virtual 
            instance void  GetDebugInfo([in][out] unsigned int32& pIDD,
                                        [in] unsigned int32 cData,
                                        [out] unsigned int32& pcData,
                                        [out] native int data) runtime managed internalcall
    {
      .param [1]
      .custom instance void [mscorlib]System.Runtime.InteropServices.ComAliasNameAttribute::.ctor(string) = ( 01 00 13 43 6F 72 53 79 6D 4C 69 62 2E 55 4C 4F   // ...DebuggerInterop.Symbols.ULO
                                                                                                              4E 47 5F 50 54 52 00 00 )                         // NG_PTR..
      .override DebuggerInterop.Symbols.ISymUnmanagedWriter::GetDebugInfo
    } // end of method CorSymWriter_SxSClass::GetDebugInfo

    .method public hidebysig newslot virtual 
            instance void  DefineSequencePoints([in] class DebuggerInterop.Symbols.ISymUnmanagedDocumentWriter  marshal( interface) document,
                                                [in] unsigned int32 spCount,
                                                [in] unsigned int32& offsets,
                                                [in] unsigned int32& lines,
                                                [in] unsigned int32& columns,
                                                [in] unsigned int32& endLines,
                                                [in] unsigned int32& endColumns) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedWriter::DefineSequencePoints
    } // end of method CorSymWriter_SxSClass::DefineSequencePoints

    .method public hidebysig newslot virtual 
            instance void  RemapToken([in] unsigned int32 oldToken,
                                      [in] unsigned int32 newToken) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedWriter::RemapToken
    } // end of method CorSymWriter_SxSClass::RemapToken

    .method public hidebysig newslot virtual 
            instance void  Initialize2([in] object  marshal( iunknown) emitter,
                                       [in] unsigned int16& tempfilename,
                                       [in] class DebuggerInterop.Symbols.IStream  marshal( interface) pIStream,
                                       [in] int32 fFullBuild,
                                       [in] unsigned int16& finalfilename) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedWriter::Initialize2
    } // end of method CorSymWriter_SxSClass::Initialize2

    .method public hidebysig newslot virtual 
            instance void  DefineConstant([in] unsigned int16& name,
                                          [in] object  marshal( struct) 'value',
                                          [in] unsigned int32 cSig,
                                          [in] unsigned int8& signature) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedWriter::DefineConstant
    } // end of method CorSymWriter_SxSClass::DefineConstant

    .method public hidebysig newslot virtual 
            instance void  Abort() runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedWriter::Abort
    } // end of method CorSymWriter_SxSClass::Abort

  } // end of class CorSymWriter_SxSClass

  .class interface public abstract auto ansi import CorSymWriter_SxS
         implements DebuggerInterop.Symbols.ISymUnmanagedWriter
  {
    .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 45 44 31 34 41 41 37 32 2D 37 38 45 32   // ..$ED14AA72-78E2
                                                                                                    2D 34 38 38 34 2D 38 34 45 32 2D 33 33 34 32 39   // -4884-84E2-33429
                                                                                                    33 41 45 35 32 31 34 00 00 )                      // 3AE5214..
    .custom instance void [mscorlib]System.Runtime.InteropServices.CoClassAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 1F 43 6F 72 53 79 6D 4C 69 62 2E 43 6F 72   // ...DebuggerInterop.Symbols.Cor
                                                                                                                            53 79 6D 57 72 69 74 65 72 5F 53 78 53 43 6C 61   // SymWriter_SxSCla
                                                                                                                            73 73 00 00 )                                     // ss..
  } // end of class CorSymWriter_SxS

  .class public auto ansi import CorSymReader_SxSClass
         extends [mscorlib]System.Object
         implements DebuggerInterop.Symbols.ISymUnmanagedReader,
                    DebuggerInterop.Symbols.CorSymReader_SxS
  {
    .custom instance void [mscorlib]System.Runtime.InteropServices.TypeLibTypeAttribute::.ctor(int16) = ( 01 00 02 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.InteropServices.ClassInterfaceAttribute::.ctor(int16) = ( 01 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 30 41 33 39 37 36 43 35 2D 34 35 32 39   // ..$0A3976C5-4529
                                                                                                    2D 34 45 46 38 2D 42 30 42 30 2D 34 32 45 45 44   // -4EF8-B0B0-42EED
                                                                                                    33 37 30 38 32 43 44 00 00 )                      // 37082CD..
    .method public specialname rtspecialname 
            instance void  .ctor() runtime managed internalcall
    {
    } // end of method CorSymReader_SxSClass::.ctor

    .method public hidebysig newslot virtual 
            instance class DebuggerInterop.Symbols.ISymUnmanagedDocument 
            marshal( interface) 
            GetDocument([in] string  marshal( lpwstr) url,
                        [in] valuetype [mscorlib]System.Guid language,
                        [in] valuetype [mscorlib]System.Guid languageVendor,
                        [in] valuetype [mscorlib]System.Guid documentType) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedReader::GetDocument
    } // end of method CorSymReader_SxSClass::GetDocument

    .method public hidebysig newslot virtual 
            instance void  GetDocuments([in] unsigned int32 cDocs,
                                        [out] unsigned int32& pcDocs,
                                        [out] native int pDocs) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedReader::GetDocuments
    } // end of method CorSymReader_SxSClass::GetDocuments

    .method public hidebysig newslot virtual 
            instance unsigned int32  GetUserEntryPoint() runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedReader::GetUserEntryPoint
    } // end of method CorSymReader_SxSClass::GetUserEntryPoint

    .method public hidebysig newslot virtual 
            instance class DebuggerInterop.Symbols.ISymUnmanagedMethod 
            marshal( interface) 
            GetMethod([in] unsigned int32 token) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedReader::GetMethod
    } // end of method CorSymReader_SxSClass::GetMethod

    .method public hidebysig newslot virtual 
            instance class DebuggerInterop.Symbols.ISymUnmanagedMethod 
            marshal( interface) 
            GetMethodByVersion([in] unsigned int32 token,
                               [in] int32 version) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedReader::GetMethodByVersion
    } // end of method CorSymReader_SxSClass::GetMethodByVersion

    .method public hidebysig newslot virtual 
            instance void  GetVariables([in] unsigned int32 parent,
                                        [in] unsigned int32 cVars,
                                        [out] unsigned int32& pcVars,
                                        [out] native int pVars) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedReader::GetVariables
    } // end of method CorSymReader_SxSClass::GetVariables

    .method public hidebysig newslot virtual 
            instance void  GetGlobalVariables([in] unsigned int32 cVars,
                                              [out] unsigned int32& pcVars,
                                              [out] native int pVars) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedReader::GetGlobalVariables
    } // end of method CorSymReader_SxSClass::GetGlobalVariables

    .method public hidebysig newslot virtual 
            instance class DebuggerInterop.Symbols.ISymUnmanagedMethod 
            marshal( interface) 
            GetMethodFromDocumentPosition([in] class DebuggerInterop.Symbols.ISymUnmanagedDocument  marshal( interface) document,
                                          [in] unsigned int32 line,
                                          [in] unsigned int32 column) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedReader::GetMethodFromDocumentPosition
    } // end of method CorSymReader_SxSClass::GetMethodFromDocumentPosition

    .method public hidebysig newslot virtual 
            instance void  GetSymAttribute([in] unsigned int32 parent,
                                           [in] unsigned int16& name,
                                           [in] unsigned int32 cBuffer,
                                           [out] unsigned int32& pcBuffer,
                                           [out] native int buffer) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedReader::GetSymAttribute
    } // end of method CorSymReader_SxSClass::GetSymAttribute

    .method public hidebysig newslot virtual 
            instance void  GetNamespaces([in] unsigned int32 cNameSpaces,
                                         [out] unsigned int32& pcNameSpaces,
                                         [out] native int namespaces) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedReader::GetNamespaces
    } // end of method CorSymReader_SxSClass::GetNamespaces

    .method public hidebysig newslot virtual 
            instance void  Initialize([in] object  marshal( iunknown) importer,
                                      [in] unsigned int16& filename,
                                      [in] unsigned int16& searchPath,
                                      [in] class DebuggerInterop.Symbols.IStream  marshal( interface) pIStream) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedReader::Initialize
    } // end of method CorSymReader_SxSClass::Initialize

    .method public hidebysig newslot virtual 
            instance void  UpdateSymbolStore([in] unsigned int16& filename,
                                             [in] class DebuggerInterop.Symbols.IStream  marshal( interface) pIStream) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedReader::UpdateSymbolStore
    } // end of method CorSymReader_SxSClass::UpdateSymbolStore

    .method public hidebysig newslot virtual 
            instance void  ReplaceSymbolStore([in] unsigned int16& filename,
                                              [in] class DebuggerInterop.Symbols.IStream  marshal( interface) pIStream) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedReader::ReplaceSymbolStore
    } // end of method CorSymReader_SxSClass::ReplaceSymbolStore

    .method public hidebysig newslot virtual 
            instance void  GetSymbolStoreFileName([in] unsigned int32 cchName,
                                                  [out] unsigned int32& pcchName,
                                                  [out] native int szName) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedReader::GetSymbolStoreFileName
    } // end of method CorSymReader_SxSClass::GetSymbolStoreFileName

    .method public hidebysig newslot virtual 
            instance void  GetMethodsFromDocumentPosition([in] class DebuggerInterop.Symbols.ISymUnmanagedDocument  marshal( interface) document,
                                                          [in] unsigned int32 line,
                                                          [in] unsigned int32 column,
                                                          [in] unsigned int32 cMethod,
                                                          [out] unsigned int32& pcMethod,
                                                          [out] native int pRetVal) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedReader::GetMethodsFromDocumentPosition
    } // end of method CorSymReader_SxSClass::GetMethodsFromDocumentPosition

    .method public hidebysig newslot virtual 
            instance void  GetDocumentVersion([in] class DebuggerInterop.Symbols.ISymUnmanagedDocument  marshal( interface) pDoc,
                                              [out] int32& version,
                                              [out] int32& pbCurrent) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedReader::GetDocumentVersion
    } // end of method CorSymReader_SxSClass::GetDocumentVersion

    .method public hidebysig newslot virtual 
            instance void  GetMethodVersion([in] class DebuggerInterop.Symbols.ISymUnmanagedMethod  marshal( interface) pMethod,
                                            [out] int32& version) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedReader::GetMethodVersion
    } // end of method CorSymReader_SxSClass::GetMethodVersion

  } // end of class CorSymReader_SxSClass

  .class interface public abstract auto ansi import CorSymReader_SxS
         implements DebuggerInterop.Symbols.ISymUnmanagedReader
  {
    .custom instance void [mscorlib]System.Runtime.InteropServices.CoClassAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 1F 43 6F 72 53 79 6D 4C 69 62 2E 43 6F 72   // ...DebuggerInterop.Symbols.Cor
                                                                                                                            53 79 6D 52 65 61 64 65 72 5F 53 78 53 43 6C 61   // SymReader_SxSCla
                                                                                                                            73 73 00 00 )                                     // ss..
    .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 42 34 43 45 36 32 38 36 2D 32 41 36 42   // ..$B4CE6286-2A6B
                                                                                                    2D 33 37 31 32 2D 41 33 42 37 2D 31 45 45 31 44   // -3712-A3B7-1EE1D
                                                                                                    41 44 34 36 37 42 35 00 00 )                      // AD467B5..
  } // end of class CorSymReader_SxS

  .class public auto ansi import CorSymBinder_SxSClass
         extends [mscorlib]System.Object
         implements DebuggerInterop.Symbols.ISymUnmanagedBinder,
                    DebuggerInterop.Symbols.CorSymBinder_SxS
  {
    .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 30 41 32 39 46 46 39 45 2D 37 46 39 43   // ..$0A29FF9E-7F9C
                                                                                                    2D 34 34 33 37 2D 38 42 31 31 2D 46 34 32 34 34   // -4437-8B11-F4244
                                                                                                    39 31 45 33 39 33 31 00 00 )                      // 91E3931..
    .custom instance void [mscorlib]System.Runtime.InteropServices.TypeLibTypeAttribute::.ctor(int16) = ( 01 00 02 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.InteropServices.ClassInterfaceAttribute::.ctor(int16) = ( 01 00 00 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor() runtime managed internalcall
    {
    } // end of method CorSymBinder_SxSClass::.ctor

    .method public hidebysig newslot virtual 
            instance int32 
            GetReaderForFile([in] object  marshal( iunknown) importer,
                             [in] native int filename,
                             [in] native int searchPath,
                             [out] class DebuggerInterop.Symbols.ISymUnmanagedReader& marshal( interface) reader) runtime managed internalcall preservesig
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedBinder::GetReaderForFile
    } // end of method CorSymBinder_SxSClass::GetReaderForFile

    .method public hidebysig newslot virtual 
            instance class DebuggerInterop.Symbols.ISymUnmanagedReader 
            marshal( interface) 
            GetReaderFromStream([in] object  marshal( iunknown) importer,
                                [in] class DebuggerInterop.Symbols.IStream  marshal( interface) pstream) runtime managed internalcall
    {
      .override DebuggerInterop.Symbols.ISymUnmanagedBinder::GetReaderFromStream
    } // end of method CorSymBinder_SxSClass::GetReaderFromStream

  } // end of class CorSymBinder_SxSClass

  .class interface public abstract auto ansi import CorSymBinder_SxS
         implements DebuggerInterop.Symbols.ISymUnmanagedBinder
  {
    .custom instance void [mscorlib]System.Runtime.InteropServices.CoClassAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 1F 43 6F 72 53 79 6D 4C 69 62 2E 43 6F 72   // ...DebuggerInterop.Symbols.Cor
                                                                                                                            53 79 6D 42 69 6E 64 65 72 5F 53 78 53 43 6C 61   // SymBinder_SxSCla
                                                                                                                            73 73 00 00 )                                     // ss..
    .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 41 41 35 34 34 44 34 32 2D 32 38 43 42   // ..$AA544D42-28CB
                                                                                                    2D 31 31 44 33 2D 42 44 32 32 2D 30 30 30 30 46   // -11D3-BD22-0000F
                                                                                                    38 30 38 34 39 42 44 00 00 )                      // 80849BD..
  } // end of class CorSymBinder_SxS

  .class interface public abstract auto ansi import ISymUnmanagedVariable
  {
    .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 39 46 36 30 45 45 42 45 2D 32 44 39 41   // ..$9F60EEBE-2D9A
                                                                                                    2D 33 46 37 43 2D 42 46 35 38 2D 38 30 42 43 39   // -3F7C-BF58-80BC9
                                                                                                    39 31 43 36 30 42 42 00 00 )                      // 91C60BB..
    .custom instance void [mscorlib]System.Runtime.InteropServices.InterfaceTypeAttribute::.ctor(int16) = ( 01 00 01 00 00 00 ) 
    .method public hidebysig newslot abstract virtual 
            instance void  GetName([in] unsigned int32 cchName,
                                   [out] unsigned int32& pcchName,
                                   [out] native int szName) runtime managed internalcall
    {
    } // end of method ISymUnmanagedVariable::GetName

    .method public hidebysig newslot abstract virtual 
            instance unsigned int32  GetAttributes() runtime managed internalcall
    {
    } // end of method ISymUnmanagedVariable::GetAttributes

    .method public hidebysig newslot abstract virtual 
            instance void  GetSignature([in] unsigned int32 cSig,
                                        [out] unsigned int32& pcSig,
                                        [out] native int sig) runtime managed internalcall
    {
    } // end of method ISymUnmanagedVariable::GetSignature

    .method public hidebysig newslot abstract virtual 
            instance unsigned int32  GetAddressKind() runtime managed internalcall
    {
    } // end of method ISymUnmanagedVariable::GetAddressKind

    .method public hidebysig newslot abstract virtual 
            instance unsigned int32  GetAddressField1() runtime managed internalcall
    {
    } // end of method ISymUnmanagedVariable::GetAddressField1

    .method public hidebysig newslot abstract virtual 
            instance unsigned int32  GetAddressField2() runtime managed internalcall
    {
    } // end of method ISymUnmanagedVariable::GetAddressField2

    .method public hidebysig newslot abstract virtual 
            instance unsigned int32  GetAddressField3() runtime managed internalcall
    {
    } // end of method ISymUnmanagedVariable::GetAddressField3

    .method public hidebysig newslot abstract virtual 
            instance unsigned int32  GetStartOffset() runtime managed internalcall
    {
    } // end of method ISymUnmanagedVariable::GetStartOffset

    .method public hidebysig newslot abstract virtual 
            instance unsigned int32  GetEndOffset() runtime managed internalcall
    {
    } // end of method ISymUnmanagedVariable::GetEndOffset

  } // end of class ISymUnmanagedVariable

  .class interface public abstract auto ansi import ISymUnmanagedWriter2
         implements DebuggerInterop.Symbols.ISymUnmanagedWriter
  {
    .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 30 42 39 37 37 32 36 45 2D 39 45 36 44   // ..$0B97726E-9E6D
                                                                                                    2D 34 46 30 35 2D 39 41 32 36 2D 34 32 34 30 32   // -4F05-9A26-42402
                                                                                                    32 30 39 33 43 41 41 00 00 )                      // 2093CAA..
    .custom instance void [mscorlib]System.Runtime.InteropServices.InterfaceTypeAttribute::.ctor(int16) = ( 01 00 01 00 00 00 ) 
    .method public hidebysig newslot abstract virtual 
            instance class DebuggerInterop.Symbols.ISymUnmanagedDocumentWriter 
            marshal( interface) 
            DefineDocument([in] unsigned int16& url,
                           [in] valuetype [mscorlib]System.Guid& language,
                           [in] valuetype [mscorlib]System.Guid& languageVendor,
                           [in] valuetype [mscorlib]System.Guid& documentType) runtime managed internalcall
    {
    } // end of method ISymUnmanagedWriter2::DefineDocument

    .method public hidebysig newslot abstract virtual 
            instance void  SetUserEntryPoint([in] unsigned int32 entryMethod) runtime managed internalcall
    {
    } // end of method ISymUnmanagedWriter2::SetUserEntryPoint

    .method public hidebysig newslot abstract virtual 
            instance void  OpenMethod([in] unsigned int32 'method') runtime managed internalcall
    {
    } // end of method ISymUnmanagedWriter2::OpenMethod

    .method public hidebysig newslot abstract virtual 
            instance void  CloseMethod() runtime managed internalcall
    {
    } // end of method ISymUnmanagedWriter2::CloseMethod

    .method public hidebysig newslot abstract virtual 
            instance unsigned int32  OpenScope([in] unsigned int32 startOffset) runtime managed internalcall
    {
    } // end of method ISymUnmanagedWriter2::OpenScope

    .method public hidebysig newslot abstract virtual 
            instance void  CloseScope([in] unsigned int32 endOffset) runtime managed internalcall
    {
    } // end of method ISymUnmanagedWriter2::CloseScope

    .method public hidebysig newslot abstract virtual 
            instance void  SetScopeRange([in] unsigned int32 scopeID,
                                         [in] unsigned int32 startOffset,
                                         [in] unsigned int32 endOffset) runtime managed internalcall
    {
    } // end of method ISymUnmanagedWriter2::SetScopeRange

    .method public hidebysig newslot abstract virtual 
            instance void  DefineLocalVariable([in] unsigned int16& name,
                                               [in] unsigned int32 attributes,
                                               [in] unsigned int32 cSig,
                                               [in] unsigned int8& signature,
                                               [in] unsigned int32 addrKind,
                                               [in] unsigned int32 addr1,
                                               [in] unsigned int32 addr2,
                                               [in] unsigned int32 addr3,
                                               [in] unsigned int32 startOffset,
                                               [in] unsigned int32 endOffset) runtime managed internalcall
    {
    } // end of method ISymUnmanagedWriter2::DefineLocalVariable

    .method public hidebysig newslot abstract virtual 
            instance void  DefineParameter([in] unsigned int16& name,
                                           [in] unsigned int32 attributes,
                                           [in] unsigned int32 sequence,
                                           [in] unsigned int32 addrKind,
                                           [in] unsigned int32 addr1,
                                           [in] unsigned int32 addr2,
                                           [in] unsigned int32 addr3) runtime managed internalcall
    {
    } // end of method ISymUnmanagedWriter2::DefineParameter

    .method public hidebysig newslot abstract virtual 
            instance void  DefineField([in] unsigned int32 parent,
                                       [in] unsigned int16& name,
                                       [in] unsigned int32 attributes,
                                       [in] unsigned int32 cSig,
                                       [in] unsigned int8& signature,
                                       [in] unsigned int32 addrKind,
                                       [in] unsigned int32 addr1,
                                       [in] unsigned int32 addr2,
                                       [in] unsigned int32 addr3) runtime managed internalcall
    {
    } // end of method ISymUnmanagedWriter2::DefineField

    .method public hidebysig newslot abstract virtual 
            instance void  DefineGlobalVariable([in] unsigned int16& name,
                                                [in] unsigned int32 attributes,
                                                [in] unsigned int32 cSig,
                                                [in] unsigned int8& signature,
                                                [in] unsigned int32 addrKind,
                                                [in] unsigned int32 addr1,
                                                [in] unsigned int32 addr2,
                                                [in] unsigned int32 addr3) runtime managed internalcall
    {
    } // end of method ISymUnmanagedWriter2::DefineGlobalVariable

    .method public hidebysig newslot abstract virtual 
            instance void  Close() runtime managed internalcall
    {
    } // end of method ISymUnmanagedWriter2::Close

    .method public hidebysig newslot abstract virtual 
            instance void  SetSymAttribute([in] unsigned int32 parent,
                                           [in] unsigned int16& name,
                                           [in] unsigned int32 cData,
                                           [in] unsigned int8& data) runtime managed internalcall
    {
    } // end of method ISymUnmanagedWriter2::SetSymAttribute

    .method public hidebysig newslot abstract virtual 
            instance void  OpenNamespace([in] unsigned int16& name) runtime managed internalcall
    {
    } // end of method ISymUnmanagedWriter2::OpenNamespace

    .method public hidebysig newslot abstract virtual 
            instance void  CloseNamespace() runtime managed internalcall
    {
    } // end of method ISymUnmanagedWriter2::CloseNamespace

    .method public hidebysig newslot abstract virtual 
            instance void  UsingNamespace([in] unsigned int16& fullName) runtime managed internalcall
    {
    } // end of method ISymUnmanagedWriter2::UsingNamespace

    .method public hidebysig newslot abstract virtual 
            instance void  SetMethodSourceRange([in] class DebuggerInterop.Symbols.ISymUnmanagedDocumentWriter  marshal( interface) startDoc,
                                                [in] unsigned int32 startLine,
                                                [in] unsigned int32 startColumn,
                                                [in] class DebuggerInterop.Symbols.ISymUnmanagedDocumentWriter  marshal( interface) endDoc,
                                                [in] unsigned int32 endLine,
                                                [in] unsigned int32 endColumn) runtime managed internalcall
    {
    } // end of method ISymUnmanagedWriter2::SetMethodSourceRange

    .method public hidebysig newslot abstract virtual 
            instance void  Initialize([in] object  marshal( iunknown) emitter,
                                      [in] unsigned int16& filename,
                                      [in] class DebuggerInterop.Symbols.IStream  marshal( interface) pIStream,
                                      [in] int32 fFullBuild) runtime managed internalcall
    {
    } // end of method ISymUnmanagedWriter2::Initialize

    .method public hidebysig newslot abstract virtual 
            instance void  GetDebugInfo([in][out] unsigned int32& pIDD,
                                        [in] unsigned int32 cData,
                                        [out] unsigned int32& pcData,
                                        [out] native int data) runtime managed internalcall
    {
      .param [1]
      .custom instance void [mscorlib]System.Runtime.InteropServices.ComAliasNameAttribute::.ctor(string) = ( 01 00 13 43 6F 72 53 79 6D 4C 69 62 2E 55 4C 4F   // ...DebuggerInterop.Symbols.ULO
                                                                                                              4E 47 5F 50 54 52 00 00 )                         // NG_PTR..
    } // end of method ISymUnmanagedWriter2::GetDebugInfo

    .method public hidebysig newslot abstract virtual 
            instance void  DefineSequencePoints([in] class DebuggerInterop.Symbols.ISymUnmanagedDocumentWriter  marshal( interface) document,
                                                [in] unsigned int32 spCount,
                                                [in] unsigned int32& offsets,
                                                [in] unsigned int32& lines,
                                                [in] unsigned int32& columns,
                                                [in] unsigned int32& endLines,
                                                [in] unsigned int32& endColumns) runtime managed internalcall
    {
    } // end of method ISymUnmanagedWriter2::DefineSequencePoints

    .method public hidebysig newslot abstract virtual 
            instance void  RemapToken([in] unsigned int32 oldToken,
                                      [in] unsigned int32 newToken) runtime managed internalcall
    {
    } // end of method ISymUnmanagedWriter2::RemapToken

    .method public hidebysig newslot abstract virtual 
            instance void  Initialize2([in] object  marshal( iunknown) emitter,
                                       [in] unsigned int16& tempfilename,
                                       [in] class DebuggerInterop.Symbols.IStream  marshal( interface) pIStream,
                                       [in] int32 fFullBuild,
                                       [in] unsigned int16& finalfilename) runtime managed internalcall
    {
    } // end of method ISymUnmanagedWriter2::Initialize2

    .method public hidebysig newslot abstract virtual 
            instance void  DefineConstant([in] unsigned int16& name,
                                          [in] object  marshal( struct) 'value',
                                          [in] unsigned int32 cSig,
                                          [in] unsigned int8& signature) runtime managed internalcall
    {
    } // end of method ISymUnmanagedWriter2::DefineConstant

    .method public hidebysig newslot abstract virtual 
            instance void  Abort() runtime managed internalcall
    {
    } // end of method ISymUnmanagedWriter2::Abort

    .method public hidebysig newslot abstract virtual 
            instance void  DefineLocalVariable2([in] unsigned int16& name,
                                                [in] unsigned int32 attributes,
                                                [in] unsigned int32 sigToken,
                                                [in] unsigned int32 addrKind,
                                                [in] unsigned int32 addr1,
                                                [in] unsigned int32 addr2,
                                                [in] unsigned int32 addr3,
                                                [in] unsigned int32 startOffset,
                                                [in] unsigned int32 endOffset) runtime managed internalcall
    {
    } // end of method ISymUnmanagedWriter2::DefineLocalVariable2

    .method public hidebysig newslot abstract virtual 
            instance void  DefineGlobalVariable2([in] unsigned int16& name,
                                                 [in] unsigned int32 attributes,
                                                 [in] unsigned int32 sigToken,
                                                 [in] unsigned int32 addrKind,
                                                 [in] unsigned int32 addr1,
                                                 [in] unsigned int32 addr2,
                                                 [in] unsigned int32 addr3) runtime managed internalcall
    {
    } // end of method ISymUnmanagedWriter2::DefineGlobalVariable2

    .method public hidebysig newslot abstract virtual 
            instance void  DefineConstant2([in] unsigned int16& name,
                                           [in] object  marshal( struct) 'value',
                                           [in] unsigned int32 sigToken) runtime managed internalcall
    {
    } // end of method ISymUnmanagedWriter2::DefineConstant2

  } // end of class ISymUnmanagedWriter2


// =============================================================

} // end of namespace Debugger.Symbols

//*********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file DebuggerInterop.Symbols.res
