// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

using Debugger.Wrappers.CorDebug;

namespace Debugger
{
	public partial class Process: DebuggerObject
	{
		NDebugger debugger;
		
		ICorDebugProcess corProcess;
		ManagedCallback callbackInterface;
		
		string workingDirectory = String.Empty;
		
		#region IExpirable
		
		bool hasExited = false;
		
		public event EventHandler Exited;
		
		public bool HasExited {
			get {
				return hasExited;
			}
		}
		
		internal void NotifyHasExited()
		{
			if(!hasExited) {
				hasExited = true;
				if (Exited != null) {
					Exited(this, new ProcessEventArgs(this));
				}
				// Expire pause seesion first
				if (IsPaused) {
					NotifyResumed(DebuggeeStateAction.Clear);
				}
				debugger.RemoveProcess(this);
			}
		}
		
		#endregion
		
		public NDebugger Debugger {
			get {
				return debugger;
			}
		}
		
		public Options Options {
			get {
				return debugger.Options;
			}
		}
		
		public string WorkingDirectory {
			get { return workingDirectory; }
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
		
		static unsafe public Process CreateProcess(NDebugger debugger, string filename, string workingDirectory, string arguments)
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
			
			Process process = new Process(debugger, outProcess);
			process.workingDirectory = workingDirectory;
			return process;
		}
		
		public string DebuggeeVersion {
			get {
				return debugger.DebuggeeVersion;
			}
		}
		
		public StackFrame SelectedStackFrame {
			get {
				if (SelectedThread == null) {
					return null;
				} else {
					return SelectedThread.SelectedStackFrame;
				}
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
		
		public void TraceMessage(string message, params object[] args)
		{
			TraceMessage(string.Format(message, args));
		}
		
		public void TraceMessage(string message)
		{
			System.Diagnostics.Debug.WriteLine("Debugger:" + message);
			debugger.OnDebuggerTraceMessage(new MessageEventArgs(this, message));
		}
		
		public SourcecodeSegment NextStatement { 
			get {
				if (SelectedStackFrame == null || IsRunning) {
					return null;
				} else {
					return SelectedStackFrame.NextStatement;
				}
			}
		}
		
		/// <summary> Read the specified amount of memory at the given memory address </summary>
		/// <returns> The content of the memory.  The amount of the read memory may be less then requested. </returns>
		public unsafe byte[] ReadMemory(ulong address, int size)
		{
			byte[] buffer = new byte[size];
			int readCount;
			fixed(byte* pBuffer = buffer) {
				readCount = (int)corProcess.ReadMemory(address, (uint)size, new IntPtr(pBuffer));
			}
			if (readCount != size) Array.Resize(ref buffer, readCount);
			return buffer;
		}
		
		/// <summary> Writes the given buffer at the specified memory address </summary>
		/// <returns> The number of bytes written </returns>
		public unsafe int WriteMemory(ulong address, byte[] buffer)
		{
			if (buffer.Length == 0) return 0;
			int written;
			fixed(byte* pBuffer = buffer) {
				written = (int)corProcess.WriteMemory(address, (uint)buffer.Length, new IntPtr(pBuffer));
			}
			return written;
		}
	}
	
	[Serializable]
	public class ProcessEventArgs: DebuggerEventArgs
	{
		Process process;
		
		[Debugger.Tests.Ignore]
		public Process Process {
			get {
				return process;
			}
		}
		
		public ProcessEventArgs(Process process): base(process == null ? null : process.Debugger)
		{
			this.process = process;
		}
	}
}
