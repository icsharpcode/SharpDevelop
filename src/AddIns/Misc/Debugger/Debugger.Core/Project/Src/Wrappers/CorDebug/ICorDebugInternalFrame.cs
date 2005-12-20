// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Wrappers.CorDebug
{
	using System;
	
	
	public class ICorDebugInternalFrame
	{
		
		private Debugger.Interop.CorDebug.ICorDebugInternalFrame wrappedObject;
		
		internal Debugger.Interop.CorDebug.ICorDebugInternalFrame WrappedObject
		{
			get
			{
				return this.wrappedObject;
			}
		}
		
		public ICorDebugInternalFrame(Debugger.Interop.CorDebug.ICorDebugInternalFrame wrappedObject)
		{
			this.wrappedObject = wrappedObject;
		}
		
		public static ICorDebugInternalFrame Wrap(Debugger.Interop.CorDebug.ICorDebugInternalFrame objectToWrap)
		{
			return new ICorDebugInternalFrame(objectToWrap);
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
		
		public static bool operator ==(ICorDebugInternalFrame o1, ICorDebugInternalFrame o2)
		{
			return ((object)o1 == null && (object)o2 == null) ||
			       ((object)o1 != null && (object)o2 != null && o1.WrappedObject == o2.WrappedObject);
		}
		
		public static bool operator !=(ICorDebugInternalFrame o1, ICorDebugInternalFrame o2)
		{
			return !(o1 == o2);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		public override bool Equals(object o)
		{
			ICorDebugInternalFrame casted = o as ICorDebugInternalFrame;
			return (casted != null) && (casted.WrappedObject == wrappedObject);
		}
		
		
		public void GetChain(out ICorDebugChain ppChain)
		{
			Debugger.Interop.CorDebug.ICorDebugChain out_ppChain;
			this.WrappedObject.GetChain(out out_ppChain);
			ppChain = ICorDebugChain.Wrap(out_ppChain);
		}
		
		public void GetCode(out ICorDebugCode ppCode)
		{
			Debugger.Interop.CorDebug.ICorDebugCode out_ppCode;
			this.WrappedObject.GetCode(out out_ppCode);
			ppCode = ICorDebugCode.Wrap(out_ppCode);
		}
		
		public void GetFunction(out ICorDebugFunction ppFunction)
		{
			Debugger.Interop.CorDebug.ICorDebugFunction out_ppFunction;
			this.WrappedObject.GetFunction(out out_ppFunction);
			ppFunction = ICorDebugFunction.Wrap(out_ppFunction);
		}
		
		public void GetFunctionToken(out uint pToken)
		{
			this.WrappedObject.GetFunctionToken(out pToken);
		}
		
		public void GetStackRange(out ulong pStart, out ulong pEnd)
		{
			this.WrappedObject.GetStackRange(out pStart, out pEnd);
		}
		
		public void GetCaller(out ICorDebugFrame ppFrame)
		{
			Debugger.Interop.CorDebug.ICorDebugFrame out_ppFrame;
			this.WrappedObject.GetCaller(out out_ppFrame);
			ppFrame = ICorDebugFrame.Wrap(out_ppFrame);
		}
		
		public void GetCallee(out ICorDebugFrame ppFrame)
		{
			Debugger.Interop.CorDebug.ICorDebugFrame out_ppFrame;
			this.WrappedObject.GetCallee(out out_ppFrame);
			ppFrame = ICorDebugFrame.Wrap(out_ppFrame);
		}
		
		public void CreateStepper(out ICorDebugStepper ppStepper)
		{
			Debugger.Interop.CorDebug.ICorDebugStepper out_ppStepper;
			this.WrappedObject.CreateStepper(out out_ppStepper);
			ppStepper = ICorDebugStepper.Wrap(out_ppStepper);
		}
		
		public void GetFrameType(out CorDebugInternalFrameType pType)
		{
			Debugger.Interop.CorDebug.CorDebugInternalFrameType out_pType;
			this.WrappedObject.GetFrameType(out out_pType);
			pType = ((CorDebugInternalFrameType)(out_pType));
		}
	}
}
