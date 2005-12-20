// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Wrappers.CorDebug
{
	using System;
	
	
	public class ICorDebugObjectValue2
	{
		
		private Debugger.Interop.CorDebug.ICorDebugObjectValue2 wrappedObject;
		
		internal Debugger.Interop.CorDebug.ICorDebugObjectValue2 WrappedObject
		{
			get
			{
				return this.wrappedObject;
			}
		}
		
		public ICorDebugObjectValue2(Debugger.Interop.CorDebug.ICorDebugObjectValue2 wrappedObject)
		{
			this.wrappedObject = wrappedObject;
		}
		
		public static ICorDebugObjectValue2 Wrap(Debugger.Interop.CorDebug.ICorDebugObjectValue2 objectToWrap)
		{
			return new ICorDebugObjectValue2(objectToWrap);
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
		
		public static bool operator ==(ICorDebugObjectValue2 o1, ICorDebugObjectValue2 o2)
		{
			return ((object)o1 == null && (object)o2 == null) ||
			       ((object)o1 != null && (object)o2 != null && o1.WrappedObject == o2.WrappedObject);
		}
		
		public static bool operator !=(ICorDebugObjectValue2 o1, ICorDebugObjectValue2 o2)
		{
			return !(o1 == o2);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		public override bool Equals(object o)
		{
			ICorDebugObjectValue2 casted = o as ICorDebugObjectValue2;
			return (casted != null) && (casted.WrappedObject == wrappedObject);
		}
		
		
		public void GetVirtualMethodAndType(uint memberRef, out ICorDebugFunction ppFunction, out ICorDebugType ppType)
		{
			Debugger.Interop.CorDebug.ICorDebugFunction out_ppFunction;
			Debugger.Interop.CorDebug.ICorDebugType out_ppType;
			this.WrappedObject.GetVirtualMethodAndType(memberRef, out out_ppFunction, out out_ppType);
			ppFunction = ICorDebugFunction.Wrap(out_ppFunction);
			ppType = ICorDebugType.Wrap(out_ppType);
		}
	}
}
