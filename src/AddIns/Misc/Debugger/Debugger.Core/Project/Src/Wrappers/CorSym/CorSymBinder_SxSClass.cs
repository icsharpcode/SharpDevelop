// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Wrappers.CorSym
{
	using System;
	
	
	public class CorSymBinder_SxSClass
	{
		
		private Debugger.Interop.CorSym.CorSymBinder_SxSClass wrappedObject;
		
		internal Debugger.Interop.CorSym.CorSymBinder_SxSClass WrappedObject
		{
			get
			{
				return this.wrappedObject;
			}
		}
		
		public CorSymBinder_SxSClass(Debugger.Interop.CorSym.CorSymBinder_SxSClass wrappedObject)
		{
			this.wrappedObject = wrappedObject;
			ResourceManager.TrackCOMObject(wrappedObject, typeof(CorSymBinder_SxSClass));
		}
		
		public static CorSymBinder_SxSClass Wrap(Debugger.Interop.CorSym.CorSymBinder_SxSClass objectToWrap)
		{
			return new CorSymBinder_SxSClass(objectToWrap);
		}
		
		~CorSymBinder_SxSClass()
		{
			object o = wrappedObject;
			wrappedObject = null;
			ResourceManager.ReleaseCOMObject(o, typeof(CorSymBinder_SxSClass));
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
		
		public static bool operator ==(CorSymBinder_SxSClass o1, CorSymBinder_SxSClass o2)
		{
			return ((object)o1 == null && (object)o2 == null) ||
			       ((object)o1 != null && (object)o2 != null && o1.WrappedObject == o2.WrappedObject);
		}
		
		public static bool operator !=(CorSymBinder_SxSClass o1, CorSymBinder_SxSClass o2)
		{
			return !(o1 == o2);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		public override bool Equals(object o)
		{
			CorSymBinder_SxSClass casted = o as CorSymBinder_SxSClass;
			return (casted != null) && (casted.WrappedObject == wrappedObject);
		}
		
		
		public ISymUnmanagedReader GetReaderForFile(object importer, ref ushort filename, ref ushort searchPath)
		{
			return ISymUnmanagedReader.Wrap(this.WrappedObject.GetReaderForFile(importer, ref filename, ref searchPath));
		}
		
		public ISymUnmanagedReader GetReaderFromStream(object importer, IStream pstream)
		{
			return ISymUnmanagedReader.Wrap(this.WrappedObject.GetReaderFromStream(importer, pstream.WrappedObject));
		}
	}
}
