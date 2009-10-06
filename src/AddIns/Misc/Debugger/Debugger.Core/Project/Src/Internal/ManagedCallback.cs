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
using System.Runtime.InteropServices;
using Debugger.Wrappers.CorDebug;

namespace Debugger
{
	/// <summary>
	/// Handles all callbacks of a given process
	/// </summary>
	class ManagedCallback
	{
		Process process;
		bool pauseOnNextExit;
		bool isInCallback = false;
		
		[Debugger.Tests.Ignore]
		public Process Process {
			get {
				return process;
			}
		}
		
		public bool IsInCallback {
			get { return isInCallback; }
		}
		
		public ManagedCallback(Process process)
		{
			this.process = process;
		}
		
		void EnterCallback(PausedReason pausedReason, string name, ICorDebugProcess pProcess)
		{
			isInCallback = true;
			
			process.TraceMessage("Callback: " + name);
			System.Diagnostics.Debug.Assert(process.CorProcess == pProcess);
			
			// After break is pressed we may receive some messages that were already queued
			if (process.IsPaused && process.PauseSession.PausedReason == PausedReason.ForcedBreak) {
				process.TraceMessage("Processing post-break callback");
				// This compensates for the break call and we are in normal callback handling mode
				process.AsyncContinue(DebuggeeStateAction.Keep);
				// Start of call back - create new pause session (as usual)
				process.NotifyPaused(pausedReason);
				// Make sure we stay pause after the callback is handled
				pauseOnNextExit = true;
				return;
			}
			
			if (process.IsRunning) {
				process.NotifyPaused(pausedReason);
				return;
			}
			
			throw new DebuggerException("Invalid state at the start of callback");
		}
		
		void EnterCallback(PausedReason pausedReason, string name, ICorDebugAppDomain pAppDomain)
		{
			EnterCallback(pausedReason, name, pAppDomain.Process);
		}
		
		void EnterCallback(PausedReason pausedReason, string name, ICorDebugThread pThread)
		{
			EnterCallback(pausedReason, name, pThread.Process);
			process.SelectedThread = process.GetThread(pThread);
		}
		
		void ExitCallback()
		{
			bool hasQueuedCallbacks = process.CorProcess.HasQueuedCallbacks();
			if (hasQueuedCallbacks) {
				process.TraceMessage("Process has queued callbacks");
			}
			
			// Ignore events during property evaluation
			if (!pauseOnNextExit || process.Evaluating || hasQueuedCallbacks) {
				process.AsyncContinue(DebuggeeStateAction.Keep);
			} else {
				if (process.Options.Verbose) {
					process.TraceMessage("Callback exit: Paused");
				}
				pauseOnNextExit = false;
				Pause();
			}
			
			isInCallback = false;
		}
		
		void Pause()
		{
			if (process.PauseSession.PausedReason == PausedReason.EvalComplete ||
			    process.PauseSession.PausedReason == PausedReason.ExceptionIntercepted) {
				process.DisableAllSteppers();
				process.CheckSelectedStackFrames();
				// Do not set selected stack frame
				// Do not raise events
			} else {
				// Raise the pause event outside the callback
				process.Debugger.MTA2STA.AsyncCall(process.RaisePausedEvents);
			}
		}
		
		
		#region Program folow control
		
		public void StepComplete(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugStepper pStepper, CorDebugStepReason reason)
		{
			EnterCallback(PausedReason.StepComplete, "StepComplete (" + reason.ToString() + ")", pThread);
			
			Thread thread = process.GetThread(pThread);
			Stepper stepper = thread.GetStepper(pStepper);
			
			StackFrame currentStackFrame = process.SelectedThread.MostRecentStackFrame;
			process.TraceMessage(" - stopped at {0} because of {1}", currentStackFrame.MethodInfo.FullName, stepper.ToString());
			
			thread.Steppers.Remove(stepper);
			stepper.OnStepComplete(reason);
			
			if (stepper.Ignore) {
				// The stepper is ignored
				process.TraceMessage(" - ignored");
			} else if (thread.CurrentStepIn != null &&
				       thread.CurrentStepIn.StackFrame.Equals(currentStackFrame) &&
			           thread.CurrentStepIn.IsInStepRanges((int)currentStackFrame.CorInstructionPtr)) {
				Stepper.StepIn(currentStackFrame, thread.CurrentStepIn.StepRanges, "finishing step in");
				process.TraceMessage(" - finishing step in");
			} else if (currentStackFrame.MethodInfo.StepOver) {
				if (process.Options.EnableJustMyCode) {
					currentStackFrame.MethodInfo.MarkAsNonUserCode();
					process.TraceMessage(" - method {0} marked as non user code", currentStackFrame.MethodInfo.FullName);
					Stepper.StepIn(currentStackFrame, new int[] {0, int.MaxValue}, "seeking user code");
					process.TraceMessage(" - seeking user code");
				} else {
					Stepper.StepOut(currentStackFrame, "stepping out of non-user code");
					process.TraceMessage(" - stepping out of non-user code");
				}
			} else {
				// User-code method
				pauseOnNextExit = true;
				process.TraceMessage(" - pausing in user code");
			}
			
			ExitCallback();
		}
		
