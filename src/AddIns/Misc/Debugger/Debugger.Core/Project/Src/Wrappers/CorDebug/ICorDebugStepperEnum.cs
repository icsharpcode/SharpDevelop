// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Wrappers.CorDebug
{
	using System;
	
	
	public class ICorDebugStepperEnum
	{
		
		private Debugger.Interop.CorDebug.ICorDebugStepperEnum wrappedObject;
		
		internal Debugger.Interop.CorDebug.ICorDebugStepperEnum WrappedObject
		{
			get
			{
				return this.wrappedObject;
			}
		}
		
		public ICorDebugStepperEnum(Debugger.Interop.CorDebug.ICorDebugStepperEnum wrappedObject)
		{
			this.wrappedObject = wrappedObject;
		}
		
		public static ICorDebugStepperEnum Wrap(Debugger.Interop.CorDebug.ICorDebugStepperEnum objectToWrap)
		{
			return new ICorDebugStepperEnum(objectToWrap);
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
		
		public static bool operator ==(ICorDebugStepperEnum o1, ICorDebugStepperEnum o2)
		{
			return ((object)o1 == null && (object)o2 == null) ||
			       ((object)o1 != null && (object)o2 != null && o1.WrappedObject == o2.WrappedObject);
		}
		
		public static bool operator !=(ICorDebugStepperEnum o1, ICorDebugStepperEnum o2)
		{
			return !(o1 == o2);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		public override bool Equals(object o)
		{
			ICorDebugStepperEnum casted = o as ICorDebugStepperEnum;
			return (casted != null) && (casted.WrappedObject == wrappedObject);
		}
		
		
		public void Skip(uint celt)
		{
			this.WrappedObject.Skip(celt);
		}
		
		public void Reset()
		{
			this.WrappedObject.Reset();
		}
		
		public void Clone(out ICorDebugEnum ppEnum)
		{
			Debugger.Interop.CorDebug.ICorDebugEnum out_ppEnum;
			this.WrappedObject.Clone(out out_ppEnum);
			ppEnum = ICorDebugEnum.Wrap(out_ppEnum);
		}
		
		public void GetCount(out uint pcelt)
		{
			this.WrappedObject.GetCount(out pcelt);
		}
		
		public void Next(uint celt, System.IntPtr steppers, out uint pceltFetched)
		{
			this.WrappedObject.Next(celt, steppers, out pceltFetched);
		}
	}
}
