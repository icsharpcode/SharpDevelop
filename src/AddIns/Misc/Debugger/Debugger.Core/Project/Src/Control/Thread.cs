// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using Debugger.Expressions;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Debugger.Wrappers.CorDebug;

namespace Debugger
{
	public partial class Thread: DebuggerObject
	{
		Process process;
		
		uint id;
		ICorDebugThread corThread;
		bool hasExited = false;
		
		Stepper currentStepIn;
		List<Stepper> steppers = new List<Stepper>();
		
		StackFrame selectedStackFrame;
		
		Exception     currentException;
		DebuggeeState currentException_DebuggeeState;
		ExceptionType currentExceptionType;
		bool          currentExceptionIsUnhandled;

		public event EventHandler<ThreadEventArgs> NameChanged;
		public event EventHandler<ThreadEventArgs> Exited;
		
		[Debugger.Tests.Ignore]
		public Process Process {
			get { return process; }
		}

		[Debugger.Tests.Ignore]
		public uint ID { 
			get{ return id; } 
		}
		
		public bool HasExited {
			get { return hasExited; }
			private set { hasExited = value; }
		}
		
		internal Stepper CurrentStepIn {
			get { return currentStepIn; }
			set { currentStepIn = value; }
		}
		
		/// <summary> From time to time the thread may be in invalid state. </summary>
		public bool IsInValidState {
			get {
				try {
					CorThread.UserState.ToString();
					return true;
				} catch (COMException e) {
					// The state of the thread is invalid.
					if ((uint)e.ErrorCode == 0x8013132D) {
						return false;
					}
					throw;
				}
			}
		}
		
