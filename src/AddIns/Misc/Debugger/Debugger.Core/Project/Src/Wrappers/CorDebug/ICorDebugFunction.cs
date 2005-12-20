// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Wrappers.CorDebug
{
	using System;
	
	
	public class ICorDebugFunction
	{
		
		private Debugger.Interop.CorDebug.ICorDebugFunction wrappedObject;
		
		internal Debugger.Interop.CorDebug.ICorDebugFunction WrappedObject
		{
			get
			{
				return this.wrappedObject;
			}
		}
		
		public ICorDebugFunction(Debugger.Interop.CorDebug.ICorDebugFunction wrappedObject)
		{
			this.wrappedObject = wrappedObject;
		}
		
		public static ICorDebugFunction Wrap(Debugger.Interop.CorDebug.ICorDebugFunction objectToWrap)
		{
			return new ICorDebugFunction(objectToWrap);
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
		
		public static bool operator ==(ICorDebugFunction o1, ICorDebugFunction o2)
		{
			return ((object)o1 == null && (object)o2 == null) ||
			       ((object)o1 != null && (object)o2 != null && o1.WrappedObject == o2.WrappedObject);
		}
		
		public static bool operator !=(ICorDebugFunction o1, ICorDebugFunction o2)
		{
			return !(o1 == o2);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		public override bool Equals(object o)
		{
			ICorDebugFunction casted = o as ICorDebugFunction;
			return (casted != null) && (casted.WrappedObject == wrappedObject);
		}
		
		
		public void GetModule(out ICorDebugModule ppModule)
		{
			Debugger.Interop.CorDebug.ICorDebugModule out_ppModule;
			this.WrappedObject.GetModule(out out_ppModule);
			ppModule = ICorDebugModule.Wrap(out_ppModule);
		}
		
		public void GetClass(out ICorDebugClass ppClass)
		{
			Debugger.Interop.CorDebug.ICorDebugClass out_ppClass;
			this.WrappedObject.GetClass(out out_ppClass);
			ppClass = ICorDebugClass.Wrap(out_ppClass);
		}
		
		public void GetToken(out uint pMethodDef)
		{
			this.WrappedObject.GetToken(out pMethodDef);
		}
		
		public void GetILCode(out ICorDebugCode ppCode)
		{
			Debugger.Interop.CorDebug.ICorDebugCode out_ppCode;
			this.WrappedObject.GetILCode(out out_ppCode);
			ppCode = ICorDebugCode.Wrap(out_ppCode);
		}
		
		public void GetNativeCode(out ICorDebugCode ppCode)
		{
			Debugger.Interop.CorDebug.ICorDebugCode out_ppCode;
			this.WrappedObject.GetNativeCode(out out_ppCode);
			ppCode = ICorDebugCode.Wrap(out_ppCode);
		}
		
		public void CreateBreakpoint(out ICorDebugFunctionBreakpoint ppBreakpoint)
		{
			Debugger.Interop.CorDebug.ICorDebugFunctionBreakpoint out_ppBreakpoint;
			this.WrappedObject.CreateBreakpoint(out out_ppBreakpoint);
			ppBreakpoint = ICorDebugFunctionBreakpoint.Wrap(out_ppBreakpoint);
		}
		
		public void GetLocalVarSigToken(out uint pmdSig)
		{
			this.WrappedObject.GetLocalVarSigToken(out pmdSig);
		}
		
		public void GetCurrentVersionNumber(out uint pnCurrentVersion)
		{
			this.WrappedObject.GetCurrentVersionNumber(out pnCurrentVersion);
		}
	}
}
