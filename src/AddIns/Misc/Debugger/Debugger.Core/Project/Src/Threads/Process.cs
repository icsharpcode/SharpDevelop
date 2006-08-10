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
					Expired(this, new DebuggerEventArgs(debugger));
				}
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

		internal Process(NDebugger debugger, ICorDebugProcess corProcess)
		{
			this.debugger = debugger;
			this.corProcess = corProcess;
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

		public IList<Thread> Threads {
			get {
				List<Thread> threads = new List<Thread>();
				foreach(Thread thread in debugger.Threads) {
					if (thread.Process == this) {
						threads.Add(thread);
					}
				}
				return threads;
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
		
		internal void Break()
		{
			AssertRunning();
			
			corProcess.Stop(5000); // TODO: Hardcoded value
			
			pauseSession = new PauseSession(PausedReason.ForcedBreak);
			debugger.SelectedProcess = this;
			
			debugger.Pause(true);
		}
		
		public void Continue()
		{
			AssertPaused();

			pauseSession.NotifyHasExpired();
			pauseSession = null;
			debugger.OnDebuggingResumed();
			debugger.pausedHandle.Reset();
			
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
	}
}
