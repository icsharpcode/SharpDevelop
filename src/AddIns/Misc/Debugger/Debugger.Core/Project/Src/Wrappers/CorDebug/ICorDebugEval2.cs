// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Wrappers.CorDebug
{
	using System;
	
	
	public class ICorDebugEval2
	{
		
		private Debugger.Interop.CorDebug.ICorDebugEval2 wrappedObject;
		
		internal Debugger.Interop.CorDebug.ICorDebugEval2 WrappedObject
		{
			get
			{
				return this.wrappedObject;
			}
		}
		
		public ICorDebugEval2(Debugger.Interop.CorDebug.ICorDebugEval2 wrappedObject)
		{
			this.wrappedObject = wrappedObject;
		}
		
		public static ICorDebugEval2 Wrap(Debugger.Interop.CorDebug.ICorDebugEval2 objectToWrap)
		{
			return new ICorDebugEval2(objectToWrap);
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
		
		public static bool operator ==(ICorDebugEval2 o1, ICorDebugEval2 o2)
		{
			return ((object)o1 == null && (object)o2 == null) ||
			       ((object)o1 != null && (object)o2 != null && o1.WrappedObject == o2.WrappedObject);
		}
		
		public static bool operator !=(ICorDebugEval2 o1, ICorDebugEval2 o2)
		{
			return !(o1 == o2);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		public override bool Equals(object o)
		{
			ICorDebugEval2 casted = o as ICorDebugEval2;
			return (casted != null) && (casted.WrappedObject == wrappedObject);
		}
		
		
		public void CallParameterizedFunction(ICorDebugFunction pFunction, uint nTypeArgs, ref ICorDebugType ppTypeArgs, uint nArgs, ref ICorDebugValue ppArgs)
		{
			Debugger.Interop.CorDebug.ICorDebugType ref_ppTypeArgs = ppTypeArgs.WrappedObject;
			Debugger.Interop.CorDebug.ICorDebugValue ref_ppArgs = ppArgs.WrappedObject;
			this.WrappedObject.CallParameterizedFunction(pFunction.WrappedObject, nTypeArgs, ref ref_ppTypeArgs, nArgs, ref ref_ppArgs);
			ppTypeArgs = ICorDebugType.Wrap(ref_ppTypeArgs);
			ppArgs = ICorDebugValue.Wrap(ref_ppArgs);
		}
		
		public void CreateValueForType(ICorDebugType pType, out ICorDebugValue ppValue)
		{
			Debugger.Interop.CorDebug.ICorDebugValue out_ppValue;
			this.WrappedObject.CreateValueForType(pType.WrappedObject, out out_ppValue);
			ppValue = ICorDebugValue.Wrap(out_ppValue);
		}
		
		public void NewParameterizedObject(ICorDebugFunction pConstructor, uint nTypeArgs, ref ICorDebugType ppTypeArgs, uint nArgs, ref ICorDebugValue ppArgs)
		{
			Debugger.Interop.CorDebug.ICorDebugType ref_ppTypeArgs = ppTypeArgs.WrappedObject;
			Debugger.Interop.CorDebug.ICorDebugValue ref_ppArgs = ppArgs.WrappedObject;
			this.WrappedObject.NewParameterizedObject(pConstructor.WrappedObject, nTypeArgs, ref ref_ppTypeArgs, nArgs, ref ref_ppArgs);
			ppTypeArgs = ICorDebugType.Wrap(ref_ppTypeArgs);
			ppArgs = ICorDebugValue.Wrap(ref_ppArgs);
		}
		
		public void NewParameterizedObjectNoConstructor(ICorDebugClass pClass, uint nTypeArgs, ref ICorDebugType ppTypeArgs)
		{
			Debugger.Interop.CorDebug.ICorDebugType ref_ppTypeArgs = ppTypeArgs.WrappedObject;
			this.WrappedObject.NewParameterizedObjectNoConstructor(pClass.WrappedObject, nTypeArgs, ref ref_ppTypeArgs);
			ppTypeArgs = ICorDebugType.Wrap(ref_ppTypeArgs);
		}
		
		public void NewParameterizedArray(ICorDebugType pElementType, uint rank, ref uint dims, ref uint lowBounds)
		{
			this.WrappedObject.NewParameterizedArray(pElementType.WrappedObject, rank, ref dims, ref lowBounds);
		}
		
		public void NewStringWithLength(string @string, uint uiLength)
		{
			this.WrappedObject.NewStringWithLength(@string, uiLength);
		}
		
		public void RudeAbort()
		{
			this.WrappedObject.RudeAbort();
		}
	}
}
