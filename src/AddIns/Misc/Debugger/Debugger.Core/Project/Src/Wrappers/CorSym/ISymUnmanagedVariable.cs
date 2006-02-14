// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Wrappers.CorSym
{
	using System;
	
	
	public class ISymUnmanagedVariable
	{
		
		private Debugger.Interop.CorSym.ISymUnmanagedVariable wrappedObject;
		
		internal Debugger.Interop.CorSym.ISymUnmanagedVariable WrappedObject
		{
			get
			{
				return this.wrappedObject;
			}
		}
		
		public ISymUnmanagedVariable(Debugger.Interop.CorSym.ISymUnmanagedVariable wrappedObject)
		{
			this.wrappedObject = wrappedObject;
			ResourceManager.TrackCOMObject(wrappedObject, typeof(ISymUnmanagedVariable));
		}
		
		public static ISymUnmanagedVariable Wrap(Debugger.Interop.CorSym.ISymUnmanagedVariable objectToWrap)
		{
			return new ISymUnmanagedVariable(objectToWrap);
		}
		
		~ISymUnmanagedVariable()
		{
			object o = wrappedObject;
			wrappedObject = null;
			ResourceManager.ReleaseCOMObject(o, typeof(ISymUnmanagedVariable));
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
		
		public static bool operator ==(ISymUnmanagedVariable o1, ISymUnmanagedVariable o2)
		{
			return ((object)o1 == null && (object)o2 == null) ||
			       ((object)o1 != null && (object)o2 != null && o1.WrappedObject == o2.WrappedObject);
		}
		
		public static bool operator !=(ISymUnmanagedVariable o1, ISymUnmanagedVariable o2)
		{
			return !(o1 == o2);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		public override bool Equals(object o)
		{
			ISymUnmanagedVariable casted = o as ISymUnmanagedVariable;
			return (casted != null) && (casted.WrappedObject == wrappedObject);
		}
		
		
		public void GetName(uint cchName, out uint pcchName, System.IntPtr szName)
		{
			this.WrappedObject.GetName(cchName, out pcchName, szName);
		}
		
		public uint Attributes
		{
			get
			{
				return this.WrappedObject.GetAttributes();
			}
		}
		
		public void GetSignature(uint cSig, out uint pcSig, System.IntPtr sig)
		{
			this.WrappedObject.GetSignature(cSig, out pcSig, sig);
		}
		
		public uint AddressKind
		{
			get
			{
				return this.WrappedObject.GetAddressKind();
			}
		}
		
		public uint AddressField1
		{
			get
			{
				return this.WrappedObject.GetAddressField1();
			}
		}
		
		public uint AddressField2
		{
			get
			{
				return this.WrappedObject.GetAddressField2();
			}
		}
		
		public uint AddressField3
		{
			get
			{
				return this.WrappedObject.GetAddressField3();
			}
		}
		
		public uint StartOffset
		{
			get
			{
				return this.WrappedObject.GetStartOffset();
			}
		}
		
		public uint EndOffset
		{
			get
			{
				return this.WrappedObject.GetEndOffset();
			}
		}
	}
}
