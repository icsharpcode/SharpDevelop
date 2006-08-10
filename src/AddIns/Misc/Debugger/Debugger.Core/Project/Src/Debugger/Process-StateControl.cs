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
	public partial class Process
	{
		bool pauseOnHandledException = false;
		internal ManualResetEvent pausedHandle = new ManualResetEvent(false);
		
		DebugeeState debugeeState;
		
		Process selectedProcess;
		
		public event EventHandler<ExceptionEventArgs> ExceptionThrown;
		public event EventHandler<ProcessEventArgs> DebuggingResumed;
		public event EventHandler<ProcessEventArgs> DebuggingPaused;
		public event EventHandler<ProcessEventArgs> DebuggeeStateChanged;
		
		public bool PauseOnHandledException {
			get {
				return pauseOnHandledException;
			}
			set {
				pauseOnHandledException = value;
			}
		}
		
		protected virtual void OnExceptionThrown(ExceptionEventArgs e)
		{
			if (ExceptionThrown != null) {
				ExceptionThrown(this, e);
			}
		}
		
		internal virtual void OnDebuggingResumed()
		{
			TraceMessage ("Debugger event: OnDebuggingResumed()");
			if (DebuggingResumed != null) {
				DebuggingResumed(this, new ProcessEventArgs(this));
			}
		}
		
		protected virtual void OnDebuggingPaused()
		{
			TraceMessage ("Debugger event: OnDebuggingPaused (" + PausedReason.ToString() + ")");
			if (DebuggingPaused != null) {
				DebuggingPaused(this, new ProcessEventArgs(this));
			}
		}
		
		// HACK: should not be public
		public virtual void OnDebuggeeStateChanged()
		{
			TraceMessage ("Debugger event: OnDebuggeeStateChanged (" + PausedReason.ToString() + ")");
			if (DebuggeeStateChanged != null) {
				DebuggeeStateChanged(this, new ProcessEventArgs(this));
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
		/// Indentification of the state of the debugee. This value changes whenever the state of the debugee significatntly changes
		/// </summary>
		public DebugeeState DebugeeState {
			get {
				return debugeeState;
			}
		}
		
		/// <summary>
		/// The reason why the debugger is paused.
		/// Thows an DebuggerException if debugger is not paused.
		/// </summary>
		public PausedReason PausedReason {
			get {
				AssertPaused();
				return PauseSession.PausedReason;
			}
		}
		
		internal void Pause(bool debuggeeStateChanged)
		{
			if (this.SelectedThread == null && this.Threads.Count > 0) {
				this.SelectedThread = this.Threads[0];
			}
			
			if (this.SelectedThread != null) {
				// Disable all steppers - do not Deactivate since function tracking still needs them
				foreach(Stepper s in this.SelectedThread.Steppers) {
					s.PauseWhenComplete = false;
				}
				
				this.SelectedThread.SelectedFunction = this.SelectedThread.LastFunctionWithLoadedSymbols;
			}
			
			if (debuggeeStateChanged) {
				DebugeeState oldDebugeeState = debugeeState;
				debugeeState = new DebugeeState(this);
				OnDebuggeeStateChanged();
				if (oldDebugeeState != null) {
					oldDebugeeState.NotifyHasExpired();
				}
			}
			OnDebuggingPaused();
			if (PausedReason == PausedReason.Exception) {
				ExceptionEventArgs args = new ExceptionEventArgs(this, SelectedThread.CurrentException);
				OnExceptionThrown(args);
				if (args.Continue) {
					this.Continue();
				}
			}
			// Debugger state is unknown after calling OnDebuggingPaused (it may be resumed)
			if (IsPaused) {
				pausedHandle.Set();
			}
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
	}
}
