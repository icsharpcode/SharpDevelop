// <file>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
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
		static NDebugger instance = new NDebugger();

		public static NDebugger Instance {
			get {
				return instance;
			}
			set {
				instance = value;
			}
		}


		// Some variables that are used to get strings
		// They are used all over the library and
		// they are here so I don't have to decare them every time I need them
		static internal uint unused;
		static internal IntPtr unusedPtr = IntPtr.Zero;
		       internal const int pStringLen = 65536;
		static internal IntPtr pString = Marshal.AllocHGlobal(pStringLen*2);
		static internal string pStringAsUnicode { 
			get { 
				return Marshal.PtrToStringUni(pString); 
			} 
		}

		static ICorDebug                  corDebug;
		static ManagedCallback            managedCallback;
		static ManagedCallbackProxy       managedCallbackProxy;

		static Process                    mainProcess;
		
		public static bool CatchHandledExceptions = false;

		static internal ICorDebug CorDebug {
			get {
				return corDebug;
			}
		}

		static internal Process CurrentProcess {
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

        internal static ManagedCallback ManagedCallback {
			get {
				return managedCallback;
			}
		}

		#region Basic functions

		private NDebugger()
		{
			InitDebugger();
			ResetEnvironment();

			this.ModuleLoaded += new ModuleEventHandler(SetBreakpointsInModule);
		}
		
		~NDebugger() //TODO
		{
			corDebug.Terminate();
			Marshal.FreeHGlobal(pString);
		}

		static internal void InitDebugger()
		{
            int size;
            NativeMethods.GetCORVersion(null, 0, out size);
            StringBuilder sb = new StringBuilder(size);
            int hr = NativeMethods.GetCORVersion(sb, sb.Capacity, out size);

            NativeMethods.CreateDebuggingInterfaceFromVersion(3, sb.ToString(), out corDebug);

			//corDebug              = new CorDebugClass();
			managedCallback       = new ManagedCallback();
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

		static public event DebuggerEventHandler DebuggingStarted;

		static internal void OnDebuggingStarted()
		{
			TraceMessage ("Debugger event: OnDebuggingStarted()");
			if (DebuggingStarted != null)
				DebuggingStarted(null, new DebuggerEventArgs());
		}


		static public event DebuggingPausedEventHandler DebuggingPaused;

		static internal void OnDebuggingPaused(PausedReason reason)
		{
			TraceMessage ("Debugger event: OnDebuggingPaused(" + reason.ToString() + ")");
			if (DebuggingPaused != null)
				DebuggingPaused(null, new DebuggingPausedEventArgs(reason));
		}


		static public event DebuggingIsResumingEventHandler DebuggingIsResuming;

		static internal void OnDebuggingIsResuming(ref bool abort)
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


		static public event DebuggerEventHandler DebuggingResumed;

		static internal void OnDebuggingResumed()
		{
			TraceMessage ("Debugger event: OnDebuggingResumed()");
			if (DebuggingResumed != null)
				DebuggingResumed(null, new DebuggerEventArgs());
		}


		static public event DebuggerEventHandler DebuggingStopped;

		static internal void OnDebuggingStopped()
		{
			TraceMessage ("Debugger event: OnDebuggingStopped()");
			if (DebuggingStopped != null)
				DebuggingStopped(null, new DebuggerEventArgs());
		}


		static public event DebuggerEventHandler IsProcessRunningChanged;

		static internal void OnIsProcessRunningChanged()
		{
			TraceMessage ("Debugger event: OnIsProcessRunningChanged()");
			if (IsProcessRunningChanged != null)
				IsProcessRunningChanged(null, new DebuggerEventArgs());
		}

		
		static public event DebuggerEventHandler IsDebuggingChanged;

		static internal void OnIsDebuggingChanged()
		{
			TraceMessage ("Debugger event: OnIsDebuggingChanged()");
			if (IsDebuggingChanged != null)
				IsDebuggingChanged(null, new DebuggerEventArgs());
		}

		/// <summary>
		/// Fired when System.Diagnostics.Trace.WriteLine() is called in debuged process
		/// </summary>
		static public event MessageEventHandler LogMessage;

		static internal void OnLogMessage(string message)
		{
			TraceMessage ("Debugger event: OnLogMessage(\"" + message + "\")");
			if (LogMessage != null)
				LogMessage(null, new MessageEventArgs(message));
		}

		/// <summary>
		/// Internal: Used to debug the debugger library.
		/// </summary>
		static public event MessageEventHandler DebuggerTraceMessage;

		static internal void TraceMessage(string message)
		{
			if (DebuggerTraceMessage != null)
				DebuggerTraceMessage(null, new MessageEventArgs(message));
		}


		#endregion

		#region Execution control

		static internal void Continue(ICorDebugAppDomain pAppDomain)
		{
			ICorDebugProcess outProcess;
			pAppDomain.GetProcess(out outProcess);
			outProcess.Continue(0);
		}

		static public void StartWithoutDebugging(System.Diagnostics.ProcessStartInfo psi)
		{		
			System.Diagnostics.Process process;
			process = new System.Diagnostics.Process();
			process.StartInfo = psi;
			process.Start();
		}
		
		static public void Start(string filename, string workingDirectory, string arguments)		
		{
			CurrentProcess = Process.CreateProcess(filename, workingDirectory, arguments);
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


		static public bool IsProcessRunning { 
			get {
				if (!IsDebugging) return false;
				return CurrentProcess.IsProcessRunning;
			}
			set {
				if (CurrentProcess == null) return;
				CurrentProcess.IsProcessRunning = value;
			}
		}

		static public bool IsDebugging {
			get {
				return (CurrentProcess != null);
			}
		}

		static public Thread CurrentThread {
			get {
				return CurrentProcess.CurrentThread;
			}
			set {
				CurrentProcess.CurrentThread = value;
			}
		}

		static public Thread MainThread {
			get {
				return CurrentProcess.MainThread;
			}
			set {
				CurrentProcess.MainThread = value;
			}
		}

		static public SourcecodeSegment NextStatement { 
			get{
				return CurrentProcess.NextStatement;
			}
		}

		static public VariableCollection LocalVariables { 
			get{
				return CurrentProcess.LocalVariables;
			}
		}

		static public void Break()
		{
			CurrentProcess.Break();
		}

		static public void StepInto()
		{
			CurrentProcess.StepInto();
		}

		static public void StepOver()
		{
			CurrentProcess.StepOver();
		}

		static public void StepOut()
		{
			CurrentProcess.StepOut();
		}

		static public void Continue()
		{
			CurrentProcess.Continue();
		}

		static public void Terminate()
		{
			CurrentProcess.Terminate();
		}
	}
}
