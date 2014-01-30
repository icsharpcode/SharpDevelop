// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

using Debugger.Interop.CorDebug;

namespace Debugger
{
	public enum ExceptionType
	{
		FirstChance = 1,
		UserFirstChance = 2,
		CatchHandlerFound = 3,
		Unhandled = 4,
	}
		
	public class Thread: DebuggerObject
	{
		// AppDomain for thread can be changing
		Process   process;
		
		uint id;
		ICorDebugThread corThread;
		bool hasExited = false;
		
		Stepper currentStepIn;
		
		public Process Process {
			get { return process; }
		}
		
		public AppDomain AppDomain {
			get { return process.GetAppDomain(this.corThread.GetAppDomain()); }
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
		
		[Debugger.Tests.Ignore]
		public ExceptionType CurrentExceptionType { get; set; }
		
		[Debugger.Tests.Ignore]
		public Value CurrentException {
			get {
				return new Value(this.AppDomain, this.CorThread.GetCurrentException());
			}
		}
		
		/// <summary> From time to time the thread may be in invalid state. </summary>
		public bool IsInValidState {
			get {
				try {
					CorThread.GetUserState().ToString();
					CorThread.EnumerateChains();
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
			this.id = CorThread.GetID();
		}
		
		internal void NotifyExited()
		{
			if (this.HasExited) throw new DebuggerException("Already exited");
			
			process.TraceMessage("Thread " + this.ID + " exited");
			
			this.HasExited = true;
			process.threads.Remove(this);
		}
		
		/// <summary> If the thread is not at safe point, it is not posible to evaluate
		/// on it </summary>
		/// <remarks> Returns false if the thread is in invalid state </remarks>
		public bool IsAtSafePoint {
			get {
				process.AssertPaused();
				
				if (!IsInValidState) return false;
				
				return CorThread.GetUserState() != CorDebugUserState.USER_UNSAFE_POINT;
			}
		}
		
		public bool Suspended {
			get {
				process.AssertPaused();
				
				if (!IsInValidState) return false;
				
				return (CorThread.GetDebugState() == CorDebugThreadState.THREAD_SUSPEND);
			}
			set {
				CorThread.SetDebugState(value ? CorDebugThreadState.THREAD_SUSPEND : CorDebugThreadState.THREAD_RUN);
			}
		}
		
		/// <remarks> Returns Normal if the thread is in invalid state </remarks>
		public ThreadPriority Priority {
			get {
				process.AssertPaused();
				
				if (!IsInValidState) return ThreadPriority.Normal;

				Value runTimeValue = RuntimeValue;
				if (runTimeValue.IsNull) return ThreadPriority.Normal;
				return (ThreadPriority)(int)runTimeValue.GetFieldValue("m_Priority").PrimitiveValue;
			}
		}
		
		/// <summary> Returns value representing the System.Threading.Thread object </summary>
		/// <remarks> The value is null while the thread is being created (the CreateThread callback) and
		/// it may stay null during the run of the program. (probaly until the debuggee accesses
		/// the System.Threading.Thread object which forces the creation)</remarks>
		public Value RuntimeValue {
			get {
				process.AssertPaused();
				
				ICorDebugValue corValue = this.CorThread.GetObject();
				return new Value(process.GetAppDomain(this.CorThread.GetAppDomain()), corValue);
			}
		}
		
		/// <remarks> Returns empty string if the thread is in invalid state </remarks>
		public string Name {
			get {
				process.AssertPaused();
				
				if (!IsInValidState) return string.Empty;
				Value runtimeValue = RuntimeValue;
				if (runtimeValue.IsNull) return string.Empty;
				Value runtimeName = runtimeValue.GetFieldValue("m_Name");
				if (runtimeName.IsNull) return string.Empty;
				return runtimeName.AsString(100);
			}
		}
		
		/// <summary> Tryies to intercept the current exception. </summary>
		/// <returns> False, if the exception can not be intercepted. </returns>
		public bool InterceptException()
		{
			if (!(this.CorThread is ICorDebugThread2)) return false; // Is the debuggee .NET 2.0?
			if (this.CorThread.GetCurrentException() == null) return false; // Is there any exception
			if (this.MostRecentStackFrame == null) return false; // Is frame available?  It is not at StackOverflow
			
			// Intercepting an exception on an optimized/NGENed frame seems to sometimes
			// freeze the debugee (as of .NET 4.0, it was ok in .NET 2.0)
			// eg. Convert.ToInt64(ulong.MaxValue) causes such freeze
			StackFrame mostRecentUnoptimized = null;
			foreach(StackFrame sf in this.Callstack) {
				if (sf.Module.CorModule2.GetJITCompilerFlags() != 1) { // CORDEBUG_JIT_DEFAULT 
					mostRecentUnoptimized = sf;
					break;
				}
			}
			if (mostRecentUnoptimized == null) return false;
			
			try {
				((ICorDebugThread2)this.CorThread).InterceptCurrentException(mostRecentUnoptimized.CorILFrame);
			} catch (COMException e) {
				// 0x80131C02: Cannot intercept this exception
				if ((uint)e.ErrorCode == 0x80131C02)
					return false;
				// 0x80131C33: Interception of the current exception is not legal
				if ((uint)e.ErrorCode == 0x80131C33)
					return false;
				// 0x80004005: Error HRESULT E_FAIL has been returned from a call to a COM component.
				// Use this to reproduce: new FileIOPermission(PermissionState.Unrestricted).Deny();
				if ((uint)e.ErrorCode == 0x80004005)
					return false;
				throw;
			} catch (ArgumentException) {
				// May happen in release code with does not have any symbols
				return false;
			}
			process.AsyncContinue(DebuggeeStateAction.Keep);
			process.WaitForPause();
			return true;
		}
		
		public override string ToString()
		{
			return String.Format("Thread Name = {1} Suspended = {2}", ID, Name, Suspended);
		}
		
		/// <summary> Gets the whole callstack of the Thread. </summary>
		/// <remarks> If the thread is in invalid state returns empty array </remarks>
		[Debugger.Tests.Ignore]
		public StackFrame[] GetCallstack()
		{
			return new List<StackFrame>(Callstack).ToArray();
		}
		
		/// <summary> Get given number of frames from the callstack </summary>
		public StackFrame[] GetCallstack(int maxFrames)
		{
			List<StackFrame> frames = new List<StackFrame>();
			foreach(StackFrame frame in Callstack) {
				frames.Add(frame);
				if (frames.Count == maxFrames) break;
			}
			return frames.ToArray();
		}
		
		public IEnumerable<StackFrame> Callstack {
			get {
				process.AssertPaused();
				
				if (!IsInValidState) {
					yield break;
				}
				
				uint corChainIndex = CorThread.EnumerateChains().GetCount();
				foreach(ICorDebugChain corChain in CorThread.EnumerateChains().GetEnumerator()) {
					corChainIndex--;
					if (corChain.IsManaged() == 0) continue; // Only managed ones
					uint corFrameIndex = corChain.EnumerateFrames().GetCount();
					foreach(ICorDebugFrame corFrame in corChain.EnumerateFrames().GetEnumerator()) {
						corFrameIndex--;
						if (!(corFrame is ICorDebugILFrame)) continue; // Only IL frames
						StackFrame stackFrame;
						try {
							stackFrame = new StackFrame(this, (ICorDebugILFrame)corFrame, corChainIndex, corFrameIndex);
						} catch (COMException) { // TODO: Remove
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
			if (chainIndex >= corChainEnum.GetCount()) throw new DebuggerException("The requested chain index is too big");
			corChainEnum.Skip(corChainEnum.GetCount() - chainIndex - 1);
			ICorDebugChain corChain = corChainEnum.Next();
			
			if (corChain.IsManaged() == 0) throw new DebuggerException("The requested chain is not managed");
			
			ICorDebugFrameEnum corFrameEnum = corChain.EnumerateFrames();
			if (frameIndex >= corFrameEnum.GetCount()) throw new DebuggerException("The requested frame index is too big");
			corFrameEnum.Skip(corFrameEnum.GetCount() - frameIndex - 1);
			ICorDebugFrame corFrame = corFrameEnum.Next();
			
			if (!(corFrame is ICorDebugILFrame)) throw new DebuggerException("The rquested frame is not IL frame");
			
			StackFrame stackFrame = new StackFrame(this, (ICorDebugILFrame)corFrame, chainIndex, frameIndex);
			
			return stackFrame;
		}
		
		[Debugger.Tests.Ignore]
		public string GetStackTrace()
		{
			return GetStackTrace("at {0} in {1}:line {2}", "at {0}");
		}
		
		public string GetStackTrace(string formatSymbols, string formatNoSymbols)
		{
			StringBuilder stackTrace = new StringBuilder();
			foreach(StackFrame stackFrame in this.GetCallstack(100)) {
				SequencePoint loc = stackFrame.NextStatement;
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
		
		/// <summary>
		/// Returns the most recent stack frame (the one that is currently executing).
		/// Returns null if callstack is empty.
		/// </summary>
		public StackFrame MostRecentStackFrame {
			get {
				foreach(StackFrame stackFrame in this.Callstack) {
					return stackFrame;
				}
				return null;
			}
		}
		
		[Debugger.Tests.Ignore]
		public StackFrame MostRecentUserStackFrame {
			get {
				foreach (StackFrame stackFrame in this.Callstack) {
					if (!stackFrame.IsNonUserCode) {
						return stackFrame;
					}
				}
				return null;
			}
		}
		
		public bool IsInNativeCode {
			get {
				process.AssertPaused();
				if (this.IsInValidState) {
					return corThread.GetActiveChain().IsManaged() == 0;
				} else {
					return false;
				}
			}
		}
	}
}
