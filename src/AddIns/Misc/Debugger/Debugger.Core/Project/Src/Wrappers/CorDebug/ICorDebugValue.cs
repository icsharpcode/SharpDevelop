// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Wrappers.CorDebug
{
	using System;
	
	
	public class ICorDebugValue
	{
		
		private Debugger.Interop.CorDebug.ICorDebugValue wrappedObject;
		
		internal Debugger.Interop.CorDebug.ICorDebugValue WrappedObject
		{
			get
			{
				return this.wrappedObject;
			}
		}
		
		public ICorDebugValue(Debugger.Interop.CorDebug.ICorDebugValue wrappedObject)
		{
			this.wrappedObject = wrappedObject;
		}
		
		public static ICorDebugValue Wrap(Debugger.Interop.CorDebug.ICorDebugValue objectToWrap)
		{
			return new ICorDebugValue(objectToWrap);
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
		
		public static bool operator ==(ICorDebugValue o1, ICorDebugValue o2)
		{
			return ((object)o1 == null && (object)o2 == null) ||
			       ((object)o1 != null && (object)o2 != null && o1.WrappedObject == o2.WrappedObject);
		}
		
		public static bool operator !=(ICorDebugValue o1, ICorDebugValue o2)
		{
			return !(o1 == o2);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		public override bool Equals(object o)
		{
			ICorDebugValue casted = o as ICorDebugValue;
			return (casted != null) && (casted.WrappedObject == wrappedObject);
		}
		
		
		public void GetType(out uint pType)
		{
			this.WrappedObject.GetType(out pType);
		}
		
		public void GetSize(out uint pSize)
		{
			this.WrappedObject.GetSize(out pSize);
		}
		
		public void GetAddress(out ulong pAddress)
		{
			this.WrappedObject.GetAddress(out pAddress);
		}
		
		public void CreateBreakpoint(out ICorDebugValueBreakpoint ppBreakpoint)
		{
			Debugger.Interop.CorDebug.ICorDebugValueBreakpoint out_ppBreakpoint;
			this.WrappedObject.CreateBreakpoint(out out_ppBreakpoint);
			ppBreakpoint = ICorDebugValueBreakpoint.Wrap(out_ppBreakpoint);
		}
	}
}
