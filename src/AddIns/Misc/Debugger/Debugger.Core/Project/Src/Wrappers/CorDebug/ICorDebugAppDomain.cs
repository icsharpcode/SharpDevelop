// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Wrappers.CorDebug
{
	using System;
	
	
	public class ICorDebugAppDomain
	{
		
		private Debugger.Interop.CorDebug.ICorDebugAppDomain wrappedObject;
		
		internal Debugger.Interop.CorDebug.ICorDebugAppDomain WrappedObject
		{
			get
			{
				return this.wrappedObject;
			}
		}
		
		public ICorDebugAppDomain(Debugger.Interop.CorDebug.ICorDebugAppDomain wrappedObject)
		{
			this.wrappedObject = wrappedObject;
		}
		
		public static ICorDebugAppDomain Wrap(Debugger.Interop.CorDebug.ICorDebugAppDomain objectToWrap)
		{
			return new ICorDebugAppDomain(objectToWrap);
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
		
		public static bool operator ==(ICorDebugAppDomain o1, ICorDebugAppDomain o2)
		{
			return ((object)o1 == null && (object)o2 == null) ||
			       ((object)o1 != null && (object)o2 != null && o1.WrappedObject == o2.WrappedObject);
		}
		
		public static bool operator !=(ICorDebugAppDomain o1, ICorDebugAppDomain o2)
		{
			return !(o1 == o2);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		public override bool Equals(object o)
		{
			ICorDebugAppDomain casted = o as ICorDebugAppDomain;
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
		
		public void GetProcess(out ICorDebugProcess ppProcess)
		{
			Debugger.Interop.CorDebug.ICorDebugProcess out_ppProcess;
			this.WrappedObject.GetProcess(out out_ppProcess);
			ppProcess = ICorDebugProcess.Wrap(out_ppProcess);
		}
		
		public void EnumerateAssemblies(out ICorDebugAssemblyEnum ppAssemblies)
		{
			Debugger.Interop.CorDebug.ICorDebugAssemblyEnum out_ppAssemblies;
			this.WrappedObject.EnumerateAssemblies(out out_ppAssemblies);
			ppAssemblies = ICorDebugAssemblyEnum.Wrap(out_ppAssemblies);
		}
		
		public void GetModuleFromMetaDataInterface(object pIMetaData, out ICorDebugModule ppModule)
		{
			Debugger.Interop.CorDebug.ICorDebugModule out_ppModule;
			this.WrappedObject.GetModuleFromMetaDataInterface(pIMetaData, out out_ppModule);
			ppModule = ICorDebugModule.Wrap(out_ppModule);
		}
		
		public void EnumerateBreakpoints(out ICorDebugBreakpointEnum ppBreakpoints)
		{
			Debugger.Interop.CorDebug.ICorDebugBreakpointEnum out_ppBreakpoints;
			this.WrappedObject.EnumerateBreakpoints(out out_ppBreakpoints);
			ppBreakpoints = ICorDebugBreakpointEnum.Wrap(out_ppBreakpoints);
		}
		
		public void EnumerateSteppers(out ICorDebugStepperEnum ppSteppers)
		{
			Debugger.Interop.CorDebug.ICorDebugStepperEnum out_ppSteppers;
			this.WrappedObject.EnumerateSteppers(out out_ppSteppers);
			ppSteppers = ICorDebugStepperEnum.Wrap(out_ppSteppers);
		}
		
		public void IsAttached(out int pbAttached)
		{
			this.WrappedObject.IsAttached(out pbAttached);
		}
		
		public void GetName(uint cchName, out uint pcchName, System.IntPtr szName)
		{
			this.WrappedObject.GetName(cchName, out pcchName, szName);
		}
		
		public void GetObject(out ICorDebugValue ppObject)
		{
			Debugger.Interop.CorDebug.ICorDebugValue out_ppObject;
			this.WrappedObject.GetObject(out out_ppObject);
			ppObject = ICorDebugValue.Wrap(out_ppObject);
		}
		
		public void Attach()
		{
			this.WrappedObject.Attach();
		}
		
		public void GetID(out uint pId)
		{
			this.WrappedObject.GetID(out pId);
		}
	}
}
