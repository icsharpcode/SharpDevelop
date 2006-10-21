// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

using Debugger.Wrappers.CorDebug;

namespace Debugger
{
	public partial class Process: RemotingObjectBase, IExpirable
	{
		NDebugger debugger;
		
		ICorDebugProcess corProcess;
		ManagedCallback callbackInterface;
		
		Thread selectedThread;
		PauseSession pauseSession;
		
		bool hasExpired = false;
		
		public event EventHandler Expired;
		
		public bool HasExpired {
			get {
				return hasExpired;
			}
		}
		
		internal void NotifyHasExpired()
		{
			if(!hasExpired) {
				hasExpired = true;
				if (Expired != null) {
					Expired(this, new ProcessEventArgs(this));
				}
				debugger.RemoveProcess(this);
			}
		}
		
		/// <summary>
		/// Indentification of the current debugger session. This value changes whenever debugger is continued
		/// </summary>
		public PauseSession PauseSession {
			get {
				return pauseSession;
			}
		}
		
		internal void NotifyPaused(PauseSession pauseSession)
		{
			this.pauseSession = pauseSession;
		}
		
		public NDebugger Debugger {
			get {
				return debugger;
			}
		}
		
		internal ManagedCallback CallbackInterface {
			get {
				return callbackInterface;
			}
		}
		
		internal Process(NDebugger debugger, ICorDebugProcess corProcess)
		{
			this.debugger = debugger;
			this.corProcess = corProcess;
			
			this.callbackInterface = new ManagedCallback(this);
		}

		internal ICorDebugProcess CorProcess {
			get {
				return corProcess;
			}
		}

		public Thread SelectedThread {
			get {
				return selectedThread;
			}
			set {
				selectedThread = value;
			}
		}
		
		static public Process CreateProcess(NDebugger debugger, string filename, string workingDirectory, string arguments)
		{
			return debugger.MTA2STA.Call<Process>(delegate{
			                                      	return StartInternal(debugger, filename, workingDirectory, arguments);
			                                      });
		}
		
		static unsafe Process StartInternal(NDebugger debugger, string filename, string workingDirectory, string arguments)
		{
			debugger.TraceMessage("Executing " + filename);
			
			uint[] processStartupInfo = new uint[17];
			processStartupInfo[0] = sizeof(uint) * 17;
			uint[] processInfo = new uint[4];
			
			ICorDebugProcess outProcess;
			
			if (workingDirectory == null || workingDirectory == "") {
				workingDirectory = System.IO.Path.GetDirectoryName(filename);
			}
			
			fixed (uint* pprocessStartupInfo = processStartupInfo)
				fixed (uint* pprocessInfo = processInfo)
					outProcess =
						debugger.CorDebug.CreateProcess(
							filename,   // lpApplicationName
							  // If we do not prepend " ", the first argument migh just get lost
							" " + arguments,                       // lpCommandLine
							ref _SECURITY_ATTRIBUTES.Default,                       // lpProcessAttributes
							ref _SECURITY_ATTRIBUTES.Default,                      // lpThreadAttributes
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
		
		public void Break()
		{
			AssertRunning();
			
			corProcess.Stop(5000); // TODO: Hardcoded value
			
			pauseSession = new PauseSession(PausedReason.ForcedBreak);

			// TODO: Code duplication from enter callback
			// Remove expired threads and functions
			foreach(Thread thread in this.Threads) {
				thread.CheckExpiration();
			}
			
			Pause(true);
		}
		
		public void Continue()
		{
			AssertPaused();

			pauseSession.NotifyHasExpired();
			pauseSession = null;
			OnDebuggingResumed();
			pausedHandle.Reset();
			
			corProcess.Continue(0);
		}
		
		public void Terminate()
		{
			// Resume stoped tread
			if (corProcess.IsRunning == 0) {
				corProcess.Continue(0); // TODO: Remove this...
			}
			// Stop&terminate - both must be called
			corProcess.Stop(5000); // TODO: ...and this
			corProcess.Terminate(0);
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
		
		public void AssertPaused()
		{
			if (IsRunning) {
				throw new DebuggerException("Process is not paused.");
			}
		}
		
		public void AssertRunning()
		{
			if (IsPaused) {
				throw new DebuggerException("Process is not running.");
			}
		}
		
		
		public string DebuggeeVersion {
			get {
				return debugger.DebuggeeVersion;
			}
		}
		
		/// <summary>
		/// Fired when System.Diagnostics.Trace.WriteLine() is called in debuged process
		/// </summary>
		public event EventHandler<MessageEventArgs> LogMessage;
		
		protected internal virtual void OnLogMessage(MessageEventArgs arg)
		{
			TraceMessage ("Debugger event: OnLogMessage");
			if (LogMessage != null) {
				LogMessage(this, arg);
			}
		}
		
		internal void TraceMessage(string message)
		{
			System.Diagnostics.Debug.WriteLine("Debugger:" + message);
			debugger.OnDebuggerTraceMessage(new MessageEventArgs(this, message));
		}
		
		public SourcecodeSegment NextStatement { 
			get {
				if (SelectedFunction == null || IsRunning) {
					return null;
				} else {
					return SelectedFunction.NextStatement;
				}
			}
		}
		
		public VariableCollection LocalVariables { 
			get {
				if (SelectedFunction == null || IsRunning) {
					return VariableCollection.Empty;
				} else {
					return SelectedFunction.Variables;
				}
			}
		}
	}
}
