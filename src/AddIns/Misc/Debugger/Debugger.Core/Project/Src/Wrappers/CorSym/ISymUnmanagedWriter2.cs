// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Wrappers.CorSym
{
	using System;
	
	
	public class ISymUnmanagedWriter2
	{
		
		private Debugger.Interop.CorSym.ISymUnmanagedWriter2 wrappedObject;
		
		internal Debugger.Interop.CorSym.ISymUnmanagedWriter2 WrappedObject
		{
			get
			{
				return this.wrappedObject;
			}
		}
		
		public ISymUnmanagedWriter2(Debugger.Interop.CorSym.ISymUnmanagedWriter2 wrappedObject)
		{
			this.wrappedObject = wrappedObject;
			ResourceManager.TrackCOMObject(wrappedObject, typeof(ISymUnmanagedWriter2));
		}
		
		public static ISymUnmanagedWriter2 Wrap(Debugger.Interop.CorSym.ISymUnmanagedWriter2 objectToWrap)
		{
			return new ISymUnmanagedWriter2(objectToWrap);
		}
		
		~ISymUnmanagedWriter2()
		{
			object o = wrappedObject;
			wrappedObject = null;
			ResourceManager.ReleaseCOMObject(o, typeof(ISymUnmanagedWriter2));
		}
		
		public bool Is<T>() where T: class
		{
			try {
				CastTo<T>();
				return true;
			} catch {
				return false;
			}
		}
		
		public T As<T>() where T: class
		{
			try {
				return CastTo<T>();
			} catch {
				return null;
			}
		}
		
		public T CastTo<T>() where T: class
		{
			return (T)Activator.CreateInstance(typeof(T), this.WrappedObject);
		}
		
		public static bool operator ==(ISymUnmanagedWriter2 o1, ISymUnmanagedWriter2 o2)
		{
			return ((object)o1 == null && (object)o2 == null) ||
			       ((object)o1 != null && (object)o2 != null && o1.WrappedObject == o2.WrappedObject);
		}
		
		public static bool operator !=(ISymUnmanagedWriter2 o1, ISymUnmanagedWriter2 o2)
		{
			return !(o1 == o2);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		public override bool Equals(object o)
		{
			ISymUnmanagedWriter2 casted = o as ISymUnmanagedWriter2;
			return (casted != null) && (casted.WrappedObject == wrappedObject);
		}
		
		
		public ISymUnmanagedDocumentWriter DefineDocument(ref ushort url, ref System.Guid language, ref System.Guid languageVendor, ref System.Guid documentType)
		{
			return ISymUnmanagedDocumentWriter.Wrap(this.WrappedObject.DefineDocument(ref url, ref language, ref languageVendor, ref documentType));
		}
		
		public void SetUserEntryPoint(uint entryMethod)
		{
			this.WrappedObject.SetUserEntryPoint(entryMethod);
		}
		
		public void OpenMethod(uint method)
		{
			this.WrappedObject.OpenMethod(method);
		}
		
		public void CloseMethod()
		{
			this.WrappedObject.CloseMethod();
		}
		
		public uint OpenScope(uint startOffset)
		{
			return this.WrappedObject.OpenScope(startOffset);
		}
		
		public void CloseScope(uint endOffset)
		{
			this.WrappedObject.CloseScope(endOffset);
		}
		
		public void SetScopeRange(uint scopeID, uint startOffset, uint endOffset)
		{
			this.WrappedObject.SetScopeRange(scopeID, startOffset, endOffset);
		}
		
		public void DefineLocalVariable(ref ushort name, uint attributes, uint cSig, ref byte signature, uint addrKind, uint addr1, uint addr2, uint addr3, uint startOffset, uint endOffset)
		{
			this.WrappedObject.DefineLocalVariable(ref name, attributes, cSig, ref signature, addrKind, addr1, addr2, addr3, startOffset, endOffset);
		}
		
		public void DefineParameter(ref ushort name, uint attributes, uint sequence, uint addrKind, uint addr1, uint addr2, uint addr3)
		{
			this.WrappedObject.DefineParameter(ref name, attributes, sequence, addrKind, addr1, addr2, addr3);
		}
		
		public void DefineField(uint parent, ref ushort name, uint attributes, uint cSig, ref byte signature, uint addrKind, uint addr1, uint addr2, uint addr3)
		{
			this.WrappedObject.DefineField(parent, ref name, attributes, cSig, ref signature, addrKind, addr1, addr2, addr3);
		}
		
		public void DefineGlobalVariable(ref ushort name, uint attributes, uint cSig, ref byte signature, uint addrKind, uint addr1, uint addr2, uint addr3)
		{
			this.WrappedObject.DefineGlobalVariable(ref name, attributes, cSig, ref signature, addrKind, addr1, addr2, addr3);
		}
		
		public void Close()
		{
			this.WrappedObject.Close();
		}
		
		public void SetSymAttribute(uint parent, ref ushort name, uint cData, ref byte data)
		{
			this.WrappedObject.SetSymAttribute(parent, ref name, cData, ref data);
		}
		
		public void OpenNamespace(ref ushort name)
		{
			this.WrappedObject.OpenNamespace(ref name);
		}
		
		public void CloseNamespace()
		{
			this.WrappedObject.CloseNamespace();
		}
		
		public void UsingNamespace(ref ushort fullName)
		{
			this.WrappedObject.UsingNamespace(ref fullName);
		}
		
		public void SetMethodSourceRange(ISymUnmanagedDocumentWriter startDoc, uint startLine, uint startColumn, ISymUnmanagedDocumentWriter endDoc, uint endLine, uint endColumn)
		{
			this.WrappedObject.SetMethodSourceRange(startDoc.WrappedObject, startLine, startColumn, endDoc.WrappedObject, endLine, endColumn);
		}
		
		public void Initialize(object emitter, ref ushort filename, IStream pIStream, int fFullBuild)
		{
			this.WrappedObject.Initialize(emitter, ref filename, pIStream.WrappedObject, fFullBuild);
		}
		
		public void GetDebugInfo(ref uint pIDD, uint cData, out uint pcData, System.IntPtr data)
		{
			this.WrappedObject.GetDebugInfo(ref pIDD, cData, out pcData, data);
		}
		
		public void DefineSequencePoints(ISymUnmanagedDocumentWriter document, uint spCount, ref uint offsets, ref uint lines, ref uint columns, ref uint endLines, ref uint endColumns)
		{
			this.WrappedObject.DefineSequencePoints(document.WrappedObject, spCount, ref offsets, ref lines, ref columns, ref endLines, ref endColumns);
		}
		
		public void RemapToken(uint oldToken, uint newToken)
		{
			this.WrappedObject.RemapToken(oldToken, newToken);
		}
		
		public void Initialize2(object emitter, ref ushort tempfilename, IStream pIStream, int fFullBuild, ref ushort finalfilename)
		{
			this.WrappedObject.Initialize2(emitter, ref tempfilename, pIStream.WrappedObject, fFullBuild, ref finalfilename);
		}
		
		public void DefineConstant(ref ushort name, object value, uint cSig, ref byte signature)
		{
			this.WrappedObject.DefineConstant(ref name, value, cSig, ref signature);
		}
		
		public void Abort()
		{
			this.WrappedObject.Abort();
		}
		
		public void DefineLocalVariable2(ref ushort name, uint attributes, uint sigToken, uint addrKind, uint addr1, uint addr2, uint addr3, uint startOffset, uint endOffset)
		{
			this.WrappedObject.DefineLocalVariable2(ref name, attributes, sigToken, addrKind, addr1, addr2, addr3, startOffset, endOffset);
		}
		
		public void DefineGlobalVariable2(ref ushort name, uint attributes, uint sigToken, uint addrKind, uint addr1, uint addr2, uint addr3)
		{
			this.WrappedObject.DefineGlobalVariable2(ref name, attributes, sigToken, addrKind, addr1, addr2, addr3);
		}
		
		public void DefineConstant2(ref ushort name, object value, uint sigToken)
		{
			this.WrappedObject.DefineConstant2(ref name, value, sigToken);
		}
	}
}