		// Warning! Marshaing of ICorBreakpoint fails in .NET 1.1
		public void Breakpoint(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugBreakpoint corBreakpoint)
		{
			EnterCallback(PausedReason.Breakpoint, "Breakpoint", pThread);
			
			Breakpoint breakpoint = process.Debugger.GetBreakpoint(corBreakpoint);
			// The event will be risen outside the callback
			process.BreakpointHitEventQueue.Enqueue(breakpoint);
			
			pauseOnNextExit = true;
			
			ExitCallback();
		}
		
		public void BreakpointSetError(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugBreakpoint pBreakpoint, uint dwError)
		{
			EnterCallback(PausedReason.Other, "BreakpointSetError", pThread);
			
			ExitCallback();
		}
		
		public void Break(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread)
		{
			EnterCallback(PausedReason.Break, "Break", pThread);

			pauseOnNextExit = true;
			ExitCallback();
		}

		public void ControlCTrap(ICorDebugProcess pProcess)
		{
			EnterCallback(PausedReason.ControlCTrap, "ControlCTrap", pProcess);

			pauseOnNextExit = true;
			ExitCallback();
		}

		public void Exception(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, int unhandled)
		{
			// Exception2 is used in .NET Framework 2.0
			
			if (process.DebuggeeVersion.StartsWith("v1.")) {
				// Forward the call to Exception2, which handles EnterCallback and ExitCallback
				ExceptionType exceptionType = (unhandled != 0) ? ExceptionType.Unhandled : ExceptionType.FirstChance;
				Exception2(pAppDomain, pThread, null, 0, (CorDebugExceptionCallbackType)exceptionType, 0);
			} else {
				// This callback should be ignored in v2 applications
				EnterCallback(PausedReason.Other, "Exception", pThread);
	
				ExitCallback();	
			}
		}

		#endregion

		#region Various

		public void LogSwitch(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, int lLevel, uint ulReason, string pLogSwitchName, string pParentName)
		{
			EnterCallback(PausedReason.Other, "LogSwitch", pThread);

			ExitCallback();
		}
		
		public void LogMessage(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, int lLevel, string pLogSwitchName, string pMessage)
		{
			EnterCallback(PausedReason.Other, "LogMessage", pThread);

			process.OnLogMessage(new MessageEventArgs(process, lLevel, pMessage, pLogSwitchName));

			ExitCallback();
		}

		public void EditAndContinueRemap(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugFunction pFunction, int fAccurate)
		{
			EnterCallback(PausedReason.Other, "EditAndContinueRemap", pThread);

			ExitCallback();
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
			// Let the eval know that the CorEval has finished
			Eval eval = process.GetEval(corEval);
			eval.NotifyEvaluationComplete(!exception);
			process.NotifyEvaluationComplete(eval);
			
			pauseOnNextExit = true;
			ExitCallback();
		}
		
