// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

// Regular expresion:
// ^{\t*}{(:Ll| )*{:i} *\(((.# {:i}, |\))|())^6\)*}\n\t*\{(.|\n)@\}
// Output: \1 - intention   \2 - declaration \3 - function name  \4-9 parameters

// Replace with:
// \1\2\n\1{\n\1\tEnterCallback("\3");\n\1\t\n\1\tExitCallback_Continue();\n\1}

using System;
using System.Runtime.InteropServices;

using DebuggerInterop.Core;

namespace DebuggerLibrary
{
	class ManagedCallback
	{
		NDebugger debugger;

		bool handlingCallback = false;

		public event EventHandler<CorDebugEvalEventArgs> CorDebugEvalCompleted;

		public ManagedCallback(NDebugger debugger)
		{
			this.debugger = debugger;
		}
		
		public bool HandlingCallback {
			get {
				return handlingCallback;
			}
		}

		// Sets CurrentProcess
		void EnterCallback(string name, ICorDebugProcess pProcess)
		{
			EnterCallback(name);

			Process process = debugger.GetProcess(pProcess);
			process.IsProcessRunning = false;
			debugger.CurrentProcess = process;
		}

		// Sets CurrentProcess
		void EnterCallback(string name, ICorDebugAppDomain pAppDomain)
		{
			EnterCallback(name);

			ICorDebugProcess pProcess;
			pAppDomain.GetProcess(out pProcess);
			Process process = debugger.GetProcess(pProcess);
			process.IsProcessRunning = false;
			debugger.CurrentProcess = process;
		}

		// Sets CurrentProcess, CurrentThread and CurrentFunction
		// (CurrentFunction will be set to null if there are no symbols)
		void EnterCallback(string name, ICorDebugThread pThread)
		{
			EnterCallback(name);

			Thread thread = debugger.GetThread(pThread);
			Process process = thread.Process;
			process.IsProcessRunning = false;
			debugger.CurrentProcess = process;
			process.CurrentThread = thread;
			thread.CurrentFunction = thread.LastFunctionWithLoadedSymbols;
		}

		void EnterCallback(string name)
		{
			handlingCallback = true;
			debugger.TraceMessage("Callback: " + name);
		}
		
		void ExitCallback_Continue()
		{
			debugger.Continue();
			handlingCallback = false;
		}
		
		void ExitCallback_Paused(PausedReason reason)
		{
			if (debugger.CurrentThread != null) {
				debugger.CurrentThread.DeactivateAllSteppers();
			}
			if (reason != PausedReason.EvalComplete) {
				debugger.OnIsProcessRunningChanged();
				debugger.OnDebuggingPaused(reason);
			}
			handlingCallback = false;
		}
		
		
		#region Program folow control

		public void StepComplete(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugStepper pStepper, CorDebugStepReason reason)
		{
			EnterCallback("StepComplete (" + reason.ToString() + ")", pThread);

			if (debugger.CurrentThread.LastFunction.Module.SymbolsLoaded == false) {
				debugger.TraceMessage(" - leaving code without symbols");

				ExitCallback_Continue();
			} else {
				
				ExitCallback_Paused(PausedReason.StepComplete);
			}
		}
		
		public void Breakpoint(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, IntPtr pBreakpoint)
		{
			EnterCallback("Breakpoint", pThread);
			
			ExitCallback_Paused(PausedReason.Breakpoint);
			
			foreach (Breakpoint b in debugger.Breakpoints) {
				if (b.Equals(pBreakpoint)) {
					b.OnBreakpointHit();
				}
			}
		}
		
		public void BreakpointSetError(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugBreakpoint pBreakpoint, uint dwError)
		{
			EnterCallback("BreakpointSetError", pThread);
			
			ExitCallback_Continue();
		}
		
		public unsafe void Break(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread)
		{
			EnterCallback("Break", pThread);

			ExitCallback_Paused(PausedReason.Break);
		}

