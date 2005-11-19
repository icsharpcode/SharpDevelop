// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

using Debugger.Interop.CorDebug;
using Debugger.Interop.MetaData;
using System.Collections.Generic;

namespace Debugger
{
	public partial class NDebugger
	{
		PausedReason? pausedReason = null;
		bool pauseOnHandledException = false;
		EventWaitHandle waitForPauseHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
		
		Process currentProcess;
		
		public event EventHandler<DebuggingPausedEventArgs> DebuggingPaused;
		public event EventHandler<DebuggerEventArgs> DebuggingResumed;
		
		public bool PauseOnHandledException {
			get {
				return pauseOnHandledException;
			}
			set {
				pauseOnHandledException = value;
			}
		}

		protected virtual void OnDebuggingPaused(PausedReason reason)
		{
			TraceMessage ("Debugger event: OnDebuggingPaused(" + reason.ToString() + ")");
			if (DebuggingPaused != null) {
				DebuggingPausedEventArgs args = new DebuggingPausedEventArgs(this, reason);
				DebuggingPaused(this, args);
				if (args.ResumeDebugging) {
					Continue();
				}
			}
		}

		protected virtual void OnDebuggingResumed()
		{
			TraceMessage ("Debugger event: OnDebuggingResumed()");
			if (DebuggingResumed != null) {
				DebuggingResumed(this, new DebuggerEventArgs(this));
			}
		}
		
		public Process CurrentProcess {
			get {
				if (IsRunning) return null;
				if (currentProcess == null) {
					return null;
				} else {
					if (currentProcess.IsRunning) return null;
					return currentProcess;
				}
			}
		}
		
		public Thread CurrentThread {
			get {
				if (IsRunning) return null;
				if (CurrentProcess == null) return null;
				return CurrentProcess.CurrentThread;
			}
		}
		
		public Function CurrentFunction {
			get {
				if (IsRunning) return null;
				if (CurrentThread == null) {
					return null;
				} else {
					return CurrentThread.CurrentFunction;
				}
			}
		}
		
		public void AssertPaused()
		{
			if (!IsPaused) {
				throw new DebuggerException("Debugger is not paused.");
			}
		}
		
		public void AssertRunning()
		{
			if (IsPaused) {
				throw new DebuggerException("Debugger is not running.");
			}
		}
		
		public bool IsPaused {
			get {
				return (pausedReason != null);
			}
		}
		
		public bool IsRunning {
			get {
				return (pausedReason == null);
			}
		}
		
		/// <summary>
		/// The reason why the debugger is paused.
		/// Thows an DebuggerException if debugger is not paused.
		/// </summary>
		public PausedReason PausedReason {
			get {
				AssertPaused();
				return (PausedReason)pausedReason;
			}
		}
		
		internal void Pause(PausedReason reason, Process process, Thread thread, Function function)
		{
			if (IsPaused) {
				throw new DebuggerException("Already paused");
			}
			if (process == null) {
				throw new DebuggerException("Process can not be null");
			}
			if (thread == null && function != null) {
				throw new DebuggerException("Function can not be set without thread");
			}
			
			currentProcess = process;
			currentProcess.CurrentThread = thread;
			if (currentProcess.CurrentThread != null) {
				currentProcess.CurrentThread.CurrentFunction = function;
			}
			
			pausedReason = reason;
			
			
			OnDebuggingPaused(reason);
			
			// Debugger state is unknown after calling OnDebuggingPaused (it may be resumed)
			// This is a good point to autmaticaly evaluate evals from update of variables (if there are any)
			if (IsPaused) {
				localVariables.Update();
				this.StartEvaluation();
				// Evaluation loop stoped by Function.GetPropertyVariables not adding evals
				// And PropertyVariable not setting new value
			}
			
			if (IsPaused) {
				waitForPauseHandle.Set();
			}
		}
		
		internal void FakePause(PausedReason reason, bool keepCurrentFunction)
		{
			Process process = CurrentProcess;
			Thread thread = CurrentThread;
			Function function = CurrentFunction;
			Resume();
			Pause(reason, process, thread, keepCurrentFunction ? function : null);
		}
		
		internal void Resume()
		{
			if (IsRunning) {
				throw new DebuggerException("Already resumed");
			}
			
			OnDebuggingResumed();
			waitForPauseHandle.Reset();
			
			pausedReason = null;
			
			// Remove all stored functions, they are disponsed between callbacks and they need to be regenerated
			foreach(Thread t in Threads) {
				t.CurrentFunction = null;
			}
			
			// Clear current process
			currentProcess = null;
		}
		
		/// <summary>
		/// Waits until the debugger pauses unless it is already paused.
		/// Use PausedReason to find out why it paused.
		/// </summary>
		public void WaitForPause()
		{
			if (IsRunning) {
				WaitForPauseHandle.WaitOne();
			}
		}
		
		/// <summary>
		/// Wait handle, which will be set as long as the debugger is paused
		/// </summary>
		public WaitHandle WaitForPauseHandle {
			get {
				return waitForPauseHandle;
			}
		}
		
		public void Break()
		{
			foreach(Process p in Processes) {
				if (p.IsRunning) {
					p.Break();
				}
			}
		}

		public void StepInto()
		{
			CurrentFunction.StepInto();
		}

		public void StepOver()
		{
			CurrentFunction.StepOver();
		}

		public void StepOut()
		{
			CurrentFunction.StepOut();
		}

		public void Continue()
		{
			CurrentProcess.Continue();
		}

		public void Terminate()
		{
			foreach(Process p in Processes) {
				p.Terminate();
			}
		}
	}
}
