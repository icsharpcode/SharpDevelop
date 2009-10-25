// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using Debugger.Interop.CorDebug;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Visitors;
using System.Runtime.InteropServices;

namespace Debugger
{
	internal enum DebuggeeStateAction { Keep, Clear }
	
	public class Process: DebuggerObject
	{
		NDebugger debugger;
		
		ICorDebugProcess corProcess;
		ManagedCallback callbackInterface;
		
		EvalCollection activeEvals;
		ModuleCollection modules;
		ThreadCollection threads;
		AppDomainCollection appDomains;
		
		public NDebugger Debugger {
			get { return debugger; }
		}
		
		internal ICorDebugProcess CorProcess {
			get { return corProcess; }
		}
		
		public Options Options {
			get { return debugger.Options; }
		}
		
		public string DebuggeeVersion {
			get { return debugger.DebuggeeVersion; }
		}
		
		internal ManagedCallback CallbackInterface {
			get { return callbackInterface; }
		}
		
		public EvalCollection ActiveEvals {
			get { return activeEvals; }
		}
		
		internal bool Evaluating {
			get { return activeEvals.Count > 0; }
		}
		
		public ModuleCollection Modules {
			get { return modules; }
		}
		
		public ThreadCollection Threads {
			get { return threads; }
		}
		
