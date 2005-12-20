// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Wrappers.CorDebug
{
	using System;
	
	
	public class ICorDebugGenericValue
	{
		
		private Debugger.Interop.CorDebug.ICorDebugGenericValue wrappedObject;
		
		internal Debugger.Interop.CorDebug.ICorDebugGenericValue WrappedObject
		{
			get
			{
				return this.wrappedObject;
			}
		}
		
		public ICorDebugGenericValue(Debugger.Interop.CorDebug.ICorDebugGenericValue wrappedObject)
		{
			this.wrappedObject = wrappedObject;
		}
		
		public static ICorDebugGenericValue Wrap(Debugger.Interop.CorDebug.ICorDebugGenericValue objectToWrap)
		{
			return new ICorDebugGenericValue(objectToWrap);
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
		
		public static bool operator ==(ICorDebugGenericValue o1, ICorDebugGenericValue o2)
		{
			return ((object)o1 == null && (object)o2 == null) ||
			       ((object)o1 != null && (object)o2 != null && o1.WrappedObject == o2.WrappedObject);
		}
		
		public static bool operator !=(ICorDebugGenericValue o1, ICorDebugGenericValue o2)
		{
			return !(o1 == o2);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		public override bool Equals(object o)
		{
			ICorDebugGenericValue casted = o as ICorDebugGenericValue;
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
		
		public void GetValue(System.IntPtr pTo)
		{
			this.WrappedObject.GetValue(pTo);
		}
		
		public void SetValue(System.IntPtr pFrom)
		{
			this.WrappedObject.SetValue(pFrom);
		}
	}
}