		public void ControlCTrap(ICorDebugProcess pProcess)
		{
			EnterCallback("ControlCTrap", pProcess);

			ExitCallback_Paused(PausedReason.ControlCTrap);
		}

		public unsafe void Exception(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, int unhandled)
		{
			EnterCallback("Exception", pThread);

			// Exception2 is used in .NET Framework 2.0

			/*if (!debugger.CatchHandledExceptions && (unhandled == 0)) {
				ExitCallback_Continue();
				return;
			}*/
			
			ExitCallback_Paused(PausedReason.Exception);
		}

		#endregion

		#region Various

		public void LogSwitch(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, int lLevel, uint ulReason, string pLogSwitchName, string pParentName)
		{
			EnterCallback("LogSwitch", pThread);

			ExitCallback_Continue();
		}

		public void EvalException(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugEval pEval)
		{
			EnterCallback("EvalException", pThread);

			ExitCallback_Continue();
		}

		public void LogMessage(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, int lLevel, string pLogSwitchName, string pMessage)
		{
			EnterCallback("LogMessage", pThread);

			debugger.OnLogMessage(pMessage);

			ExitCallback_Continue();
		}

		public void EditAndContinueRemap(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugFunction pFunction, int fAccurate)
		{
			EnterCallback("EditAndContinueRemap", pThread);

			ExitCallback_Continue();
		}

		public void EvalComplete(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugEval eval)
		{
			EnterCallback("EvalComplete", pThread);
			
			if (CorDebugEvalCompleted != null) {
				CorDebugEvalCompleted(this, new CorDebugEvalEventArgs(debugger, eval));
			}
			
			ExitCallback_Paused(PausedReason.EvalComplete);
		}

		public void DebuggerError(ICorDebugProcess pProcess, int errorHR, uint errorCode)
		{
			EnterCallback("DebuggerError", pProcess);

			System.Windows.Forms.MessageBox.Show("Debugger error: \nHR = " + errorHR.ToString() + "\nCode = " + errorCode.ToString());

			ExitCallback_Paused(PausedReason.DebuggerError);
		}

		public void UpdateModuleSymbols(ICorDebugAppDomain pAppDomain, ICorDebugModule pModule, DebuggerInterop.Core.IStream pSymbolStream)
		{
			EnterCallback("UpdateModuleSymbols", pAppDomain);

			ExitCallback_Continue();
		}

		#endregion

		#region Start of Application

		public void CreateProcess(ICorDebugProcess pProcess)
		{
			EnterCallback("CreateProcess", pProcess);

			// Process is added in NDebugger.Start

			ExitCallback_Continue();
		}

		public void CreateAppDomain(ICorDebugProcess pProcess, ICorDebugAppDomain pAppDomain)
		{
			EnterCallback("CreateAppDomain", pAppDomain);

			pAppDomain.Attach();

			ExitCallback_Continue();
		}

		public void LoadAssembly(ICorDebugAppDomain pAppDomain, ICorDebugAssembly pAssembly)
		{
			EnterCallback("LoadAssembly", pAppDomain);

			ExitCallback_Continue();
		}

		public unsafe void LoadModule(ICorDebugAppDomain pAppDomain, ICorDebugModule pModule)
		{
			EnterCallback("LoadModule", pAppDomain);

			debugger.AddModule(pModule);

			ExitCallback_Continue();
		}

		public void NameChange(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread)
		{
			if (pAppDomain != null)	{
				EnterCallback("NameChange: pAppDomain", pAppDomain);
				ExitCallback_Continue();
				return;
			}
			if (pThread != null) {
				EnterCallback("NameChange: pThread", pThread);
				Thread thread = debugger.GetThread(pThread);
				thread.HasBeenLoaded = true;
				ExitCallback_Continue();
				return;
			}
		}

		public void CreateThread(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread)
		{
			// We can not use pThread since it has not been added yet
			// and we continue from this callback anyway
			EnterCallback("CreateThread", pAppDomain);

			debugger.AddThread(pThread);

			ExitCallback_Continue();
		}

