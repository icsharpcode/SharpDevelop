// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Wrappers.CorDebug
{
	using System;
	
	
	public class ICorDebugThread2
	{
		
		private Debugger.Interop.CorDebug.ICorDebugThread2 wrappedObject;
		
		internal Debugger.Interop.CorDebug.ICorDebugThread2 WrappedObject
		{
			get
			{
				return this.wrappedObject;
			}
		}
		
		public ICorDebugThread2(Debugger.Interop.CorDebug.ICorDebugThread2 wrappedObject)
		{
			this.wrappedObject = wrappedObject;
		}
		
		public static ICorDebugThread2 Wrap(Debugger.Interop.CorDebug.ICorDebugThread2 objectToWrap)
		{
			return new ICorDebugThread2(objectToWrap);
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
		
		public static bool operator ==(ICorDebugThread2 o1, ICorDebugThread2 o2)
		{
			return ((object)o1 == null && (object)o2 == null) ||
			       ((object)o1 != null && (object)o2 != null && o1.WrappedObject == o2.WrappedObject);
		}
		
		public static bool operator !=(ICorDebugThread2 o1, ICorDebugThread2 o2)
		{
			return !(o1 == o2);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		public override bool Equals(object o)
		{
			ICorDebugThread2 casted = o as ICorDebugThread2;
			return (casted != null) && (casted.WrappedObject == wrappedObject);
		}
		
		
		public void GetActiveFunctions(uint cFunctions, out uint pcFunctions, System.IntPtr pFunctions)
		{
			this.WrappedObject.GetActiveFunctions(cFunctions, out pcFunctions, pFunctions);
		}
		
		public void GetConnectionID(out uint pdwConnectionId)
		{
			this.WrappedObject.GetConnectionID(out pdwConnectionId);
		}
		
		public void GetTaskID(out ulong pTaskId)
		{
			this.WrappedObject.GetTaskID(out pTaskId);
		}
		
		public void GetVolatileOSThreadID(out uint pdwTid)
		{
			this.WrappedObject.GetVolatileOSThreadID(out pdwTid);
		}
		
		public void InterceptCurrentException(ICorDebugFrame pFrame)
		{
			this.WrappedObject.InterceptCurrentException(pFrame.WrappedObject);
		}
	}
}
