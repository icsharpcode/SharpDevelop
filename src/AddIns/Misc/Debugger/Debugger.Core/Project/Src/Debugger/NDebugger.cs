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
	public partial class NDebugger: RemotingObjectBase
	{
		ICorDebug                  corDebug;
		ManagedCallback            managedCallback;
		ManagedCallbackProxy       managedCallbackProxy;
		
		MTA2STA mta2sta = new MTA2STA();
		
		VariableCollection localVariables;
		
		string debuggeeVersion;
		
		public MTA2STA MTA2STA {
			get {
				return mta2sta;
			}
		}

		internal ICorDebug CorDebug {
			get {
				return corDebug;
			}
		}
		
		public string DebuggeeVersion {
			get {
				return debuggeeVersion;
			}
		}
		
        internal ManagedCallback ManagedCallback {
			get {
				return managedCallback;
			}
		}

		public NDebugger()
		{
			if (ApartmentState.STA == System.Threading.Thread.CurrentThread.GetApartmentState()) {
				mta2sta.CallMethod = CallMethod.HiddenFormWithTimeout;
			} else {
				mta2sta.CallMethod = CallMethod.DirectCall;
			}
			
			this.ModuleLoaded += SetBreakpointsInModule;
			
			localVariables = new VariableCollection(this);
			
			Wrappers.ResourceManager.TraceMessagesEnabled = false;
			Wrappers.ResourceManager.TraceMessage += delegate (object s, MessageEventArgs e) { 
				TraceMessage(e.Message);
			};
		}
		
		/// <summary>
		/// Get the .NET version of the process that called this function
		/// </summary>
		public string GetDebuggerVersion()
		{
			int size;
			NativeMethods.GetCORVersion(null, 0, out size);
			StringBuilder sb = new StringBuilder(size);
			int hr = NativeMethods.GetCORVersion(sb, sb.Capacity, out size);
			return sb.ToString();
		}
		
		/// <summary>
		/// Get the .NET version of a given program - eg. "v1.1.4322"
		/// </summary>
		public string GetProgramVersion(string exeFilename)
		{
			int size;
			NativeMethods.GetRequestedRuntimeVersion(exeFilename, null, 0, out size);
			StringBuilder sb = new StringBuilder(size);
			NativeMethods.GetRequestedRuntimeVersion(exeFilename, sb, sb.Capacity, out size);
			return sb.ToString();
		}
		
		/// <summary>
		/// Prepares the debugger
		/// </summary>
		/// <param name="debuggeeVersion">Version of the program to debug - eg. "v1.1.4322"
		/// If null, the version of the executing process will be used</param>
		internal void InitDebugger(string debuggeeVersion)
		{
			if (debuggeeVersion != null && debuggeeVersion.Length > 1) {
				this.debuggeeVersion = debuggeeVersion;
			} else {
				this.debuggeeVersion = GetDebuggerVersion();
			}
			
			corDebug = new ICorDebug(NativeMethods.CreateDebuggingInterfaceFromVersion(3, this.debuggeeVersion));
			
			managedCallback = new ManagedCallback(this);
			managedCallbackProxy = new ManagedCallbackProxy(managedCallback);
			
			corDebug.Initialize();
			corDebug.SetManagedHandler(new ICorDebugManagedCallback(managedCallbackProxy));
			
			localVariables.Updating += OnUpdatingLocalVariables;
			
			TraceMessage("ICorDebug initialized, debugee version " + debuggeeVersion);
		}
		
		internal void TerminateDebugger()
		{
			localVariables.Clear();
			
			ClearModules();
			
			ResetBreakpoints();
			
			ClearThreads();
			
			currentProcess = null;
			pausedHandle.Reset();
			pauseSession = null;
			
			pendingEvalsCollection.Clear();
			
			TraceMessage("Reset done");
			
			corDebug.Terminate();
			
			TraceMessage("ICorDebug terminated");
			
			Wrappers.ResourceManager.TraceMessagesEnabled = true;
			Wrappers.ResourceManager.ReleaseAllTrackedCOMObjects();
			Wrappers.ResourceManager.TraceMessagesEnabled = false;
			
			TraceMessage("Tracked COM objects released");
			
			noProcessesHandle.Set();
		}
		
		
		/// <summary>
		/// Fired when System.Diagnostics.Trace.WriteLine() is called in debuged process
		/// </summary>
		public event EventHandler<MessageEventArgs> LogMessage;

		protected internal virtual void OnLogMessage(string message)
		{
			TraceMessage ("Debugger event: OnLogMessage");
			if (LogMessage != null) {
				LogMessage(this, new MessageEventArgs(this, message));
			}
		}

		/// <summary>
		/// Internal: Used to debug the debugger library.
		/// </summary>
		public event EventHandler<MessageEventArgs> DebuggerTraceMessage;

		protected internal virtual void OnDebuggerTraceMessage(string message)
		{
			if (DebuggerTraceMessage != null) {
				DebuggerTraceMessage(this, new MessageEventArgs(this, message));
			}
		}

		internal void TraceMessage(string message)
		{
			System.Diagnostics.Debug.WriteLine("Debugger:" + message);
			OnDebuggerTraceMessage(message);
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
			InitDebugger(GetProgramVersion(filename));
			Process process = Process.CreateProcess(this, filename, workingDirectory, arguments);
			AddProcess(process);
		}

		public SourcecodeSegment NextStatement { 
			get {
				if (CurrentFunction == null) {
					return null;
				} else {
					return CurrentFunction.NextStatement;
				}
			}
		}
		
		public VariableCollection LocalVariables { 
			get {
				localVariables.Update();
				return localVariables;
			}
		}
		
		void OnUpdatingLocalVariables(object sender, VariableCollectionEventArgs e)
		{
			if (CurrentFunction == null) {
				e.VariableCollection.UpdateTo(new Variable[] {}); // Make it empty
			} else {
				e.VariableCollection.UpdateTo(CurrentFunction.Variables);
				CurrentFunction.Expired += delegate { 
					e.VariableCollection.UpdateTo(new Variable[] {});
				};
			}
		}
	}
}