		internal ICorDebugThread CorThread {
			get {
				if (hasExited) {
					throw new DebuggerException("Thread has exited");
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
		
		internal void NotifyExited()
		{
			if (this.hasExited) throw new DebuggerException("Already exited");
			
			process.TraceMessage("Thread " + this.ID + " exited");
			if (process.SelectedThread == this) {
				process.SelectedThread = null;
			}
			
			this.HasExited = true;
			OnExited(new ThreadEventArgs(this));
		}
		
		protected virtual void OnExited(ThreadEventArgs e)
		{
			if (Exited != null) {
				Exited(this, e);
			}
		}
		
		/// <summary> If the thread is not at safe point, it is not posible to evaluate
		/// on it </summary>
		/// <remarks> Returns false if the thread is in invalid state </remarks>
		public bool IsAtSafePoint {
			get {
				process.AssertPaused();
				
				if (!IsInValidState) return false;
				
				return CorThread.UserState != CorDebugUserState.USER_UNSAFE_POINT;
			}
		}
		
		/// <remarks> Returns false if the thread is in invalid state </remarks>
		public bool Suspended {
			get {
				process.AssertPaused();
				
				if (!IsInValidState) return false;
				
				return (CorThread.DebugState == CorDebugThreadState.THREAD_SUSPEND);
			}
			set {
				CorThread.SetDebugState((value==true) ? CorDebugThreadState.THREAD_SUSPEND : CorDebugThreadState.THREAD_RUN);
			}
		}
		
		/// <remarks> Returns Normal if the thread is in invalid state </remarks>
		public ThreadPriority Priority {
			get {
				process.AssertPaused();
				
				if (!IsInValidState) return ThreadPriority.Normal;

				Value runTimeValue = RuntimeValue;
				if (runTimeValue.IsNull) return ThreadPriority.Normal;
				return (ThreadPriority)(int)runTimeValue.GetMemberValue("m_Priority").PrimitiveValue;
			}
		}
		
		/// <summary> Returns value representing the System.Threading.Thread object </summary>
		/// <remarks> The value is null while the thread is being created (the CreateThread callback) and
		/// it may stay null during the run of the program. (probaly until the debuggee accesses
		/// the System.Threading.Thread object which forces the creation)</remarks>
		public Value RuntimeValue {
			get {
				process.AssertPaused();
				
				ICorDebugValue corValue = this.CorThread.Object;
				return new Value(process, new EmptyExpression(), corValue);
			}
		}
		
		/// <remarks> Returns empty string if the thread is in invalid state </remarks>
		public string Name {
			get {
				process.AssertPaused();
				
				if (!IsInValidState) return string.Empty;
				Value runtimeValue = RuntimeValue;
				if (runtimeValue.IsNull) return string.Empty;
				Value runtimeName = runtimeValue.GetMemberValue("m_Name");
				if (runtimeName.IsNull) return string.Empty;
				return runtimeName.AsString.ToString();
			}
		}
		
		protected virtual void OnNameChanged(ThreadEventArgs e)
		{
			if (NameChanged != null) {
				NameChanged(this, e);
			}
		}
		
		internal void NotifyNameChanged()
		{
			OnNameChanged(new ThreadEventArgs(this));
		}
		
		public Exception CurrentException {
			get {
				if (currentException_DebuggeeState == this.Process.DebuggeeState) {
					return currentException;
				} else {
					return null;
				}
			}
			internal set { currentException = value; }
		}
		
		internal DebuggeeState CurrentException_DebuggeeState {
			get { return currentException_DebuggeeState; }
			set { currentException_DebuggeeState = value; }
		}
		
		public ExceptionType CurrentExceptionType {
			get { return currentExceptionType; }
			internal set { currentExceptionType = value; }
		}
		
		public bool CurrentExceptionIsUnhandled {
			get { return currentExceptionIsUnhandled; }
			internal set { currentExceptionIsUnhandled = value; }
		}
		
		/// <summary> Tryies to intercept the current exception.
		/// The intercepted expression stays available through the CurrentException property. </summary>
		/// <returns> False, if the exception was already intercepted or 
		/// if it can not be intercepted. </returns>
		public bool InterceptCurrentException()
		{
			if (!this.CorThread.Is<ICorDebugThread2>()) return false; // Is the debuggee .NET 2.0?
			if (this.CorThread.CurrentException == null) return false; // Is there any exception
			if (this.MostRecentStackFrame == null) return false; // Is frame available?  It is not at StackOverflow
			
			try {
				// Interception will expire the CorValue so keep permanent reference
				currentException.MakeValuePermanent();
				this.CorThread.CastTo<ICorDebugThread2>().InterceptCurrentException(this.MostRecentStackFrame.CorILFrame.CastTo<ICorDebugFrame>());
			} catch (COMException e) {
				// 0x80131C02: Cannot intercept this exception
				if ((uint)e.ErrorCode == 0x80131C02)
					return false;
				// 0x80131C33: Interception of the current exception is not legal
				if ((uint)e.ErrorCode == 0x80131C33)
					return false;
				throw;
			} catch (ArgumentException) {
				// May happen in release code with does not have any symbols
				return false;
			}
			
			Process.AsyncContinue(DebuggeeStateAction.Keep);
			Process.WaitForPause();
			return true;
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
		
		public override string ToString()
		{
			return String.Format("Thread Name = {1} Suspended = {2}", ID, Name, Suspended);
		}
		
		/// <summary> Gets the whole callstack of the Thread. </summary>
		/// <remarks> If the thread is in invalid state returns empty array </remarks>
		public StackFrame[] GetCallstack()
		{
			return new List<StackFrame>(CallstackEnum).ToArray();
		}
		
		/// <summary> Get given number of frames from the callstack </summary>
		public StackFrame[] GetCallstack(int maxFrames)
		{
			List<StackFrame> frames = new List<StackFrame>();
			foreach(StackFrame frame in CallstackEnum) {
				frames.Add(frame);
				if (frames.Count == maxFrames) break;
			}
			return frames.ToArray();
		}
		
		IEnumerable<StackFrame> CallstackEnum {
			get {
				process.AssertPaused();
				
				if (!IsInValidState) {
					yield break;
				}
				
				foreach(ICorDebugChain corChain in CorThread.EnumerateChains().Enumerator) {
					if (corChain.IsManaged == 0) continue; // Only managed ones
					foreach(ICorDebugFrame corFrame in corChain.EnumerateFrames().Enumerator) {
						if (!corFrame.Is<ICorDebugILFrame>()) continue; // Only IL frames
						StackFrame stackFrame;
						try {
							stackFrame = new StackFrame(this, corFrame.CastTo<ICorDebugILFrame>(), corChain.Index, corFrame.Index);
						} catch (COMException) { // TODO
							continue;
						};
						yield return stackFrame;
					}
				}
			}
		}
		
		internal StackFrame GetStackFrameAt(uint chainIndex, uint frameIndex)
		{
			process.AssertPaused();
			
			ICorDebugChainEnum corChainEnum = CorThread.EnumerateChains();
			if (chainIndex >= corChainEnum.Count) throw new DebuggerException("The requested chain index is too big");
			corChainEnum.Skip(corChainEnum.Count - chainIndex - 1);
			ICorDebugChain corChain = corChainEnum.Next();
			
			if (corChain.IsManaged == 0) throw new DebuggerException("The requested chain is not managed");
			
			ICorDebugFrameEnum corFrameEnum = corChain.EnumerateFrames();
			if (frameIndex >= corFrameEnum.Count) throw new DebuggerException("The requested frame index is too big");
			corFrameEnum.Skip(corFrameEnum.Count - frameIndex - 1);
			ICorDebugFrame corFrame = corFrameEnum.Next();
			
			if (!corFrame.Is<ICorDebugILFrame>()) throw new DebuggerException("The rquested frame is not IL frame");
			
			StackFrame stackFrame = new StackFrame(this, corFrame.CastTo<ICorDebugILFrame>(), chainIndex, frameIndex);
			
			return stackFrame;
		}
		
		public string GetStackTrace()
		{
			return GetStackTrace("at {0} in {1}:line {2}", "at {0}");
		}
		
		public string GetStackTrace(string formatSymbols, string formatNoSymbols)
		{
			StringBuilder stackTrace = new StringBuilder();
			foreach(StackFrame stackFrame in this.GetCallstack(100)) {
				SourcecodeSegment loc = stackFrame.NextStatement;
				stackTrace.Append("   ");
				if (loc != null) {
					stackTrace.AppendFormat(formatSymbols, stackFrame.MethodInfo.FullName, loc.Filename, loc.StartLine);
				} else {
					stackTrace.AppendFormat(formatNoSymbols, stackFrame.MethodInfo.FullName);
				}
				stackTrace.AppendLine();
			}
			return stackTrace.ToString();
		}
		
		public StackFrame SelectedStackFrame {
			get {
				if (selectedStackFrame != null && selectedStackFrame.IsInvalid) return null;
				if (process.IsRunning) return null;
				return selectedStackFrame;
			}
			set {
				if (value != null && !value.HasSymbols) {
					throw new DebuggerException("SelectedFunction must have symbols");
				}
				selectedStackFrame = value;
			}
		}
		
		#region Convenience methods
		
		public StackFrame MostRecentStackFrameWithLoadedSymbols {
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
		/// Returns the most recent stack frame (the one that is currently executing).
		/// Returns null if callstack is empty.
		/// </summary>
		public StackFrame MostRecentStackFrame {
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
		public StackFrame OldestStackFrame {
			get {
				StackFrame first = null;
				foreach(StackFrame stackFrame in CallstackEnum) {
					first = stackFrame;
				}
				return first;
			}
		}
		
		#endregion
		
		public bool IsMostRecentStackFrameNative {
			get {
				process.AssertPaused();
				if (this.IsInValidState) {
					return corThread.ActiveChain.IsManaged == 0;
				} else {
					return false;
				}
			}
		}
	}
	
	[Serializable]
	public class ThreadEventArgs : ProcessEventArgs
	{
		Thread thread;
		
		[Debugger.Tests.Ignore]
		public Thread Thread {
			get {
				return thread;
			}
		}
		
		public ThreadEventArgs(Thread thread): base(thread.Process)
		{
			this.thread = thread;
		}
	}
}