		public Thread SelectedThread {
			get { return this.Threads.Selected; }
			set { this.Threads.Selected = value; }
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
		
		public SourcecodeSegment NextStatement { 
			get {
				if (SelectedStackFrame == null || IsRunning) {
					return null;
				} else {
					return SelectedStackFrame.NextStatement;
				}
			}
		}
		
		public AppDomainCollection AppDomains {
			get { return appDomains; }
		}
		
		internal Process(NDebugger debugger, ICorDebugProcess corProcess)
		{
			this.debugger = debugger;
			this.corProcess = corProcess;
			
			this.callbackInterface = new ManagedCallback(this);
			
			activeEvals = new EvalCollection(debugger);
			modules = new ModuleCollection(debugger);
			threads = new ThreadCollection(debugger);
			appDomains = new AppDomainCollection(debugger);
		}
		
		static unsafe public Process CreateProcess(NDebugger debugger, string filename, string workingDirectory, string arguments)
		{
			debugger.TraceMessage("Executing " + filename);
			
			uint[] processStartupInfo = new uint[17];
			processStartupInfo[0] = sizeof(uint) * 17;
			uint[] processInfo = new uint[4];
			
			ICorDebugProcess outProcess;
			
			if (workingDirectory == null || workingDirectory == "") {
				workingDirectory = System.IO.Path.GetDirectoryName(filename);
			}
			
			_SECURITY_ATTRIBUTES secAttr = new _SECURITY_ATTRIBUTES();
			secAttr.bInheritHandle = 0;
			secAttr.lpSecurityDescriptor = IntPtr.Zero;
			secAttr.nLength = (uint)sizeof(_SECURITY_ATTRIBUTES);
			
			fixed (uint* pprocessStartupInfo = processStartupInfo)
				fixed (uint* pprocessInfo = processInfo)
					outProcess =
						debugger.CorDebug.CreateProcess(
							filename,   // lpApplicationName
							  // If we do not prepend " ", the first argument migh just get lost
							" " + arguments,                       // lpCommandLine
							ref secAttr,                       // lpProcessAttributes
							ref secAttr,                      // lpThreadAttributes
							1,//TRUE                    // bInheritHandles
							0x00000010 /*CREATE_NEW_CONSOLE*/,    // dwCreationFlags
							IntPtr.Zero,                       // lpEnvironment
							workingDirectory,                       // lpCurrentDirectory
							(uint)pprocessStartupInfo,        // lpStartupInfo
							(uint)pprocessInfo,               // lpProcessInformation,
							CorDebugCreateProcessFlags.DEBUG_NO_SPECIAL_OPTIONS   // debuggingFlags
							);
			
			return new Process(debugger, outProcess);
		}
		
		/// <summary> Fired when System.Diagnostics.Trace.WriteLine() is called in debuged process </summary>
		public event EventHandler<MessageEventArgs> LogMessage;
		
		protected internal virtual void OnLogMessage(MessageEventArgs arg)
		{
			TraceMessage ("Debugger event: OnLogMessage");
			if (LogMessage != null) {
				LogMessage(this, arg);
			}
		}
		
		public void TraceMessage(string message, params object[] args)
		{
			if (args.Length > 0)
				message = string.Format(message, args);
			System.Diagnostics.Debug.WriteLine("Debugger:" + message);
			debugger.OnDebuggerTraceMessage(new MessageEventArgs(this, message));
		}
		
		/// <summary> Read the specified amount of memory at the given memory address </summary>
		/// <returns> The content of the memory.  The amount of the read memory may be less then requested. </returns>
		public unsafe byte[] ReadMemory(ulong address, int size)
		{
			byte[] buffer = new byte[size];
			int readCount;
			fixed(byte* pBuffer = buffer) {
				readCount = (int)corProcess.ReadMemory(address, (uint)size, new IntPtr(pBuffer));
			}
			if (readCount != size) Array.Resize(ref buffer, readCount);
			return buffer;
		}
		
		/// <summary> Writes the given buffer at the specified memory address </summary>
		/// <returns> The number of bytes written </returns>
		public unsafe int WriteMemory(ulong address, byte[] buffer)
		{
			if (buffer.Length == 0) return 0;
			int written;
			fixed(byte* pBuffer = buffer) {
				written = (int)corProcess.WriteMemory(address, (uint)buffer.Length, new IntPtr(pBuffer));
			}
			return written;
		}
		
		#region Exceptions
		
		bool pauseOnHandledException = false;
		
		public event EventHandler<ExceptionEventArgs> ExceptionThrown;
		
		public bool PauseOnHandledException {
			get { return pauseOnHandledException; }
			set { pauseOnHandledException = value; }
		}
		
		protected internal virtual void OnExceptionThrown(ExceptionEventArgs e)
		{
			TraceMessage ("Debugger event: OnExceptionThrown()");
			if (ExceptionThrown != null) {
				ExceptionThrown(this, e);
			}
		}
		
		#endregion
		
		// State control for the process
		
		internal bool TerminateCommandIssued = false;
		internal Queue<Breakpoint> BreakpointHitEventQueue = new Queue<Breakpoint>();
		internal Dictionary<INode, TypedValue> ExpressionsCache = new Dictionary<INode, TypedValue>();
		
		#region Events
		
		public event EventHandler<ProcessEventArgs> Paused;
		public event EventHandler<ProcessEventArgs> Resumed;
		
		// HACK: public
		public virtual void OnPaused()
		{
			AssertPaused();
			// No real purpose - just additional check
			if (callbackInterface.IsInCallback) throw new DebuggerException("Can not raise event within callback.");
			TraceMessage ("Debugger event: OnPaused()");
			if (Paused != null) {
				foreach(Delegate d in Paused.GetInvocationList()) {
					if (IsRunning) {
						TraceMessage ("Skipping OnPaused delegate because process has resumed");
						break;
					}
					if (this.TerminateCommandIssued || this.HasExited) {
						TraceMessage ("Skipping OnPaused delegate because process has exited");
						break;
					}
					d.DynamicInvoke(this, new ProcessEventArgs(this));
				}
			}
		}
		
		protected virtual void OnResumed()
		{
			AssertRunning();
			if (callbackInterface.IsInCallback)
				throw new DebuggerException("Can not raise event within callback.");
			TraceMessage ("Debugger event: OnResumed()");
			if (Resumed != null) {
				Resumed(this, new ProcessEventArgs(this));
			}
		}
		
		
		#endregion
		
		#region PauseSession & DebugeeState
		
		PauseSession pauseSession;
		DebuggeeState debuggeeState;
		
		/// <summary>
		/// Indentification of the current debugger session. This value changes whenever debugger is continued
		/// </summary>
		public PauseSession PauseSession {
			get { return pauseSession; }
		}
		
		/// <summary>
		/// Indentification of the state of the debugee. This value changes whenever the state of the debugee significatntly changes
		/// </summary>
		public DebuggeeState DebuggeeState {
			get { return debuggeeState; }
		}
		
		/// <summary> Puts the process into a paused state </summary>
		internal void NotifyPaused(PausedReason pauseReason)
		{
			AssertRunning();
			pauseSession = new PauseSession(this, pauseReason);
			if (debuggeeState == null) {
				debuggeeState = new DebuggeeState(this);
			}
		}
		
		/// <summary> Puts the process into a resumed state </summary>
		internal void NotifyResumed(DebuggeeStateAction action)
		{
			AssertPaused();
			pauseSession = null;
			if (action == DebuggeeStateAction.Clear) {
				if (debuggeeState == null) throw new DebuggerException("Debugee state already cleared");
				debuggeeState = null;
				this.ExpressionsCache.Clear();
			}
		}
		
		/// <summary> Sets up the eviroment and raises user events </summary>
		internal void RaisePausedEvents()
		{
			DisableAllSteppers();
			CheckSelectedStackFrames();
			SelectMostRecentStackFrameWithLoadedSymbols();
			
			if (this.PauseSession.PausedReason == PausedReason.Exception) {
				ExceptionEventArgs args = new ExceptionEventArgs(this, this.SelectedThread.CurrentException, this.SelectedThread.CurrentExceptionType, this.SelectedThread.CurrentExceptionIsUnhandled);
				OnExceptionThrown(args);
				// The event could have resumed or killed the process
				if (this.IsRunning || this.TerminateCommandIssued || this.HasExited) return;
			}
			
			while(BreakpointHitEventQueue.Count > 0) {
				Breakpoint breakpoint = BreakpointHitEventQueue.Dequeue();
				breakpoint.NotifyHit();
				// The event could have resumed or killed the process
				if (this.IsRunning || this.TerminateCommandIssued || this.HasExited) return;
			}
			
			OnPaused();
			// The event could have resumed the process
			if (this.IsRunning || this.TerminateCommandIssued || this.HasExited) return;
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
			get { return pauseSession == null; }
		}
		
		public bool IsPaused {
			get { return !IsRunning; }
		}
		
		bool hasExited = false;
		
		public event EventHandler Exited;
		
		public bool HasExited {
			get {
				return hasExited;
			}
		}
		
		internal void NotifyHasExited()
		{
			if(!hasExited) {
				hasExited = true;
				if (Exited != null) {
					Exited(this, new ProcessEventArgs(this));
				}
				// Expire pause seesion first
				if (IsPaused) {
					NotifyResumed(DebuggeeStateAction.Clear);
				}
				debugger.Processes.Remove(this);
			}
		}
		
		public void Break()
		{
			AssertRunning();
			
			corProcess.Stop(uint.MaxValue); // Infinite; ignored anyway
			
			NotifyPaused(PausedReason.ForcedBreak);
			RaisePausedEvents();
		}
		
		public void Detach()
		{
			if (IsRunning) {
				corProcess.Stop(uint.MaxValue);
				NotifyPaused(PausedReason.ForcedBreak);
			}
			corProcess.Detach();
			NotifyHasExited();			
		}
		
		public void Continue()
		{
			AsyncContinue();
			WaitForPause();
		}
		
		internal Thread[] UnsuspendedThreads {
			get {
				List<Thread> threadsToRun = new List<Thread>(this.Threads.Count);
				foreach(Thread t in this.Threads) {
					if (!t.Suspended)
						threadsToRun.Add(t);
				}
				return threadsToRun.ToArray();
			}
		}
		
		/// <summary>
		/// Resume execution and run all threads not marked by the user as susspended.
		/// </summary>
		public void AsyncContinue()
		{
			AsyncContinue(DebuggeeStateAction.Clear, this.UnsuspendedThreads, CorDebugThreadState.THREAD_RUN);
		}
		
		internal CorDebugThreadState NewThreadState = CorDebugThreadState.THREAD_RUN;
		
		internal void AsyncContinue(DebuggeeStateAction action, Thread[] threadsToRun, CorDebugThreadState? newThreadState)
		{
			AssertPaused();
			
			if (threadsToRun != null) {
//				corProcess.SetAllThreadsDebugState(CorDebugThreadState.THREAD_SUSPEND, null);
//				TODO: There is second unreported thread, stopping it prevents the debugee from exiting
//				uint count = corProcess.EnumerateThreads().GetCount();
//				ICorDebugThread[] ts = new ICorDebugThread[count];
//				corProcess.EnumerateThreads().Next((uint)ts.Length, ts);
//				uint helper = corProcess.GetHelperThreadID();
				try {
					foreach(Thread t in this.Threads) {
						t.CorThread.SetDebugState(CorDebugThreadState.THREAD_SUSPEND);
					}
					foreach(Thread t in threadsToRun) {
						t.CorThread.SetDebugState(CorDebugThreadState.THREAD_RUN);
					}
				} catch (COMException e) {
					// The state of the thread is invalid. (Exception from HRESULT: 0x8013132D)
					// It can happen for threads that have not started yet
					if ((uint)e.ErrorCode == 0x8013132D) {
					} else {
						throw;
					}
				}
			}
			
			if (newThreadState != null) {
				this.NewThreadState = newThreadState.Value;
			}
			
			NotifyResumed(action);
			corProcess.Continue(0);
			if (this.Options.Verbose) {
				this.TraceMessage("Continue");
			}
			
			if (action == DebuggeeStateAction.Clear) {
				OnResumed();
			}
		}
		
		/// <summary> Terminates the execution of the process </summary>
		public void Terminate()
		{
			AsyncTerminate();
			// Wait until ExitProcess callback is received
			WaitForExit();
		}
		
		/// <summary> Terminates the execution of the process </summary>
		public void AsyncTerminate()
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
			this.TerminateCommandIssued = true;
			
			// Do not mark the process as exited
			// This is done once ExitProcess callback is received
		}
		