		public void DebuggerError(ICorDebugProcess pProcess, int errorHR, uint errorCode)
		{
			EnterCallback(PausedReason.DebuggerError, "DebuggerError", pProcess);

			string errorText = String.Format("Debugger error: \nHR = 0x{0:X} \nCode = 0x{1:X}", errorHR, errorCode);
			
			if ((uint)errorHR == 0x80131C30) {
				errorText += "\n\nDebugging 64-bit processes is currently not supported.\n" +
					"If you are running a 64-bit system, this setting might help:\n" +
					"Project -> Project Options -> Compiling -> Target CPU = 32-bit Intel";
			}
			
			if (Environment.UserInteractive)
				System.Windows.Forms.MessageBox.Show(errorText);
			else
				throw new DebuggerException(errorText);

			try {
				pauseOnNextExit = true;
				ExitCallback();
			} catch (COMException) {
			} catch (InvalidComObjectException) {
				// ignore errors during shutdown after debugger error
			}
		}

		public void UpdateModuleSymbols(ICorDebugAppDomain pAppDomain, ICorDebugModule pModule, IStream pSymbolStream)
		{
			EnterCallback(PausedReason.Other, "UpdateModuleSymbols", pAppDomain);
			
			foreach (Module module in process.Modules) {
				if (module.CorModule == pModule) {
					process.TraceMessage("UpdateModuleSymbols: Found module: " + pModule.Name);
					module.UpdateSymbolsFromStream(pSymbolStream);
					process.Debugger.SetBreakpointsInModule(module);
					break;
				}
			}
			
			ExitCallback();
		}

		#endregion

		#region Start of Application

		public void CreateProcess(ICorDebugProcess pProcess)
		{
			EnterCallback(PausedReason.Other, "CreateProcess", pProcess);

			// Process is added in NDebugger.Start

			ExitCallback();
		}

		public void CreateAppDomain(ICorDebugProcess pProcess, ICorDebugAppDomain pAppDomain)
		{
			EnterCallback(PausedReason.Other, "CreateAppDomain", pAppDomain);

			pAppDomain.Attach();

			ExitCallback();
		}

		public void LoadAssembly(ICorDebugAppDomain pAppDomain, ICorDebugAssembly pAssembly)
		{
			EnterCallback(PausedReason.Other, "LoadAssembly", pAppDomain);

			ExitCallback();
		}

		public void LoadModule(ICorDebugAppDomain pAppDomain, ICorDebugModule pModule)
		{
			EnterCallback(PausedReason.Other, "LoadModule " + pModule.Name, pAppDomain);
			
			process.AddModule(pModule);
			
			ExitCallback();
		}
		
		public void NameChange(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread)
		{
			if (pAppDomain != null) {
				
				EnterCallback(PausedReason.Other, "NameChange: pAppDomain", pAppDomain);
				
				ExitCallback();
				
			}
			if (pThread != null) {
				
				EnterCallback(PausedReason.Other, "NameChange: pThread", pThread);
				
				Thread thread = process.GetThread(pThread);
				thread.NotifyNameChanged();
				
				ExitCallback();
				
			}
		}
		
		public void CreateThread(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread)
		{
			// We can not use pThread since it has not been added yet
			// and we continue from this callback anyway
			EnterCallback(PausedReason.Other, "CreateThread " + pThread.ID, pAppDomain);
			
			process.AddThread(pThread);
			
			ExitCallback();
		}
		
		public void LoadClass(ICorDebugAppDomain pAppDomain, ICorDebugClass c)
		{
			EnterCallback(PausedReason.Other, "LoadClass", pAppDomain);
			
			ExitCallback();
		}
		
		#endregion
		
		#region Exit of Application
		
		public void UnloadClass(ICorDebugAppDomain pAppDomain, ICorDebugClass c)
		{
			EnterCallback(PausedReason.Other, "UnloadClass", pAppDomain);
			
			ExitCallback();
		}
		
		public void UnloadModule(ICorDebugAppDomain pAppDomain, ICorDebugModule pModule)
		{
			EnterCallback(PausedReason.Other, "UnloadModule", pAppDomain);
			
			process.RemoveModule(pModule);
			
			ExitCallback();
		}
		
		public void UnloadAssembly(ICorDebugAppDomain pAppDomain, ICorDebugAssembly pAssembly)
		{
			EnterCallback(PausedReason.Other, "UnloadAssembly", pAppDomain);
			
			ExitCallback();
		}
		
