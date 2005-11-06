// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

using DebuggerInterop.Core;
using DebuggerInterop.MetaData;
using System.Collections.Generic;

namespace Debugger
{
	public partial class NDebugger: RemotingObjectBase
	{
		ICorDebug                  corDebug;
		ManagedCallback            managedCallback;
		ManagedCallbackProxy       managedCallbackProxy;

		ApartmentState requiredApartmentState;

		EvalQueue evalQueue;

		internal EvalQueue EvalQueue {
			get { 
				return evalQueue;
			}
		}

		public ApartmentState RequiredApartmentState {
			get  {
				 return requiredApartmentState;
			}
		}

		internal ICorDebug CorDebug {
			get {
				return corDebug;
			}
		}
		
        internal ManagedCallback ManagedCallback {
			get {
				return managedCallback;
			}
		}

		public NDebugger()
		{
			requiredApartmentState = System.Threading.Thread.CurrentThread.GetApartmentState();
			
			this.ModuleLoaded += new EventHandler<ModuleEventArgs>(SetBreakpointsInModule);
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
			int hr = NativeMethods.GetRequestedRuntimeVersion(exeFilename, sb, sb.Capacity, out size);
			return sb.ToString();
		}
		
		/// <summary>
		/// Prepares the debugger
		/// </summary>
		/// <param name="debuggeeVersion">Version of the program to debug - eg. "v1.1.4322"
		/// If null, the version of the executing process will be used</param>
		internal void InitDebugger(string debuggeeVersion)
		{
			string version;
			if (debuggeeVersion != null) {
				version = debuggeeVersion;
			} else {
				version = GetDebuggerVersion();
			}
			
			NativeMethods.CreateDebuggingInterfaceFromVersion(3, version, out corDebug);
			
			managedCallback = new ManagedCallback(this);
			managedCallbackProxy = new ManagedCallbackProxy(this, managedCallback);
			
			corDebug.Initialize();
			corDebug.SetManagedHandler(managedCallbackProxy);
			
			TraceMessage("ICorDebug initialized, debugee version " + debuggeeVersion);
		}
		
		internal void TerminateDebugger()
		{
			ClearModules();
			
			ResetBreakpoints();
			
			ClearThreads();
			
			currentProcess = null;
			
			evalQueue = new EvalQueue(this);
			
			TraceMessage("Reset done");
			
			corDebug.Terminate();
			
			TraceMessage("ICorDebug terminated");
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
				if (CurrentFunction == null) {
					return VariableCollection.Empty;
				} else {
					return CurrentFunction.GetVariables();
				}
			}
		}
	}
}
