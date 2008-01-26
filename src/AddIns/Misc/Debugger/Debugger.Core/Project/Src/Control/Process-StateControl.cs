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
		// Order:
		//  Update PauseSession
		//  Update DebugSession
		//  Raise event
		
		#region Events
		
		public event EventHandler<ProcessEventArgs> Paused;
		public event EventHandler<ProcessEventArgs> Resumed;
		
		// HACK: public
		public virtual void OnPaused()
		{
			AssertPaused();
			TraceMessage ("Debugger event: OnPaused()");
			if (Paused != null) {
				foreach(Delegate d in Paused.GetInvocationList()) {
					if (IsRunning) {
						TraceMessage ("Skipping OnPaused delegate becuase process has resumed");
						break;
					}
					d.DynamicInvoke(this, new ProcessEventArgs(this));
				}
			}
		}
		
		protected virtual void OnResumed()
		{
			AssertRunning();
			TraceMessage ("Debugger event: OnResumed()");
			if (Resumed != null) {
				Resumed(this, new ProcessEventArgs(this));
			}
		}
		
		
		#endregion
		
		#region PauseSession
		
		PauseSession pauseSession;
		
		/// <summary>
		/// Indentification of the current debugger session. This value changes whenever debugger is continued
		/// </summary>
		public PauseSession PauseSession {
			get {
				return pauseSession;
			}
		}
		
		internal void CreatePauseSession(PausedReason pauseReason)
		{
			if (pauseSession != null) {
				throw new DebuggerException("Pause session already created");
			}
			pauseSession = new PauseSession(pauseReason);
		}
		
		internal void ExpirePauseSession()
		{
			if (pauseSession == null) {
				throw new DebuggerException("Pause session already expired");
			}
			PauseSession oldPauseSession = pauseSession;
			pauseSession = null;
			oldPauseSession.NotifyHasExpired();
		}
		
		#endregion
		
		#region DebugeeState
		
		DebuggeeState debuggeeState;
		
		/// <summary>
		/// Indentification of the state of the debugee. This value changes whenever the state of the debugee significatntly changes
		/// </summary>
		public DebuggeeState DebuggeeState {
			get {
				return debuggeeState;
			}
		}
		
		internal void CreateDebuggeeState()
		{
			if (debuggeeState != null) {
				throw new DebuggerException("Debugee state already created");
			}
			if (pauseSession == null) {
				throw new DebuggerException("Pause session must exist (update order error)");
			}
			debuggeeState = new DebuggeeState(this);
		}
		
		internal void ExpireDebuggeeState()
		{
			if (debuggeeState == null) {
				throw new DebuggerException("Debugee state already expired");
			}
			if (pauseSession != null) {
				throw new DebuggerException("Pause session must not exist (update order error)");
			}
			DebuggeeState oldDebugeeState = debuggeeState;
			debuggeeState = null;
			oldDebugeeState.NotifyHasExpired();
		}
		
		#endregion
		
		#region Exceptions
		
		bool pauseOnHandledException = false;
		
		public event EventHandler<ExceptionEventArgs> ExceptionThrown;
		
		public bool PauseOnHandledException {
			get {
				return pauseOnHandledException;
			}
			set {
				pauseOnHandledException = value;
			}
		}
		
		protected internal virtual void OnExceptionThrown(ExceptionEventArgs e)
		{
			TraceMessage ("Debugger event: OnExceptionThrown()");
			if (ExceptionThrown != null) {
				ExceptionThrown(this, e);
			}
		}
		
		#endregion
		
		internal void AssertPaused()
		{
			if (IsRunning) {
				throw new DebuggerException("Process is not paused.");
			}
		}
		
		internal void AssertRunning()
		{
			if (IsPaused) {
				throw new DebuggerException("Process is not running.");
			}
		}
		
		public bool IsRunning { 
			get {
				return pauseSession == null;
			}
		}
		
		public bool IsPaused {
			get {
				return !IsRunning;
			}
		}
		
		public void Break()
		{
			AssertRunning();
			
			corProcess.Stop(uint.MaxValue); // Infinite; ignored anyway
			
			CreatePauseSession(PausedReason.ForcedBreak);
			CreateDebuggeeState();
			
			SelectMostRecentStackFrameWithLoadedSymbols();
			DisableAllSteppers();
			
			OnPaused();
		}
		
		#region Convenience methods
		
		public void Continue()
		{
			AsyncContinue();
			WaitForPause();
		}
		
		#endregion
		
		public void AsyncContinue()
		{
			AssertPaused();
			
			if (callbackInterface.IsInCallback) {
				throw new DebuggerException("Can not continue from within callback.");
			}
			
			ExpirePauseSession();
			ExpireDebuggeeState();
			OnResumed();
			corProcess.Continue(0);
		}
		
		internal void AsyncContinue_KeepDebuggeeState()
		{
			AssertPaused();
			
			ExpirePauseSession();
			corProcess.Continue(0);
		}
		
		public void Terminate()
		{
			// Resume stoped tread
			if (this.IsPaused) {
				// We might get more callbacks so we should maintain consistent sate
				//AsyncContinue(); // Continue the process to get remaining callbacks
			}
			
			// Expose race condition - drain callback queue
			System.Threading.Thread.Sleep(0);
			
			// Stop&terminate - both must be called
			corProcess.Stop(uint.MaxValue);
			corProcess.Terminate(0);
			
			this.NotifyHasExpired();
		}
		
		internal void SelectMostRecentStackFrameWithLoadedSymbols()
		{
			if (this.SelectedThread == null && this.Threads.Count > 0) {
				this.SelectedThread = this.Threads[0];
			}
			if (this.SelectedThread != null) {
				this.SelectedThread.SelectedStackFrame = this.SelectedThread.MostRecentStackFrameWithLoadedSymbols;
			}
		}
		
		internal void DisableAllSteppers()
		{
			foreach(Thread thread in this.Threads) {
				foreach(Stepper stepper in thread.Steppers) {
					stepper.PauseWhenComplete = false;
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
		
		public void WaitForPause(TimeSpan timeout)
		{
			DateTime endTime = Util.HighPrecisionTimer.Now + timeout;
			while(this.IsRunning && !this.HasExpired) {
				TimeSpan timeLeft = endTime - Util.HighPrecisionTimer.Now;
				if (timeLeft <= TimeSpan.FromMilliseconds(10)) break;
				//this.TraceMessage("Time left: " + timeLeft.TotalMilliseconds);
				debugger.MTA2STA.WaitForCall(timeLeft);
				debugger.MTA2STA.PerformCall();
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
