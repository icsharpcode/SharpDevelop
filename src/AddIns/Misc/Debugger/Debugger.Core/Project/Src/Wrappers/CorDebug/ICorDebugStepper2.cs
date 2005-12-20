// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Wrappers.CorDebug
{
	using System;
	
	
	public class ICorDebugStepper2
	{
		
		private Debugger.Interop.CorDebug.ICorDebugStepper2 wrappedObject;
		
		internal Debugger.Interop.CorDebug.ICorDebugStepper2 WrappedObject
		{
			get
			{
				return this.wrappedObject;
			}
		}
		
		public ICorDebugStepper2(Debugger.Interop.CorDebug.ICorDebugStepper2 wrappedObject)
		{
			this.wrappedObject = wrappedObject;
		}
		
		public static ICorDebugStepper2 Wrap(Debugger.Interop.CorDebug.ICorDebugStepper2 objectToWrap)
		{
			return new ICorDebugStepper2(objectToWrap);
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
		
		public static bool operator ==(ICorDebugStepper2 o1, ICorDebugStepper2 o2)
		{
			return ((object)o1 == null && (object)o2 == null) ||
			       ((object)o1 != null && (object)o2 != null && o1.WrappedObject == o2.WrappedObject);
		}
		
		public static bool operator !=(ICorDebugStepper2 o1, ICorDebugStepper2 o2)
		{
			return !(o1 == o2);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		public override bool Equals(object o)
		{
			ICorDebugStepper2 casted = o as ICorDebugStepper2;
			return (casted != null) && (casted.WrappedObject == wrappedObject);
		}
		
		
		public void SetJMC(int fIsJMCStepper)
		{
			this.WrappedObject.SetJMC(fIsJMCStepper);
		}
	}
}
