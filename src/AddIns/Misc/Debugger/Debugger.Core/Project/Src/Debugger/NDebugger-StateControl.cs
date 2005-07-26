// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision: 257 $</version>
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
	public partial class NDebugger
	{
		bool isPaused;
		bool pauseOnHandledException = false;
		
		public event EventHandler<DebuggingPausedEventArgs> DebuggingPaused;
		public event EventHandler<DebuggerEventArgs> DebuggingResumed;
		
		public bool PauseOnHandledException {
			get {
				return pauseOnHandledException;
			}
			set {
				pauseOnHandledException = value;
			}
		}

		protected virtual void OnDebuggingPaused(PausedReason reason)
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

		protected virtual void OnDebuggingResumed()
		{
			TraceMessage ("Debugger event: OnDebuggingResumed()");
			if (DebuggingResumed != null) {
				DebuggingResumed(this, new DebuggerEventArgs(this));
			}
		}
		
		public Process CurrentProcess {
			get {
				if (IsRunning) return null;
				if (currentProcess == null) {
					return null;
				} else {
					if (currentProcess.IsRunning) return null;
					return currentProcess;
				}
			}
			set {
				currentProcess = value;
			}
		}
		
		public Thread CurrentThread {
			get {
				if (IsRunning) return null;
				if (CurrentProcess == null) return null;
				return CurrentProcess.CurrentThread;
			}
			set {
				if (CurrentProcess != null) {
					CurrentProcess.CurrentThread = value;
				}
			}
		}
		
		public Function CurrentFunction {
			get {
				if (IsRunning) return null;
				if (CurrentThread == null) {
					return null;
				} else {
					return CurrentThread.CurrentFunction;
				}
			}
			set {
				if (CurrentThread != null) {
					CurrentThread.CurrentFunction = value;
				}
			}
		}
		
		public void AssertPaused()
		{
			if (!IsPaused) {
				throw new DebuggerException("Debugger is not paused.");
			}
		}
		
		public void AssertRunning()
		{
			if (IsPaused) {
				throw new DebuggerException("Debugger is not running.");
			}
		}
		
		public bool IsPaused {
			get {
				return isPaused;
			}
		}
		
		public bool IsRunning {
			get {
				return !isPaused;
			}
		}
		
		internal void Pause(PausedReason reason, Process process, Thread thread)
		{
			if (IsPaused) {
				throw new DebuggerException("Already paused");
			}
			isPaused = true;
			SetUpEnvironment(process, thread);
			OnDebuggingPaused(reason);
		}
		
		internal void FakePause(PausedReason reason)
		{
			Process process = CurrentProcess;
			Thread thread = CurrentThread;
			Resume();
			Pause(reason, process, thread);
		}
		
		internal void Resume()
		{
			if (!IsPaused) {
				throw new DebuggerException("Already resumed");
			}
			OnDebuggingResumed();
			isPaused = false;
		}
		
		void SetUpEnvironment(Process process, Thread thread)
		{
			CleanEnvironment();
				
			// Set the CurrentProcess
			if (process == null) {
				if (thread != null) {
					process = thread.Process;
				}
			}
			currentProcess = process;
			
			// Set the CurrentThread
			if (process != null) {
				if (thread != null) {
					process.SetCurrentThread(thread);
				} else {
					IList<Thread> threads = process.Threads;
					if (threads.Count > 0) {
						process.SetCurrentThread(threads[0]);
					} else {
						process.SetCurrentThread(null);
					}
				}
			}
			
			// CurrentFunctions in threads are fetched on request
		}
		
		void CleanEnvironment()
		{
			// Remove all stored functions,
			// they are disponsed between callbacks and
			// they need to be regenerated
			foreach(Thread t in Threads) {
				t.ClearCurrentFunction();
			}
			
			// Clear current thread
			if (currentProcess != null) {
				currentProcess.SetCurrentThread(null);
			}
			// Clear current process
			currentProcess = null;
		}
		
		public void Break()
		{
			foreach(Process p in Processes) {
				if (p.IsRunning) {
					p.Break();
				}
			}
		}

		public void StepInto()
		{
			CurrentFunction.StepInto();
		}

		public void StepOver()
		{
			CurrentFunction.StepOver();
		}

		public void StepOut()
		{
			CurrentFunction.StepOut();
		}

		public void Continue()
		{
			CurrentProcess.Continue();
		}

		public void Terminate()
		{
			foreach(Process p in Processes) {
				p.Terminate();
			}
		}
	}
}
