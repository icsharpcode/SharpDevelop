// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Wrappers.CorDebug
{
	using System;
	
	
	public class ICorDebugBreakpoint
	{
		
		private Debugger.Interop.CorDebug.ICorDebugBreakpoint wrappedObject;
		
		internal Debugger.Interop.CorDebug.ICorDebugBreakpoint WrappedObject
		{
			get
			{
				return this.wrappedObject;
			}
		}
		
		public ICorDebugBreakpoint(Debugger.Interop.CorDebug.ICorDebugBreakpoint wrappedObject)
		{
			this.wrappedObject = wrappedObject;
		}
		
		public static ICorDebugBreakpoint Wrap(Debugger.Interop.CorDebug.ICorDebugBreakpoint objectToWrap)
		{
			return new ICorDebugBreakpoint(objectToWrap);
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
		
		public static bool operator ==(ICorDebugBreakpoint o1, ICorDebugBreakpoint o2)
		{
			return ((object)o1 == null && (object)o2 == null) ||
			       ((object)o1 != null && (object)o2 != null && o1.WrappedObject == o2.WrappedObject);
		}
		
		public static bool operator !=(ICorDebugBreakpoint o1, ICorDebugBreakpoint o2)
		{
			return !(o1 == o2);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		public override bool Equals(object o)
		{
			ICorDebugBreakpoint casted = o as ICorDebugBreakpoint;
			return (casted != null) && (casted.WrappedObject == wrappedObject);
		}
		
		
		public void Activate(int bActive)
		{
			this.WrappedObject.Activate(bActive);
		}
		
		public void IsActive(out int pbActive)
		{
			this.WrappedObject.IsActive(out pbActive);
		}
	}
}
