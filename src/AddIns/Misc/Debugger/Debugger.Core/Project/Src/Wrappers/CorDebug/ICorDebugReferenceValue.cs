// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Wrappers.CorDebug
{
	using System;
	
	
	public class ICorDebugReferenceValue
	{
		
		private Debugger.Interop.CorDebug.ICorDebugReferenceValue wrappedObject;
		
		internal Debugger.Interop.CorDebug.ICorDebugReferenceValue WrappedObject
		{
			get
			{
				return this.wrappedObject;
			}
		}
		
		public ICorDebugReferenceValue(Debugger.Interop.CorDebug.ICorDebugReferenceValue wrappedObject)
		{
			this.wrappedObject = wrappedObject;
		}
		
		public static ICorDebugReferenceValue Wrap(Debugger.Interop.CorDebug.ICorDebugReferenceValue objectToWrap)
		{
			return new ICorDebugReferenceValue(objectToWrap);
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
		
		public static bool operator ==(ICorDebugReferenceValue o1, ICorDebugReferenceValue o2)
		{
			return ((object)o1 == null && (object)o2 == null) ||
			       ((object)o1 != null && (object)o2 != null && o1.WrappedObject == o2.WrappedObject);
		}
		
		public static bool operator !=(ICorDebugReferenceValue o1, ICorDebugReferenceValue o2)
		{
			return !(o1 == o2);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		public override bool Equals(object o)
		{
			ICorDebugReferenceValue casted = o as ICorDebugReferenceValue;
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
		
		public void IsNull(out int pbNull)
		{
			this.WrappedObject.IsNull(out pbNull);
		}
		
		public void GetValue(out ulong pValue)
		{
			this.WrappedObject.GetValue(out pValue);
		}
		
		public void SetValue(ulong value)
		{
			this.WrappedObject.SetValue(value);
		}
		
		public void Dereference(out ICorDebugValue ppValue)
		{
			Debugger.Interop.CorDebug.ICorDebugValue out_ppValue;
			this.WrappedObject.Dereference(out out_ppValue);
			ppValue = ICorDebugValue.Wrap(out_ppValue);
		}
		
		public void DereferenceStrong(out ICorDebugValue ppValue)
		{
			Debugger.Interop.CorDebug.ICorDebugValue out_ppValue;
			this.WrappedObject.DereferenceStrong(out out_ppValue);
			ppValue = ICorDebugValue.Wrap(out_ppValue);
		}
	}
}
