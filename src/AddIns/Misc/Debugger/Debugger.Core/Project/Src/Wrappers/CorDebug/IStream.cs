// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Wrappers.CorDebug
{
	using System;
	
	
	public class IStream
	{
		
		private Debugger.Interop.CorDebug.IStream wrappedObject;
		
		internal Debugger.Interop.CorDebug.IStream WrappedObject
		{
			get
			{
				return this.wrappedObject;
			}
		}
		
		public IStream(Debugger.Interop.CorDebug.IStream wrappedObject)
		{
			this.wrappedObject = wrappedObject;
		}
		
		public static IStream Wrap(Debugger.Interop.CorDebug.IStream objectToWrap)
		{
			return new IStream(objectToWrap);
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
		
		public static bool operator ==(IStream o1, IStream o2)
		{
			return ((object)o1 == null && (object)o2 == null) ||
			       ((object)o1 != null && (object)o2 != null && o1.WrappedObject == o2.WrappedObject);
		}
		
		public static bool operator !=(IStream o1, IStream o2)
		{
			return !(o1 == o2);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		public override bool Equals(object o)
		{
			IStream casted = o as IStream;
			return (casted != null) && (casted.WrappedObject == wrappedObject);
		}
		
		
		public void RemoteRead(out byte pv, uint cb, out uint pcbRead)
		{
			this.WrappedObject.RemoteRead(out pv, cb, out pcbRead);
		}
		
		public void RemoteWrite(ref byte pv, uint cb, out uint pcbWritten)
		{
			this.WrappedObject.RemoteWrite(ref pv, cb, out pcbWritten);
		}
		
		public void RemoteSeek(Debugger.Interop.CorDebug._LARGE_INTEGER dlibMove, uint dwOrigin, out Debugger.Interop.CorDebug._ULARGE_INTEGER plibNewPosition)
		{
			this.WrappedObject.RemoteSeek(dlibMove, dwOrigin, out plibNewPosition);
		}
		
		public void SetSize(Debugger.Interop.CorDebug._ULARGE_INTEGER libNewSize)
		{
			this.WrappedObject.SetSize(libNewSize);
		}
		
		public void RemoteCopyTo(IStream pstm, Debugger.Interop.CorDebug._ULARGE_INTEGER cb, out Debugger.Interop.CorDebug._ULARGE_INTEGER pcbRead, out Debugger.Interop.CorDebug._ULARGE_INTEGER pcbWritten)
		{
			this.WrappedObject.RemoteCopyTo(pstm.WrappedObject, cb, out pcbRead, out pcbWritten);
		}
		
		public void Commit(uint grfCommitFlags)
		{
			this.WrappedObject.Commit(grfCommitFlags);
		}
		
		public void Revert()
		{
			this.WrappedObject.Revert();
		}
		
		public void LockRegion(Debugger.Interop.CorDebug._ULARGE_INTEGER libOffset, Debugger.Interop.CorDebug._ULARGE_INTEGER cb, uint dwLockType)
		{
			this.WrappedObject.LockRegion(libOffset, cb, dwLockType);
		}
		
		public void UnlockRegion(Debugger.Interop.CorDebug._ULARGE_INTEGER libOffset, Debugger.Interop.CorDebug._ULARGE_INTEGER cb, uint dwLockType)
		{
			this.WrappedObject.UnlockRegion(libOffset, cb, dwLockType);
		}
		
		public void Stat(out Debugger.Interop.CorDebug.tagSTATSTG pstatstg, uint grfStatFlag)
		{
			this.WrappedObject.Stat(out pstatstg, grfStatFlag);
		}
		
		public void Clone(out IStream ppstm)
		{
			Debugger.Interop.CorDebug.IStream out_ppstm;
			this.WrappedObject.Clone(out out_ppstm);
			ppstm = IStream.Wrap(out_ppstm);
		}
	}
}
