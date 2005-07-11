// <file>
//     <owner name="David Srbeck" email="dsrbecky@post.cz"/>
// </file>

using System;
using System.Diagnostics;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;

using DebuggerInterop.Core;
using DebuggerInterop.MetaData;

namespace DebuggerLibrary
{
	public partial class NDebugger
	{
		ICorDebug                  corDebug;
		ManagedCallback            managedCallback;
		ManagedCallbackProxy       managedCallbackProxy;

		Process                    mainProcess;
		
		public bool CatchHandledExceptions = false;

		internal ICorDebug CorDebug {
			get {
				return corDebug;
			}
		}

		internal Process CurrentProcess {
			get {
				return mainProcess;
			}
			set {
				if (mainProcess == value) return;

				mainProcess = value;
				if (value != null) {
					OnDebuggingStarted();
					OnIsDebuggingChanged();
					OnIsProcessRunningChanged();
				} else {
					//OnDebuggingPaused(PausedReason.ProcessExited);
					OnIsProcessRunningChanged();
					OnDebuggingStopped();
					OnIsDebuggingChanged();
				}
			}
		}

        internal ManagedCallback ManagedCallback {
			get {
				return managedCallback;
			}
		}

		#region Basic functions

		public NDebugger()
		{
			InitDebugger();
			ResetEnvironment();

			this.ModuleLoaded += new ModuleEventHandler(SetBreakpointsInModule);
		}
		
		~NDebugger() //TODO
		{
			//corDebug.Terminate();
		}

		internal void InitDebugger()
		{
            int size;
            NativeMethods.GetCORVersion(null, 0, out size);
            StringBuilder sb = new StringBuilder(size);
            int hr = NativeMethods.GetCORVersion(sb, sb.Capacity, out size);

            NativeMethods.CreateDebuggingInterfaceFromVersion(3, sb.ToString(), out corDebug);

			//corDebug              = new CorDebugClass();
			managedCallback       = new ManagedCallback(this);
			managedCallbackProxy  = new ManagedCallbackProxy(managedCallback);

			corDebug.Initialize();
			corDebug.SetManagedHandler(managedCallbackProxy);
		}

		internal void ResetEnvironment()
		{
			ClearModules();
			
			ResetBreakpoints();
			
			ClearThreads();
			
			CurrentProcess   = null;
			
			GC.Collect(GC.MaxGeneration);
			GC.WaitForPendingFinalizers();
			TraceMessage("Reset done");
		}

		#endregion

		#region Public events

		public event DebuggerEventHandler DebuggingStarted;

		protected internal virtual void OnDebuggingStarted()
		{
			TraceMessage ("Debugger event: OnDebuggingStarted()");
			if (DebuggingStarted != null) {
				DebuggingStarted(null, new DebuggerEventArgs());
			}
		}


		public event DebuggingPausedEventHandler DebuggingPaused;

		protected internal virtual void OnDebuggingPaused(PausedReason reason)
		{
			TraceMessage ("Debugger event: OnDebuggingPaused(" + reason.ToString() + ")");
			if (DebuggingPaused != null) {
				DebuggingPaused(null, new DebuggingPausedEventArgs(reason));
			}
		}


		public event DebuggingIsResumingEventHandler DebuggingIsResuming;

		protected internal virtual void OnDebuggingIsResuming(ref bool abort)
		{
			if (DebuggingIsResuming != null) {
				TraceMessage ("Debugger event: OnDebuggingIsResuming(" + abort.ToString() + ")");
				foreach(Delegate d in DebuggingIsResuming.GetInvocationList()) {
					DebuggingIsResumingEventArgs eventHandler = new DebuggingIsResumingEventArgs();
					d.DynamicInvoke(new object[] {null, eventHandler});
					if (eventHandler.Abort == true) {
						abort = true;
						break;
					}
				}
			}
		}


		public event DebuggerEventHandler DebuggingResumed;

		protected internal virtual void OnDebuggingResumed()
		{
			TraceMessage ("Debugger event: OnDebuggingResumed()");
			if (DebuggingResumed != null) {
				DebuggingResumed(null, new DebuggerEventArgs());
			}
		}


		public event DebuggerEventHandler DebuggingStopped;

		protected internal virtual void OnDebuggingStopped()
		{
			TraceMessage ("Debugger event: OnDebuggingStopped()");
			if (DebuggingStopped != null) {
				DebuggingStopped(null, new DebuggerEventArgs());
			}
		}


		public event DebuggerEventHandler IsProcessRunningChanged;

