// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Wrappers.CorDebug
{
	using System;
	
	
	public class ICorDebugObjectEnum
	{
		
		private Debugger.Interop.CorDebug.ICorDebugObjectEnum wrappedObject;
		
		internal Debugger.Interop.CorDebug.ICorDebugObjectEnum WrappedObject
		{
			get
			{
				return this.wrappedObject;
			}
		}
		
		public ICorDebugObjectEnum(Debugger.Interop.CorDebug.ICorDebugObjectEnum wrappedObject)
		{
			this.wrappedObject = wrappedObject;
		}
		
		public static ICorDebugObjectEnum Wrap(Debugger.Interop.CorDebug.ICorDebugObjectEnum objectToWrap)
		{
			return new ICorDebugObjectEnum(objectToWrap);
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
		
		public static bool operator ==(ICorDebugObjectEnum o1, ICorDebugObjectEnum o2)
		{
			return ((object)o1 == null && (object)o2 == null) ||
			       ((object)o1 != null && (object)o2 != null && o1.WrappedObject == o2.WrappedObject);
		}
		
		public static bool operator !=(ICorDebugObjectEnum o1, ICorDebugObjectEnum o2)
		{
			return !(o1 == o2);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		public override bool Equals(object o)
		{
			ICorDebugObjectEnum casted = o as ICorDebugObjectEnum;
			return (casted != null) && (casted.WrappedObject == wrappedObject);
		}
		
		
		public void Skip(uint celt)
		{
			this.WrappedObject.Skip(celt);
		}
		
		public void Reset()
		{
			this.WrappedObject.Reset();
		}
		
		public void Clone(out ICorDebugEnum ppEnum)
		{
			Debugger.Interop.CorDebug.ICorDebugEnum out_ppEnum;
			this.WrappedObject.Clone(out out_ppEnum);
			ppEnum = ICorDebugEnum.Wrap(out_ppEnum);
		}
		
		public void GetCount(out uint pcelt)
		{
			this.WrappedObject.GetCount(out pcelt);
		}
		
		public void Next(uint celt, System.IntPtr objects, out uint pceltFetched)
		{
			this.WrappedObject.Next(celt, objects, out pceltFetched);
		}
	}
}
