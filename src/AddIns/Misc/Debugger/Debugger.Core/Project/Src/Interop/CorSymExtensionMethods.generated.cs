// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Debugger.Interop.CorSym
{
	public static partial class CorSymExtensionMethods
	{
		static void ProcessOutParameter(object parameter)
		{
		}
		
		public static ISymUnmanagedReader GetReaderForFile(this CorSymBinder_SxSClass instance, object importer, IntPtr filename, IntPtr searchPath)
		{
			ISymUnmanagedReader returnValue = instance.GetReaderForFile(importer, filename, searchPath);
			ProcessOutParameter(returnValue);
			return returnValue;
		}
		
		public static ISymUnmanagedReader GetReaderFromStream(this CorSymBinder_SxSClass instance, object importer, IStream pstream)
		{
			ISymUnmanagedReader returnValue = instance.GetReaderFromStream(importer, pstream);
			ProcessOutParameter(returnValue);
			return returnValue;
		}
		
		public static ISymUnmanagedReader GetReaderForFile(this CorSymBinder_deprecatedClass instance, object importer, IntPtr filename, IntPtr searchPath)
		{
			ISymUnmanagedReader returnValue = instance.GetReaderForFile(importer, filename, searchPath);
			ProcessOutParameter(returnValue);
			return returnValue;
		}
		
		public static ISymUnmanagedReader GetReaderFromStream(this CorSymBinder_deprecatedClass instance, object importer, IStream pstream)
		{
			ISymUnmanagedReader returnValue = instance.GetReaderFromStream(importer, pstream);
			ProcessOutParameter(returnValue);
			return returnValue;
		}
		
		public static ISymUnmanagedDocument GetDocument(this CorSymReader_SxSClass instance, IntPtr url, Guid language, Guid languageVendor, Guid documentType)
		{
			ISymUnmanagedDocument returnValue = instance.GetDocument(url, language, languageVendor, documentType);
			ProcessOutParameter(returnValue);
			return returnValue;
		}
		
		public static void GetDocuments(this CorSymReader_SxSClass instance, uint cDocs, out uint pcDocs, ISymUnmanagedDocument[] pDocs)
		{
			instance.GetDocuments(cDocs, out pcDocs, pDocs);
			ProcessOutParameter(pDocs);
		}
		
		public static void GetDocumentVersion(this CorSymReader_SxSClass instance, ISymUnmanagedDocument pDoc, out int version, out int pbCurrent)
		{
			instance.GetDocumentVersion(pDoc, out version, out pbCurrent);
		}
		
		public static void GetGlobalVariables(this CorSymReader_SxSClass instance, uint cVars, out uint pcVars, IntPtr pVars)
		{
			instance.GetGlobalVariables(cVars, out pcVars, pVars);
		}
		
		public static ISymUnmanagedMethod GetMethod(this CorSymReader_SxSClass instance, uint token)
		{
			ISymUnmanagedMethod returnValue = instance.GetMethod(token);
			ProcessOutParameter(returnValue);
			return returnValue;
		}
		
		public static ISymUnmanagedMethod GetMethodByVersion(this CorSymReader_SxSClass instance, uint token, int version)
		{
			ISymUnmanagedMethod returnValue = instance.GetMethodByVersion(token, version);
			ProcessOutParameter(returnValue);
			return returnValue;
		}
		
		public static ISymUnmanagedMethod GetMethodFromDocumentPosition(this CorSymReader_SxSClass instance, ISymUnmanagedDocument document, uint line, uint column)
		{
			ISymUnmanagedMethod returnValue = instance.GetMethodFromDocumentPosition(document, line, column);
			ProcessOutParameter(returnValue);
			return returnValue;
		}
		
		public static void GetMethodsFromDocumentPosition(this CorSymReader_SxSClass instance, ISymUnmanagedDocument document, uint line, uint column, uint cMethod, out uint pcMethod, IntPtr pRetVal)
		{
			instance.GetMethodsFromDocumentPosition(document, line, column, cMethod, out pcMethod, pRetVal);
		}
		
		public static int GetMethodVersion(this CorSymReader_SxSClass instance, ISymUnmanagedMethod pMethod)
		{
			int version;
			instance.GetMethodVersion(pMethod, out version);
			return version;
		}
		
		public static void GetNamespaces(this CorSymReader_SxSClass instance, uint cNameSpaces, out uint pcNameSpaces, IntPtr namespaces)
		{
			instance.GetNamespaces(cNameSpaces, out pcNameSpaces, namespaces);
		}
		
		public static void GetSymAttribute(this CorSymReader_SxSClass instance, uint parent, IntPtr name, uint cBuffer, out uint pcBuffer, IntPtr buffer)
		{
			instance.GetSymAttribute(parent, name, cBuffer, out pcBuffer, buffer);
		}
		
		public static void GetSymbolStoreFileName(this CorSymReader_SxSClass instance, uint cchName, out uint pcchName, IntPtr szName)
		{
			instance.GetSymbolStoreFileName(cchName, out pcchName, szName);
		}
		
		public static uint GetUserEntryPoint(this CorSymReader_SxSClass instance)
		{
			return instance.GetUserEntryPoint();
		}
		
		public static void GetVariables(this CorSymReader_SxSClass instance, uint parent, uint cVars, out uint pcVars, IntPtr pVars)
		{
			instance.GetVariables(parent, cVars, out pcVars, pVars);
		}
		
		public static void Initialize(this CorSymReader_SxSClass instance, object importer, IntPtr filename, IntPtr searchPath, IStream pIStream)
		{
			instance.Initialize(importer, filename, searchPath, pIStream);
		}
		
		public static void ReplaceSymbolStore(this CorSymReader_SxSClass instance, IntPtr filename, IStream pIStream)
		{
			instance.ReplaceSymbolStore(filename, pIStream);
		}
		
		public static void UpdateSymbolStore(this CorSymReader_SxSClass instance, IntPtr filename, IStream pIStream)
		{
			instance.UpdateSymbolStore(filename, pIStream);
		}
		
		public static ISymUnmanagedDocument GetDocument(this CorSymReader_deprecatedClass instance, IntPtr url, Guid language, Guid languageVendor, Guid documentType)
		{
			ISymUnmanagedDocument returnValue = instance.GetDocument(url, language, languageVendor, documentType);
			ProcessOutParameter(returnValue);
			return returnValue;
		}
		
		public static void GetDocuments(this CorSymReader_deprecatedClass instance, uint cDocs, out uint pcDocs, ISymUnmanagedDocument[] pDocs)
		{
			instance.GetDocuments(cDocs, out pcDocs, pDocs);
			ProcessOutParameter(pDocs);
		}
		
		public static void GetDocumentVersion(this CorSymReader_deprecatedClass instance, ISymUnmanagedDocument pDoc, out int version, out int pbCurrent)
		{
			instance.GetDocumentVersion(pDoc, out version, out pbCurrent);
		}
		
		public static void GetGlobalVariables(this CorSymReader_deprecatedClass instance, uint cVars, out uint pcVars, IntPtr pVars)
		{
			instance.GetGlobalVariables(cVars, out pcVars, pVars);
		}
		
		public static ISymUnmanagedMethod GetMethod(this CorSymReader_deprecatedClass instance, uint token)
		{
			ISymUnmanagedMethod returnValue = instance.GetMethod(token);
			ProcessOutParameter(returnValue);
			return returnValue;
		}
		
		public static ISymUnmanagedMethod GetMethodByVersion(this CorSymReader_deprecatedClass instance, uint token, int version)
		{
			ISymUnmanagedMethod returnValue = instance.GetMethodByVersion(token, version);
			ProcessOutParameter(returnValue);
			return returnValue;
		}
		
		public static ISymUnmanagedMethod GetMethodFromDocumentPosition(this CorSymReader_deprecatedClass instance, ISymUnmanagedDocument document, uint line, uint column)
		{
			ISymUnmanagedMethod returnValue = instance.GetMethodFromDocumentPosition(document, line, column);
			ProcessOutParameter(returnValue);
			return returnValue;
		}
		
		public static void GetMethodsFromDocumentPosition(this CorSymReader_deprecatedClass instance, ISymUnmanagedDocument document, uint line, uint column, uint cMethod, out uint pcMethod, IntPtr pRetVal)
		{
			instance.GetMethodsFromDocumentPosition(document, line, column, cMethod, out pcMethod, pRetVal);
		}
		
		public static int GetMethodVersion(this CorSymReader_deprecatedClass instance, ISymUnmanagedMethod pMethod)
		{
			int version;
			instance.GetMethodVersion(pMethod, out version);
			return version;
		}
		
		public static void GetNamespaces(this CorSymReader_deprecatedClass instance, uint cNameSpaces, out uint pcNameSpaces, IntPtr namespaces)
		{
			instance.GetNamespaces(cNameSpaces, out pcNameSpaces, namespaces);
		}
		
		public static void GetSymAttribute(this CorSymReader_deprecatedClass instance, uint parent, IntPtr name, uint cBuffer, out uint pcBuffer, IntPtr buffer)
		{
			instance.GetSymAttribute(parent, name, cBuffer, out pcBuffer, buffer);
		}
		
		public static void GetSymbolStoreFileName(this CorSymReader_deprecatedClass instance, uint cchName, out uint pcchName, IntPtr szName)
		{
			instance.GetSymbolStoreFileName(cchName, out pcchName, szName);
		}
		
		public static uint GetUserEntryPoint(this CorSymReader_deprecatedClass instance)
		{
			return instance.GetUserEntryPoint();
		}
		
		public static void GetVariables(this CorSymReader_deprecatedClass instance, uint parent, uint cVars, out uint pcVars, IntPtr pVars)
		{
			instance.GetVariables(parent, cVars, out pcVars, pVars);
		}
		
		public static void Initialize(this CorSymReader_deprecatedClass instance, object importer, IntPtr filename, IntPtr searchPath, IStream pIStream)
		{
			instance.Initialize(importer, filename, searchPath, pIStream);
		}
		
		public static void ReplaceSymbolStore(this CorSymReader_deprecatedClass instance, IntPtr filename, IStream pIStream)
		{
			instance.ReplaceSymbolStore(filename, pIStream);
		}
		
		public static void UpdateSymbolStore(this CorSymReader_deprecatedClass instance, IntPtr filename, IStream pIStream)
		{
			instance.UpdateSymbolStore(filename, pIStream);
		}
		
		public static void Abort(this CorSymWriter_SxSClass instance)
		{
			instance.Abort();
		}
		
		public static void Close(this CorSymWriter_SxSClass instance)
		{
			instance.Close();
		}
		
		public static void CloseMethod(this CorSymWriter_SxSClass instance)
		{
			instance.CloseMethod();
		}
		
		public static void CloseNamespace(this CorSymWriter_SxSClass instance)
		{
			instance.CloseNamespace();
		}
		
		public static void CloseScope(this CorSymWriter_SxSClass instance, uint endOffset)
		{
			instance.CloseScope(endOffset);
		}
		
		public static void DefineConstant(this CorSymWriter_SxSClass instance, IntPtr name, object value, uint cSig, ref byte signature)
		{
			instance.DefineConstant(name, value, cSig, ref signature);
			ProcessOutParameter(signature);
		}
		
		public static ISymUnmanagedDocumentWriter DefineDocument(this CorSymWriter_SxSClass instance, IntPtr url, ref Guid language, ref Guid languageVendor, ref Guid documentType)
		{
			ISymUnmanagedDocumentWriter returnValue = instance.DefineDocument(url, ref language, ref languageVendor, ref documentType);
			ProcessOutParameter(returnValue);
			return returnValue;
		}
		
		public static void DefineField(this CorSymWriter_SxSClass instance, uint parent, IntPtr name, uint attributes, uint cSig, ref byte signature, uint addrKind, uint addr1, uint addr2, uint addr3)
		{
			instance.DefineField(parent, name, attributes, cSig, ref signature, addrKind, addr1, addr2, addr3);
			ProcessOutParameter(signature);
		}
		
		public static void DefineGlobalVariable(this CorSymWriter_SxSClass instance, IntPtr name, uint attributes, uint cSig, ref byte signature, uint addrKind, uint addr1, uint addr2, uint addr3)
		{
			instance.DefineGlobalVariable(name, attributes, cSig, ref signature, addrKind, addr1, addr2, addr3);
			ProcessOutParameter(signature);
		}
		
		public static void DefineLocalVariable(this CorSymWriter_SxSClass instance, IntPtr name, uint attributes, uint cSig, ref byte signature, uint addrKind, uint addr1, uint addr2, uint addr3, uint startOffset,
		uint endOffset)
		{
			instance.DefineLocalVariable(name, attributes, cSig, ref signature, addrKind, addr1, addr2, addr3, startOffset, endOffset);
			ProcessOutParameter(signature);
		}
		
		public static void DefineParameter(this CorSymWriter_SxSClass instance, IntPtr name, uint attributes, uint sequence, uint addrKind, uint addr1, uint addr2, uint addr3)
		{
			instance.DefineParameter(name, attributes, sequence, addrKind, addr1, addr2, addr3);
		}
		
		public static void DefineSequencePoints(this CorSymWriter_SxSClass instance, ISymUnmanagedDocumentWriter document, uint spCount, ref uint offsets, ref uint lines, ref uint columns, ref uint endLines, ref uint endColumns)
		{
			instance.DefineSequencePoints(document, spCount, ref offsets, ref lines, ref columns, ref endLines, ref endColumns);
		}
		
		public static void GetDebugInfo(this CorSymWriter_SxSClass instance, ref uint pIDD, uint cData, out uint pcData, IntPtr data)
		{
			instance.GetDebugInfo(ref pIDD, cData, out pcData, data);
		}
		
		public static void Initialize(this CorSymWriter_SxSClass instance, object emitter, IntPtr filename, IStream pIStream, int fFullBuild)
		{
			instance.Initialize(emitter, filename, pIStream, fFullBuild);
		}
		
		public static void Initialize2(this CorSymWriter_SxSClass instance, object emitter, IntPtr tempfilename, IStream pIStream, int fFullBuild, IntPtr finalfilename)
		{
			instance.Initialize2(emitter, tempfilename, pIStream, fFullBuild, finalfilename);
		}
		
		public static void OpenMethod(this CorSymWriter_SxSClass instance, uint method)
		{
			instance.OpenMethod(method);
		}
		
		public static void OpenNamespace(this CorSymWriter_SxSClass instance, IntPtr name)
		{
			instance.OpenNamespace(name);
		}
		
		public static uint OpenScope(this CorSymWriter_SxSClass instance, uint startOffset)
		{
			return instance.OpenScope(startOffset);
		}
		
		public static void RemapToken(this CorSymWriter_SxSClass instance, uint oldToken, uint newToken)
		{
			instance.RemapToken(oldToken, newToken);
		}
		
		public static void SetMethodSourceRange(this CorSymWriter_SxSClass instance, ISymUnmanagedDocumentWriter startDoc, uint startLine, uint startColumn, ISymUnmanagedDocumentWriter endDoc, uint endLine, uint endColumn)
		{
			instance.SetMethodSourceRange(startDoc, startLine, startColumn, endDoc, endLine, endColumn);
		}
		
		public static void SetScopeRange(this CorSymWriter_SxSClass instance, uint scopeID, uint startOffset, uint endOffset)
		{
			instance.SetScopeRange(scopeID, startOffset, endOffset);
		}
		
		public static void SetSymAttribute(this CorSymWriter_SxSClass instance, uint parent, IntPtr name, uint cData, ref byte data)
		{
			instance.SetSymAttribute(parent, name, cData, ref data);
			ProcessOutParameter(data);
		}
		
		public static void SetUserEntryPoint(this CorSymWriter_SxSClass instance, uint entryMethod)
		{
			instance.SetUserEntryPoint(entryMethod);
		}
		
		public static void UsingNamespace(this CorSymWriter_SxSClass instance, IntPtr fullName)
		{
			instance.UsingNamespace(fullName);
		}
		
		public static void Abort(this CorSymWriter_deprecatedClass instance)
		{
			instance.Abort();
		}
		
		public static void Close(this CorSymWriter_deprecatedClass instance)
		{
			instance.Close();
		}
		
		public static void CloseMethod(this CorSymWriter_deprecatedClass instance)
		{
			instance.CloseMethod();
		}
		
		public static void CloseNamespace(this CorSymWriter_deprecatedClass instance)
		{
			instance.CloseNamespace();
		}
		
		public static void CloseScope(this CorSymWriter_deprecatedClass instance, uint endOffset)
		{
			instance.CloseScope(endOffset);
		}
		
		public static void DefineConstant(this CorSymWriter_deprecatedClass instance, IntPtr name, object value, uint cSig, ref byte signature)
		{
			instance.DefineConstant(name, value, cSig, ref signature);
			ProcessOutParameter(signature);
		}
		
		public static ISymUnmanagedDocumentWriter DefineDocument(this CorSymWriter_deprecatedClass instance, IntPtr url, ref Guid language, ref Guid languageVendor, ref Guid documentType)
		{
			ISymUnmanagedDocumentWriter returnValue = instance.DefineDocument(url, ref language, ref languageVendor, ref documentType);
			ProcessOutParameter(returnValue);
			return returnValue;
		}
		
		public static void DefineField(this CorSymWriter_deprecatedClass instance, uint parent, IntPtr name, uint attributes, uint cSig, ref byte signature, uint addrKind, uint addr1, uint addr2, uint addr3)
		{
			instance.DefineField(parent, name, attributes, cSig, ref signature, addrKind, addr1, addr2, addr3);
			ProcessOutParameter(signature);
		}
		
		public static void DefineGlobalVariable(this CorSymWriter_deprecatedClass instance, IntPtr name, uint attributes, uint cSig, ref byte signature, uint addrKind, uint addr1, uint addr2, uint addr3)
		{
			instance.DefineGlobalVariable(name, attributes, cSig, ref signature, addrKind, addr1, addr2, addr3);
			ProcessOutParameter(signature);
		}
		
		public static void DefineLocalVariable(this CorSymWriter_deprecatedClass instance, IntPtr name, uint attributes, uint cSig, ref byte signature, uint addrKind, uint addr1, uint addr2, uint addr3, uint startOffset,
		uint endOffset)
		{
			instance.DefineLocalVariable(name, attributes, cSig, ref signature, addrKind, addr1, addr2, addr3, startOffset, endOffset);
			ProcessOutParameter(signature);
		}
		
		public static void DefineParameter(this CorSymWriter_deprecatedClass instance, IntPtr name, uint attributes, uint sequence, uint addrKind, uint addr1, uint addr2, uint addr3)
		{
			instance.DefineParameter(name, attributes, sequence, addrKind, addr1, addr2, addr3);
		}
		
		public static void DefineSequencePoints(this CorSymWriter_deprecatedClass instance, ISymUnmanagedDocumentWriter document, uint spCount, ref uint offsets, ref uint lines, ref uint columns, ref uint endLines, ref uint endColumns)
		{
			instance.DefineSequencePoints(document, spCount, ref offsets, ref lines, ref columns, ref endLines, ref endColumns);
		}
		
		public static void GetDebugInfo(this CorSymWriter_deprecatedClass instance, ref uint pIDD, uint cData, out uint pcData, IntPtr data)
		{
			instance.GetDebugInfo(ref pIDD, cData, out pcData, data);
		}
		
		public static void Initialize(this CorSymWriter_deprecatedClass instance, object emitter, IntPtr filename, IStream pIStream, int fFullBuild)
		{
			instance.Initialize(emitter, filename, pIStream, fFullBuild);
		}
		
		public static void Initialize2(this CorSymWriter_deprecatedClass instance, object emitter, IntPtr tempfilename, IStream pIStream, int fFullBuild, IntPtr finalfilename)
		{
			instance.Initialize2(emitter, tempfilename, pIStream, fFullBuild, finalfilename);
		}
		
		public static void OpenMethod(this CorSymWriter_deprecatedClass instance, uint method)
		{
			instance.OpenMethod(method);
		}
		
		public static void OpenNamespace(this CorSymWriter_deprecatedClass instance, IntPtr name)
		{
			instance.OpenNamespace(name);
		}
		
		public static uint OpenScope(this CorSymWriter_deprecatedClass instance, uint startOffset)
		{
			return instance.OpenScope(startOffset);
		}
		
		public static void RemapToken(this CorSymWriter_deprecatedClass instance, uint oldToken, uint newToken)
		{
			instance.RemapToken(oldToken, newToken);
		}
		
		public static void SetMethodSourceRange(this CorSymWriter_deprecatedClass instance, ISymUnmanagedDocumentWriter startDoc, uint startLine, uint startColumn, ISymUnmanagedDocumentWriter endDoc, uint endLine, uint endColumn)
		{
			instance.SetMethodSourceRange(startDoc, startLine, startColumn, endDoc, endLine, endColumn);
		}
		
		public static void SetScopeRange(this CorSymWriter_deprecatedClass instance, uint scopeID, uint startOffset, uint endOffset)
		{
			instance.SetScopeRange(scopeID, startOffset, endOffset);
		}
		
		public static void SetSymAttribute(this CorSymWriter_deprecatedClass instance, uint parent, IntPtr name, uint cData, ref byte data)
		{
			instance.SetSymAttribute(parent, name, cData, ref data);
			ProcessOutParameter(data);
		}
		
		public static void SetUserEntryPoint(this CorSymWriter_deprecatedClass instance, uint entryMethod)
		{
			instance.SetUserEntryPoint(entryMethod);
		}
		
		public static void UsingNamespace(this CorSymWriter_deprecatedClass instance, IntPtr fullName)
		{
			instance.UsingNamespace(fullName);
		}
		
		public static void RemoteRead(this ISequentialStream instance, out byte pv, uint cb, out uint pcbRead)
		{
			instance.RemoteRead(out pv, cb, out pcbRead);
			ProcessOutParameter(pv);
		}
		
		public static uint RemoteWrite(this ISequentialStream instance, ref byte pv, uint cb)
		{
			uint pcbWritten;
			instance.RemoteWrite(ref pv, cb, out pcbWritten);
			ProcessOutParameter(pv);
			return pcbWritten;
		}
		
		public static void RemoteRead(this IStream instance, out byte pv, uint cb, out uint pcbRead)
		{
			instance.RemoteRead(out pv, cb, out pcbRead);
			ProcessOutParameter(pv);
		}
		
		public static uint RemoteWrite(this IStream instance, ref byte pv, uint cb)
		{
			uint pcbWritten;
			instance.RemoteWrite(ref pv, cb, out pcbWritten);
			ProcessOutParameter(pv);
			return pcbWritten;
		}
		
		public static _ULARGE_INTEGER RemoteSeek(this IStream instance, _LARGE_INTEGER dlibMove, uint dwOrigin)
		{
			_ULARGE_INTEGER plibNewPosition;
			instance.RemoteSeek(dlibMove, dwOrigin, out plibNewPosition);
			ProcessOutParameter(plibNewPosition);
			return plibNewPosition;
		}
		
		public static void SetSize(this IStream instance, _ULARGE_INTEGER libNewSize)
		{
			instance.SetSize(libNewSize);
		}
		
		public static void RemoteCopyTo(this IStream instance, IStream pstm, _ULARGE_INTEGER cb, out _ULARGE_INTEGER pcbRead, out _ULARGE_INTEGER pcbWritten)
		{
			instance.RemoteCopyTo(pstm, cb, out pcbRead, out pcbWritten);
			ProcessOutParameter(pcbRead);
			ProcessOutParameter(pcbWritten);
		}
		
		public static void Commit(this IStream instance, uint grfCommitFlags)
		{
			instance.Commit(grfCommitFlags);
		}
		
		public static void Revert(this IStream instance)
		{
			instance.Revert();
		}
		
		public static void LockRegion(this IStream instance, _ULARGE_INTEGER libOffset, _ULARGE_INTEGER cb, uint dwLockType)
		{
			instance.LockRegion(libOffset, cb, dwLockType);
		}
		
		public static void UnlockRegion(this IStream instance, _ULARGE_INTEGER libOffset, _ULARGE_INTEGER cb, uint dwLockType)
		{
			instance.UnlockRegion(libOffset, cb, dwLockType);
		}
		
		public static void Stat(this IStream instance, out tagSTATSTG pstatstg, uint grfStatFlag)
		{
			instance.Stat(out pstatstg, grfStatFlag);
			ProcessOutParameter(pstatstg);
		}
		
		public static IStream Clone(this IStream instance)
		{
			IStream ppstm;
			instance.Clone(out ppstm);
			ProcessOutParameter(ppstm);
			return ppstm;
		}
		
		public static ISymUnmanagedReader GetReaderForFile(this ISymUnmanagedBinder instance, object importer, IntPtr filename, IntPtr searchPath)
		{
			ISymUnmanagedReader returnValue = instance.GetReaderForFile(importer, filename, searchPath);
			ProcessOutParameter(returnValue);
			return returnValue;
		}
		
		public static ISymUnmanagedReader GetReaderFromStream(this ISymUnmanagedBinder instance, object importer, IStream pstream)
		{
			ISymUnmanagedReader returnValue = instance.GetReaderFromStream(importer, pstream);
			ProcessOutParameter(returnValue);
			return returnValue;
		}
		
		public static void Destroy(this ISymUnmanagedDispose instance)
		{
			instance.Destroy();
		}
		
		public static void GetURL(this ISymUnmanagedDocument instance, uint cchUrl, out uint pcchUrl, IntPtr szUrl)
		{
			instance.GetURL(cchUrl, out pcchUrl, szUrl);
		}
		
		public static Guid GetDocumentType(this ISymUnmanagedDocument instance)
		{
			return instance.GetDocumentType();
		}
		
		public static Guid GetLanguage(this ISymUnmanagedDocument instance)
		{
			return instance.GetLanguage();
		}
		
		public static Guid GetLanguageVendor(this ISymUnmanagedDocument instance)
		{
			return instance.GetLanguageVendor();
		}
		
		public static Guid GetCheckSumAlgorithmId(this ISymUnmanagedDocument instance)
		{
			return instance.GetCheckSumAlgorithmId();
		}
		
		public static void GetCheckSum(this ISymUnmanagedDocument instance, uint cData, out uint pcData, IntPtr data)
		{
			instance.GetCheckSum(cData, out pcData, data);
		}
		
		public static uint FindClosestLine(this ISymUnmanagedDocument instance, uint line)
		{
			return instance.FindClosestLine(line);
		}
		
		public static int HasEmbeddedSource(this ISymUnmanagedDocument instance)
		{
			return instance.HasEmbeddedSource();
		}
		
		public static uint GetSourceLength(this ISymUnmanagedDocument instance)
		{
			return instance.GetSourceLength();
		}
		
		public static void GetSourceRange(this ISymUnmanagedDocument instance, uint startLine, uint startColumn, uint endLine, uint endColumn, uint cSourceBytes, out uint pcSourceBytes, IntPtr source)
		{
			instance.GetSourceRange(startLine, startColumn, endLine, endColumn, cSourceBytes, out pcSourceBytes, source);
		}
		
		public static void SetSource(this ISymUnmanagedDocumentWriter instance, uint sourceSize, ref byte source)
		{
			instance.SetSource(sourceSize, ref source);
			ProcessOutParameter(source);
		}
		
		public static void SetCheckSum(this ISymUnmanagedDocumentWriter instance, Guid algorithmId, uint checkSumSize, ref byte checkSum)
		{
			instance.SetCheckSum(algorithmId, checkSumSize, ref checkSum);
			ProcessOutParameter(checkSum);
		}
		
		public static uint GetToken(this ISymUnmanagedMethod instance)
		{
			return instance.GetToken();
		}
		
		public static uint GetSequencePointCount(this ISymUnmanagedMethod instance)
		{
			return instance.GetSequencePointCount();
		}
		
		public static ISymUnmanagedScope GetRootScope(this ISymUnmanagedMethod instance)
		{
			ISymUnmanagedScope returnValue = instance.GetRootScope();
			ProcessOutParameter(returnValue);
			return returnValue;
		}
		
		public static ISymUnmanagedScope GetScopeFromOffset(this ISymUnmanagedMethod instance, uint offset)
		{
			ISymUnmanagedScope returnValue = instance.GetScopeFromOffset(offset);
			ProcessOutParameter(returnValue);
			return returnValue;
		}
		
		public static uint GetOffset(this ISymUnmanagedMethod instance, ISymUnmanagedDocument document, uint line, uint column)
		{
			return instance.GetOffset(document, line, column);
		}
		
		public static void GetRanges(this ISymUnmanagedMethod instance, ISymUnmanagedDocument document, uint line, uint column, uint cRanges, out uint pcRanges, IntPtr ranges)
		{
			instance.GetRanges(document, line, column, cRanges, out pcRanges, ranges);
		}
		
		public static void GetParameters(this ISymUnmanagedMethod instance, uint cParams, out uint pcParams, IntPtr @params)
		{
			instance.GetParameters(cParams, out pcParams, @params);
		}
		
		public static ISymUnmanagedNamespace GetNamespace(this ISymUnmanagedMethod instance)
		{
			ISymUnmanagedNamespace pRetVal;
			instance.GetNamespace(out pRetVal);
			ProcessOutParameter(pRetVal);
			return pRetVal;
		}
		
		public static int GetSourceStartEnd(this ISymUnmanagedMethod instance, ISymUnmanagedDocument[] docs, uint[] lines, uint[] columns)
		{
			int pRetVal;
			instance.GetSourceStartEnd(docs, lines, columns, out pRetVal);
			ProcessOutParameter(docs);
			return pRetVal;
		}
		
		public static void GetSequencePoints(this ISymUnmanagedMethod instance, uint cPoints, out uint pcPoints, uint[] offsets, ISymUnmanagedDocument[] documents, uint[] lines, uint[] columns, uint[] endLines, uint[] endColumns)
		{
			instance.GetSequencePoints(cPoints, out pcPoints, offsets, documents, lines, columns, endLines, endColumns);
			ProcessOutParameter(documents);
		}
		
		public static void GetName(this ISymUnmanagedNamespace instance, uint cchName, out uint pcchName, IntPtr szName)
		{
			instance.GetName(cchName, out pcchName, szName);
		}
		
		public static void GetNamespaces(this ISymUnmanagedNamespace instance, uint cNameSpaces, out uint pcNameSpaces, IntPtr namespaces)
		{
			instance.GetNamespaces(cNameSpaces, out pcNameSpaces, namespaces);
		}
		
		public static void GetVariables(this ISymUnmanagedNamespace instance, uint cVars, out uint pcVars, IntPtr pVars)
		{
			instance.GetVariables(cVars, out pcVars, pVars);
		}
		
		public static ISymUnmanagedDocument GetDocument(this ISymUnmanagedReader instance, IntPtr url, Guid language, Guid languageVendor, Guid documentType)
		{
			ISymUnmanagedDocument returnValue = instance.GetDocument(url, language, languageVendor, documentType);
			ProcessOutParameter(returnValue);
			return returnValue;
		}
		
		public static void GetDocuments(this ISymUnmanagedReader instance, uint cDocs, out uint pcDocs, ISymUnmanagedDocument[] pDocs)
		{
			instance.GetDocuments(cDocs, out pcDocs, pDocs);
			ProcessOutParameter(pDocs);
		}
		
		public static uint GetUserEntryPoint(this ISymUnmanagedReader instance)
		{
			return instance.GetUserEntryPoint();
		}
		
		public static ISymUnmanagedMethod GetMethod(this ISymUnmanagedReader instance, uint token)
		{
			ISymUnmanagedMethod returnValue = instance.GetMethod(token);
			ProcessOutParameter(returnValue);
			return returnValue;
		}
		
		public static ISymUnmanagedMethod GetMethodByVersion(this ISymUnmanagedReader instance, uint token, int version)
		{
			ISymUnmanagedMethod returnValue = instance.GetMethodByVersion(token, version);
			ProcessOutParameter(returnValue);
			return returnValue;
		}
		
		public static void GetVariables(this ISymUnmanagedReader instance, uint parent, uint cVars, out uint pcVars, IntPtr pVars)
		{
			instance.GetVariables(parent, cVars, out pcVars, pVars);
		}
		
		public static void GetGlobalVariables(this ISymUnmanagedReader instance, uint cVars, out uint pcVars, IntPtr pVars)
		{
			instance.GetGlobalVariables(cVars, out pcVars, pVars);
		}
		
		public static ISymUnmanagedMethod GetMethodFromDocumentPosition(this ISymUnmanagedReader instance, ISymUnmanagedDocument document, uint line, uint column)
		{
			ISymUnmanagedMethod returnValue = instance.GetMethodFromDocumentPosition(document, line, column);
			ProcessOutParameter(returnValue);
			return returnValue;
		}
		
		public static void GetSymAttribute(this ISymUnmanagedReader instance, uint parent, IntPtr name, uint cBuffer, out uint pcBuffer, IntPtr buffer)
		{
			instance.GetSymAttribute(parent, name, cBuffer, out pcBuffer, buffer);
		}
		
		public static void GetNamespaces(this ISymUnmanagedReader instance, uint cNameSpaces, out uint pcNameSpaces, IntPtr namespaces)
		{
			instance.GetNamespaces(cNameSpaces, out pcNameSpaces, namespaces);
		}
		
		public static void Initialize(this ISymUnmanagedReader instance, object importer, IntPtr filename, IntPtr searchPath, IStream pIStream)
		{
			instance.Initialize(importer, filename, searchPath, pIStream);
		}
		
		public static void UpdateSymbolStore(this ISymUnmanagedReader instance, IntPtr filename, IStream pIStream)
		{
			instance.UpdateSymbolStore(filename, pIStream);
		}
		
		public static void ReplaceSymbolStore(this ISymUnmanagedReader instance, IntPtr filename, IStream pIStream)
		{
			instance.ReplaceSymbolStore(filename, pIStream);
		}
		
		public static void GetSymbolStoreFileName(this ISymUnmanagedReader instance, uint cchName, out uint pcchName, IntPtr szName)
		{
			instance.GetSymbolStoreFileName(cchName, out pcchName, szName);
		}
		
		public static void GetMethodsFromDocumentPosition(this ISymUnmanagedReader instance, ISymUnmanagedDocument document, uint line, uint column, uint cMethod, out uint pcMethod, IntPtr pRetVal)
		{
			instance.GetMethodsFromDocumentPosition(document, line, column, cMethod, out pcMethod, pRetVal);
		}
		
		public static void GetDocumentVersion(this ISymUnmanagedReader instance, ISymUnmanagedDocument pDoc, out int version, out int pbCurrent)
		{
			instance.GetDocumentVersion(pDoc, out version, out pbCurrent);
		}
		
		public static int GetMethodVersion(this ISymUnmanagedReader instance, ISymUnmanagedMethod pMethod)
		{
			int version;
			instance.GetMethodVersion(pMethod, out version);
			return version;
		}
		
		public static uint GetSymbolSearchInfoCount(this ISymUnmanagedReaderSymbolSearchInfo instance)
		{
			uint pcSearchInfo;
			instance.GetSymbolSearchInfoCount(out pcSearchInfo);
			return pcSearchInfo;
		}
		
		public static void GetSymbolSearchInfo(this ISymUnmanagedReaderSymbolSearchInfo instance, uint cSearchInfo, out uint pcSearchInfo, out ISymUnmanagedSymbolSearchInfo rgpSearchInfo)
		{
			instance.GetSymbolSearchInfo(cSearchInfo, out pcSearchInfo, out rgpSearchInfo);
			ProcessOutParameter(rgpSearchInfo);
		}
		
		public static ISymUnmanagedMethod GetMethod(this ISymUnmanagedScope instance)
		{
			ISymUnmanagedMethod returnValue = instance.GetMethod();
			ProcessOutParameter(returnValue);
			return returnValue;
		}
		
		public static ISymUnmanagedScope GetParent(this ISymUnmanagedScope instance)
		{
			ISymUnmanagedScope returnValue = instance.GetParent();
			ProcessOutParameter(returnValue);
			return returnValue;
		}
		
		public static void GetChildren(this ISymUnmanagedScope instance, uint cChildren, out uint pcChildren, ISymUnmanagedScope[] children)
		{
			instance.GetChildren(cChildren, out pcChildren, children);
			ProcessOutParameter(children);
		}
		
		public static uint GetStartOffset(this ISymUnmanagedScope instance)
		{
			return instance.GetStartOffset();
		}
		
		public static uint GetEndOffset(this ISymUnmanagedScope instance)
		{
			return instance.GetEndOffset();
		}
		
		public static uint GetLocalCount(this ISymUnmanagedScope instance)
		{
			return instance.GetLocalCount();
		}
		
		public static void GetLocals(this ISymUnmanagedScope instance, uint cLocals, out uint pcLocals, ISymUnmanagedVariable[] locals)
		{
			instance.GetLocals(cLocals, out pcLocals, locals);
			ProcessOutParameter(locals);
		}
		
		public static void GetNamespaces(this ISymUnmanagedScope instance, uint cNameSpaces, out uint pcNameSpaces, ISymUnmanagedNamespace[] namespaces)
		{
			instance.GetNamespaces(cNameSpaces, out pcNameSpaces, namespaces);
			ProcessOutParameter(namespaces);
		}
		
		public static uint GetSearchPathLength(this ISymUnmanagedSymbolSearchInfo instance)
		{
			uint pcchPath;
			instance.GetSearchPathLength(out pcchPath);
			return pcchPath;
		}
		
		public static void GetSearchPath(this ISymUnmanagedSymbolSearchInfo instance, uint cchPath, out uint pcchPath, IntPtr szPath)
		{
			instance.GetSearchPath(cchPath, out pcchPath, szPath);
		}
		
		public static int GetHRESULT(this ISymUnmanagedSymbolSearchInfo instance)
		{
			int phr;
			instance.GetHRESULT(out phr);
			return phr;
		}
		
		public static void GetName(this ISymUnmanagedVariable instance, uint cchName, out uint pcchName, IntPtr szName)
		{
			instance.GetName(cchName, out pcchName, szName);
		}
		
		public static uint GetAttributes(this ISymUnmanagedVariable instance)
		{
			return instance.GetAttributes();
		}
		
		public static void GetSignature(this ISymUnmanagedVariable instance, uint cSig, out uint pcSig, IntPtr sig)
		{
			instance.GetSignature(cSig, out pcSig, sig);
		}
		
		public static uint GetAddressKind(this ISymUnmanagedVariable instance)
		{
			return instance.GetAddressKind();
		}
		
		public static uint GetAddressField1(this ISymUnmanagedVariable instance)
		{
			return instance.GetAddressField1();
		}
		
		public static uint GetAddressField2(this ISymUnmanagedVariable instance)
		{
			return instance.GetAddressField2();
		}
		
		public static uint GetAddressField3(this ISymUnmanagedVariable instance)
		{
			return instance.GetAddressField3();
		}
		
		public static uint GetStartOffset(this ISymUnmanagedVariable instance)
		{
			return instance.GetStartOffset();
		}
		
		public static uint GetEndOffset(this ISymUnmanagedVariable instance)
		{
			return instance.GetEndOffset();
		}
		
		public static ISymUnmanagedDocumentWriter DefineDocument(this ISymUnmanagedWriter instance, IntPtr url, ref Guid language, ref Guid languageVendor, ref Guid documentType)
		{
			ISymUnmanagedDocumentWriter returnValue = instance.DefineDocument(url, ref language, ref languageVendor, ref documentType);
			ProcessOutParameter(returnValue);
			return returnValue;
		}
		
		public static void SetUserEntryPoint(this ISymUnmanagedWriter instance, uint entryMethod)
		{
			instance.SetUserEntryPoint(entryMethod);
		}
		
		public static void OpenMethod(this ISymUnmanagedWriter instance, uint method)
		{
			instance.OpenMethod(method);
		}
		
		public static void CloseMethod(this ISymUnmanagedWriter instance)
		{
			instance.CloseMethod();
		}
		
		public static uint OpenScope(this ISymUnmanagedWriter instance, uint startOffset)
		{
			return instance.OpenScope(startOffset);
		}
		
		public static void CloseScope(this ISymUnmanagedWriter instance, uint endOffset)
		{
			instance.CloseScope(endOffset);
		}
		
		public static void SetScopeRange(this ISymUnmanagedWriter instance, uint scopeID, uint startOffset, uint endOffset)
		{
			instance.SetScopeRange(scopeID, startOffset, endOffset);
		}
		
		public static void DefineLocalVariable(this ISymUnmanagedWriter instance, IntPtr name, uint attributes, uint cSig, ref byte signature, uint addrKind, uint addr1, uint addr2, uint addr3, uint startOffset,
		uint endOffset)
		{
			instance.DefineLocalVariable(name, attributes, cSig, ref signature, addrKind, addr1, addr2, addr3, startOffset, endOffset);
			ProcessOutParameter(signature);
		}
		
		public static void DefineParameter(this ISymUnmanagedWriter instance, IntPtr name, uint attributes, uint sequence, uint addrKind, uint addr1, uint addr2, uint addr3)
		{
			instance.DefineParameter(name, attributes, sequence, addrKind, addr1, addr2, addr3);
		}
		
		public static void DefineField(this ISymUnmanagedWriter instance, uint parent, IntPtr name, uint attributes, uint cSig, ref byte signature, uint addrKind, uint addr1, uint addr2, uint addr3)
		{
			instance.DefineField(parent, name, attributes, cSig, ref signature, addrKind, addr1, addr2, addr3);
			ProcessOutParameter(signature);
		}
		
		public static void DefineGlobalVariable(this ISymUnmanagedWriter instance, IntPtr name, uint attributes, uint cSig, ref byte signature, uint addrKind, uint addr1, uint addr2, uint addr3)
		{
			instance.DefineGlobalVariable(name, attributes, cSig, ref signature, addrKind, addr1, addr2, addr3);
			ProcessOutParameter(signature);
		}
		
		public static void Close(this ISymUnmanagedWriter instance)
		{
			instance.Close();
		}
		
		public static void SetSymAttribute(this ISymUnmanagedWriter instance, uint parent, IntPtr name, uint cData, ref byte data)
		{
			instance.SetSymAttribute(parent, name, cData, ref data);
			ProcessOutParameter(data);
		}
		
		public static void OpenNamespace(this ISymUnmanagedWriter instance, IntPtr name)
		{
			instance.OpenNamespace(name);
		}
		
		public static void CloseNamespace(this ISymUnmanagedWriter instance)
		{
			instance.CloseNamespace();
		}
		
		public static void UsingNamespace(this ISymUnmanagedWriter instance, IntPtr fullName)
		{
			instance.UsingNamespace(fullName);
		}
		
		public static void SetMethodSourceRange(this ISymUnmanagedWriter instance, ISymUnmanagedDocumentWriter startDoc, uint startLine, uint startColumn, ISymUnmanagedDocumentWriter endDoc, uint endLine, uint endColumn)
		{
			instance.SetMethodSourceRange(startDoc, startLine, startColumn, endDoc, endLine, endColumn);
		}
		
		public static void Initialize(this ISymUnmanagedWriter instance, object emitter, IntPtr filename, IStream pIStream, int fFullBuild)
		{
			instance.Initialize(emitter, filename, pIStream, fFullBuild);
		}
		
		public static void GetDebugInfo(this ISymUnmanagedWriter instance, ref uint pIDD, uint cData, out uint pcData, IntPtr data)
		{
			instance.GetDebugInfo(ref pIDD, cData, out pcData, data);
		}
		
		public static void DefineSequencePoints(this ISymUnmanagedWriter instance, ISymUnmanagedDocumentWriter document, uint spCount, ref uint offsets, ref uint lines, ref uint columns, ref uint endLines, ref uint endColumns)
		{
			instance.DefineSequencePoints(document, spCount, ref offsets, ref lines, ref columns, ref endLines, ref endColumns);
		}
		
		public static void RemapToken(this ISymUnmanagedWriter instance, uint oldToken, uint newToken)
		{
			instance.RemapToken(oldToken, newToken);
		}
		
		public static void Initialize2(this ISymUnmanagedWriter instance, object emitter, IntPtr tempfilename, IStream pIStream, int fFullBuild, IntPtr finalfilename)
		{
			instance.Initialize2(emitter, tempfilename, pIStream, fFullBuild, finalfilename);
		}
		
		public static void DefineConstant(this ISymUnmanagedWriter instance, IntPtr name, object value, uint cSig, ref byte signature)
		{
			instance.DefineConstant(name, value, cSig, ref signature);
			ProcessOutParameter(signature);
		}
		
		public static void Abort(this ISymUnmanagedWriter instance)
		{
			instance.Abort();
		}
		
		public static ISymUnmanagedDocumentWriter DefineDocument(this ISymUnmanagedWriter2 instance, IntPtr url, ref Guid language, ref Guid languageVendor, ref Guid documentType)
		{
			ISymUnmanagedDocumentWriter returnValue = instance.DefineDocument(url, ref language, ref languageVendor, ref documentType);
			ProcessOutParameter(returnValue);
			return returnValue;
		}
		
		public static void SetUserEntryPoint(this ISymUnmanagedWriter2 instance, uint entryMethod)
		{
			instance.SetUserEntryPoint(entryMethod);
		}
		
		public static void OpenMethod(this ISymUnmanagedWriter2 instance, uint method)
		{
			instance.OpenMethod(method);
		}
		
		public static void CloseMethod(this ISymUnmanagedWriter2 instance)
		{
			instance.CloseMethod();
		}
		
		public static uint OpenScope(this ISymUnmanagedWriter2 instance, uint startOffset)
		{
			return instance.OpenScope(startOffset);
		}
		
		public static void CloseScope(this ISymUnmanagedWriter2 instance, uint endOffset)
		{
			instance.CloseScope(endOffset);
		}
		
		public static void SetScopeRange(this ISymUnmanagedWriter2 instance, uint scopeID, uint startOffset, uint endOffset)
		{
			instance.SetScopeRange(scopeID, startOffset, endOffset);
		}
		
		public static void DefineLocalVariable(this ISymUnmanagedWriter2 instance, IntPtr name, uint attributes, uint cSig, ref byte signature, uint addrKind, uint addr1, uint addr2, uint addr3, uint startOffset,
		uint endOffset)
		{
			instance.DefineLocalVariable(name, attributes, cSig, ref signature, addrKind, addr1, addr2, addr3, startOffset, endOffset);
			ProcessOutParameter(signature);
		}
		
		public static void DefineParameter(this ISymUnmanagedWriter2 instance, IntPtr name, uint attributes, uint sequence, uint addrKind, uint addr1, uint addr2, uint addr3)
		{
			instance.DefineParameter(name, attributes, sequence, addrKind, addr1, addr2, addr3);
		}
		
		public static void DefineField(this ISymUnmanagedWriter2 instance, uint parent, IntPtr name, uint attributes, uint cSig, ref byte signature, uint addrKind, uint addr1, uint addr2, uint addr3)
		{
			instance.DefineField(parent, name, attributes, cSig, ref signature, addrKind, addr1, addr2, addr3);
			ProcessOutParameter(signature);
		}
		
		public static void DefineGlobalVariable(this ISymUnmanagedWriter2 instance, IntPtr name, uint attributes, uint cSig, ref byte signature, uint addrKind, uint addr1, uint addr2, uint addr3)
		{
			instance.DefineGlobalVariable(name, attributes, cSig, ref signature, addrKind, addr1, addr2, addr3);
			ProcessOutParameter(signature);
		}
		
		public static void Close(this ISymUnmanagedWriter2 instance)
		{
			instance.Close();
		}
		
		public static void SetSymAttribute(this ISymUnmanagedWriter2 instance, uint parent, IntPtr name, uint cData, ref byte data)
		{
			instance.SetSymAttribute(parent, name, cData, ref data);
			ProcessOutParameter(data);
		}
		
		public static void OpenNamespace(this ISymUnmanagedWriter2 instance, IntPtr name)
		{
			instance.OpenNamespace(name);
		}
		
		public static void CloseNamespace(this ISymUnmanagedWriter2 instance)
		{
			instance.CloseNamespace();
		}
		
		public static void UsingNamespace(this ISymUnmanagedWriter2 instance, IntPtr fullName)
		{
			instance.UsingNamespace(fullName);
		}
		
		public static void SetMethodSourceRange(this ISymUnmanagedWriter2 instance, ISymUnmanagedDocumentWriter startDoc, uint startLine, uint startColumn, ISymUnmanagedDocumentWriter endDoc, uint endLine, uint endColumn)
		{
			instance.SetMethodSourceRange(startDoc, startLine, startColumn, endDoc, endLine, endColumn);
		}
		
		public static void Initialize(this ISymUnmanagedWriter2 instance, object emitter, IntPtr filename, IStream pIStream, int fFullBuild)
		{
			instance.Initialize(emitter, filename, pIStream, fFullBuild);
		}
		
		public static void GetDebugInfo(this ISymUnmanagedWriter2 instance, ref uint pIDD, uint cData, out uint pcData, IntPtr data)
		{
			instance.GetDebugInfo(ref pIDD, cData, out pcData, data);
		}
		
		public static void DefineSequencePoints(this ISymUnmanagedWriter2 instance, ISymUnmanagedDocumentWriter document, uint spCount, ref uint offsets, ref uint lines, ref uint columns, ref uint endLines, ref uint endColumns)
		{
			instance.DefineSequencePoints(document, spCount, ref offsets, ref lines, ref columns, ref endLines, ref endColumns);
		}
		
		public static void RemapToken(this ISymUnmanagedWriter2 instance, uint oldToken, uint newToken)
		{
			instance.RemapToken(oldToken, newToken);
		}
		
		public static void Initialize2(this ISymUnmanagedWriter2 instance, object emitter, IntPtr tempfilename, IStream pIStream, int fFullBuild, IntPtr finalfilename)
		{
			instance.Initialize2(emitter, tempfilename, pIStream, fFullBuild, finalfilename);
		}
		
		public static void DefineConstant(this ISymUnmanagedWriter2 instance, IntPtr name, object value, uint cSig, ref byte signature)
		{
			instance.DefineConstant(name, value, cSig, ref signature);
			ProcessOutParameter(signature);
		}
		
		public static void Abort(this ISymUnmanagedWriter2 instance)
		{
			instance.Abort();
		}
		
		public static void DefineLocalVariable2(this ISymUnmanagedWriter2 instance, IntPtr name, uint attributes, uint sigToken, uint addrKind, uint addr1, uint addr2, uint addr3, uint startOffset, uint endOffset)
		{
			instance.DefineLocalVariable2(name, attributes, sigToken, addrKind, addr1, addr2, addr3, startOffset, endOffset);
		}
		
		public static void DefineGlobalVariable2(this ISymUnmanagedWriter2 instance, IntPtr name, uint attributes, uint sigToken, uint addrKind, uint addr1, uint addr2, uint addr3)
		{
			instance.DefineGlobalVariable2(name, attributes, sigToken, addrKind, addr1, addr2, addr3);
		}
		
		public static void DefineConstant2(this ISymUnmanagedWriter2 instance, IntPtr name, object value, uint sigToken)
		{
			instance.DefineConstant2(name, value, sigToken);
		}
		
	}
}
