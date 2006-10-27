// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using CORDBLib_1_0;
using NoGoop.Util;

namespace NoGoop.Debug
{
	// Interface class to the debugger
	public class Debugger : ICorDebugManagedCallback
	{
		protected ICorDebug             _debug;
		protected ICorDebugProcess      _activeProcess;
		
		public Debugger()
		{
		}
		
		protected void Init()
		{
			_debug = new CorDebugClass();
			_debug.Initialize();
			_debug.SetManagedHandler(this);
		}
		
		// API for this class
		public void Attach(uint processId)
		{
			if (_debug == null)
				Init();
			_activeProcess = _debug.DebugActiveProcess(processId, 0);
			ICorDebugThreadEnum threads = 
				_activeProcess.EnumerateThreads();
			uint count = 0;
			//threads.GetCount(out count);
			count = threads.GetCount();
			Console.WriteLine("thread count: " + count);
			/*
			Type type = threads.GetType();
			MethodInfo mi = type.GetMethod("GetCount");
			count = (uint)mi.Invoke(threads, new Object[] { });
			Console.WriteLine("thread count: " + count);
			*/
		}
		
		public void AttachDebuggedProcess(int processId)
		{
			TraceUtil.WriteLineInfo(this, "Debug process: " 
									+ processId);
			ICorDebugProcess dbgProcess;
			dbgProcess = _debug.GetProcess((uint)processId);
			
			if (dbgProcess != null) {
				dbgProcess =  _debug.DebugActiveProcess((uint)processId, 0);
			}
		}
		
		//
		// The rest of these are callbacks from the debugger
		//
		public void Break(ICorDebugAppDomain appDomain, ICorDebugThread thd)
		{
		}
		
		public void Breakpoint(ICorDebugAppDomain appDomain,
							  ICorDebugThread thd,
							  ICorDebugBreakpoint breakPoint)
		{
		}
		
		public void BreakpointSetError(ICorDebugAppDomain appDomain,
									  ICorDebugThread thd,
									  ICorDebugBreakpoint breakPoint,
									  uint xxx)
		{
		}
		
		public void ControlCTrap(ICorDebugProcess process)
		{
		}
		
		public void CreateAppDomain(ICorDebugProcess process,
									ICorDebugAppDomain appDomain)
		{
			MessageBox.Show("Create App Domain: " + appDomain);
		}
		
		public void CreateProcess(ICorDebugProcess process)
		{
			MessageBox.Show("Create Process: " + process);
		}
		
		public void CreateThread(ICorDebugAppDomain appDomain,
								ICorDebugThread thd)
		{
			MessageBox.Show("Create Thead: " + thd);
		}
		
		public void DebuggerError(ICorDebugProcess process,
								 int xxx,
								 uint yyy)
		{
		}
		
		public void EditAndContinueRemap(ICorDebugAppDomain appDomain,
										ICorDebugThread thd,
										ICorDebugFunction function,
										int xxx)
		{
		}
		
		public void EvalComplete(ICorDebugAppDomain appDomain,
								ICorDebugThread thd,
								ICorDebugEval eval)
		{
		}
		
		public void EvalException(ICorDebugAppDomain appDomain,
								 ICorDebugThread thd,
								 ICorDebugEval eval)
		{
		}
		
		public void Exception(ICorDebugAppDomain appDomain,
							 ICorDebugThread thd,
							 int xxx)
		{
		}
		
		public void ExitAppDomain(ICorDebugProcess process,
								 ICorDebugAppDomain appDomain)
		{
		}
		
		public void ExitProcess(ICorDebugProcess process)
		{
		}
		
		public void ExitThread(ICorDebugAppDomain appDomain,
							  ICorDebugThread thd)
		{
		}
		
		public void LoadAssembly(ICorDebugAppDomain appDomain,
								ICorDebugAssembly assy)
		{
			MessageBox.Show("Load assy: " + assy);
		}
		
		public void LoadClass(ICorDebugAppDomain appDomain,
							 ICorDebugClass cls)
		{
		}
		
		public void LoadModule(ICorDebugAppDomain appDomain,
							  ICorDebugModule module)
		{
			MessageBox.Show("Load module: " + module);
		}
			/**
		public void LogMessage(ICorDebugAppDomain appDomain,
							   ICorDebugThread thd,
							   int xxx,
							   ref UInt16 yyy,
							   ref UInt16 zzz)
		{
		}
		public void LogSwitch(ICorDebugAppDomain appDomain,
							   ICorDebugThread thd,
							   int xxx,
							   uint xxx1,
							   ref UInt16 yyy,
							   ref UInt16 zzz)
		{
		}
		**/
		
		public void LogMessage(ICorDebugAppDomain appDomain,
							  ICorDebugThread thd,
							  int xxx,
							  UInt16 yyy,
							  UInt16 zzz)
		{
		}
		
		public void LogSwitch(ICorDebugAppDomain appDomain,
							  ICorDebugThread thd,
							  int xxx,
							  uint xxx1,
							  UInt16 yyy,
							  UInt16 zzz)
		{
		}
		
		public void NameChange(ICorDebugAppDomain appDomain,
							  ICorDebugThread thd)
		{
		}
		
		public void StepComplete(ICorDebugAppDomain appDomain,
								ICorDebugThread thd,
								ICorDebugStepper stepper,
								CorDebugStepReason reason)
		{
		}
		
		public void UnloadAssembly(ICorDebugAppDomain appDomain,
								ICorDebugAssembly assy)
		{
		}
		
		public void UnloadClass(ICorDebugAppDomain appDomain,
							 ICorDebugClass cls)
		{
		}
		
		public void UnloadModule(ICorDebugAppDomain appDomain,
							  ICorDebugModule module)
		{
		}
		
		public void UpdateModuleSymbols(ICorDebugAppDomain appDomain,
										ICorDebugModule module,
										CORDBLib_1_0.IStream stream)
		{
		}
	}
}
