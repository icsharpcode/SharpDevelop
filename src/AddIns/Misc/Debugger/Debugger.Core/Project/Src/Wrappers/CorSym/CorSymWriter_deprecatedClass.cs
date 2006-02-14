// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Wrappers.CorSym
{
	using System;
	
	
	public class CorSymWriter_deprecatedClass
	{
		
		private Debugger.Interop.CorSym.CorSymWriter_deprecatedClass wrappedObject;
		
		internal Debugger.Interop.CorSym.CorSymWriter_deprecatedClass WrappedObject
		{
			get
			{
				return this.wrappedObject;
			}
		}
		
		public CorSymWriter_deprecatedClass(Debugger.Interop.CorSym.CorSymWriter_deprecatedClass wrappedObject)
		{
			this.wrappedObject = wrappedObject;
			ResourceManager.TrackCOMObject(wrappedObject, typeof(CorSymWriter_deprecatedClass));
		}
		
		public static CorSymWriter_deprecatedClass Wrap(Debugger.Interop.CorSym.CorSymWriter_deprecatedClass objectToWrap)
		{
			return new CorSymWriter_deprecatedClass(objectToWrap);
		}
		
		~CorSymWriter_deprecatedClass()
		{
			object o = wrappedObject;
			wrappedObject = null;
			ResourceManager.ReleaseCOMObject(o, typeof(CorSymWriter_deprecatedClass));
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
		
		public static bool operator ==(CorSymWriter_deprecatedClass o1, CorSymWriter_deprecatedClass o2)
		{
			return ((object)o1 == null && (object)o2 == null) ||
			       ((object)o1 != null && (object)o2 != null && o1.WrappedObject == o2.WrappedObject);
		}
		
		public static bool operator !=(CorSymWriter_deprecatedClass o1, CorSymWriter_deprecatedClass o2)
		{
			return !(o1 == o2);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		public override bool Equals(object o)
		{
			CorSymWriter_deprecatedClass casted = o as CorSymWriter_deprecatedClass;
			return (casted != null) && (casted.WrappedObject == wrappedObject);
		}
		
		
		public void Abort()
		{
			this.WrappedObject.Abort();
		}
		
		public void Close()
		{
			this.WrappedObject.Close();
		}
		
		public void CloseMethod()
		{
			this.WrappedObject.CloseMethod();
		}
		
		public void CloseNamespace()
		{
			this.WrappedObject.CloseNamespace();
		}
		
		public void CloseScope(uint endOffset)
		{
			this.WrappedObject.CloseScope(endOffset);
		}
		
		public void DefineConstant(ref ushort name, object value, uint cSig, ref byte signature)
		{
			this.WrappedObject.DefineConstant(ref name, value, cSig, ref signature);
		}
		
		public ISymUnmanagedDocumentWriter DefineDocument(ref ushort url, ref System.Guid language, ref System.Guid languageVendor, ref System.Guid documentType)
		{
			return ISymUnmanagedDocumentWriter.Wrap(this.WrappedObject.DefineDocument(ref url, ref language, ref languageVendor, ref documentType));
		}
		
		public void DefineField(uint parent, ref ushort name, uint attributes, uint cSig, ref byte signature, uint addrKind, uint addr1, uint addr2, uint addr3)
		{
			this.WrappedObject.DefineField(parent, ref name, attributes, cSig, ref signature, addrKind, addr1, addr2, addr3);
		}
		
		public void DefineGlobalVariable(ref ushort name, uint attributes, uint cSig, ref byte signature, uint addrKind, uint addr1, uint addr2, uint addr3)
		{
			this.WrappedObject.DefineGlobalVariable(ref name, attributes, cSig, ref signature, addrKind, addr1, addr2, addr3);
		}
		
		public void DefineLocalVariable(ref ushort name, uint attributes, uint cSig, ref byte signature, uint addrKind, uint addr1, uint addr2, uint addr3, uint startOffset, uint endOffset)
		{
			this.WrappedObject.DefineLocalVariable(ref name, attributes, cSig, ref signature, addrKind, addr1, addr2, addr3, startOffset, endOffset);
		}
		
		public void DefineParameter(ref ushort name, uint attributes, uint sequence, uint addrKind, uint addr1, uint addr2, uint addr3)
		{
			this.WrappedObject.DefineParameter(ref name, attributes, sequence, addrKind, addr1, addr2, addr3);
		}
		
		public void DefineSequencePoints(ISymUnmanagedDocumentWriter document, uint spCount, ref uint offsets, ref uint lines, ref uint columns, ref uint endLines, ref uint endColumns)
		{
			this.WrappedObject.DefineSequencePoints(document.WrappedObject, spCount, ref offsets, ref lines, ref columns, ref endLines, ref endColumns);
		}
		
		public void GetDebugInfo(ref uint pIDD, uint cData, out uint pcData, System.IntPtr data)
		{
			this.WrappedObject.GetDebugInfo(ref pIDD, cData, out pcData, data);
		}
		
		public void Initialize(object emitter, ref ushort filename, IStream pIStream, int fFullBuild)
		{
			this.WrappedObject.Initialize(emitter, ref filename, pIStream.WrappedObject, fFullBuild);
		}
		
		public void Initialize2(object emitter, ref ushort tempfilename, IStream pIStream, int fFullBuild, ref ushort finalfilename)
		{
			this.WrappedObject.Initialize2(emitter, ref tempfilename, pIStream.WrappedObject, fFullBuild, ref finalfilename);
		}
		
		public void OpenMethod(uint method)
		{
			this.WrappedObject.OpenMethod(method);
		}
		
		public void OpenNamespace(ref ushort name)
		{
			this.WrappedObject.OpenNamespace(ref name);
		}
		
		public uint OpenScope(uint startOffset)
		{
			return this.WrappedObject.OpenScope(startOffset);
		}
		
		public void RemapToken(uint oldToken, uint newToken)
		{
			this.WrappedObject.RemapToken(oldToken, newToken);
		}
		
		public void SetMethodSourceRange(ISymUnmanagedDocumentWriter startDoc, uint startLine, uint startColumn, ISymUnmanagedDocumentWriter endDoc, uint endLine, uint endColumn)
		{
			this.WrappedObject.SetMethodSourceRange(startDoc.WrappedObject, startLine, startColumn, endDoc.WrappedObject, endLine, endColumn);
		}
		
		public void SetScopeRange(uint scopeID, uint startOffset, uint endOffset)
		{
			this.WrappedObject.SetScopeRange(scopeID, startOffset, endOffset);
		}
		
		public void SetSymAttribute(uint parent, ref ushort name, uint cData, ref byte data)
		{
			this.WrappedObject.SetSymAttribute(parent, ref name, cData, ref data);
		}
		
		public void SetUserEntryPoint(uint entryMethod)
		{
			this.WrappedObject.SetUserEntryPoint(entryMethod);
		}
		
		public void UsingNamespace(ref ushort fullName)
		{
			this.WrappedObject.UsingNamespace(ref fullName);
		}
	}
}
