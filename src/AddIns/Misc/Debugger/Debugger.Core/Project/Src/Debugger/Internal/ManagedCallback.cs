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
// \1\2\n\1{\n\1\tEnterCallback(PausedReason.Other, "\3");\n\1\t\n\1\tExitCallback_Continue();\n\1}

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Debugger.Wrappers.CorDebug;

namespace Debugger
{
	class ManagedCallback
	{
		NDebugger debugger;
		
		public NDebugger Debugger {
			get {
				return debugger;
			}
		}
		
		public ManagedCallback(NDebugger debugger)
		{
			this.debugger = debugger;
		}
		
		void EnterCallback(PausedReason pausedReason, string name, ICorDebugProcess pProcess)
		{
			debugger.TraceMessage("Callback: " + name);
			// ExitProcess may be called at any time when debuggee is killed
			if (name != "ExitProcess") debugger.AssertRunning();
			debugger.PauseSession = new PauseSession(pausedReason);
			debugger.SelectedProcess = debugger.GetProcess(pProcess);
			debugger.SelectedProcess.IsRunning = false;
		}
		
		void EnterCallback(PausedReason pausedReason, string name, ICorDebugAppDomain pAppDomain)
		{
			EnterCallback(pausedReason, name, pAppDomain.Process);
		}
		
		void EnterCallback(PausedReason pausedReason, string name, ICorDebugThread pThread)
		{
			debugger.TraceMessage("Callback: " + name);
			// ExitProcess may be called at any time when debuggee is killed
			if (name != "ExitProcess") debugger.AssertRunning();
			Thread thread = debugger.GetThread(pThread);
			debugger.PauseSession = new PauseSession(pausedReason);
			debugger.SelectedProcess = thread.Process;
			debugger.SelectedProcess.IsRunning = false;
			debugger.SelectedProcess.SelectedThread = thread;
		}
		
		void ExitCallback_Continue()
		{
			debugger.SelectedProcess.Continue();
		}
		
		void ExitCallback_Paused()
		{
			if (debugger.Evaluating) {
				// Ignore events during property evaluation
				ExitCallback_Continue();
			} else {
				if (debugger.SelectedThread != null) {
					// Disable all steppers - do not Deactivate since function tracking still needs them
					foreach(Stepper s in debugger.SelectedThread.Steppers) {
						s.PauseWhenComplete = false;
					}
					
					debugger.SelectedThread.SelectedFunction = debugger.SelectedThread.LastFunctionWithLoadedSymbols;
				}
				debugger.Pause();
			}
		}
		
		
		#region Program folow control
		
		public void StepComplete(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugStepper pStepper, CorDebugStepReason reason)
		{
			EnterCallback(PausedReason.StepComplete, "StepComplete (" + reason.ToString() + ")", pThread);
			
			Thread thread = debugger.GetThread(pThread);
			Stepper stepper = thread.GetStepper(pStepper);

			thread.Steppers.Remove(stepper);
			stepper.OnStepComplete();
			if (stepper.PauseWhenComplete) {
				if (debugger.SelectedThread.LastFunction.HasSymbols) {
					ExitCallback_Paused();
				} else {
					// This should not happen with JMC enabled
					debugger.TraceMessage(" - leaving code without symbols");
					
					ExitCallback_Continue();
				}
			} else {
				ExitCallback_Continue();
			}
		}
		
		// Do not pass the pBreakpoint parameter as ICorDebugBreakpoint - marshaling of it fails in .NET 1.1
		public void Breakpoint(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, IntPtr pBreakpoint)
		{
			EnterCallback(PausedReason.Breakpoint, "Breakpoint", pThread);
			
			ExitCallback_Paused();
			
//			foreach (Breakpoint b in debugger.Breakpoints) {
//				if (b.Equals(pBreakpoint)) {
//					// TODO: Check that this works
//					b.OnHit();
//				}
//			}
		}
		
		public void BreakpointSetError(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugBreakpoint pBreakpoint, uint dwError)
		{
			EnterCallback(PausedReason.Other, "BreakpointSetError", pThread);
			
			ExitCallback_Continue();
		}
		
		public unsafe void Break(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread)
		{
			EnterCallback(PausedReason.Break, "Break", pThread);

			ExitCallback_Paused();
		}

		public void ControlCTrap(ICorDebugProcess pProcess)
		{
			EnterCallback(PausedReason.ControlCTrap, "ControlCTrap", pProcess);

			ExitCallback_Paused();
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
				EnterCallback(PausedReason.Other, "Exception", pThread);
	
				ExitCallback_Continue();	
			}
		}