		void SelectSomeThread()
		{
			if (this.SelectedThread != null && !this.SelectedThread.IsInValidState) {
				this.SelectedThread = null;
			}
			if (this.SelectedThread == null) {
				foreach(Thread thread in this.Threads) {
					if (thread.IsInValidState) {
						this.SelectedThread = thread;
						break;
					}
				}
			}
		}
		
		internal void CheckSelectedStackFrames()
		{
			foreach(Thread thread in this.Threads) {
				if (thread.IsInValidState) {
					if (thread.SelectedStackFrame != null && thread.SelectedStackFrame.IsInvalid) {
						thread.SelectedStackFrame = null;
					}
				} else {
					thread.SelectedStackFrame = null;
				}
			}
		}
		
		internal void SelectMostRecentStackFrameWithLoadedSymbols()
		{
			SelectSomeThread();
			if (this.SelectedThread != null) {
				this.SelectedThread.SelectedStackFrame = this.SelectedThread.MostRecentStackFrameWithLoadedSymbols;
			}
		}
		
		internal void DisableAllSteppers()
		{
			foreach(Thread thread in this.Threads) {
				thread.CurrentStepIn = null;
				foreach(Stepper stepper in thread.Steppers) {
					stepper.Ignore = true;
				}
			}
		}
		
		/// <summary>
		/// Waits until the debugger pauses unless it is already paused.
		/// Use PausedReason to find out why it paused.
		/// </summary>
		public void WaitForPause()
		{
			while(this.IsRunning && !this.HasExited) {
				debugger.MTA2STA.WaitForCall();
				debugger.MTA2STA.PerformAllCalls();
			}
			if (this.HasExited) throw new ProcessExitedException();
		}
		
		public void WaitForPause(TimeSpan timeout)
		{
			System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
			watch.Start();
			while(this.IsRunning && !this.HasExited) {
				TimeSpan timeLeft = timeout - watch.Elapsed;
				if (timeLeft <= TimeSpan.FromMilliseconds(10)) break;
				//this.TraceMessage("Time left: " + timeLeft.TotalMilliseconds);
				debugger.MTA2STA.WaitForCall(timeLeft);
				debugger.MTA2STA.PerformCall();
			}
			if (this.HasExited) throw new ProcessExitedException();
		}
		
		/// <summary>
		/// Waits until the precesses exits.
		/// </summary>
		public void WaitForExit()
		{
			while(!this.HasExited) {
				debugger.MTA2STA.WaitForCall();
				debugger.MTA2STA.PerformAllCalls();
			}
		}
	}
}
