// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Wrappers.CorDebug
{
	using System;
	
	
	public class ICorDebugILFrame
	{
		
		private Debugger.Interop.CorDebug.ICorDebugILFrame wrappedObject;
		
		internal Debugger.Interop.CorDebug.ICorDebugILFrame WrappedObject
		{
			get
			{
				return this.wrappedObject;
			}
		}
		
		public ICorDebugILFrame(Debugger.Interop.CorDebug.ICorDebugILFrame wrappedObject)
		{
			this.wrappedObject = wrappedObject;
		}
		
		public static ICorDebugILFrame Wrap(Debugger.Interop.CorDebug.ICorDebugILFrame objectToWrap)
		{
			return new ICorDebugILFrame(objectToWrap);
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
		
		public static bool operator ==(ICorDebugILFrame o1, ICorDebugILFrame o2)
		{
			return ((object)o1 == null && (object)o2 == null) ||
			       ((object)o1 != null && (object)o2 != null && o1.WrappedObject == o2.WrappedObject);
		}
		
		public static bool operator !=(ICorDebugILFrame o1, ICorDebugILFrame o2)
		{
			return !(o1 == o2);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		public override bool Equals(object o)
		{
			ICorDebugILFrame casted = o as ICorDebugILFrame;
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
		
		public void GetIP(out uint pnOffset, out CorDebugMappingResult pMappingResult)
		{
			Debugger.Interop.CorDebug.CorDebugMappingResult out_pMappingResult;
			this.WrappedObject.GetIP(out pnOffset, out out_pMappingResult);
			pMappingResult = ((CorDebugMappingResult)(out_pMappingResult));
		}
		
		public void SetIP(uint nOffset)
		{
			this.WrappedObject.SetIP(nOffset);
		}
		
		public void EnumerateLocalVariables(out ICorDebugValueEnum ppValueEnum)
		{
			Debugger.Interop.CorDebug.ICorDebugValueEnum out_ppValueEnum;
			this.WrappedObject.EnumerateLocalVariables(out out_ppValueEnum);
			ppValueEnum = ICorDebugValueEnum.Wrap(out_ppValueEnum);
		}
		
		public void GetLocalVariable(uint dwIndex, out ICorDebugValue ppValue)
		{
			Debugger.Interop.CorDebug.ICorDebugValue out_ppValue;
			this.WrappedObject.GetLocalVariable(dwIndex, out out_ppValue);
			ppValue = ICorDebugValue.Wrap(out_ppValue);
		}
		
		public void EnumerateArguments(out ICorDebugValueEnum ppValueEnum)
		{
			Debugger.Interop.CorDebug.ICorDebugValueEnum out_ppValueEnum;
			this.WrappedObject.EnumerateArguments(out out_ppValueEnum);
			ppValueEnum = ICorDebugValueEnum.Wrap(out_ppValueEnum);
		}
		
		public void GetArgument(uint dwIndex, out ICorDebugValue ppValue)
		{
			Debugger.Interop.CorDebug.ICorDebugValue out_ppValue;
			this.WrappedObject.GetArgument(dwIndex, out out_ppValue);
			ppValue = ICorDebugValue.Wrap(out_ppValue);
		}
		
		public void GetStackDepth(out uint pDepth)
		{
			this.WrappedObject.GetStackDepth(out pDepth);
		}
		
		public void GetStackValue(uint dwIndex, out ICorDebugValue ppValue)
		{
			Debugger.Interop.CorDebug.ICorDebugValue out_ppValue;
			this.WrappedObject.GetStackValue(dwIndex, out out_ppValue);
			ppValue = ICorDebugValue.Wrap(out_ppValue);
		}
		
		public void CanSetIP(uint nOffset)
		{
			this.WrappedObject.CanSetIP(nOffset);
		}
	}
}
