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

using DebuggerInterop.Core;
using DebuggerInterop.MetaData;

namespace DebuggerLibrary
{
	public partial class Thread: RemotingObjectBase
	{
		NDebugger debugger;

		ICorDebugThread corThread;

		internal ExceptionType currentExceptionType;

		Process process;
		List<ICorDebugStepper> activeSteppers = new List<ICorDebugStepper>();

		uint id;
		bool lastSuspendedState = false;
		ThreadPriority lastPriority = ThreadPriority.Normal;
		string lastName = string.Empty;
		bool hasBeenLoaded = false;

		Function currentFunction;

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
				if (!process.IsProcessSafeForInspection) return lastSuspendedState;

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
				if (!process.IsProcessSafeForInspection) return lastPriority;

				Variable runTimeVar = RuntimeVariable;
				if (runTimeVar is NullRefVariable) return ThreadPriority.Normal;
				lastPriority = (ThreadPriority)(int)(runTimeVar.SubVariables["m_Priority"] as BuiltInVariable).Value;
				return lastPriority;
			}
		}

		public Variable RuntimeVariable {
			get {
				if (!HasBeenLoaded) throw new DebuggerException("Thread has not started jet");
				process.CheckThatProcessIsSafeForInspection();

				ICorDebugValue corValue;
				corThread.GetObject(out corValue);
				return VariableFactory.CreateVariable(debugger, corValue, "Thread" + ID);
			}
		}

		public string Name {
			get	{
				if (!HasBeenLoaded) return lastName;
				if (!process.IsProcessSafeForInspection) return lastName;
				Variable runtimeVar  = RuntimeVariable;
				if (runtimeVar is NullRefVariable) return lastName;
				Variable runtimeName = runtimeVar.SubVariables["m_Name"];
				if (runtimeName is NullRefVariable) return string.Empty;
				lastName = runtimeName.Value.ToString();
				return lastName;
			}
		}
		
		public void InterceptCurrentException()
		{
			((ICorDebugThread2)corThread).InterceptCurrentException(LastFunction.CorILFrame);
			process.Continue();
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
				ThreadStateChanged(this, new ThreadEventArgs(debugger, this));
		}


		public override string ToString()
		{
			return String.Format("ID = {0,-10} Name = {1,-20} Suspended = {2,-8}", ID, Name, Suspended);
		}


		public Exception CurrentException {
			get {
				return new Exception(debugger, this);
			}
		}

		public unsafe List<Function> Callstack {
			get {
				List<Function> callstack = new List<Function>();

				if (!process.IsProcessSafeForInspection) return callstack;

				ICorDebugChainEnum corChainEnum;
				corThread.EnumerateChains(out corChainEnum);

				while (true) {
					uint chainsFetched;
					ICorDebugChain[] corChains = new ICorDebugChain[1]; // One at time
					corChainEnum.Next(1, corChains, out chainsFetched);
					if (chainsFetched == 0) break; // We are done
					
					int isManaged;
					corChains[0].IsManaged(out isManaged);
					if (isManaged == 0) continue; // Only managed ones

					ICorDebugFrameEnum corFrameEnum;
					corChains[0].EnumerateFrames(out corFrameEnum);

					for(;;)
					{
						uint framesFetched;
						ICorDebugFrame[] corFrames = new ICorDebugFrame[1]; // Only one at time
						corFrameEnum.Next(1, corFrames, out framesFetched);
						if (framesFetched == 0) break; // We are done

                        try {
							if (corFrames[0] is ICorDebugILFrame) {
								callstack.Add(new Function(debugger, (ICorDebugILFrame)corFrames[0]));
							}
                        } catch (COMException) {
							// TODO
						};
					}
				} // for(;;)
				return callstack;
			} // get
		} // End of public FunctionCollection Callstack
		
		public Function CurrentFunction {
			get {
				process.CheckThatProcessIsSafeForInspection();
				
				if (currentFunction == null) {
					currentFunction = LastFunctionWithLoadedSymbols;
				}

				return currentFunction;
			}
			set {
				if (value != null && !value.HasSymbols) {
					throw new DebuggerException("CurrentFunction must have symbols");
				}
				
				currentFunction = value;
				
				if (debugger.ManagedCallback.HandlingCallback == false) {
					debugger.OnDebuggingPaused(PausedReason.CurrentFunctionChanged);
				}
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

		public Function LastFunction {
			get {
				List<Function> callstack = Callstack;
				if (callstack.Count > 0) {
					return Callstack[0];
				} else {
					return null;
				}
			}
		}
	}
}
