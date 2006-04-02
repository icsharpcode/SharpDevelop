// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

using Debugger.Wrappers.CorDebug;

namespace Debugger
{
	public partial class NDebugger
	{
		bool pauseOnHandledException = false;
		ManualResetEvent pausedHandle = new ManualResetEvent(false);
		
		PauseSession pauseSession;
		DebugeeState debugeeState;
		
		Process currentProcess;
		
		public event EventHandler<DebuggerEventArgs> DebuggingResumed;
		public event EventHandler<DebuggingPausedEventArgs> DebuggingPaused;
		public event EventHandler<DebuggerEventArgs> DebuggeeStateChanged;
		
		public bool PauseOnHandledException {
			get {
				return pauseOnHandledException;
			}
			set {
				pauseOnHandledException = value;
			}
		}
		
		protected virtual void OnDebuggingResumed()
		{
			TraceMessage ("Debugger event: OnDebuggingResumed()");
			if (DebuggingResumed != null) {
				DebuggingResumed(this, new DebuggerEventArgs(this));
			}
		}
		
		protected virtual void OnDebuggingPaused()
		{
			TraceMessage ("Debugger event: OnDebuggingPaused (" + PausedReason.ToString() + ")");
			if (DebuggingPaused != null) {
				DebuggingPausedEventArgs args = new DebuggingPausedEventArgs(this, PausedReason);
				DebuggingPaused(this, args);
				if (args.ResumeDebugging) {
					Continue();
				}
			}
		}
		
		protected virtual void OnDebuggeeStateChanged()
		{
			TraceMessage ("Debugger event: OnDebuggeeStateChanged (" + PausedReason.ToString() + ")");
			if (DebuggeeStateChanged != null) {
				DebuggeeStateChanged(this, new DebuggerEventArgs(this));
			}
		}
		
		public Process CurrentProcess {
			get {
				if (IsRunning) return null;
				if (currentProcess == null) {
					return null;
				} else {
					return currentProcess;
				}
			}
			set {
				currentProcess = value;
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
		
		/// <summary>
		/// Indentification of the current debugger session. This value changes whenever debugger is continued
		/// </summary>
		public PauseSession PauseSession {
			get {
				return pauseSession;
			}
			internal set {
				pauseSession = value;
			}
		}
		
		/// <summary>
		/// Indentification of the state of the debugee. This value changes whenever the state of the debugee significatntly changes
		/// </summary>
		public DebugeeState DebugeeState {
			get {
				return debugeeState;
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
				return (pauseSession != null);
			}
		}
		
		public bool IsRunning {
			get {
				return (pauseSession == null);
			}
		}
		
		/// <summary>
		/// The reason why the debugger is paused.
		/// Thows an DebuggerException if debugger is not paused.
		/// </summary>
		public PausedReason PausedReason {
			get {
				AssertPaused();
				return pauseSession.PausedReason;
			}
		}
		
		internal void Pause()
		{
			OnDebuggingPaused();
			
			// Debugger state is unknown after calling OnDebuggingPaused (it may be resumed)
			
			if (IsPaused) {
				if (PausedReason != PausedReason.EvalComplete) {
					debugeeState = new DebugeeState();
					OnDebuggeeStateChanged();
				}
			}
			
			if (IsPaused) {
				pausedHandle.Set();
			}
		}
		
		internal void Resume()
		{
			if (IsRunning) {
				throw new DebuggerException("Already resumed");
			}
			
			OnDebuggingResumed();
			
			pausedHandle.Reset();
			
			pauseSession = null;
			
			currentProcess = null;
		}
		
		/// <summary>
		/// Waits until the debugger pauses unless it is already paused.
		/// Use PausedReason to find out why it paused.
		/// </summary>
		public void WaitForPause()
		{
			if (this.MTA2STA.SoftWait(PausedHandle, noProcessesHandle) == 1) {
				throw new DebuggerException("Process exited before pausing");
			}
		}
		
		/// <summary>
		/// Waits until all debugged precesses exit. Returns imideately if there are no running processes.
		/// </summary>
		public void WaitForPrecessExit()
		{
			this.MTA2STA.SoftWait(noProcessesHandle);
		}
		
		/// <summary>
		/// Wait handle, which will be set as long as the debugger is paused
		/// </summary>
		public WaitHandle PausedHandle {
			get {
				return pausedHandle;
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
