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

namespace DebuggerLibrary
{
	public partial class NDebugger: RemotingObjectBase
	{
		ICorDebug                  corDebug;
		ManagedCallback            managedCallback;
		ManagedCallbackProxy       managedCallbackProxy;

		ApartmentState requiredApartmentState;

		EvalQueue evalQueue;
		
		bool pauseOnHandledException = false;

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
		
		public bool PauseOnHandledException {
			get {
				return pauseOnHandledException;
			}
			set {
				pauseOnHandledException = value;
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

			InitDebugger();
			ResetEnvironment();

			this.ModuleLoaded += new EventHandler<ModuleEventArgs>(SetBreakpointsInModule);
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

			managedCallback       = new ManagedCallback(this);
			managedCallbackProxy  = new ManagedCallbackProxy(this, managedCallback);

			corDebug.Initialize();
			corDebug.SetManagedHandler(managedCallbackProxy);
		}

		internal void ResetEnvironment()
		{
			ClearModules();
			
			ResetBreakpoints();
			
			ClearThreads();
			
			currentProcess = null;

			evalQueue = new EvalQueue(this);
			
			GC.Collect(GC.MaxGeneration);
			GC.WaitForPendingFinalizers();
			TraceMessage("Reset done");
		}

		#region Public events

		public event EventHandler<DebuggingPausedEventArgs> DebuggingPaused;

		protected internal virtual void OnDebuggingPaused(PausedReason reason)
		{
			TraceMessage ("Debugger event: OnDebuggingPaused(" + reason.ToString() + ")");
			if (DebuggingPaused != null) {
				DebuggingPausedEventArgs args = new DebuggingPausedEventArgs(this, reason);
				DebuggingPaused(this, args);
				if (args.ResumeDebugging) {
					Continue();
				}
			}
		}


		public event EventHandler<DebuggerEventArgs> DebuggingResumed;

		protected internal virtual void OnDebuggingResumed()
		{
			TraceMessage ("Debugger event: OnDebuggingResumed()");
			if (DebuggingResumed != null) {
				DebuggingResumed(this, new DebuggerEventArgs(this));
			}
		}

		/// <summary>
		/// Fired when System.Diagnostics.Trace.WriteLine() is called in debuged process
		/// </summary>
		public event EventHandler<MessageEventArgs> LogMessage;

		protected internal virtual void OnLogMessage(string message)
		{
			TraceMessage ("Debugger event: OnLogMessage(\"" + message + "\")");
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


		#endregion

		public void StartWithoutDebugging(System.Diagnostics.ProcessStartInfo psi)
		{		
			System.Diagnostics.Process process;
			process = new System.Diagnostics.Process();
			process.StartInfo = psi;
			process.Start();
		}
		
		public void Start(string filename, string workingDirectory, string arguments)		
		{
			Process process = Process.CreateProcess(this, filename, workingDirectory, arguments);
			AddProcess(process);
		}

		public bool IsCurrentProcessSafeForInspection {
			get {
				if (CurrentProcess == null) {
					return false;
				} else {
					return CurrentProcess.IsProcessSafeForInspection;
				}
			}
		}

		internal void CheckThatCurrentProcessIsSafeForInspection()
		{
			if (CurrentProcess == null) {
				throw new DebuggerException("There is no process being debugged.");
			} else {
				CurrentProcess.CheckThatProcessIsSafeForInspection();
			}
		}

		public Thread CurrentThread {
			get {
				if (CurrentProcess == null) return null;
				return CurrentProcess.CurrentThread;
			}
			set {
				CurrentProcess.CurrentThread = value;
			}
		}

		public SourcecodeSegment NextStatement { 
			get {
				if (!IsCurrentProcessSafeForInspection) return null;
				if (CurrentProcess.CurrentThread.CurrentFunction == null) {
					return null;
				} else {
					return CurrentProcess.CurrentThread.CurrentFunction.NextStatement;
				}
			}
		}

		public VariableCollection LocalVariables { 
			get {
				if (!IsCurrentProcessSafeForInspection) return VariableCollection.Empty;
				return CurrentProcess.CurrentThread.CurrentFunction.GetVariables();
			}
		}

		public void Break()
		{
			CurrentProcess.Break();
		}

		public void StepInto()
		{
			CurrentProcess.CurrentThread.CurrentFunction.StepInto();
		}

		public void StepOver()
		{
			CurrentProcess.CurrentThread.CurrentFunction.StepOver();
		}

		public void StepOut()
		{
			CurrentProcess.CurrentThread.CurrentFunction.StepOut();
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
