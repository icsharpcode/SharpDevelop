// <file>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
// </file>

using System;
using System.Runtime.InteropServices;

using DebuggerInterop.Core;
using DebuggerInterop.Symbols;

namespace DebuggerLibrary
{
	class ManagedCallback
	{		
		bool handlingCallback = false;
		
		public bool HandlingCallback {
			get {
				return handlingCallback;
			}
		}

		void EnterCallback(string name)
		{
			handlingCallback = true;
			NDebugger.IsProcessRunning = false;
			NDebugger.CurrentThread = null;
			NDebugger.TraceMessage("Callback: " + name);
		}
		
		void ExitCallback_Continue(ICorDebugAppDomain pAppDomain)
		{
			NDebugger.Continue(pAppDomain);
			handlingCallback = false;
		}
		
		void ExitCallback_Continue()
		{
			NDebugger.Continue();
			handlingCallback = false;
		}
		
		void ExitCallback_Paused(PausedReason reason)
		{
			NDebugger.OnDebuggingPaused(reason);
			NDebugger.OnIsProcessRunningChanged();
			handlingCallback = false;
		}
		
		
		#region Program folow control

		public void StepComplete(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugStepper pStepper, CorDebugStepReason reason)
		{
			EnterCallback("StepComplete");

			NDebugger.CurrentThread = NDebugger.Threads[pThread];

			if (NDebugger.CurrentThread.CurrentFunction.Module.SymbolsLoaded == false) {
				NDebugger.TraceMessage(" - stepping out of code without symbols");
				NDebugger.StepOut();
				return;
			}
            
			ExitCallback_Paused(PausedReason.StepComplete);
		}
		
		public void Breakpoint(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, IntPtr pBreakpoint)
		{
			EnterCallback("Breakpoint");
			
			NDebugger.CurrentThread = NDebugger.Threads[pThread];
			
			ExitCallback_Paused(PausedReason.Breakpoint);
			
			foreach (Breakpoint b in NDebugger.Breakpoints) {
				if (b.Equals(pBreakpoint)) {
					b.OnBreakpointHit();
				}
			}
		}
		
		public void BreakpointSetError(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugBreakpoint pBreakpoint, uint dwError)
		{
			EnterCallback("BreakpointSetError");
			
			ExitCallback_Continue(pAppDomain);
		}
		
		public unsafe void Break(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread)
		{
			EnterCallback("Break");

			NDebugger.CurrentThread = NDebugger.Threads[pThread];

			ExitCallback_Paused(PausedReason.Break);
		}

		public void ControlCTrap(ICorDebugProcess pProcess)
		{
			EnterCallback("ControlCTrap");

			ExitCallback_Paused(PausedReason.ControlCTrap);
		}

		public unsafe void Exception(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, int unhandled)
		{
			EnterCallback("Exception");
			
			//if (!NDebugger.CatchHandledExceptions && (unhandled == 0)) {
			//	ExitCallback_Continue();
			//	return;
			//}

			NDebugger.CurrentThread = NDebugger.Threads[pThread];
			NDebugger.CurrentThread.CurrentExceptionIsHandled = (unhandled == 0);

			ExitCallback_Paused(PausedReason.Exception);
		}

		#endregion

		#region Various

		public void LogSwitch(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, int lLevel, uint ulReason, string pLogSwitchName, string pParentName)
		{
			EnterCallback("LogSwitch");

			ExitCallback_Continue(pAppDomain);
		}

		public void EvalException(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugEval pEval)
		{
			EnterCallback("EvalException");

			ExitCallback_Continue(pAppDomain);
		}

		public void LogMessage(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, int lLevel, string pLogSwitchName, string pMessage)
		{
			EnterCallback("LogMessage");
			
			NDebugger.OnLogMessage(pMessage);

			ExitCallback_Continue(pAppDomain);
		}

		public void EditAndContinueRemap(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugFunction pFunction, int fAccurate)
		{
			EnterCallback("EditAndContinueRemap");

			ExitCallback_Continue(pAppDomain);
		}

		public void EvalComplete(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugEval eval)
		{
			EnterCallback("EvalComplete");
			/*
			foreach (Eval e in Eval.waitingEvals) {
				if (e.corEval == eval) {
					Eval.waitingEvals.Remove(e);
					e.OnEvalComplete();
					break;
				}
			}
			
			NDebugger.OnIsProcessRunningChanged();
			handlingCallback = false;
			Eval.PerformNextEval();*/
			
			ExitCallback_Continue(pAppDomain);
		}

