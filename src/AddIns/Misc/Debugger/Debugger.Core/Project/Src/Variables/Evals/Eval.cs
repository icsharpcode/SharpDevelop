// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Debugger.Wrappers.CorDebug;

namespace Debugger
{
	public enum EvalState {WaitingForRequest, EvaluationScheduled, Evaluating, EvaluatedSuccessfully, EvaluatedException, EvaluatedNoResult, EvaluatedError};
	
	/// <summary>
	/// This class holds information about function evaluation.
	/// </summary>
	public class Eval 
	{
		NDebugger debugger;
		
		PersistentValue   pValue;
		
		ICorDebugEval     corEval;
		ICorDebugFunction corFunction;
		bool              reevaluateAfterDebuggeeStateChange;
		PersistentValue   thisValue;
		PersistentValue[] args;
		
		EvalState         evalState = EvalState.WaitingForRequest;
		ICorDebugValue    result;
		DebugeeState      debugeeStateOfResult;
		string            error;
		
		public event EventHandler<EvalEventArgs> EvalStarted;
		public event EventHandler<EvalEventArgs> EvalComplete;
		
		public NDebugger Debugger {
			get {
				return debugger;
			}
		}
		
		public EvalState EvalState {
			get {
				return evalState;
			}
			set {
				evalState = value;
				if (Evaluated) {
					debugeeStateOfResult = debugger.DebugeeState;
					OnEvalComplete(new EvalEventArgs(this));
				}
				pValue.NotifyValueChange();
			}
		}
		
		public bool Evaluated {
			get {
				return evalState == EvalState.EvaluatedSuccessfully ||
				       evalState == EvalState.EvaluatedException ||
				       evalState == EvalState.EvaluatedNoResult ||
				       evalState == EvalState.EvaluatedError;
			}
		}
		
		public PersistentValue PersistentValue {
			get {
				return pValue;
			}
		}
		
		internal ICorDebugEval CorEval {
			get {
				return corEval;
			}
		}
		
		internal Eval(NDebugger debugger, ICorDebugFunction corFunction, bool reevaluateAfterDebuggeeStateChange, PersistentValue thisValue, PersistentValue[] args)
		{
			this.debugger = debugger;
			this.corFunction = corFunction;
			this.reevaluateAfterDebuggeeStateChange = reevaluateAfterDebuggeeStateChange;
			this.thisValue = thisValue;
			this.args = args;
			
			List<PersistentValue> dependencies = new List<PersistentValue>();
			if (thisValue != null) dependencies.Add(thisValue);
			dependencies.AddRange(args);
			
			pValue = new PersistentValue(debugger,
			                             dependencies.ToArray(),
			                             delegate { return GetCorValue(); });
			
			foreach(PersistentValue dependency in dependencies) {
				dependency.ValueChanged += delegate { EvalState = EvalState.WaitingForRequest; };
			}
		}
		
		ICorDebugValue GetCorValue()
		{
			if (Evaluated && reevaluateAfterDebuggeeStateChange && debugger.DebugeeState != debugeeStateOfResult) {
				ScheduleEvaluation();
			}
			
			switch(this.EvalState) {
				case EvalState.WaitingForRequest: ScheduleEvaluation(); goto case EvalState.EvaluationScheduled;
				case EvalState.EvaluationScheduled: throw new CannotGetValueException("Evaluation pending");
				case EvalState.Evaluating: throw new CannotGetValueException("Evaluating...");
				case EvalState.EvaluatedSuccessfully: return result;
				case EvalState.EvaluatedException: return result;
				case EvalState.EvaluatedNoResult: throw new CannotGetValueException("No return value");
				case EvalState.EvaluatedError: throw new CannotGetValueException(error);
				default: throw new DebuggerException("Unknown state");
			}
		}
		
		void ScheduleEvaluation()
		{
			debugger.AddEval(this);
			debugger.MTA2STA.AsyncCall(delegate {
			                           	if (debugger.IsPaused) debugger.StartEvaluation();
			                           });
			EvalState = EvalState.EvaluationScheduled;
		}
		
		/// <returns>True if setup was successful</returns>
		internal bool SetupEvaluation(Thread targetThread)
		{
			debugger.AssertPaused();
			
			if (targetThread.IsLastFunctionNative) {
				OnError("Can not evaluate because native frame is on top of stack");
				return false;
			}
			
			List<ICorDebugValue> corArgs = new List<ICorDebugValue>();
			try {
				if (thisValue != null) {
					Value val = thisValue.Value;
					if (!(val is ObjectValue)) {
						OnError("Can not evaluate on a value which is not an object");
						return false;
					}
					if (!((ObjectValue)val).IsSuperClass(corFunction.Class)) {
						OnError("Can not evaluate because the object does not contain specified function");
						return false;
					}
					corArgs.Add(thisValue.SoftReference);
				}
				foreach(PersistentValue arg in args) {
					corArgs.Add(arg.SoftReference);
				}
			} catch (CannotGetValueException e) {
				OnError(e.Message);
				return false;
			}
			
			// TODO: What if this thread is not suitable?
			corEval = targetThread.CorThread.CreateEval();
			
			try {
				corEval.CallFunction(corFunction, (uint)corArgs.Count, corArgs.ToArray());
			} catch (COMException e) {
				if ((uint)e.ErrorCode == 0x80131C26) {
					OnError("Can not evaluate in optimized code");
					return false;
				}
			}
			
			EvalState = EvalState.Evaluating;
			
			OnEvalStarted(new EvalEventArgs(this));
			
			return true;
		}
		
		void OnError(string msg)
		{
			error = msg;
			EvalState = EvalState.EvaluatedError;
		}
		
		protected virtual void OnEvalStarted(EvalEventArgs e)
		{
			if (EvalStarted != null) {
				EvalStarted(this, e);
			}
		}
		
		protected virtual void OnEvalComplete(EvalEventArgs e)
		{
			if (EvalComplete != null) {
				EvalComplete(this, e);
			}
		}
		
		internal void NotifyEvaluationComplete(bool successful) 
		{
			// Eval result should be ICorDebugHandleValue so it should survive Continue()
			result = corEval.Result;
			
			if (result == null) {
				EvalState = EvalState.EvaluatedNoResult;
			} else {
				if (successful) {
					EvalState = EvalState.EvaluatedSuccessfully;
				} else {
					EvalState = EvalState.EvaluatedException;
				}
			}
		}
	}
}
