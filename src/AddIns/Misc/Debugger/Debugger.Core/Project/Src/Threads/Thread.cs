// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

using Debugger.Interop.CorDebug;
using Debugger.Interop.MetaData;

namespace Debugger
{
	public partial class Thread: RemotingObjectBase
	{
		NDebugger debugger;

		ICorDebugThread corThread;

		internal ExceptionType currentExceptionType;

		Process process;
		List<ICorDebugStepper> activeSteppers = new List<ICorDebugStepper>();
		List<Stepper> steppers = new List<Stepper>();

		uint id;
		bool lastSuspendedState = false;
		ThreadPriority lastPriority = ThreadPriority.Normal;
		string lastName = string.Empty;
		bool hasBeenLoaded = false;

		Function currentFunction;
		
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
			corThread.GetID(out id);

			ICorDebugProcess corProcess;
			corThread.GetProcess(out corProcess);
			this.process = debugger.GetProcess(corProcess);
		}

		public bool Suspended {
			get	{
				if (process.IsRunning) return lastSuspendedState;

				CorDebugThreadState state;
				corThread.GetDebugState(out state);
				lastSuspendedState = (state == CorDebugThreadState.THREAD_SUSPEND);
				return lastSuspendedState;
			}
			set	{
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

				ICorDebugValue corValue;
				corThread.GetObject(out corValue);
				return Value.CreateValue(debugger, corValue);
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
			if (corThread is ICorDebugThread2) { // Is the debuggee .NET 2.0?
				((ICorDebugThread2)corThread).InterceptCurrentException(LastFunction.CorILFrame);
			}
			process.Continue();
		}
		
		internal Stepper CreateStepper()
		{
			ICorDebugStepper corStepper;
			corThread.CreateStepper(out corStepper);
			
			Stepper stepper = new Stepper(debugger, corStepper);
			stepper.StepComplete += delegate {
				steppers.Remove(stepper);
			};
			
			steppers.Add(stepper);
			
			return stepper;
		}
		
		internal Stepper GetStepper(ICorDebugStepper corStepper)
		{
			foreach(Stepper stepper in steppers) {
				if (stepper.CorStepper == corStepper) {
					return stepper;
				}
			}
			return null;
		}
		
		internal IList<Stepper> Steppers {
			get {
				return steppers.AsReadOnly();
			}
		}
		
		internal IList<ICorDebugStepper> ActiveSteppers {
			get {
				return activeSteppers.AsReadOnly();
			}
		}

		internal void AddActiveStepper(ICorDebugStepper stepper)
		{
			activeSteppers.Add(stepper);
		}

		internal void DeactivateAllSteppers()
		{
			foreach(ICorDebugStepper stepper in activeSteppers) {
				int active;
				stepper.IsActive(out active);
				if (active != 0) {
					stepper.Deactivate();
					debugger.TraceMessage("Stepper deactivated");
				}
			}
			activeSteppers.Clear();
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
				return GetCallstackAt(uint.MaxValue, uint.MaxValue);
			}
		}
		
		internal Function GetFunctionAt(uint firstChainIndex, uint firstFrameIndex)
		{
			foreach(Function f in GetCallstackAt(firstChainIndex, firstFrameIndex)) {
				return f;
			}
			throw new DebuggerException("Function not found");
		}
		
		internal IEnumerable<Function> GetCallstackAt(uint firstChainIndex, uint firstFrameIndex)
		{
			process.AssertPaused();
			
			ICorDebugChainEnum corChainEnum;
			corThread.EnumerateChains(out corChainEnum);
			
			uint chainCount;
			corChainEnum.GetCount(out chainCount);
			
			uint chainIndex = chainCount;
			
			if (firstChainIndex != uint.MaxValue) {
				int skipCount = (int)chainCount - (int)firstChainIndex - 1;
				if (skipCount < 0) throw new ArgumentException("Chain index too big", "firstChainIndex");
				corChainEnum.Skip((uint)skipCount);
				chainIndex -= (uint)skipCount;
				firstChainIndex = chainIndex - 1;
			}
			
			while (true) {
				uint chainsFetched;
				ICorDebugChain[] corChains = new ICorDebugChain[1]; // One at time
				corChainEnum.Next(1, corChains, out chainsFetched);
				if (chainsFetched == 0) break; // We are done
				
				chainIndex--;
				
				CorDebugChainReason reason;
				corChains[0].GetReason(out reason);
				
				int isManaged;
				corChains[0].IsManaged(out isManaged);
				if (isManaged == 0) continue; // Only managed ones
				
				ICorDebugFrameEnum corFrameEnum;
				corChains[0].EnumerateFrames(out corFrameEnum);
				
				uint frameCount;
				corFrameEnum.GetCount(out frameCount);
				
				uint frameIndex = frameCount;
				
				if (firstFrameIndex != uint.MaxValue && chainIndex == firstChainIndex) {
					int skipCount = (int)frameCount - (int)firstFrameIndex - 1;
					if (skipCount < 0) throw new ArgumentException("Frame index too big", "firstFrameIndex");
					corFrameEnum.Skip((uint)skipCount);
					frameIndex -= (uint)skipCount;
				}
				
				while (true) {
					uint framesFetched;
					ICorDebugFrame[] corFrames = new ICorDebugFrame[1]; // Only one at time
					corFrameEnum.Next(1, corFrames, out framesFetched);
					if (framesFetched == 0) break; // We are done
					
					frameIndex--;
					
					Function function = null;
					try {
						if (corFrames[0] is ICorDebugILFrame) {
							function = new Function(this, chainIndex, frameIndex, (ICorDebugILFrame)corFrames[0]);
						}
					} catch (COMException) {
						// TODO
					};
					
					if (function != null) {
						yield return function;
					}
				}
			}
		}
		
		public Function CurrentFunction {
			get {
				process.AssertPaused();
				
				if (currentFunction == null) {
					currentFunction = LastFunctionWithLoadedSymbols;
				}
				
				if (currentFunction != null && currentFunction.HasSymbols) {
					return currentFunction;
				} else {
					return null;
				}
			}
			internal set {
				if (value != null && !value.HasSymbols) {
					throw new DebuggerException("CurrentFunction must have symbols");
				}
				
				currentFunction = value;
			}
		}
		
		public void SetCurrentFunction(Function function)
		{
			CurrentFunction = function;
			
			debugger.FakePause(PausedReason.CurrentFunctionChanged, true);
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
