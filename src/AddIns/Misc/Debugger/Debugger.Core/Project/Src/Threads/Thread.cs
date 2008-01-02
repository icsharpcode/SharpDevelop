// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

using Debugger.Wrappers.CorDebug;

namespace Debugger
{
	public partial class Thread: DebuggerObject, IExpirable
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

		Function selectedFunction;
		
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
			Debug.Assert(!this.hasExpired);
			
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
				lastPriority = (ThreadPriority)(int)runTimeValue.GetMember("m_Priority").PrimitiveValue;
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
				Value runtimeName = runtimeValue.GetMember("m_Name");
				if (runtimeName.IsNull) return string.Empty;
				lastName = runtimeName.AsString.ToString();
				return lastName;
			}
		}
		
		public bool InterceptCurrentException()
		{
			if (!CorThread.Is<ICorDebugThread2>()) return false; // Is the debuggee .NET 2.0?
			if (LastFunction == null) return false; // Is frame available?  It is not at StackOverflow
			
			try {
				CorThread.CastTo<ICorDebugThread2>().InterceptCurrentException(LastFunction.CorILFrame.CastTo<ICorDebugFrame>());
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
		
		public IList<Function> Callstack {
			get {
				return new List<Function>(CallstackEnum).AsReadOnly();
			}
		}
		
		IEnumerable<Function> CallstackEnum {
			get {
				process.AssertPaused();
				
				foreach(ICorDebugChain corChain in CorThread.EnumerateChains().Enumerator) {
					if (corChain.IsManaged == 0) continue; // Only managed ones
					foreach(ICorDebugFrame corFrame in corChain.EnumerateFrames().Enumerator) {
						if (corFrame.Is<ICorDebugILFrame>()) {
							Function function;
							try {
								function = GetFunctionFromCache(new FrameID(corChain.Index, corFrame.Index), corFrame.As<ICorDebugILFrame>());
							} catch (COMException) { // TODO
								continue;
							};
							yield return function;
						}
					}
				}
			}
		}
		
		Dictionary<FrameID, Function> functionCache = new Dictionary<FrameID, Function>();
		
		Function GetFunctionFromCache(FrameID frameID, ICorDebugILFrame corFrame)
		{
			Function function;
			if (functionCache.TryGetValue(frameID, out function) && !function.HasExpired) {
				function.CorILFrame = corFrame;
				return function;
			} else {
				function = new Function(this, frameID, corFrame);
				functionCache[frameID] = function;
				return function;
			}
		}
		
		internal ICorDebugFrame GetFrameAt(FrameID frameID)
		{
			process.AssertPaused();
			
			ICorDebugChainEnum corChainEnum = CorThread.EnumerateChains();
			if (frameID.ChainIndex >= corChainEnum.Count) throw new ArgumentException("Chain index too big", "chainIndex");
			corChainEnum.Skip(corChainEnum.Count - frameID.ChainIndex - 1);
			
			ICorDebugChain corChain = corChainEnum.Next();
			
			if (corChain.IsManaged == 0) throw new ArgumentException("Chain is not managed", "chainIndex");
			
			ICorDebugFrameEnum corFrameEnum = corChain.EnumerateFrames();
			if (frameID.FrameIndex >= corFrameEnum.Count) throw new ArgumentException("Frame index too big", "frameIndex");
			corFrameEnum.Skip(corFrameEnum.Count - frameID.FrameIndex - 1);
			
			return corFrameEnum.Next();
		}
		
		// See docs\Stepping.txt
		internal void CheckExpiration()
		{
			try {
				ICorDebugChainEnum chainEnum = CorThread.EnumerateChains();
			} catch (COMException e) {
				// 0x8013132D: The state of the thread is invalid.
				// 0x8013134F: Object is in a zombie state
				// 0x80131301: Process was terminated.
				if ((uint)e.ErrorCode == 0x8013132D ||
				    (uint)e.ErrorCode == 0x8013134F ||
				    (uint)e.ErrorCode == 0x80131301) {
					
					this.Expire();
					return;
				} else throw;
			}
			
			if (process.Evaluating) return;
			
			ICorDebugChainEnum corChainEnum = CorThread.EnumerateChains();
			int maxChainIndex = (int)corChainEnum.Count - 1;
			
			ICorDebugFrameEnum corFrameEnum = corChainEnum.Next().EnumerateFrames();
			// corFrameEnum.Count can return 0 in ExitThread callback
			int maxFrameIndex = (int)corFrameEnum.Count - 1;
			
			ICorDebugFrame lastFrame = corFrameEnum.Next();
			
			// Check the token of the current function - function can change if there are multiple handlers for an event
			Function function;
			if (lastFrame != null && 
			    functionCache.TryGetValue(new FrameID((uint)maxChainIndex, (uint)maxFrameIndex), out function) &&
			    function.MethodInfo.MetadataToken != lastFrame.FunctionToken) {
				
				functionCache.Remove(new FrameID((uint)maxChainIndex, (uint)maxFrameIndex));
				function.OnExpired(EventArgs.Empty);
			}

			// Expire all functions behind the current maximum
			// Multiple functions can expire at once (test case: Step out of Button1Click in simple winforms application)
			List<KeyValuePair<FrameID, Function>> toBeRemoved = new List<KeyValuePair<FrameID, Function>>();
			foreach(KeyValuePair<FrameID, Function> kvp in functionCache) {
				if ((kvp.Key.ChainIndex > maxChainIndex) ||
				    (kvp.Key.ChainIndex == maxChainIndex && kvp.Key.FrameIndex > maxFrameIndex)) {
					
					toBeRemoved.Add(kvp);
				}
			}
			foreach(KeyValuePair<FrameID, Function> kvp in toBeRemoved){
				functionCache.Remove(kvp.Key);
				kvp.Value.OnExpired(EventArgs.Empty);
			}
		}
		
		[Debugger.Tests.ToStringOnly]
		public Function SelectedFunction {
			get {
				return selectedFunction;
			}
			set {
				if (value != null && !value.HasSymbols) {
					throw new DebuggerException("SelectedFunction must have symbols");
				}
				
				selectedFunction = value;
			}
		}
		
		[Debugger.Tests.ToStringOnly]
		public Function LastFunctionWithLoadedSymbols {
			get {
				foreach (Function function in CallstackEnum) {
					if (function.HasSymbols) {
						return function;
					}
				}
				return null;
			}
		}
		
		/// <summary>
		/// Returns the most recent function on callstack.
		/// Returns null if callstack is empty.
		/// </summary>
		[Debugger.Tests.ToStringOnly]
		public Function LastFunction {
			get {
				foreach(Function function in CallstackEnum) {
					return function;
				}
				return null;
			}
		}
		
		/// <summary>
		/// Returns the first function that was called on thread
		/// </summary>
		[Debugger.Tests.ToStringOnly]
		public Function FirstFunction {
			get {
				Function first = null;
				foreach(Function function in Callstack) {
					first = function;
				}
				return first;
			}
		}
		
		public bool IsLastFunctionNative {
			get {
				process.AssertPaused();
				return corThread.ActiveChain.IsManaged == 0;
			}
		}
	}
}
