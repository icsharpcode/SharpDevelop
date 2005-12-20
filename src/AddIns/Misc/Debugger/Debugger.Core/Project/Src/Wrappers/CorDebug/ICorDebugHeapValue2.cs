// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Wrappers.CorDebug
{
	using System;
	
	
	public class ICorDebugHeapValue2
	{
		
		private Debugger.Interop.CorDebug.ICorDebugHeapValue2 wrappedObject;
		
		internal Debugger.Interop.CorDebug.ICorDebugHeapValue2 WrappedObject
		{
			get
			{
				return this.wrappedObject;
			}
		}
		
		public ICorDebugHeapValue2(Debugger.Interop.CorDebug.ICorDebugHeapValue2 wrappedObject)
		{
			this.wrappedObject = wrappedObject;
		}
		
		public static ICorDebugHeapValue2 Wrap(Debugger.Interop.CorDebug.ICorDebugHeapValue2 objectToWrap)
		{
			return new ICorDebugHeapValue2(objectToWrap);
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
		
		public static bool operator ==(ICorDebugHeapValue2 o1, ICorDebugHeapValue2 o2)
		{
			return ((object)o1 == null && (object)o2 == null) ||
			       ((object)o1 != null && (object)o2 != null && o1.WrappedObject == o2.WrappedObject);
		}
		
		public static bool operator !=(ICorDebugHeapValue2 o1, ICorDebugHeapValue2 o2)
		{
			return !(o1 == o2);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		public override bool Equals(object o)
		{
			ICorDebugHeapValue2 casted = o as ICorDebugHeapValue2;
			return (casted != null) && (casted.WrappedObject == wrappedObject);
		}
		
		
		public void CreateHandle(CorDebugHandleType type, out ICorDebugHandleValue ppHandle)
		{
			Debugger.Interop.CorDebug.ICorDebugHandleValue out_ppHandle;
			this.WrappedObject.CreateHandle(((Debugger.Interop.CorDebug.CorDebugHandleType)(type)), out out_ppHandle);
			ppHandle = ICorDebugHandleValue.Wrap(out_ppHandle);
		}
	}
}
