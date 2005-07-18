// <file>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
// </file>

// Regular expresion:
// ^{\t*}{(:Ll| )*{:i} *\(((.# {:i}, |\))|())^6\)*}\n\t*\{(.|\n)@\}
// Output: \1 - intention   \2 - declaration \3 - function name  \4-9 parameters

// Replace with:
// \1\2\n\1{\n\1\tEnterCallback("\3");\n\1\t\n\1\tExitCallback_Continue(pAppDomain);\n\1}

using System;
using System.Runtime.InteropServices;

using DebuggerInterop.Core;

namespace DebuggerLibrary
{
	class ManagedCallback
	{
		NDebugger debugger;

		bool handlingCallback = false;
		public event CorDebugEvalEventHandler CorDebugEvalCompleted;

		public ManagedCallback(NDebugger debugger)
		{
			this.debugger = debugger;
		}
		
		public bool HandlingCallback {
			get {
				return handlingCallback;
			}
		}

		void EnterCallback(string name)
		{
			handlingCallback = true;
			debugger.IsProcessRunning = false;
			debugger.CurrentThread = null;
			debugger.TraceMessage("Callback: " + name);
		}
		
		void ExitCallback_Continue(ICorDebugAppDomain pAppDomain)
		{
			debugger.Continue(pAppDomain);
			handlingCallback = false;
		}
		
		void ExitCallback_Continue()
		{
			debugger.Continue();
			handlingCallback = false;
		}
		
		void ExitCallback_Paused(PausedReason reason)
		{
			debugger.CurrentThread.CurrentFunction = debugger.CurrentThread.LastFunctionWithLoadedSymbols;
			if (reason != PausedReason.EvalComplete) {
				debugger.OnDebuggingPaused(reason);
				debugger.OnIsProcessRunningChanged();
			}
			handlingCallback = false;
		}
		
		
		#region Program folow control

		public void StepComplete(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugStepper pStepper, CorDebugStepReason reason)
		{
			EnterCallback("StepComplete");

			debugger.CurrentThread = debugger.GetThread(pThread);

			if (debugger.CurrentThread.CurrentFunction.Module.SymbolsLoaded == false) {
				debugger.TraceMessage(" - stepping out of code without symbols");
				debugger.StepOut();
				return;
			}
			
			ExitCallback_Paused(PausedReason.StepComplete);
		}
		
		public void Breakpoint(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, IntPtr pBreakpoint)
		{
			EnterCallback("Breakpoint");

			debugger.CurrentThread = debugger.GetThread(pThread);
			
			ExitCallback_Paused(PausedReason.Breakpoint);
			
			foreach (Breakpoint b in debugger.Breakpoints) {
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

			debugger.CurrentThread = debugger.GetThread(pThread);

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
			/*
			if (!NDebugger.CatchHandledExceptions && (unhandled == 0)) {
				ExitCallback_Continue();
				return;
			}

			NDebugger.CurrentThread = NDebugger.Instance.GetThread(pThread);
			NDebugger.CurrentThread.CurrentExceptionIsHandled = (unhandled == 0);
			*/
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

			debugger.OnLogMessage(pMessage);

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
			
			if (CorDebugEvalCompleted != null) {
				CorDebugEvalCompleted(this, new CorDebugEvalEventArgs(eval));
			}
			
			ExitCallback_Paused(PausedReason.EvalComplete);
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

			debugger.AddModule(pModule);

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
				Thread thread = debugger.GetThread(pThread);
				thread.HasBeenLoaded = true;
				thread.OnThreadStateChanged();
				ExitCallback_Continue();
				return;
			}
		}

		public void CreateThread(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread)
		{
			EnterCallback("CreateThread");

			debugger.AddThread(pThread);

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

			debugger.RemoveModule(pModule);

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

			Thread thread = debugger.GetThread(pThread);

			if (debugger.CurrentThread == thread) {
				debugger.CurrentThread = null;
			}

			debugger.RemoveThread(thread);

			ExitCallback_Continue(pAppDomain);
		}

		public void ExitAppDomain(ICorDebugProcess pProcess, ICorDebugAppDomain pAppDomain)
		{
			EnterCallback("ExitAppDomain");

			ExitCallback_Continue();
		}

		public void ExitProcess(ICorDebugProcess pProcess)
		{
			EnterCallback("ExitProcess");

			Process process = debugger.GetProcess(pProcess);

			if (debugger.CurrentProcess == process) {
				debugger.CurrentProcess = null;
			}

			debugger.RemoveProcess(process);

			if (debugger.Processes.Count == 0) {
				debugger.ResetEnvironment();
			}
		}

		#endregion

		#region ICorDebugManagedCallback2 Members

		public void ChangeConnection(ICorDebugProcess pProcess, uint dwConnectionId)
		{
			EnterCallback("ChangeConnection");
			
			ExitCallback_Continue();
		}

		public void CreateConnection(ICorDebugProcess pProcess, uint dwConnectionId, ref ushort pConnName)
		{
			EnterCallback("CreateConnection");
			
			ExitCallback_Continue();
		}

		public void DestroyConnection(ICorDebugProcess pProcess, uint dwConnectionId)
		{
			EnterCallback("DestroyConnection");
			
			ExitCallback_Continue();
		}

		public void Exception2(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugFrame pFrame, uint nOffset, CorDebugExceptionCallbackType dwEventType, uint dwFlags)
		{
			EnterCallback("Exception2");
			
			//if (!NDebugger.CatchHandledExceptions && dwEventType != CorDebugExceptionCallbackType.DEBUG_EXCEPTION_UNHANDLED) {
			//	ExitCallback_Continue(pAppDomain);
			//	return;
			//}

			debugger.CurrentThread = debugger.GetThread(pThread);
			debugger.CurrentThread.CurrentExceptionType = (ExceptionType)dwEventType;

			ExitCallback_Paused(PausedReason.Exception);
		}

		public void ExceptionUnwind(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, CorDebugExceptionUnwindCallbackType dwEventType, uint dwFlags)
		{
			EnterCallback("ExceptionUnwind");
			
			ExitCallback_Continue(pAppDomain);
		}

		public void FunctionRemapComplete(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugFunction pFunction)
		{
			EnterCallback("FunctionRemapComplete");
			
			ExitCallback_Continue(pAppDomain);
		}

		public void FunctionRemapOpportunity(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugFunction pOldFunction, ICorDebugFunction pNewFunction, uint oldILOffset)
		{
			EnterCallback("FunctionRemapOpportunity");
			
			ExitCallback_Continue(pAppDomain);
		}

		public void MDANotification(ICorDebugController c, ICorDebugThread t, ICorDebugMDA mda)
		{
			EnterCallback("MDANotification");
			
			ExitCallback_Continue();
		}

		#endregion
	}
}