		protected internal virtual void OnIsProcessRunningChanged()
		{
			TraceMessage ("Debugger event: OnIsProcessRunningChanged()");
			if (IsProcessRunningChanged != null) {
				IsProcessRunningChanged(null, new DebuggerEventArgs());
			}
		}

		
		public event DebuggerEventHandler IsDebuggingChanged;

		protected internal virtual void OnIsDebuggingChanged()
		{
			TraceMessage ("Debugger event: OnIsDebuggingChanged()");
			if (IsDebuggingChanged != null) {
				IsDebuggingChanged(null, new DebuggerEventArgs());
			}
		}

		/// <summary>
		/// Fired when System.Diagnostics.Trace.WriteLine() is called in debuged process
		/// </summary>
		public event MessageEventHandler LogMessage;

		protected internal virtual void OnLogMessage(string message)
		{
			TraceMessage ("Debugger event: OnLogMessage(\"" + message + "\")");
			if (LogMessage != null) {
				LogMessage(null, new MessageEventArgs(message));
			}
		}

		/// <summary>
		/// Internal: Used to debug the debugger library.
		/// </summary>
		public event MessageEventHandler DebuggerTraceMessage;

		protected internal virtual void OnDebuggerTraceMessage(string message)
		{
			if (DebuggerTraceMessage != null) {
				DebuggerTraceMessage(null, new MessageEventArgs(message));
			}
		}

		internal void TraceMessage(string message)
		{
			OnDebuggerTraceMessage(message);
		}


		#endregion

		#region Execution control

		internal void Continue(ICorDebugAppDomain pAppDomain)
		{
			ICorDebugProcess outProcess;
			pAppDomain.GetProcess(out outProcess);
			outProcess.Continue(0);
		}

		public void StartWithoutDebugging(System.Diagnostics.ProcessStartInfo psi)
		{		
			System.Diagnostics.Process process;
			process = new System.Diagnostics.Process();
			process.StartInfo = psi;
			process.Start();
		}
		
		public void Start(string filename, string workingDirectory, string arguments)		
		{
			CurrentProcess = Process.CreateProcess(this, filename, workingDirectory, arguments);
		}



		#endregion

		public void ToggleBreakpointAt(string fileName, int line, int column) 
		{
			// Check if there is breakpoint on that line
			foreach (Breakpoint breakpoint in Breakpoints) {
				// TODO check filename too
				if (breakpoint.SourcecodeSegment.StartLine == line) {
					RemoveBreakpoint(breakpoint);
					return;
				}
			}

			// Add the breakpoint 
			Breakpoint addedBreakpoint = AddBreakpoint(fileName, line, column);

            // Check if it wasn't forced to move to different line with breakpoint
			foreach (Breakpoint breakpoint in Breakpoints) {
				if (breakpoint != addedBreakpoint) { // Only the old ones
					if (breakpoint.SourcecodeSegment.StartLine == addedBreakpoint.SourcecodeSegment.StartLine) {
						// Whops! We have two breakpoint on signle line, delete one
						RemoveBreakpoint(addedBreakpoint);
						return;
					}
				}
			}
		}


		public bool IsProcessRunning { 
			get {
				if (!IsDebugging) return false;
				return CurrentProcess.IsProcessRunning;
			}
			set {
				if (CurrentProcess == null) return;
				CurrentProcess.IsProcessRunning = value;
			}
		}

		public bool IsDebugging {
			get {
				return (CurrentProcess != null);
			}
		}

		public Thread CurrentThread {
			get {
				if (!IsDebugging) return null;
				return CurrentProcess.CurrentThread;
			}
			set {
				CurrentProcess.CurrentThread = value;
			}
		}

		public Thread MainThread {
			get {
				if (!IsDebugging) return null;
				return CurrentProcess.MainThread;
			}
			set {
				CurrentProcess.MainThread = value;
			}
		}

		public SourcecodeSegment NextStatement { 
			get {
				if (!IsDebugging) return null;
				return CurrentProcess.NextStatement;
			}
		}

		public VariableCollection LocalVariables { 
			get {
				if (!IsDebugging) return null;
				return CurrentProcess.LocalVariables;
			}
		}

		public void Break()
		{
			CurrentProcess.Break();
		}

		public void StepInto()
		{
			CurrentProcess.StepInto();
		}

		public void StepOver()
		{
			CurrentProcess.StepOver();
		}

		public void StepOut()
		{
			CurrentProcess.StepOut();
		}

		public void Continue()
		{
			CurrentProcess.Continue();
		}

		public void Terminate()
		{
			CurrentProcess.Terminate();
		}
	}
}
