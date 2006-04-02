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
		
		Process selectedProcess;
		
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
		
		// HACK: should not be public
		public virtual void OnDebuggeeStateChanged()
		{
			TraceMessage ("Debugger event: OnDebuggeeStateChanged (" + PausedReason.ToString() + ")");
			if (DebuggeeStateChanged != null) {
				DebuggeeStateChanged(this, new DebuggerEventArgs(this));
			}
		}
		
		public Process SelectedProcess {
			get {
				return selectedProcess;
			}
			set {
				selectedProcess = value;
			}
		}
		
		public Thread SelectedThread {
			get {
				if (SelectedProcess == null) {
					return null;
				} else {
					return SelectedProcess.SelectedThread;
				}
			}
		}
		
		public Function SelectedFunction {
			get {
				if (SelectedThread == null) {
					return null;
				} else {
					return SelectedThread.SelectedFunction;
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
			private set {
				debugeeState = value;
				OnDebuggeeStateChanged();
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
			if (PausedReason != PausedReason.EvalComplete) {
				DebugeeState = new DebugeeState();
			}
			
			OnDebuggingPaused();
			
			// Debugger state is unknown after calling OnDebuggingPaused (it may be resumed)
			
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
			SelectedFunction.StepInto();
		}

		public void StepOver()
		{
			SelectedFunction.StepOver();
		}

		public void StepOut()
		{
			SelectedFunction.StepOut();
		}

		public void Continue()
		{
			SelectedProcess.Continue();
		}

		public void Terminate()
		{
			foreach(Process p in Processes) {
				p.Terminate();
			}
		}
	}
}