		public void ExitThread(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread)
		{
			// It seems that ICorDebugThread is still not dead and can be used
			EnterCallback(PausedReason.Other, "ExitThread " + pThread.ID, pThread);
			
			process.GetThread(pThread).NotifyExited();
			
			try {
				ExitCallback();
			} catch (COMException e) {
				// For some reason this sometimes happens in .NET 1.1
				process.TraceMessage("Continue failed in ExitThread callback: " + e.Message);
			}
		}
		
		public void ExitAppDomain(ICorDebugProcess pProcess, ICorDebugAppDomain pAppDomain)
		{
			EnterCallback(PausedReason.Other, "ExitAppDomain", pAppDomain);
			
			ExitCallback();
		}
		
		public void ExitProcess(ICorDebugProcess pProcess)
		{
			// ExitProcess may be called at any time when debuggee is killed
			process.TraceMessage("Callback: ExitProcess");
			
			process.NotifyHasExited();
		}
		
		#endregion
		
		#region ICorDebugManagedCallback2 Members
		
		public void ChangeConnection(ICorDebugProcess pProcess, uint dwConnectionId)
		{
			EnterCallback(PausedReason.Other, "ChangeConnection", pProcess);
			
			ExitCallback();
		}

		public void CreateConnection(ICorDebugProcess pProcess, uint dwConnectionId, IntPtr pConnName)
		{
			EnterCallback(PausedReason.Other, "CreateConnection", pProcess);
			
			ExitCallback();
		}

		public void DestroyConnection(ICorDebugProcess pProcess, uint dwConnectionId)
		{
			EnterCallback(PausedReason.Other, "DestroyConnection", pProcess);
			
			ExitCallback();
		}

		public void Exception2(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugFrame pFrame, uint nOffset, CorDebugExceptionCallbackType exceptionType, uint dwFlags)
		{
			EnterCallback(PausedReason.Exception, "Exception2 (type=" + exceptionType.ToString() + ")", pThread);
			
			// This callback is also called from Exception(...)!!!! (the .NET 1.1 version)
			// Watch out for the zeros and null!
			// Exception -> Exception2(pAppDomain, pThread, null, 0, exceptionType, 0);
			
			process.SelectedThread.CurrentException = new Exception(new Value(process, new Expressions.CurrentExceptionExpression(), process.SelectedThread.CorThread.CurrentException));
			process.SelectedThread.CurrentException_DebuggeeState = process.DebuggeeState;
			process.SelectedThread.CurrentExceptionType = (ExceptionType)exceptionType;
			process.SelectedThread.CurrentExceptionIsUnhandled = (ExceptionType)exceptionType == ExceptionType.Unhandled;
			
			if (process.SelectedThread.CurrentExceptionIsUnhandled ||
			    process.PauseOnHandledException) 
			{
				pauseOnNextExit = true;
			}
			ExitCallback();
		}

		public void ExceptionUnwind(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, CorDebugExceptionUnwindCallbackType dwEventType, uint dwFlags)
		{
			EnterCallback(PausedReason.ExceptionIntercepted, "ExceptionUnwind", pThread);
			
			if (dwEventType == CorDebugExceptionUnwindCallbackType.DEBUG_EXCEPTION_INTERCEPTED) {
				pauseOnNextExit = true;
			}
			ExitCallback();
		}

		public void FunctionRemapComplete(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugFunction pFunction)
		{
			EnterCallback(PausedReason.Other, "FunctionRemapComplete", pThread);
			
			ExitCallback();
		}

		public void FunctionRemapOpportunity(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugFunction pOldFunction, ICorDebugFunction pNewFunction, uint oldILOffset)
		{
			EnterCallback(PausedReason.Other, "FunctionRemapOpportunity", pThread);
			
			ExitCallback();
		}

	    /// <exception cref="Exception">Unknown callback argument</exception>
	    public void MDANotification(ICorDebugController c, ICorDebugThread t, ICorDebugMDA mda)
		{
			if (c.Is<ICorDebugAppDomain>()) {
				EnterCallback(PausedReason.Other, "MDANotification", c.CastTo<ICorDebugAppDomain>());
			} else if (c.Is<ICorDebugProcess>()){
				EnterCallback(PausedReason.Other, "MDANotification", c.CastTo<ICorDebugProcess>());
			} else {
				throw new System.Exception("Unknown callback argument");
			}
			
			ExitCallback();
		}

		#endregion
	}
}
