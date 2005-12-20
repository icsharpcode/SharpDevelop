// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Wrappers.CorDebug
{
	using System;
	
	
	public class ICorDebugModuleBreakpoint
	{
		
		private Debugger.Interop.CorDebug.ICorDebugModuleBreakpoint wrappedObject;
		
		internal Debugger.Interop.CorDebug.ICorDebugModuleBreakpoint WrappedObject
		{
			get
			{
				return this.wrappedObject;
			}
		}
		
		public ICorDebugModuleBreakpoint(Debugger.Interop.CorDebug.ICorDebugModuleBreakpoint wrappedObject)
		{
			this.wrappedObject = wrappedObject;
		}
		
		public static ICorDebugModuleBreakpoint Wrap(Debugger.Interop.CorDebug.ICorDebugModuleBreakpoint objectToWrap)
		{
			return new ICorDebugModuleBreakpoint(objectToWrap);
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
		
		public static bool operator ==(ICorDebugModuleBreakpoint o1, ICorDebugModuleBreakpoint o2)
		{
			return ((object)o1 == null && (object)o2 == null) ||
			       ((object)o1 != null && (object)o2 != null && o1.WrappedObject == o2.WrappedObject);
		}
		
		public static bool operator !=(ICorDebugModuleBreakpoint o1, ICorDebugModuleBreakpoint o2)
		{
			return !(o1 == o2);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		public override bool Equals(object o)
		{
			ICorDebugModuleBreakpoint casted = o as ICorDebugModuleBreakpoint;
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
		
		public void GetModule(out ICorDebugModule ppModule)
		{
			Debugger.Interop.CorDebug.ICorDebugModule out_ppModule;
			this.WrappedObject.GetModule(out out_ppModule);
			ppModule = ICorDebugModule.Wrap(out_ppModule);
		}
	}
}
