// <file>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
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
				if (IsProcessRunning) throw new DebuggerException("Process must not be running");
				if (currentThread != null) return currentThread;
				throw new DebuggerException("No current thread");
			}
			set	{
				currentThread = value;
				if (debugger.ManagedCallback.HandlingCallback == false) {
					debugger.OnDebuggingPaused(PausedReason.CurrentThreadChanged);
				}
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

		public void Break()
		{
			if (!IsProcessRunning) {
				System.Diagnostics.Debug.Fail("Invalid operation");
				return;
			}

            corProcess.Stop(5000); // TODO: Hardcoded value

			isProcessRunning = false;
			debugger.OnDebuggingPaused(PausedReason.Break);
			debugger.OnIsProcessRunningChanged();
		}

		public void Continue()
		{
			if (IsProcessRunning) {
				System.Diagnostics.Debug.Fail("Invalid operation");
				return;
			}

			bool abort = false;
			debugger.OnDebuggingIsResuming(ref abort);
			if (abort == true) return;

			isProcessRunning = true;
			if (debugger.ManagedCallback.HandlingCallback == false) {
				debugger.OnDebuggingResumed();
				debugger.OnIsProcessRunningChanged();
			}

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

		public bool IsProcessRunning { 
			get {
				return isProcessRunning;
			}
			set {
				isProcessRunning = value;
			}
		}
	}
}
