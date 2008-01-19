// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Threading;

namespace Debugger
{
	public partial class Process
	{
		bool pauseOnHandledException = false;
		
		DebugeeState debugeeState;
		
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
		
		public StackFrame SelectedStackFrame {
			get {
				if (SelectedThread == null) {
					return null;
				} else {
					return SelectedThread.SelectedStackFrame;
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
				// Disable all steppers - do not Deactivate since stack frame tracking still needs them
				foreach(Stepper s in this.SelectedThread.Steppers) {
					s.PauseWhenComplete = false;
				}
				
				this.SelectedThread.SelectedStackFrame = this.SelectedThread.MostRecentStackFrameWithLoadedSymbols;
			}
			
			if (debuggeeStateChanged) {
				if (debugeeState != null) {
					debugeeState.NotifyHasExpired();
				}
				debugeeState = new DebugeeState(this);
				OnDebuggeeStateChanged();
				// The process might have been resumed by the event
			}
			if (IsPaused) {
				OnDebuggingPaused();
				if (PausedReason == PausedReason.Exception) {
					ExceptionEventArgs args = new ExceptionEventArgs(this, SelectedThread.CurrentException);
					OnExceptionThrown(args);
					if (args.Continue) {
						this.AsyncContinue();
					}
				}
			}
		}
		
		/// <summary>
		/// Waits until the debugger pauses unless it is already paused.
		/// Use PausedReason to find out why it paused.
		/// </summary>
		public void WaitForPause()
		{
			while(this.IsRunning && !this.HasExpired) {
				debugger.MTA2STA.WaitForCall();
				debugger.MTA2STA.PerformAllCalls();
			}
			if (this.HasExpired) throw new DebuggerException("Process exited before pausing");
		}
		
		/// <summary>
		/// Waits until the precesses exits.
		/// </summary>
		public void WaitForExit()
		{
			while(!this.HasExpired) {
				debugger.MTA2STA.WaitForCall();
				debugger.MTA2STA.PerformAllCalls();
			}
		}
	}
}
