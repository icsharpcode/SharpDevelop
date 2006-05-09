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
				
				ICorDebugChainEnum corChainEnum = corThread.EnumerateChains();
				uint chainIndex = corChainEnum.Count;
				foreach(ICorDebugChain corChain in corChainEnum.Enumerator) {
					chainIndex--;
					
					if (corChain.IsManaged == 0) continue; // Only managed ones
					
					ICorDebugFrameEnum corFrameEnum = corChain.EnumerateFrames();
					uint frameIndex = corFrameEnum.Count;
					foreach(ICorDebugFrame corFrame in corFrameEnum.Enumerator) {
						frameIndex--;
						
						if (corFrame.Is<ICorDebugILFrame>()) {
							Function function = GetFunctionFromCache(chainIndex, frameIndex, corFrame.As<ICorDebugILFrame>());
							if (function != null) {
								yield return function;
							}
						}
					}
				}
			}
		}
		
		Dictionary<uint, Chain> chainCache = new Dictionary<uint, Chain>();
		
		class Chain {
			public Dictionary<uint, Function> Frames = new Dictionary<uint, Function>();
		}
		
		Function GetFunctionFromCache(uint chainIndex, uint frameIndex, ICorDebugILFrame corFrame)
		{
			try {
				if (chainCache.ContainsKey(chainIndex) &&
				    chainCache[chainIndex].Frames.ContainsKey(frameIndex) &&
				    !chainCache[chainIndex].Frames[frameIndex].HasExpired) {
					
					Function function = chainCache[chainIndex].Frames[frameIndex];
					function.CorILFrame = corFrame;
					return function;
				} else {
					Function function = new Function(this, chainIndex, frameIndex, corFrame.CastTo<ICorDebugILFrame>());
					if (!chainCache.ContainsKey(chainIndex)) chainCache[chainIndex] = new Chain();
					chainCache[chainIndex].Frames[frameIndex] = function;
					function.Expired += delegate { chainCache[chainIndex].Frames.Remove(frameIndex); };
					return function;
				}
			} catch (COMException) { // TODO
				return null;
			};
		}
		
		internal ICorDebugFrame GetFrameAt(uint chainIndex, uint frameIndex)
		{
			process.AssertPaused();
			
			ICorDebugChainEnum corChainEnum = corThread.EnumerateChains();
			if (chainIndex >= corChainEnum.Count) throw new ArgumentException("Chain index too big", "chainIndex");
			corChainEnum.Skip(corChainEnum.Count - chainIndex - 1);
			
			ICorDebugChain corChain = corChainEnum.Next();
			
			if (corChain.IsManaged == 0) throw new ArgumentException("Chain is not managed", "chainIndex");
			
			ICorDebugFrameEnum corFrameEnum = corChain.EnumerateFrames();
			if (frameIndex >= corFrameEnum.Count) throw new ArgumentException("Frame index too big", "frameIndex");
			corFrameEnum.Skip(corFrameEnum.Count - frameIndex - 1);
			
			return corFrameEnum.Next();
		}
		
		// NOTE: During evlulation some chains may be temporaly removed
		internal void CheckExpirationOfFunctions()
		{
			if (debugger.Evaluating) return;
			
			ICorDebugChainEnum corChainEnum = corThread.EnumerateChains();
			uint maxChainIndex = corChainEnum.Count - 1;
			
			ICorDebugFrameEnum corFrameEnum = corChainEnum.Next().EnumerateFrames();
			uint maxFrameIndex = corFrameEnum.Count - 1;
			
			List<Function> expiredFunctions = new List<Function>();
			
			foreach(KeyValuePair<uint, Chain> chain in chainCache) {
				if (chain.Key < maxChainIndex) continue;
				foreach(KeyValuePair<uint, Function> func in chain.Value.Frames) {
					if (chain.Key == maxChainIndex && func.Key <= maxFrameIndex) continue;
					expiredFunctions.Add(func.Value);
				}
			}
			
			foreach(Function f in expiredFunctions) {
				f.OnExpired(EventArgs.Empty);
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
	}
}