		public void DebuggerError(ICorDebugProcess pProcess, int errorHR, uint errorCode)
		{
			EnterCallback("DebuggerError");

            System.Windows.Forms.MessageBox.Show("Debugger error: \nHR = " + errorHR.ToString() + "\nCode = " + errorCode.ToString());

			ExitCallback_Paused(PausedReason.DebuggerError);
		}

		public void UpdateModuleSymbols(ICorDebugAppDomain pAppDomain, ICorDebugModule pModule, DebuggerInterop.Core.IStream pSymbolStream)
		{
			EnterCallback("UpdateModuleSymbols");

			ExitCallback_Continue(pAppDomain);
		}

		#endregion

		#region Start of Application

		public void CreateProcess(ICorDebugProcess pProcess)
		{
			EnterCallback("CreateProcess");

			ExitCallback_Continue();
		}

		public void CreateAppDomain(ICorDebugProcess pProcess, ICorDebugAppDomain pAppDomain)
		{
			EnterCallback("CreateAppDomain");

			pAppDomain.Attach();

			ExitCallback_Continue(pAppDomain);
		}

		public void LoadAssembly(ICorDebugAppDomain pAppDomain, ICorDebugAssembly pAssembly)
		{
			EnterCallback("LoadAssembly");

			ExitCallback_Continue(pAppDomain);
		}

		public unsafe void LoadModule(ICorDebugAppDomain pAppDomain, ICorDebugModule pModule)
		{
			EnterCallback("LoadModule");

			NDebugger.Modules.Add(pModule);

			ExitCallback_Continue(pAppDomain);
		}

		public void NameChange(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread)
		{
			if (pAppDomain != null)
			{
				EnterCallback("NameChange: pAppDomain");
				ExitCallback_Continue(pAppDomain);
				return;
			}
			if (pThread != null)
			{
				EnterCallback("NameChange: pThread");
				NDebugger.Threads[pThread].OnThreadStateChanged();
				ExitCallback_Continue();
				return;
			}
		}

		public void CreateThread(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread)
		{
			EnterCallback("CreateThread");
			
			NDebugger.Threads.Add(pThread);

			if (NDebugger.MainThread == null) {
				NDebugger.MainThread = NDebugger.Threads[pThread];
			}

			ExitCallback_Continue(pAppDomain);
		}

		public void LoadClass(ICorDebugAppDomain pAppDomain, ICorDebugClass c)
		{
			EnterCallback("LoadClass");

			ExitCallback_Continue(pAppDomain);
		}

		#endregion

		#region Exit of Application

		public void UnloadClass(ICorDebugAppDomain pAppDomain, ICorDebugClass c)
		{
			EnterCallback("UnloadClass");

			ExitCallback_Continue(pAppDomain);
		}

		public void UnloadModule(ICorDebugAppDomain pAppDomain, ICorDebugModule pModule)
		{
			EnterCallback("UnloadModule");

			NDebugger.Modules.Remove(pModule);

			ExitCallback_Continue(pAppDomain);
		}

		public void UnloadAssembly(ICorDebugAppDomain pAppDomain, ICorDebugAssembly pAssembly)
		{
			EnterCallback("UnloadAssembly");

			ExitCallback_Continue(pAppDomain);
		}

		public void ExitThread(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread)
		{
			EnterCallback("ExitThread");
			
			Thread thread = NDebugger.Threads[pThread];

			if (NDebugger.CurrentThread == thread)
				NDebugger.CurrentThread = null;
			
			if (NDebugger.MainThread == thread)
				NDebugger.MainThread = null;
			
			NDebugger.Threads.Remove(thread);

			try { // TODO
				ExitCallback_Continue(pAppDomain);
			} catch {}
		}

		public void ExitAppDomain(ICorDebugProcess pProcess, ICorDebugAppDomain pAppDomain)
		{
			EnterCallback("ExitAppDomain");

			ExitCallback_Continue();
		}

		public void ExitProcess(ICorDebugProcess pProcess)
		{
			EnterCallback("ExitProcess");

			NDebugger.ResetEnvironment();

			pProcess.Continue(0); //TODO
		}

		#endregion
	}
}
