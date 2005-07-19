using System;
using System.Collections.Generic;
using System.Text;
using DebuggerInterop.Core;

namespace DebuggerLibrary
{
	public partial class NDebugger
	{
		List<Process> processCollection = new List<Process>();

		Process currentProcess;

		public event ProcessEventHandler ProcessStarted;
		public event ProcessEventHandler ProcessExited;

		public Process CurrentProcess {
			get {
				if (currentProcess == null && Processes.Count > 0) {
					currentProcess = Processes[0];
				}
				return currentProcess;
			}
			set {
				currentProcess = value;
			}
		}

		public IList<Process> Processes {
			get {
				return processCollection.AsReadOnly();
			}
		}

		internal Process GetProcess(ICorDebugProcess corProcess)
		{
			foreach (Process process in Processes) {
				if (process.CorProcess == corProcess) {
					return process;
				}
			}
			throw new DebuggerException("Process is not in collection");
		}

		internal void AddProcess(Process process)
		{
			processCollection.Add(process);
			OnProcessStarted(process);
		}

		internal void RemoveProcess(Process process)
		{
			processCollection.Remove(process);
			OnProcessExited(process);
		}

		protected virtual void OnProcessStarted(Process process)
		{
			if (ProcessStarted != null) {
				ProcessStarted(this, new ProcessEventArgs(this, process));
			}
		}

		protected virtual void OnProcessExited(Process process)
		{
			if (ProcessExited != null) {
				ProcessExited(this, new ProcessEventArgs(this, process));
			}
		}
	}
}
