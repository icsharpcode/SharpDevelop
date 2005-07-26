// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

using DebuggerInterop.Core;
using DebuggerInterop.MetaData;

namespace DebuggerLibrary
{
	public class Process: RemotingObjectBase
	{
		NDebugger debugger;

		ICorDebugProcess corProcess;

		Thread currentThread;
		bool isProcessRunning = true;

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
			Process createdProcess = null;
			if (debugger.RequiredApartmentState == ApartmentState.STA) {
				MTA2STA m2s = new MTA2STA();
				createdProcess = (Process)m2s.CallInSTA(typeof(Process), "StartInternal", new Object[] {debugger, filename, workingDirectory, arguments});
			} else {
				createdProcess = StartInternal(debugger, filename, workingDirectory, arguments);
			}
			return createdProcess;
		}

		static public unsafe Process StartInternal(NDebugger debugger, string filename, string workingDirectory, string arguments)
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
						arguments,                       // lpCommandLine
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
			corProcess.Continue(0);
		}
		
		internal void ContinueCallback()
		{
			if (isProcessRunning) {
				throw new DebuggerException("Invalid operation");
			}

			isProcessRunning = true;
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
