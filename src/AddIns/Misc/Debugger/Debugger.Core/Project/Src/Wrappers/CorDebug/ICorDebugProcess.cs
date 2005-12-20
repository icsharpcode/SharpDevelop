// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Wrappers.CorDebug
{
	using System;
	
	
	public class ICorDebugProcess
	{
		
		private Debugger.Interop.CorDebug.ICorDebugProcess wrappedObject;
		
		internal Debugger.Interop.CorDebug.ICorDebugProcess WrappedObject
		{
			get
			{
				return this.wrappedObject;
			}
		}
		
		public ICorDebugProcess(Debugger.Interop.CorDebug.ICorDebugProcess wrappedObject)
		{
			this.wrappedObject = wrappedObject;
		}
		
		public static ICorDebugProcess Wrap(Debugger.Interop.CorDebug.ICorDebugProcess objectToWrap)
		{
			return new ICorDebugProcess(objectToWrap);
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
		
		public static bool operator ==(ICorDebugProcess o1, ICorDebugProcess o2)
		{
			return ((object)o1 == null && (object)o2 == null) ||
			       ((object)o1 != null && (object)o2 != null && o1.WrappedObject == o2.WrappedObject);
		}
		
		public static bool operator !=(ICorDebugProcess o1, ICorDebugProcess o2)
		{
			return !(o1 == o2);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		public override bool Equals(object o)
		{
			ICorDebugProcess casted = o as ICorDebugProcess;
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
		
		public void GetID(out uint pdwProcessId)
		{
			this.WrappedObject.GetID(out pdwProcessId);
		}
		
		public void GetHandle(out uint phProcessHandle)
		{
			this.WrappedObject.GetHandle(out phProcessHandle);
		}
		
		public void GetThread(uint dwThreadId, out ICorDebugThread ppThread)
		{
			Debugger.Interop.CorDebug.ICorDebugThread out_ppThread;
			this.WrappedObject.GetThread(dwThreadId, out out_ppThread);
			ppThread = ICorDebugThread.Wrap(out_ppThread);
		}
		
		public void EnumerateObjects(out ICorDebugObjectEnum ppObjects)
		{
			Debugger.Interop.CorDebug.ICorDebugObjectEnum out_ppObjects;
			this.WrappedObject.EnumerateObjects(out out_ppObjects);
			ppObjects = ICorDebugObjectEnum.Wrap(out_ppObjects);
		}
		
		public void IsTransitionStub(ulong address, out int pbTransitionStub)
		{
			this.WrappedObject.IsTransitionStub(address, out pbTransitionStub);
		}
		
		public void IsOSSuspended(uint threadID, out int pbSuspended)
		{
			this.WrappedObject.IsOSSuspended(threadID, out pbSuspended);
		}
		
		public void GetThreadContext(uint threadID, uint contextSize, System.IntPtr context)
		{
			this.WrappedObject.GetThreadContext(threadID, contextSize, context);
		}
		
		public void SetThreadContext(uint threadID, uint contextSize, System.IntPtr context)
		{
			this.WrappedObject.SetThreadContext(threadID, contextSize, context);
		}
		
		public void ReadMemory(ulong address, uint size, System.IntPtr buffer, out uint read)
		{
			this.WrappedObject.ReadMemory(address, size, buffer, out read);
		}
		
		public void WriteMemory(ulong address, uint size, ref byte buffer, out uint written)
		{
			this.WrappedObject.WriteMemory(address, size, ref buffer, out written);
		}
		
		public void ClearCurrentException(uint threadID)
		{
			this.WrappedObject.ClearCurrentException(threadID);
		}
		
		public void EnableLogMessages(int fOnOff)
		{
			this.WrappedObject.EnableLogMessages(fOnOff);
		}
		
		public void ModifyLogSwitch(ref ushort pLogSwitchName, int lLevel)
		{
			this.WrappedObject.ModifyLogSwitch(ref pLogSwitchName, lLevel);
		}
		
		public void EnumerateAppDomains(out ICorDebugAppDomainEnum ppAppDomains)
		{
			Debugger.Interop.CorDebug.ICorDebugAppDomainEnum out_ppAppDomains;
			this.WrappedObject.EnumerateAppDomains(out out_ppAppDomains);
			ppAppDomains = ICorDebugAppDomainEnum.Wrap(out_ppAppDomains);
		}
		
		public void GetObject(out ICorDebugValue ppObject)
		{
			Debugger.Interop.CorDebug.ICorDebugValue out_ppObject;
			this.WrappedObject.GetObject(out out_ppObject);
			ppObject = ICorDebugValue.Wrap(out_ppObject);
		}
		
		public void ThreadForFiberCookie(uint fiberCookie, out ICorDebugThread ppThread)
		{
			Debugger.Interop.CorDebug.ICorDebugThread out_ppThread;
			this.WrappedObject.ThreadForFiberCookie(fiberCookie, out out_ppThread);
			ppThread = ICorDebugThread.Wrap(out_ppThread);
		}
		
		public void GetHelperThreadID(out uint pThreadID)
		{
			this.WrappedObject.GetHelperThreadID(out pThreadID);
		}
	}
}
