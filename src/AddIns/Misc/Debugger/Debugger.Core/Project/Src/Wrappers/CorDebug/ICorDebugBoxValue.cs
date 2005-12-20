// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Wrappers.CorDebug
{
	using System;
	
	
	public class ICorDebugBoxValue
	{
		
		private Debugger.Interop.CorDebug.ICorDebugBoxValue wrappedObject;
		
		internal Debugger.Interop.CorDebug.ICorDebugBoxValue WrappedObject
		{
			get
			{
				return this.wrappedObject;
			}
		}
		
		public ICorDebugBoxValue(Debugger.Interop.CorDebug.ICorDebugBoxValue wrappedObject)
		{
			this.wrappedObject = wrappedObject;
		}
		
		public static ICorDebugBoxValue Wrap(Debugger.Interop.CorDebug.ICorDebugBoxValue objectToWrap)
		{
			return new ICorDebugBoxValue(objectToWrap);
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
		
		public static bool operator ==(ICorDebugBoxValue o1, ICorDebugBoxValue o2)
		{
			return ((object)o1 == null && (object)o2 == null) ||
			       ((object)o1 != null && (object)o2 != null && o1.WrappedObject == o2.WrappedObject);
		}
		
		public static bool operator !=(ICorDebugBoxValue o1, ICorDebugBoxValue o2)
		{
			return !(o1 == o2);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		public override bool Equals(object o)
		{
			ICorDebugBoxValue casted = o as ICorDebugBoxValue;
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
		
		public void IsValid(out int pbValid)
		{
			this.WrappedObject.IsValid(out pbValid);
		}
		
		public void CreateRelocBreakpoint(out ICorDebugValueBreakpoint ppBreakpoint)
		{
			Debugger.Interop.CorDebug.ICorDebugValueBreakpoint out_ppBreakpoint;
			this.WrappedObject.CreateRelocBreakpoint(out out_ppBreakpoint);
			ppBreakpoint = ICorDebugValueBreakpoint.Wrap(out_ppBreakpoint);
		}
		
		public void GetObject(out ICorDebugObjectValue ppObject)
		{
			Debugger.Interop.CorDebug.ICorDebugObjectValue out_ppObject;
			this.WrappedObject.GetObject(out out_ppObject);
			ppObject = ICorDebugObjectValue.Wrap(out_ppObject);
		}
	}
}
