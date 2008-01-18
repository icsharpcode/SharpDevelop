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
	public partial class Thread: DebuggerObject
	{
		Process process;

		ICorDebugThread corThread;

		internal ExceptionType currentExceptionType;

		List<Stepper> steppers = new List<Stepper>();

		uint id;
		bool lastSuspendedState = false;
		ThreadPriority lastPriority = ThreadPriority.Normal;
		string lastName = string.Empty;
		bool hasBeenLoaded = false;
		
		bool hasExpired = false;
		bool nativeThreadExited = false;

		StackFrame selectedStackFrame;
		
		public event EventHandler Expired;
		public event EventHandler<ThreadEventArgs> NativeThreadExited;
		
		public bool HasExpired {
			get {
				return hasExpired;
			}
		}
		
		[Debugger.Tests.Ignore]
		public Process Process {
			get {
				return process;
			}
		}

		internal bool HasBeenLoaded {
			get {
				return hasBeenLoaded;
			}
			set {
				hasBeenLoaded = value;
				OnStateChanged();
			}
		}
		
		[Debugger.Tests.Ignore]
		public uint ID { 
			get{ 
				return id; 
			} 
		}
		
		[Debugger.Tests.Ignore]
		public ExceptionType CurrentExceptionType {
			get {
				return currentExceptionType;
			}
			set {
				currentExceptionType = value;
			}
		}
		
		public bool IsAtSafePoint {
			get {
				return CorThread.UserState != CorDebugUserState.USER_UNSAFE_POINT;
			}
		}
		
		internal ICorDebugThread CorThread {
			get {
				if (nativeThreadExited) {
					throw new DebuggerException("Native thread has exited");
				}
				return corThread;
			}
		}
		
		internal Thread(Process process, ICorDebugThread corThread)
		{
			this.process = process;
			this.corThread = corThread;
			this.id = CorThread.ID;
		}
		
		void Expire()
		{
			System.Diagnostics.Debug.Assert(!this.hasExpired);
			
			process.TraceMessage("Thread " + this.ID + " expired");
			this.hasExpired = true;
			OnExpired(new ThreadEventArgs(this));
			if (process.SelectedThread == this) {
				process.SelectedThread = null;
			}
		}
		
		protected virtual void OnExpired(EventArgs e)
		{
			if (Expired != null) {
				Expired(this, e);
			}
		}
		
		internal void NotifyNativeThreadExited()
		{
			if (!this.hasExpired) Expire();
			
			nativeThreadExited = true;
			OnNativeThreadExited(new ThreadEventArgs(this));
		}
		
		protected virtual void OnNativeThreadExited(ThreadEventArgs e)
		{
			if (NativeThreadExited != null) {
				NativeThreadExited(this, e);
			}
		}
		
		public bool Suspended {
			get {
				if (process.IsRunning) return lastSuspendedState;
				
				lastSuspendedState = (CorThread.DebugState == CorDebugThreadState.THREAD_SUSPEND);
				return lastSuspendedState;
			}
			set {
				CorThread.SetDebugState((value==true)?CorDebugThreadState.THREAD_SUSPEND:CorDebugThreadState.THREAD_RUN);
			}
		}
		
		public ThreadPriority Priority {
			get {
				if (!HasBeenLoaded) return lastPriority;
				if (process.IsRunning) return lastPriority;

				Value runTimeValue = RuntimeValue;
				if (runTimeValue.IsNull) return ThreadPriority.Normal;
				lastPriority = (ThreadPriority)(int)runTimeValue.GetMemberValue("m_Priority").PrimitiveValue;
				return lastPriority;
			}
		}

		public Value RuntimeValue {
			get {
				if (!HasBeenLoaded) throw new DebuggerException("Thread has not started jet");
				process.AssertPaused();
				
				return new Value(process, CorThread.Object);
			}
		}
		
		public string Name {
			get {
				if (!HasBeenLoaded) return lastName;
				if (process.IsRunning) return lastName;
				Value runtimeValue  = RuntimeValue;
				if (runtimeValue.IsNull) return lastName;
				Value runtimeName = runtimeValue.GetMemberValue("m_Name");
				if (runtimeName.IsNull) return string.Empty;
				lastName = runtimeName.AsString.ToString();
				return lastName;
			}
		}
		
		public bool InterceptCurrentException()
		{
			if (!CorThread.Is<ICorDebugThread2>()) return false; // Is the debuggee .NET 2.0?
			if (LastStackFrame == null) return false; // Is frame available?  It is not at StackOverflow
			
			try {
				CorThread.CastTo<ICorDebugThread2>().InterceptCurrentException(LastStackFrame.CorILFrame.CastTo<ICorDebugFrame>());
				return true;
			} catch (COMException e) {
				// 0x80131C02: Cannot intercept this exception
				if ((uint)e.ErrorCode == 0x80131C02) {
					return false;
				}
				throw;
			}
		}
		
		internal Stepper GetStepper(ICorDebugStepper corStepper)
		{
			foreach(Stepper stepper in steppers) {
				if (stepper.IsCorStepper(corStepper)) {
					return stepper;
				}
			}
			throw new DebuggerException("Stepper is not in collection");
		}
		
		internal List<Stepper> Steppers {
			get {
				return steppers;
			}
		}
		
		public event EventHandler<ThreadEventArgs> StateChanged;
		
		protected void OnStateChanged()
		{
			if (StateChanged != null) {
				StateChanged(this, new ThreadEventArgs(this));
			}
		}
		
		
		public override string ToString()
		{
			return String.Format("ID = {0,-10} Name = {1,-20} Suspended = {2,-8}", ID, Name, Suspended);
		}
		
		
		public Exception CurrentException {
			get {
				return new Exception(this);
			}
		}
		
		public IEnumerable<StackFrame> Callstack {
			get {
				// This should be enum since callstacks can get big
				return CallstackEnum;
			}
		}
		
		IEnumerable<StackFrame> CallstackEnum {
			get {
				process.AssertPaused();
				
				int depth = 0;
				foreach(ICorDebugChain corChain in CorThread.EnumerateChains().Enumerator) {
					if (corChain.IsManaged == 0) continue; // Only managed ones
					foreach(ICorDebugFrame corFrame in corChain.EnumerateFrames().Enumerator) {
						if (corFrame.Is<ICorDebugILFrame>()) {
							StackFrame stackFrame;
							try {
								stackFrame = new StackFrame(this, corFrame.CastTo<ICorDebugILFrame>(), depth);
								depth++;
							} catch (COMException) { // TODO
								continue;
							};
							yield return stackFrame;
						}
					}
				}
			}
		}
		
		public StackFrame SelectedStackFrame {
			get {
				// Forum-20456: Do not return expired StackFrame
				if (selectedStackFrame != null && selectedStackFrame.HasExpired) return null;
				return selectedStackFrame;
			}
			set {
				if (value != null && !value.HasSymbols) {
					throw new DebuggerException("SelectedFunction must have symbols");
				}
				selectedStackFrame = value;
			}
		}
		
		public StackFrame LastStackFrameWithLoadedSymbols {
			get {
				foreach (StackFrame stackFrame in CallstackEnum) {
					if (stackFrame.HasSymbols) {
						return stackFrame;
					}
				}
				return null;
			}
		}
		
		/// <summary>
		/// Returns the most recent stack frame on callstack.
		/// Returns null if callstack is empty.
		/// </summary>
		public StackFrame LastStackFrame {
			get {
				foreach(StackFrame stackFrame in CallstackEnum) {
					return stackFrame;
				}
				return null;
			}
		}
		
		/// <summary>
		/// Returns the first stack frame that was called on thread
		/// </summary>
		public StackFrame FirstStackFrame {
			get {
				StackFrame first = null;
				foreach(StackFrame stackFrame in Callstack) {
					first = stackFrame;
				}
				return first;
			}
		}
		
		public bool IsLastStackFrameNative {
			get {
				process.AssertPaused();
				return corThread.ActiveChain.IsManaged == 0;
			}
		}
	}
}
