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

using Debugger.Interop.CorDebug;
using Debugger.Interop.MetaData;

namespace Debugger
{
	public class Process: RemotingObjectBase
	{
		NDebugger debugger;

		ICorDebugProcess corProcess;

		Thread currentThread;
		bool isProcessRunning = true;
		
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

		public Thread CurrentThread {
			get {
				if (currentThread == null) {
					IList<Thread> threads = Threads;
					if (threads.Count > 0) {
						currentThread = threads[0];
					}
				}
				return currentThread;
			}
			internal set {
				currentThread = value;
			}
		}
		
		public void SetCurrentThread(Thread thread)
		{
			CurrentThread = thread;
			
			debugger.FakePause(PausedReason.CurrentThreadChanged, false);
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
			
			_SECURITY_ATTRIBUTES secAttr = new _SECURITY_ATTRIBUTES();
			secAttr.bInheritHandle = 0;
			secAttr.lpSecurityDescriptor = IntPtr.Zero;
			secAttr.nLength = (uint)sizeof(_SECURITY_ATTRIBUTES); //=12?
			
			uint[] processStartupInfo = new uint[17];
			processStartupInfo[0] = sizeof(uint) * 17;
			uint[] processInfo = new uint[4];
			
			ICorDebugProcess outProcess;
			
			if (workingDirectory == null || workingDirectory == "") {
				workingDirectory = System.IO.Path.GetDirectoryName(filename);
			}
			
			fixed (uint* pprocessStartupInfo = processStartupInfo)
				fixed (uint* pprocessInfo = processInfo)
					debugger.CorDebug.CreateProcess(
						filename,   // lpApplicationName
						  // If we do not prepend " ", the first argument migh just get lost
						" " + arguments,                       // lpCommandLine
						ref secAttr,                       // lpProcessAttributes
						ref secAttr,                      // lpThreadAttributes
						1,//TRUE                    // bInheritHandles
						0,                          // dwCreationFlags
						IntPtr.Zero,                       // lpEnvironment
						workingDirectory,                       // lpCurrentDirectory
						(uint)pprocessStartupInfo,        // lpStartupInfo
						(uint)pprocessInfo,               // lpProcessInformation,
						CorDebugCreateProcessFlags.DEBUG_NO_SPECIAL_OPTIONS,   // debuggingFlags
						out outProcess      // ppProcess
						);
			
			return new Process(debugger, outProcess);
		}
		
		internal void Break()
		{
			if (!isProcessRunning) {
				throw new DebuggerException("Invalid operation");
			}

            corProcess.Stop(5000); // TODO: Hardcoded value

			isProcessRunning = false;
			debugger.Pause(PausedReason.Break, this, null, null);
		}

		public void Continue()
		{
			if (isProcessRunning) {
				throw new DebuggerException("Invalid operation");
			}

			debugger.Resume();
			isProcessRunning = true;
			debugger.SessionID = new object();
			corProcess.Continue(0);
		}
		
		internal void ContinueCallback()
		{
			if (isProcessRunning) {
				throw new DebuggerException("Invalid operation");
			}

			isProcessRunning = true;
			debugger.SessionID = new object();
			corProcess.Continue(0);
		}

		public void Terminate()
		{
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

		public bool IsRunning { 
			get {
				return isProcessRunning;
			}
			internal set {
				isProcessRunning = value;
			}
		}
		
		public bool IsPaused {
			get {
				return !isProcessRunning;
			}
		}
		
		public void AssertPaused()
		{
			if (!IsPaused) {
				throw new DebuggerException("Process is not paused.");
			}
		}
	}
}
