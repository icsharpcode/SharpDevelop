// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Wrappers.CorDebug
{
	using System;
	
	
	public class ICorDebugProcess2
	{
		
		private Debugger.Interop.CorDebug.ICorDebugProcess2 wrappedObject;
		
		internal Debugger.Interop.CorDebug.ICorDebugProcess2 WrappedObject
		{
			get
			{
				return this.wrappedObject;
			}
		}
		
		public ICorDebugProcess2(Debugger.Interop.CorDebug.ICorDebugProcess2 wrappedObject)
		{
			this.wrappedObject = wrappedObject;
		}
		
		public static ICorDebugProcess2 Wrap(Debugger.Interop.CorDebug.ICorDebugProcess2 objectToWrap)
		{
			return new ICorDebugProcess2(objectToWrap);
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
		
		public static bool operator ==(ICorDebugProcess2 o1, ICorDebugProcess2 o2)
		{
			return ((object)o1 == null && (object)o2 == null) ||
			       ((object)o1 != null && (object)o2 != null && o1.WrappedObject == o2.WrappedObject);
		}
		
		public static bool operator !=(ICorDebugProcess2 o1, ICorDebugProcess2 o2)
		{
			return !(o1 == o2);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		public override bool Equals(object o)
		{
			ICorDebugProcess2 casted = o as ICorDebugProcess2;
			return (casted != null) && (casted.WrappedObject == wrappedObject);
		}
		
		
		public void GetThreadForTaskID(ulong taskid, out ICorDebugThread2 ppThread)
		{
			Debugger.Interop.CorDebug.ICorDebugThread2 out_ppThread;
			this.WrappedObject.GetThreadForTaskID(taskid, out out_ppThread);
			ppThread = ICorDebugThread2.Wrap(out_ppThread);
		}
		
		public void GetVersion(out Debugger.Interop.CorDebug._COR_VERSION version)
		{
			this.WrappedObject.GetVersion(out version);
		}
		
		public void SetUnmanagedBreakpoint(ulong address, uint bufsize, System.IntPtr buffer, out uint bufLen)
		{
			this.WrappedObject.SetUnmanagedBreakpoint(address, bufsize, buffer, out bufLen);
		}
		
		public void ClearUnmanagedBreakpoint(ulong address)
		{
			this.WrappedObject.ClearUnmanagedBreakpoint(address);
		}
		
		public void SetDesiredNGENCompilerFlags(uint pdwFlags)
		{
			this.WrappedObject.SetDesiredNGENCompilerFlags(pdwFlags);
		}
		
		public void GetDesiredNGENCompilerFlags(out uint pdwFlags)
		{
			this.WrappedObject.GetDesiredNGENCompilerFlags(out pdwFlags);
		}
		
		public void GetReferenceValueFromGCHandle(uint handle, out ICorDebugReferenceValue pOutValue)
		{
			Debugger.Interop.CorDebug.ICorDebugReferenceValue out_pOutValue;
			this.WrappedObject.GetReferenceValueFromGCHandle(handle, out out_pOutValue);
			pOutValue = ICorDebugReferenceValue.Wrap(out_pOutValue);
		}
	}
}
