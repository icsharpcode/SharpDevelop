// <file>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
// </file>

using System;
using System.Runtime.InteropServices;
using System.Threading;

using DebuggerInterop.Core;
using DebuggerInterop.Symbols;
using DebuggerInterop.MetaData;

namespace DebuggerLibrary
{
	public class Thread
	{
		internal bool currentExceptionIsHandled;

		uint id;
		bool lastSuspendedState = false;
		ThreadPriority lastPriority = ThreadPriority.Normal;
		string lastName = string.Empty;
		
		readonly ICorDebugThread corThread;

		public uint ID { 
			get{ 
				return id; 
			} 
		}
		
		public bool CurrentExceptionIsHandled {
			get {
				return currentExceptionIsHandled;
			}
			set {
				currentExceptionIsHandled = value;
			}
		}
		
		public ICorDebugThread CorThread {
			get {
				return corThread;
			}
		}

		internal Thread(ICorDebugThread corThread)
		{
			this.corThread = corThread;
			corThread.GetID(out id);
		}

		public bool Suspended {
			get	{
				if (NDebugger.IsProcessRunning) return lastSuspendedState;

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
				if (NDebugger.IsProcessRunning) return lastPriority;

				Variable runTimeVar = runtimeVariable;
				if (runTimeVar is NullRefVariable) return ThreadPriority.Normal;
				lastPriority = (ThreadPriority)(int)(runtimeVariable.SubVariables["m_Priority"] as BuiltInVariable).Value;
				return lastPriority;
			}
		}


		public Variable runtimeVariable {
			get {
				if (NDebugger.IsProcessRunning) throw new UnableToGetPropertyException(this, "runtimeVariable", "Process is running");
				ICorDebugValue corValue;
				corThread.GetObject(out corValue);
				return VariableFactory.CreateVariable(corValue, "Thread" + ID);
			}
		}


		public string Name {
			get	{
				if (NDebugger.IsProcessRunning) return lastName;
				Variable runtimeVar  = runtimeVariable;
				if (runtimeVar is NullRefVariable) return lastName;
				Variable runtimeName = runtimeVar.SubVariables["m_Name"];
				if (runtimeName is NullRefVariable) return string.Empty;
				lastName = runtimeName.Value.ToString();
				return lastName;
			}
		}


		public event ThreadEventHandler ThreadStateChanged;

		internal void OnThreadStateChanged()
		{
			if (ThreadStateChanged != null)
				ThreadStateChanged(this, new ThreadEventArgs(this));
		}


		public override string ToString()
		{
			return String.Format("ID = {0,-10} Name = {1,-20} Suspended = {2,-8}", ID, Name, Suspended);
		}



		public void ClearCurrentException()
		{
			corThread.ClearCurrentException();
		}

		public Exception CurrentException {
			get {
				return new Exception(this);
			}
		}

		public unsafe FunctionCollection Callstack {
			get {
				FunctionCollection callstack = new FunctionCollection();

				if (!NDebugger.IsDebugging)    return callstack;
				if (NDebugger.IsProcessRunning) return callstack;

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
                            callstack.Add(new Function(corFrames[0]));					                                                
                        }
                        catch (COMException) {};
					}
				} // for(;;)
				return callstack;
			} // get
		} // End of public FunctionCollection Callstack
		
		public Function CurrentFunction {
			get {
				if (NDebugger.IsProcessRunning) throw new CurrentFunctionNotAviableException();
				ICorDebugFrame corFrame;
				corThread.GetActiveFrame(out corFrame);
				if (corFrame == null) {
					FunctionCollection callstack = Callstack;
					if (callstack.Count > 0) {
						return callstack[0];
					} else {
						throw new CurrentFunctionNotAviableException();
					}
				}
				return new Function(corFrame);
			}
		}
		
		public void StepInto()
		{
			try {
				CurrentFunction.StepInto();
			} catch (CurrentFunctionNotAviableException) {}
		}		

		public void StepOver()
		{
			try {
				CurrentFunction.StepOver();
			} catch (CurrentFunctionNotAviableException) {}
		}

		public void StepOut()
		{
			try {
				CurrentFunction.StepOut();
			} catch (CurrentFunctionNotAviableException) {}
		}
		
		public SourcecodeSegment NextStatement {
			get {	
				return CurrentFunction.NextStatement;	
			}
		}
		
		public VariableCollection LocalVariables { 
			get {
				try {
					return CurrentFunction.LocalVariables;	
				} catch (NotAviableException) {
					return new VariableCollection();
				}
			}
		}
	}
}
