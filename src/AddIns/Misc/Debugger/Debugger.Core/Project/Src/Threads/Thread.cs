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
	public partial class Thread: RemotingObjectBase
	{
		NDebugger debugger;

		ICorDebugThread corThread;

		internal ExceptionType currentExceptionType;

		Process process;
		List<Stepper> steppers = new List<Stepper>();

		uint id;
		bool lastSuspendedState = false;
		ThreadPriority lastPriority = ThreadPriority.Normal;
		string lastName = string.Empty;
		bool hasBeenLoaded = false;

		Function selectedFunction;
		
		public NDebugger Debugger {
			get {
				return debugger;
			}
		}

		internal bool HasBeenLoaded {
			get {
				return hasBeenLoaded;
			}
			set {
				hasBeenLoaded = value;
				OnThreadStateChanged();
			}
		}

		public uint ID { 
			get{ 
				return id; 
			} 
		}
		
		public ExceptionType CurrentExceptionType {
			get {
				return currentExceptionType;
			}
			set {
				currentExceptionType = value;
			}
		}

		public Process Process {
			get {
				return process;
			}
		}
		
		public ICorDebugThread CorThread {
			get {
				return corThread;
			}
		}

		internal Thread(NDebugger debugger, ICorDebugThread corThread)
		{
			this.debugger = debugger;
			this.corThread = corThread;
			id = corThread.ID;
			
			this.process = debugger.GetProcess(corThread.Process);
		}
		
		public bool Suspended {
			get {
				if (process.IsRunning) return lastSuspendedState;
				
				lastSuspendedState = (corThread.DebugState == CorDebugThreadState.THREAD_SUSPEND);
				return lastSuspendedState;
			}
			set {
				corThread.SetDebugState((value==true)?CorDebugThreadState.THREAD_SUSPEND:CorDebugThreadState.THREAD_RUN);
			}
		}
		
		public ThreadPriority Priority {
			get {
				if (!HasBeenLoaded) return lastPriority;
				if (process.IsRunning) return lastPriority;

				Value runTimeValue = RuntimeValue;
				if (runTimeValue is NullValue) return ThreadPriority.Normal;
				lastPriority = (ThreadPriority)(int)(runTimeValue["m_Priority"].Value as PrimitiveValue).Primitive;
				return lastPriority;
			}
		}

		public Value RuntimeValue {
			get {
				if (!HasBeenLoaded) throw new DebuggerException("Thread has not started jet");
				process.AssertPaused();
				
				return Value.CreateValue(debugger, corThread.Object);
			}
		}

		public string Name {
			get	{
				if (!HasBeenLoaded) return lastName;
				if (process.IsRunning) return lastName;
				Value runtimeVar  = RuntimeValue;
				if (runtimeVar is NullValue) return lastName;
				Value runtimeName = runtimeVar["m_Name"].Value;
				if (runtimeName is NullValue) return string.Empty;
				lastName = runtimeName.AsString.ToString();
				return lastName;
			}
		}
		
		public void InterceptCurrentException()
		{
			if (corThread.Is<ICorDebugThread2>()) { // Is the debuggee .NET 2.0?
				corThread.CastTo<ICorDebugThread2>().InterceptCurrentException(LastFunction.CorILFrame.CastTo<ICorDebugFrame>());
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

		public event EventHandler<ThreadEventArgs> ThreadStateChanged;

		protected void OnThreadStateChanged()
		{
			if (ThreadStateChanged != null)
				ThreadStateChanged(this, new ThreadEventArgs(this));
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
		
		public IEnumerable<Function> Callstack {
			get {
				process.AssertPaused();
				
				foreach(ICorDebugChain corChain in corThread.EnumerateChains().Enumerator) {
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
			
			ICorDebugChainEnum corChainEnum = corThread.EnumerateChains();
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
		internal void CheckExpirationOfFunctions()
		{
			if (debugger.Evaluating) return;
			
			ICorDebugChainEnum corChainEnum = corThread.EnumerateChains();
			uint maxChainIndex = corChainEnum.Count - 1;
			
			ICorDebugFrameEnum corFrameEnum = corChainEnum.Next().EnumerateFrames();
			uint maxFrameIndex = corFrameEnum.Count - 1;
			
			ICorDebugFrame lastFrame = corFrameEnum.Next();
			
			// Check the token of the current function - function can change if there are multiple handlers for an event
			Function function;
			if (lastFrame != null && 
			    functionCache.TryGetValue(new FrameID(maxChainIndex, maxFrameIndex), out function) &&
			    function.Token != lastFrame.FunctionToken) {
				
				functionCache.Remove(new FrameID(maxChainIndex, maxFrameIndex));
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
		
		public Function LastFunctionWithLoadedSymbols {
			get {
				foreach (Function function in Callstack) {
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
		public Function LastFunction {
			get {
				foreach(Function function in Callstack) {
					return function;
				}
				return null;
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