		#endregion

		#region Various

		public void LogSwitch(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, int lLevel, uint ulReason, string pLogSwitchName, string pParentName)
		{
			EnterCallback(PausedReason.Other, "LogSwitch", pThread);

			ExitCallback_Continue();
		}
		
		public void LogMessage(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, int lLevel, string pLogSwitchName, string pMessage)
		{
			EnterCallback(PausedReason.Other, "LogMessage", pThread);

			debugger.OnLogMessage(pMessage);

			ExitCallback_Continue();
		}

		public void EditAndContinueRemap(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugFunction pFunction, int fAccurate)
		{
			EnterCallback(PausedReason.Other, "EditAndContinueRemap", pThread);

			ExitCallback_Continue();
		}
		
		public void EvalException(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugEval corEval)
		{
			EnterCallback(PausedReason.EvalComplete, "EvalException", pThread);
			
			HandleEvalComplete(pAppDomain, pThread, corEval, true);
		}
		
		public void EvalComplete(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugEval corEval)
		{
			EnterCallback(PausedReason.EvalComplete, "EvalComplete", pThread);
			
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
				ExitCallback_Paused();
			}
		}
		
		public void DebuggerError(ICorDebugProcess pProcess, int errorHR, uint errorCode)
		{
			EnterCallback(PausedReason.DebuggerError, "DebuggerError", pProcess);

			string errorText = String.Format("Debugger error: \nHR = 0x{0:X} \nCode = 0x{1:X}", errorHR, errorCode);
			System.Windows.Forms.MessageBox.Show(errorText);

			ExitCallback_Paused();
		}

		public void UpdateModuleSymbols(ICorDebugAppDomain pAppDomain, ICorDebugModule pModule, IStream pSymbolStream)
		{
			EnterCallback(PausedReason.Other, "UpdateModuleSymbols", pAppDomain);

			ExitCallback_Continue();
		}

		#endregion

		#region Start of Application

		public void CreateProcess(ICorDebugProcess pProcess)
		{
			EnterCallback(PausedReason.Other, "CreateProcess", pProcess);

			// Process is added in NDebugger.Start

			ExitCallback_Continue();
		}

		public void CreateAppDomain(ICorDebugProcess pProcess, ICorDebugAppDomain pAppDomain)
		{
			EnterCallback(PausedReason.Other, "CreateAppDomain", pAppDomain);

			pAppDomain.Attach();

			ExitCallback_Continue();
		}

		public void LoadAssembly(ICorDebugAppDomain pAppDomain, ICorDebugAssembly pAssembly)
		{
			EnterCallback(PausedReason.Other, "LoadAssembly", pAppDomain);

			ExitCallback_Continue();
		}

		public unsafe void LoadModule(ICorDebugAppDomain pAppDomain, ICorDebugModule pModule)
		{
			EnterCallback(PausedReason.Other, "LoadModule", pAppDomain);

			debugger.AddModule(pModule);

			ExitCallback_Continue();
		}

		public void NameChange(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread)
		{
			if (pAppDomain != null)	{

				EnterCallback(PausedReason.Other, "NameChange: pAppDomain", pAppDomain);

				ExitCallback_Continue();

			}
			if (pThread != null) {

				EnterCallback(PausedReason.Other, "NameChange: pThread", pThread);

				Thread thread = debugger.GetThread(pThread);
				thread.HasBeenLoaded = true;

				ExitCallback_Continue();

			}
		}

		public void CreateThread(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread)
		{
			// We can not use pThread since it has not been added yet
			// and we continue from this callback anyway
			EnterCallback(PausedReason.Other, "CreateThread", pAppDomain);

			debugger.AddThread(pThread);

			ExitCallback_Continue();
		}

		public void LoadClass(ICorDebugAppDomain pAppDomain, ICorDebugClass c)
		{
			EnterCallback(PausedReason.Other, "LoadClass", pAppDomain);

			ExitCallback_Continue();
		}

		#endregion

		#region Exit of Application

		public void UnloadClass(ICorDebugAppDomain pAppDomain, ICorDebugClass c)
		{
			EnterCallback(PausedReason.Other, "UnloadClass", pAppDomain);

			ExitCallback_Continue();
		}

		public void UnloadModule(ICorDebugAppDomain pAppDomain, ICorDebugModule pModule)
		{
			EnterCallback(PausedReason.Other, "UnloadModule", pAppDomain);

			debugger.RemoveModule(pModule);

			ExitCallback_Continue();
		}

		public void UnloadAssembly(ICorDebugAppDomain pAppDomain, ICorDebugAssembly pAssembly)
		{
			EnterCallback(PausedReason.Other, "UnloadAssembly", pAppDomain);

			ExitCallback_Continue();
		}

		public void ExitThread(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread)
		{
			EnterCallback(PausedReason.Other, "ExitThread", pThread);

			Thread thread = debugger.GetThread(pThread);

			debugger.RemoveThread(thread);

			if (thread.Process.SelectedThread == thread) {
				thread.Process.SelectedThread = null;
			}

			try {
				ExitCallback_Continue();
			} catch (COMException e) {
				// For some reason this sometimes happens in .NET 1.1
				debugger.TraceMessage("Continue failed in ExitThread callback: " + e.Message);
			}
		}

		public void ExitAppDomain(ICorDebugProcess pProcess, ICorDebugAppDomain pAppDomain)
		{
			EnterCallback(PausedReason.Other, "ExitAppDomain", pAppDomain);

			ExitCallback_Continue();
		}
		
		public void ExitProcess(ICorDebugProcess pProcess)
		{
			EnterCallback(PausedReason.Other, "ExitProcess", pProcess);
			
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
			EnterCallback(PausedReason.Other, "ChangeConnection", pProcess);
			
			ExitCallback_Continue();
		}

		public void CreateConnection(ICorDebugProcess pProcess, uint dwConnectionId, IntPtr pConnName)
		{
			EnterCallback(PausedReason.Other, "CreateConnection", pProcess);
			
			ExitCallback_Continue();
		}

		public void DestroyConnection(ICorDebugProcess pProcess, uint dwConnectionId)
		{
			EnterCallback(PausedReason.Other, "DestroyConnection", pProcess);
			
			ExitCallback_Continue();
		}

		public void Exception2(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugFrame pFrame, uint nOffset, CorDebugExceptionCallbackType exceptionType, uint dwFlags)
		{
			EnterCallback(PausedReason.Exception, "Exception2", pThread);
			
			// This callback is also called from Exception(...)!!!! (the .NET 1.1 version)
			// Whatch out for the zeros and null!
			// Exception -> Exception2(pAppDomain, pThread, null, 0, exceptionType, 0);
			
			debugger.SelectedThread.CurrentExceptionType = (ExceptionType)exceptionType;
			
			if (ExceptionType.DEBUG_EXCEPTION_UNHANDLED != (ExceptionType)exceptionType) {
				// Handled exception
				if (debugger.PauseOnHandledException) {
					ExitCallback_Paused();
				} else {
					ExitCallback_Continue();					
				}
			} else {
				// Unhandled exception				
				ExitCallback_Paused();
			}
		}

		public void ExceptionUnwind(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, CorDebugExceptionUnwindCallbackType dwEventType, uint dwFlags)
		{
			EnterCallback(PausedReason.ExceptionIntercepted, "ExceptionUnwind", pThread);
			
			if (dwEventType == CorDebugExceptionUnwindCallbackType.DEBUG_EXCEPTION_INTERCEPTED) {
				ExitCallback_Paused();
			} else {
				ExitCallback_Continue();
			}
		}

		public void FunctionRemapComplete(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugFunction pFunction)
		{
			EnterCallback(PausedReason.Other, "FunctionRemapComplete", pThread);
			
			ExitCallback_Continue();
		}

		public void FunctionRemapOpportunity(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugFunction pOldFunction, ICorDebugFunction pNewFunction, uint oldILOffset)
		{
			EnterCallback(PausedReason.Other, "FunctionRemapOpportunity", pThread);
			
			ExitCallback_Continue();
		}

		public void MDANotification(ICorDebugController c, ICorDebugThread t, ICorDebugMDA mda)
		{
			if (c.Is<ICorDebugAppDomain>()) {
				EnterCallback(PausedReason.Other, "MDANotification", c.CastTo<ICorDebugAppDomain>());
			} else if (c.Is<ICorDebugProcess>()){
				EnterCallback(PausedReason.Other, "MDANotification", c.CastTo<ICorDebugProcess>());
			} else {
				throw new System.Exception("Unknown callback argument");
			}
			
			ExitCallback_Continue();
		}

		#endregion
	}
}
