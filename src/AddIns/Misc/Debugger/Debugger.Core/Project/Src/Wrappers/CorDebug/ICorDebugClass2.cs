// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Wrappers.CorDebug
{
	using System;
	
	
	public class ICorDebugClass2
	{
		
		private Debugger.Interop.CorDebug.ICorDebugClass2 wrappedObject;
		
		internal Debugger.Interop.CorDebug.ICorDebugClass2 WrappedObject
		{
			get
			{
				return this.wrappedObject;
			}
		}
		
		public ICorDebugClass2(Debugger.Interop.CorDebug.ICorDebugClass2 wrappedObject)
		{
			this.wrappedObject = wrappedObject;
		}
		
		public static ICorDebugClass2 Wrap(Debugger.Interop.CorDebug.ICorDebugClass2 objectToWrap)
		{
			return new ICorDebugClass2(objectToWrap);
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
		
		public static bool operator ==(ICorDebugClass2 o1, ICorDebugClass2 o2)
		{
			return ((object)o1 == null && (object)o2 == null) ||
			       ((object)o1 != null && (object)o2 != null && o1.WrappedObject == o2.WrappedObject);
		}
		
		public static bool operator !=(ICorDebugClass2 o1, ICorDebugClass2 o2)
		{
			return !(o1 == o2);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		public override bool Equals(object o)
		{
			ICorDebugClass2 casted = o as ICorDebugClass2;
			return (casted != null) && (casted.WrappedObject == wrappedObject);
		}
		
		
		public ICorDebugType GetParameterizedType(uint elementType, uint nTypeArgs, ref ICorDebugType ppTypeArgs)
		{
			ICorDebugType ppType;
			Debugger.Interop.CorDebug.ICorDebugType ref_ppTypeArgs = ppTypeArgs.WrappedObject;
			Debugger.Interop.CorDebug.ICorDebugType out_ppType;
			this.WrappedObject.GetParameterizedType(elementType, nTypeArgs, ref ref_ppTypeArgs, out out_ppType);
			ppTypeArgs = ICorDebugType.Wrap(ref_ppTypeArgs);
			ppType = ICorDebugType.Wrap(out_ppType);
			return ppType;
		}
		
		public void SetJMCStatus(int bIsJustMyCode)
		{
			this.WrappedObject.SetJMCStatus(bIsJustMyCode);
		}
	}
}
