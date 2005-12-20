// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Wrappers.CorDebug
{
	using System;
	
	
	public class ICorDebugController
	{
		
		private Debugger.Interop.CorDebug.ICorDebugController wrappedObject;
		
		internal Debugger.Interop.CorDebug.ICorDebugController WrappedObject
		{
			get
			{
				return this.wrappedObject;
			}
		}
		
		public ICorDebugController(Debugger.Interop.CorDebug.ICorDebugController wrappedObject)
		{
			this.wrappedObject = wrappedObject;
		}
		
		public static ICorDebugController Wrap(Debugger.Interop.CorDebug.ICorDebugController objectToWrap)
		{
			return new ICorDebugController(objectToWrap);
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
		
		public static bool operator ==(ICorDebugController o1, ICorDebugController o2)
		{
			return ((object)o1 == null && (object)o2 == null) ||
			       ((object)o1 != null && (object)o2 != null && o1.WrappedObject == o2.WrappedObject);
		}
		
		public static bool operator !=(ICorDebugController o1, ICorDebugController o2)
		{
			return !(o1 == o2);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		public override bool Equals(object o)
		{
			ICorDebugController casted = o as ICorDebugController;
			return (casted != null) && (casted.WrappedObject == wrappedObject);
		}
		
		
		public void Stop(uint dwTimeoutIgnored)
		{
			this.WrappedObject.Stop(dwTimeoutIgnored);
		}
		
		public void Continue(int fIsOutOfBand)
		{
			this.WrappedObject.Continue(fIsOutOfBand);
		}
		
		public void IsRunning(out int pbRunning)
		{
			this.WrappedObject.IsRunning(out pbRunning);
		}
		
		public void HasQueuedCallbacks(ICorDebugThread pThread, out int pbQueued)
		{
			this.WrappedObject.HasQueuedCallbacks(pThread.WrappedObject, out pbQueued);
		}
		
		public void EnumerateThreads(out ICorDebugThreadEnum ppThreads)
		{
			Debugger.Interop.CorDebug.ICorDebugThreadEnum out_ppThreads;
			this.WrappedObject.EnumerateThreads(out out_ppThreads);
			ppThreads = ICorDebugThreadEnum.Wrap(out_ppThreads);
		}
		
		public void SetAllThreadsDebugState(CorDebugThreadState state, ICorDebugThread pExceptThisThread)
		{
			this.WrappedObject.SetAllThreadsDebugState(((Debugger.Interop.CorDebug.CorDebugThreadState)(state)), pExceptThisThread.WrappedObject);
		}
		
		public void Detach()
		{
			this.WrappedObject.Detach();
		}
		
		public void Terminate(uint exitCode)
		{
			this.WrappedObject.Terminate(exitCode);
		}
		
		public void CanCommitChanges(uint cSnapshots, ref ICorDebugEditAndContinueSnapshot pSnapshots, out ICorDebugErrorInfoEnum pError)
		{
			Debugger.Interop.CorDebug.ICorDebugEditAndContinueSnapshot ref_pSnapshots = pSnapshots.WrappedObject;
			Debugger.Interop.CorDebug.ICorDebugErrorInfoEnum out_pError;
			this.WrappedObject.CanCommitChanges(cSnapshots, ref ref_pSnapshots, out out_pError);
			pSnapshots = ICorDebugEditAndContinueSnapshot.Wrap(ref_pSnapshots);
			pError = ICorDebugErrorInfoEnum.Wrap(out_pError);
		}
		
		public void CommitChanges(uint cSnapshots, ref ICorDebugEditAndContinueSnapshot pSnapshots, out ICorDebugErrorInfoEnum pError)
		{
			Debugger.Interop.CorDebug.ICorDebugEditAndContinueSnapshot ref_pSnapshots = pSnapshots.WrappedObject;
			Debugger.Interop.CorDebug.ICorDebugErrorInfoEnum out_pError;
			this.WrappedObject.CommitChanges(cSnapshots, ref ref_pSnapshots, out out_pError);
			pSnapshots = ICorDebugEditAndContinueSnapshot.Wrap(ref_pSnapshots);
			pError = ICorDebugErrorInfoEnum.Wrap(out_pError);
		}
	}
}
