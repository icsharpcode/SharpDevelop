// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Wrappers.CorDebug
{
	using System;
	
	
	public class ICorDebugRegisterSet
	{
		
		private Debugger.Interop.CorDebug.ICorDebugRegisterSet wrappedObject;
		
		internal Debugger.Interop.CorDebug.ICorDebugRegisterSet WrappedObject
		{
			get
			{
				return this.wrappedObject;
			}
		}
		
		public ICorDebugRegisterSet(Debugger.Interop.CorDebug.ICorDebugRegisterSet wrappedObject)
		{
			this.wrappedObject = wrappedObject;
		}
		
		public static ICorDebugRegisterSet Wrap(Debugger.Interop.CorDebug.ICorDebugRegisterSet objectToWrap)
		{
			return new ICorDebugRegisterSet(objectToWrap);
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
		
		public static bool operator ==(ICorDebugRegisterSet o1, ICorDebugRegisterSet o2)
		{
			return ((object)o1 == null && (object)o2 == null) ||
			       ((object)o1 != null && (object)o2 != null && o1.WrappedObject == o2.WrappedObject);
		}
		
		public static bool operator !=(ICorDebugRegisterSet o1, ICorDebugRegisterSet o2)
		{
			return !(o1 == o2);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		public override bool Equals(object o)
		{
			ICorDebugRegisterSet casted = o as ICorDebugRegisterSet;
			return (casted != null) && (casted.WrappedObject == wrappedObject);
		}
		
		
		public void GetRegistersAvailable(out ulong pAvailable)
		{
			this.WrappedObject.GetRegistersAvailable(out pAvailable);
		}
		
		public void GetRegisters(ulong mask, uint regCount, System.IntPtr regBuffer)
		{
			this.WrappedObject.GetRegisters(mask, regCount, regBuffer);
		}
		
		public void SetRegisters(ulong mask, uint regCount, ref ulong regBuffer)
		{
			this.WrappedObject.SetRegisters(mask, regCount, ref regBuffer);
		}
		
		public void GetThreadContext(uint contextSize, System.IntPtr context)
		{
			this.WrappedObject.GetThreadContext(contextSize, context);
		}
		
		public void SetThreadContext(uint contextSize, System.IntPtr context)
		{
			this.WrappedObject.SetThreadContext(contextSize, context);
		}
	}
}