		public void LoadClass(ICorDebugAppDomain pAppDomain, ICorDebugClass c)
		{
			EnterCallback("LoadClass", pAppDomain);

			ExitCallback_Continue();
		}

		#endregion

		#region Exit of Application

		public void UnloadClass(ICorDebugAppDomain pAppDomain, ICorDebugClass c)
		{
			EnterCallback("UnloadClass", pAppDomain);

			ExitCallback_Continue();
		}

		public void UnloadModule(ICorDebugAppDomain pAppDomain, ICorDebugModule pModule)
		{
			EnterCallback("UnloadModule", pAppDomain);

			debugger.RemoveModule(pModule);

			ExitCallback_Continue();
		}

		public void UnloadAssembly(ICorDebugAppDomain pAppDomain, ICorDebugAssembly pAssembly)
		{
			EnterCallback("UnloadAssembly", pAppDomain);

			ExitCallback_Continue();
		}

		public void ExitThread(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread)
		{
			EnterCallback("ExitThread", pThread);

			Thread thread = debugger.GetThread(pThread);

			debugger.RemoveThread(thread);

			if (thread.Process.CurrentThread == thread) {
				thread.Process.CurrentThread = null;
			}

			ExitCallback_Continue();
		}

		public void ExitAppDomain(ICorDebugProcess pProcess, ICorDebugAppDomain pAppDomain)
		{
			EnterCallback("ExitAppDomain", pAppDomain);

			ExitCallback_Continue();
		}

		public void ExitProcess(ICorDebugProcess pProcess)
		{
			EnterCallback("ExitProcess", pProcess);

			Process process = debugger.GetProcess(pProcess);

			debugger.RemoveProcess(process);

			if (debugger.CurrentProcess == process) {
				debugger.CurrentProcess = null;
			}

			if (debugger.Processes.Count == 0) {
				debugger.ResetEnvironment();
			}
		}

		#endregion

		#region ICorDebugManagedCallback2 Members

		public void ChangeConnection(ICorDebugProcess pProcess, uint dwConnectionId)
		{
			EnterCallback("ChangeConnection", pProcess);
			
			ExitCallback_Continue();
		}

		public void CreateConnection(ICorDebugProcess pProcess, uint dwConnectionId, ref ushort pConnName)
		{
			EnterCallback("CreateConnection", pProcess);
			
			ExitCallback_Continue();
		}

		public void DestroyConnection(ICorDebugProcess pProcess, uint dwConnectionId)
		{
			EnterCallback("DestroyConnection", pProcess);
			
			ExitCallback_Continue();
		}

		public void Exception2(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugFrame pFrame, uint nOffset, CorDebugExceptionCallbackType dwEventType, uint dwFlags)
		{
			EnterCallback("Exception2", pThread);
			
			//if (!NDebugger.CatchHandledExceptions && dwEventType != CorDebugExceptionCallbackType.DEBUG_EXCEPTION_UNHANDLED) {
			//	ExitCallback_Continue();
			//	return;
			//}

			debugger.CurrentThread.CurrentExceptionType = (ExceptionType)dwEventType;

			ExitCallback_Paused(PausedReason.Exception);
		}

		public void ExceptionUnwind(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, CorDebugExceptionUnwindCallbackType dwEventType, uint dwFlags)
		{
			EnterCallback("ExceptionUnwind", pThread);
			
			ExitCallback_Continue();
		}

		public void FunctionRemapComplete(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugFunction pFunction)
		{
			EnterCallback("FunctionRemapComplete", pThread);
			
			ExitCallback_Continue();
		}

		public void FunctionRemapOpportunity(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugFunction pOldFunction, ICorDebugFunction pNewFunction, uint oldILOffset)
		{
			EnterCallback("FunctionRemapOpportunity", pThread);
			
			ExitCallback_Continue();
		}

		public void MDANotification(ICorDebugController c, ICorDebugThread t, ICorDebugMDA mda)
		{
			EnterCallback("MDANotification");
			
			ExitCallback_Continue();
		}

		#endregion
	}
}
