// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Wrappers.CorDebug
{
	using System;
	
	
	public class ICorDebugFunctionBreakpoint
	{
		
		private Debugger.Interop.CorDebug.ICorDebugFunctionBreakpoint wrappedObject;
		
		internal Debugger.Interop.CorDebug.ICorDebugFunctionBreakpoint WrappedObject
		{
			get
			{
				return this.wrappedObject;
			}
		}
		
		public ICorDebugFunctionBreakpoint(Debugger.Interop.CorDebug.ICorDebugFunctionBreakpoint wrappedObject)
		{
			this.wrappedObject = wrappedObject;
		}
		
		public static ICorDebugFunctionBreakpoint Wrap(Debugger.Interop.CorDebug.ICorDebugFunctionBreakpoint objectToWrap)
		{
			return new ICorDebugFunctionBreakpoint(objectToWrap);
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
		
		public static bool operator ==(ICorDebugFunctionBreakpoint o1, ICorDebugFunctionBreakpoint o2)
		{
			return ((object)o1 == null && (object)o2 == null) ||
			       ((object)o1 != null && (object)o2 != null && o1.WrappedObject == o2.WrappedObject);
		}
		
		public static bool operator !=(ICorDebugFunctionBreakpoint o1, ICorDebugFunctionBreakpoint o2)
		{
			return !(o1 == o2);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		public override bool Equals(object o)
		{
			ICorDebugFunctionBreakpoint casted = o as ICorDebugFunctionBreakpoint;
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
		
		public void GetFunction(out ICorDebugFunction ppFunction)
		{
			Debugger.Interop.CorDebug.ICorDebugFunction out_ppFunction;
			this.WrappedObject.GetFunction(out out_ppFunction);
			ppFunction = ICorDebugFunction.Wrap(out_ppFunction);
		}
		
		public void GetOffset(out uint pnOffset)
		{
			this.WrappedObject.GetOffset(out pnOffset);
		}
	}
}
