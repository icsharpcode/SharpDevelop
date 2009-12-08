// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Text;
using System.Threading;

using Microsoft.Win32;

using Debugger.Interop;
using Debugger.Wrappers.CorDebug;

namespace Debugger
{
	public partial class NDebugger: DebuggerObject
	{
		ICorDebug                  corDebug;
		ManagedCallbackSwitch      managedCallbackSwitch;
		ManagedCallbackProxy       managedCallbackProxy;
		
		MTA2STA mta2sta = new MTA2STA();
		
		string debuggeeVersion;
		
		Options options = new Options();
		
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
		
		public Options Options {
			get { return options; }
			set { options = value; }
		}
		
		public NDebugger()
		{
			if (ApartmentState.STA == System.Threading.Thread.CurrentThread.GetApartmentState()) {
				mta2sta.CallMethod = CallMethod.HiddenFormWithTimeout;
			} else {
				mta2sta.CallMethod = CallMethod.DirectCall;
			}
			
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
			sb.Length = size; // TODO: What is the last char?  What is the result for unmanaged apps?
			return sb.ToString();
		}
		
		/// <summary>
		/// Prepares the debugger
		/// </summary>
		/// <param name="debuggeeVersion">Version of the program to debug - eg. "v1.1.4322"
		/// If null, the version of the executing process will be used</param>
		internal void InitDebugger(string debuggeeVersion)
		{
			if (IsKernelDebuggerEnabled) {
				throw new DebuggerException("Can not debug because kernel debugger is enabled");
			}
			if (string.IsNullOrEmpty(debuggeeVersion)) {
				debuggeeVersion = GetDebuggerVersion();
			}
			this.debuggeeVersion = debuggeeVersion;
			
			corDebug = new ICorDebug(NativeMethods.CreateDebuggingInterfaceFromVersion(3, debuggeeVersion));
			
			managedCallbackSwitch = new ManagedCallbackSwitch(this);
			managedCallbackProxy = new ManagedCallbackProxy(this, managedCallbackSwitch);
			
			corDebug.Initialize();
			corDebug.SetManagedHandler(new ICorDebugManagedCallback(managedCallbackProxy));
			
			TraceMessage("ICorDebug initialized, debugee version " + debuggeeVersion);
		}
		
		internal void TerminateDebugger()
		{
			MarkBreakpointsAsDeactivated();
			
			TraceMessage("Reset done");
			
			corDebug.Terminate();
			
			TraceMessage("ICorDebug terminated");
			
			Wrappers.ResourceManager.ReleaseAllTrackedCOMObjects();
			
			TraceMessage("Tracked COM objects released");
		}
		
		/// <summary>
		/// Internal: Used to debug the debugger library.
		/// </summary>
		public event EventHandler<MessageEventArgs> DebuggerTraceMessage;
		
		protected internal virtual void OnDebuggerTraceMessage(MessageEventArgs e)
		{
			if (DebuggerTraceMessage != null) {
				DebuggerTraceMessage(this, e);
			}
		}
		
		internal void TraceMessage(string message)
		{
			System.Diagnostics.Debug.WriteLine("Debugger:" + message);
			OnDebuggerTraceMessage(new MessageEventArgs(null, message));
		}
		
		public void StartWithoutDebugging(System.Diagnostics.ProcessStartInfo psi)
		{
			System.Diagnostics.Process process;
			process = new System.Diagnostics.Process();
			process.StartInfo = psi;
			process.Start();
		}
		
		internal object ProcessIsBeingCreatedLock = new object();
		
		public Process Start(string filename, string workingDirectory, string arguments)		
		{
			InitDebugger(GetProgramVersion(filename));
			lock(ProcessIsBeingCreatedLock) {
				Process process = Process.CreateProcess(this, filename, workingDirectory, arguments);
				// Expose a race conditon
				System.Threading.Thread.Sleep(0);
				AddProcess(process);
				return process;
			}
		}
		
		public Process Attach(System.Diagnostics.Process existingProcess)		
		{
			InitDebugger(GetProgramVersion(existingProcess.MainModule.FileName));
			ICorDebugProcess corDebugProcess = corDebug.DebugActiveProcess((uint)existingProcess.Id, 0);
			Process process = new Process(this, corDebugProcess);
			AddProcess(process);
			return process;
		}
		
		public void Detach()
		{
			DeactivateBreakpoints();
			
			// Detach all processes.
			while (processCollection.Count > 0) {
				Process process = processCollection[0];
				process.Detach();
			}
		}
		
		public bool IsKernelDebuggerEnabled {
			get {
				string systemStartOptions = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\").GetValue("SystemStartOptions", string.Empty).ToString();
				// XP does not have the slash, Vista does have it
				systemStartOptions = ("/" + systemStartOptions).ToLower().Replace(" ", " /");
				if (systemStartOptions.Contains("/nodebug")) {
					// this option overrides the others
					return false;
				}
				if (systemStartOptions.Contains("/debug") || 
				    systemStartOptions.Contains("/crashdebug") || 
				    systemStartOptions.Contains("/debugport") || 
				    systemStartOptions.Contains("/baudrate")) {
					return true;
				} else {
					return false;
				}
			}
		}
		
		/// <summary> Try to load module symbols using the search path defined in the options </summary>
		public void ReloadModuleSymbols()
		{
			foreach(Process process in this.Processes) {
				foreach(Module module in process.Modules) {
					module.LoadSymbols(process.Options.SymbolsSearchPaths);
				}
			}
			TraceMessage("Reloaded symbols");
		}
		
		/// <summary> Reset the just my code status of modules.  Use this after changing any stepping options. </summary>
		public void ResetJustMyCodeStatus()
		{
			foreach(Process process in this.Processes) {
				foreach(Module module in process.Modules) {
					module.ResetJustMyCodeStatus();
				}
			}
			TraceMessage("Just my code reseted");
		}
	}
	
	[Serializable]
	public class DebuggerEventArgs : EventArgs 
	{
		NDebugger debugger;
		
		public NDebugger Debugger {
			get {
				return debugger;
			}
		}
		
		public DebuggerEventArgs(NDebugger debugger)
		{
			this.debugger = debugger;
		}
	}
	
	[Serializable]
	public class MessageEventArgs : ProcessEventArgs
	{
		int level;
		string message;
		string category;
		
		public int Level {
			get {
				return level;
			}
		}
		
		public string Message {
			get {
				return message;
			}
		}
		
		public string Category {
			get {
				return category;
			}
		}
		
		public MessageEventArgs(Process process, string message): this(process, 0, message, String.Empty)
		{
			this.message = message;
		}
		
		public MessageEventArgs(Process process, int level, string message, string category): base(process)
		{
			this.level = level;
			this.message = message;
			this.category = category;
		}
	}
}
