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

		static bool                       isProcessRunning;
		static ICorDebugProcess           mainProcess;
		static Thread                     mainThread;
		static Thread                     currentThread;
		
		public static bool CatchHandledExceptions = false;

		#region Public propeties

		static public SourcecodeSegment NextStatement { 
			get{
				try {
					return CurrentThread.NextStatement; 
				} catch (CurrentThreadNotAviableException) {
					throw new NextStatementNotAviableException();
				}
			} 
		}

		static public VariableCollection LocalVariables { 
			get{
				Thread thread;
				try {
					thread = CurrentThread;
				} 
				catch (CurrentThreadNotAviableException) {
					return new VariableCollection ();
				}
				return thread.LocalVariables;
			} 
		}

		#endregion

		static internal ICorDebugProcess MainProcess {
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
		
		public static Thread MainThread {
			get {
				return mainThread;
			}
			set {
				mainThread = value;
			}
		}
		
		static public Thread CurrentThread {
			get {
				if (!IsDebugging) throw new CurrentThreadNotAviableException();
				if (IsProcessRunning) throw new CurrentThreadNotAviableException();
				if (currentThread != null) return currentThread;
				if (mainThread != null) return mainThread;
				throw new CurrentThreadNotAviableException();
			}
			set	{
				currentThread = value;
				if (mainThread == null) {
					mainThread = value;
				}
				if (managedCallback.HandlingCallback == false) {
					OnDebuggingPaused(PausedReason.CurrentThreadChanged);
				}
			}
		}

		static internal ICorDebugProcess corProcess {
			get {
				if (MainProcess != null) return MainProcess;
				throw new UnableToGetPropertyException(null, "corProcess", "Make sure debuger is attached to process");
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
			
			MainProcess   = null;
			mainThread    = null;
			currentThread = null;
			isProcessRunning = false;
			
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

		static public void StartWithoutDebugging(System.Diagnostics.ProcessStartInfo psi)
		{		
			System.Diagnostics.Process process;
			process = new System.Diagnostics.Process();
			process.StartInfo = psi;
			process.Start();
		}

		static MTA2STA m2s = new MTA2STA();
		
		static public void Start(string filename, string workingDirectory, string arguments)		
		{
			if (IsDebugging) return;
			m2s.CallInSTA(typeof(NDebugger), "StartInternal", new Object[] {filename, workingDirectory, arguments});
			return;
		}

		static public unsafe void StartInternal(string filename, string workingDirectory, string arguments)
		{
			TraceMessage("Executing " + filename);

			_SECURITY_ATTRIBUTES secAttr = new _SECURITY_ATTRIBUTES();
			secAttr.bInheritHandle = 0;
			secAttr.lpSecurityDescriptor = IntPtr.Zero;
			secAttr.nLength = (uint)sizeof(_SECURITY_ATTRIBUTES); //=12?

			uint[] processStartupInfo = new uint[17];
			processStartupInfo[0] = sizeof(uint) * 17;
			uint[] processInfo = new uint[4];

			ICorDebugProcess outProcess;

			fixed (uint* pprocessStartupInfo = processStartupInfo)
				fixed (uint* pprocessInfo = processInfo)
					corDebug.CreateProcess(
						filename,   // lpApplicationName
						null,                       // lpCommandLine
						ref secAttr,                       // lpProcessAttributes
						ref secAttr,                      // lpThreadAttributes
						1,//TRUE                    // bInheritHandles
						0,                          // dwCreationFlags
						IntPtr.Zero,                       // lpEnvironment
						null,                       // lpCurrentDirectory
						(uint)pprocessStartupInfo,        // lpStartupInfo
						(uint)pprocessInfo,               // lpProcessInformation,
						CorDebugCreateProcessFlags.DEBUG_NO_SPECIAL_OPTIONS,   // debuggingFlags
						out outProcess      // ppProcess
						);

			isProcessRunning = true;
			MainProcess = outProcess;
		}

		static public void Break()
		{
			if (!IsDebugging) return;
			if (!IsProcessRunning) return;

            corProcess.Stop(5000); // TODO: Hardcoded value

			isProcessRunning = false;
			OnDebuggingPaused(PausedReason.Break);
			OnIsProcessRunningChanged();
		}

		static public void StepInto()
		{
			try {
				CurrentThread.StepInto();
			} catch (CurrentThreadNotAviableException) {}
		}

		static public void StepOver()
		{
			try {
				CurrentThread.StepOver();
			} catch (CurrentThreadNotAviableException) {}
		}

		static public void StepOut()
		{
			try {
				CurrentThread.StepOut();
			} catch (CurrentThreadNotAviableException) {}
		}

		static internal void Continue(ICorDebugAppDomain pAppDomain)
		{
			ICorDebugProcess outProcess;
			pAppDomain.GetProcess(out outProcess);
			if (MainProcess != outProcess) throw new DebuggerException("Request to continue AppDomain that does not belog to current process");
			Continue();
		}

		static public void Continue()
		{
			if (!IsDebugging) return;
			if (IsProcessRunning) return;

			bool abort = false;
			OnDebuggingIsResuming(ref abort);
			if (abort == true) return;

			isProcessRunning = true;
			if (managedCallback.HandlingCallback == false) {
				OnDebuggingResumed();
				OnIsProcessRunningChanged();
			}

			corProcess.Continue(0);
		}

		static public void Terminate()
		{
			if (!IsDebugging) return;

			int running;
			corProcess.IsRunning(out running);
			// Resume stoped tread
			if (running == 0) {
				Continue(); // TODO: Remove this...
			}
			// Stop&terminate - both must be called
			corProcess.Stop(5000); // TODO: ...and this
			corProcess.Terminate(0);
		}

		static public bool IsProcessRunning { 
			get {
				if (!IsDebugging) return false;
				return isProcessRunning;
			}
			set {
				isProcessRunning = value;
			}
		}

		static public bool IsDebugging { 
			get { 
				return (MainProcess != null);
			}
		}

		#endregion

		public void ToggleBreakpointAt(string fileName, int line, int column) 
		{
			// Check if there is breakpoint on that line
			foreach (Breakpoint breakpoint in Breakpoints) {
				// TODO check filename too
				if (breakpoint.SourcecodeSegment.StartLine == line) {
					Breakpoints.Remove(breakpoint);
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
	}
}
