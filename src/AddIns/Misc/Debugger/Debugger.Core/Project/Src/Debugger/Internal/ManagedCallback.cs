// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
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

using Debugger.Wrappers.CorDebug;

namespace Debugger
{
	class ManagedCallback
	{
		NDebugger debugger;

		bool handlingCallback = false;
		
		Process callingProcess;
		Thread callingThread;
		
		public NDebugger Debugger {
			get {
				return debugger;
			}
		}

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

			callingProcess = debugger.GetProcess(pProcess);
			callingProcess.IsRunning = false;
		}

		// Sets CurrentProcess
		void EnterCallback(string name, ICorDebugAppDomain pAppDomain)
		{
			EnterCallback(name);
			
			callingProcess = debugger.GetProcess(pAppDomain.Process);
			callingProcess.IsRunning = false;
		}
		
		void EnterCallback(string name, ICorDebugThread pThread)
		{
			EnterCallback(name);
			
			callingThread = debugger.GetThread(pThread);
			
			callingProcess = callingThread.Process;
			callingProcess.IsRunning = false;
		}
		
		void EnterCallback(string name)
		{
			handlingCallback = true;
			debugger.TraceMessage("Callback: " + name);
			// ExitProcess may be called at any time when debuggee is killed
			if (name != "ExitProcess") {
				debugger.AssertRunning();
			}
		}
		
		void ExitCallback_Continue()
		{
			callingProcess.ContinueCallback();
			
			callingThread = null;
			callingProcess = null;
			handlingCallback = false;
		}
		
		void ExitCallback_Paused(PausedReason reason)
		{
			if (callingThread != null) {
				callingThread.DeactivateAllSteppers();
			}

			handlingCallback = false;

			debugger.Pause(reason, callingProcess, callingThread, null);
			
			callingThread = null;
			callingProcess = null;
		}
		
		
		#region Program folow control
		
		public void StepComplete(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugStepper pStepper, CorDebugStepReason reason)
		{
			EnterCallback("StepComplete (" + reason.ToString() + ")", pThread);
			
			Stepper stepper = debugger.GetThread(pThread).GetStepper(pStepper);
			if (stepper != null) {
				stepper.OnStepComplete();
				if (!stepper.PauseWhenComplete) {
					ExitCallback_Continue();
					return;
				}
			}
			
			if (!callingThread.LastFunction.HasSymbols) {
				// This should not happen with JMC enabled
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
			// Exception2 is used in .NET Framework 2.0
			
			if (debugger.DebuggeeVersion.StartsWith("v1.")) {
				// Forward the call to Exception2, which handles EnterCallback and ExitCallback
				ExceptionType exceptionType = (unhandled != 0)?ExceptionType.DEBUG_EXCEPTION_UNHANDLED:ExceptionType.DEBUG_EXCEPTION_FIRST_CHANCE;
				Exception2(pAppDomain, pThread, null, 0, (CorDebugExceptionCallbackType)exceptionType, 0);
			} else {
				// This callback should be ignored in v2 applications
				EnterCallback("Exception", pThread);
	
				ExitCallback_Continue();	
			}
		}

		#endregion

		#region Various

		public void LogSwitch(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, int lLevel, uint ulReason, string pLogSwitchName, string pParentName)
		{
			EnterCallback("LogSwitch", pThread);

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
		
		public void EvalException(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugEval corEval)
		{
			EnterCallback("EvalException", pThread);
			
			HandleEvalComplete(pAppDomain, pThread, corEval, true);
		}
		
		public void EvalComplete(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugEval corEval)
		{
			EnterCallback("EvalComplete", pThread);
			
			HandleEvalComplete(pAppDomain, pThread, corEval, false);			
		}
		
		void HandleEvalComplete(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugEval corEval, bool exception)
		{
			// Let the eval know it that the CorEval has finished
			// this will also remove the eval form PendingEvals collection
			Eval eval = debugger.GetEval(corEval);
			if (eval != null) {
				eval.OnEvalComplete(!exception);
			}
			
			if (debugger.PendingEvals.Count > 0) {
				debugger.SetupNextEvaluation(debugger.GetThread(pThread));
				ExitCallback_Continue();
			} else {
				ExitCallback_Paused(PausedReason.AllEvalsComplete);
			}
		}
		
		public void DebuggerError(ICorDebugProcess pProcess, int errorHR, uint errorCode)
		{
			EnterCallback("DebuggerError", pProcess);

			string errorText = String.Format("Debugger error: \nHR = 0x{0:X} \nCode = 0x{1:X}", errorHR, errorCode);
			System.Windows.Forms.MessageBox.Show(errorText);

			ExitCallback_Paused(PausedReason.DebuggerError);
		}

		public void UpdateModuleSymbols(ICorDebugAppDomain pAppDomain, ICorDebugModule pModule, Debugger.Interop.CorDebug.IStream pSymbolStream)
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

			}
			if (pThread != null) {

				EnterCallback("NameChange: pThread", pThread);

				Thread thread = debugger.GetThread(pThread);
				thread.HasBeenLoaded = true;

				ExitCallback_Continue();

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
			
			if (debugger.Processes.Count == 0) {
				// Exit callback and then terminate the debugger
				debugger.MTA2STA.AsyncCall( delegate { debugger.TerminateDebugger(); } );
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

		public void Exception2(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugFrame pFrame, uint nOffset, CorDebugExceptionCallbackType exceptionType, uint dwFlags)
		{
			EnterCallback("Exception2", pThread);
			
			// This callback is also called from Exception(...)!!!! (the .NET 1.1 version)
			// Whatch out for the zeros and null!
			// Exception -> Exception2(pAppDomain, pThread, null, 0, exceptionType, 0);
			
			callingThread.CurrentExceptionType = (ExceptionType)exceptionType;
			
			if (ExceptionType.DEBUG_EXCEPTION_UNHANDLED != (ExceptionType)exceptionType) {
				// Handled exception
				if (debugger.PauseOnHandledException) {
					ExitCallback_Paused(PausedReason.Exception);
				} else {
					ExitCallback_Continue();					
				}
			} else {
				// Unhandled exception				
				ExitCallback_Paused(PausedReason.Exception);
			}
		}

		public void ExceptionUnwind(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, CorDebugExceptionUnwindCallbackType dwEventType, uint dwFlags)
		{
			EnterCallback("ExceptionUnwind", pThread);
			
			if (dwEventType == CorDebugExceptionUnwindCallbackType.DEBUG_EXCEPTION_INTERCEPTED) {
				ExitCallback_Paused(PausedReason.ExceptionIntercepted);
			} else {
				ExitCallback_Continue();
			}
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
			if (c.Is<ICorDebugAppDomain>()) {
				EnterCallback("MDANotification", c.CastTo<ICorDebugAppDomain>());
			} else if (c.Is<ICorDebugProcess>()){
				EnterCallback("MDANotification", c.CastTo<ICorDebugProcess>());
			} else {
				throw new System.Exception("Unknown callback argument");
			}
			
			ExitCallback_Continue();
		}

		#endregion
	}
}
