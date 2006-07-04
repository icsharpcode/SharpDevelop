// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Runtime.InteropServices;

using Debugger.Wrappers.CorDebug;

namespace Debugger
{
	delegate ICorDebugValue[] CorValuesGetter();
	
	public enum EvalState {Pending, Evaluating, EvaluatedSuccessfully, EvaluatedException, EvaluatedNoResult, Error, Expired};
	
	/// <summary>
	/// This class holds information about function evaluation.
	/// </summary>
	public class Eval 
	{
		NDebugger debugger;
		
		ICorDebugEval     corEval;
		ICorDebugFunction corFunction;
		CorValuesGetter   getArgs;
		
		EvalState         evalState = EvalState.Pending;
		Value             result;
		string            error;
		
		DebugeeState debugeeStateWhenEvaluated;
		
		public event EventHandler<EvalEventArgs> EvalStarted;
		public event EventHandler<EvalEventArgs> EvalComplete;
		
		public NDebugger Debugger {
			get {
				return debugger;
			}
		}
		
		public EvalState EvalState {
			get {
				if (result != null && (debugeeStateWhenEvaluated != debugger.DebugeeState || result.IsExpired)) {
					return EvalState.Expired;
				} else {
					return evalState;
				}
			}
		}
		
		/// <summary>
		/// True if the evaluation has been completed.
		/// </summary>
		public bool Evaluated {
			get {
				return this.EvalState != EvalState.Pending &&
				       this.EvalState != EvalState.Evaluating;
			}
		}
		
		public bool HasExpired {
			get {
				return this.EvalState == EvalState.Expired;
			}
		}
		
		/// <summary>
		/// The result of the evaluation. Always non-null, but it may be UnavailableValue.
		/// </summary>
		public Value Result {
			get {
				switch(this.EvalState) {
					case EvalState.Pending: return new UnavailableValue(debugger, "Evaluation pending");
					case EvalState.Evaluating: return new UnavailableValue(debugger, "Evaluating...");
					case EvalState.EvaluatedSuccessfully: return result;
					case EvalState.EvaluatedException:
						ObjectValue exception = (ObjectValue)result;
						while (exception.Type != "System.Exception") exception = exception.BaseClass;
						return new UnavailableValue(debugger, result.Type + ": " + exception["_message"].Value.AsString);
					case EvalState.EvaluatedNoResult: return new UnavailableValue(debugger, "No return value");
					case EvalState.Error: return new UnavailableValue(debugger, error);
					case EvalState.Expired: return new UnavailableValue(debugger, "Result has expired");
					default: throw new DebuggerException("Unknown state");
				}
			}
		}
		
		internal ICorDebugEval CorEval {
			get {
				return corEval;
			}
		}
		
		internal Eval(NDebugger debugger, ICorDebugFunction corFunction, CorValuesGetter getArgs)
		{
			this.debugger = debugger;
			this.corFunction = corFunction;
			this.getArgs = getArgs;
			
			// Schedule the eval for evaluation
			debugger.AddEval(this);
			debugger.MTA2STA.AsyncCall(delegate {
			                           	if (debugger.IsPaused && !this.HasExpired) {
			                           		debugger.StartEvaluation();
			                           	}
			                           });
		}
		
		/// <returns>True is setup was successful</returns>
		internal bool SetupEvaluation(Thread targetThread)
		{
			debugger.AssertPaused();
			
			if (targetThread.IsLastFunctionNative) {
				error = "Can not evaluate because native frame is on top of stack";
				evalState = EvalState.Error;
				if (EvalComplete != null) {
					EvalComplete(this, new EvalEventArgs(this));
				}
				return false;
			}
			
			ICorDebugValue[] args = getArgs();
			
			if (args == null) {
				error = "Can not get args for eval";
				evalState = EvalState.Error;
				if (EvalComplete != null) {
					EvalComplete(this, new EvalEventArgs(this));
				}
				return false;
			}
			
			// TODO: What if this thread is not suitable?
			corEval = targetThread.CorThread.CreateEval();
			
			try {
				corEval.CallFunction(corFunction, (uint)args.Length, args);
			} catch (COMException e) {
				if ((uint)e.ErrorCode == 0x80131C26) {
					error = "Can not evaluate in optimized code";
					evalState = EvalState.Error;
					if (EvalComplete != null) {
						EvalComplete(this, new EvalEventArgs(this));
					}
					return false;
				}
			}
			
			OnEvalStarted(new EvalEventArgs(this));
			
			evalState = EvalState.Evaluating;
			return true;
		}
		
		protected virtual void OnEvalStarted(EvalEventArgs e)
		{
			if (EvalStarted != null) {
				EvalStarted(this, e);
			}
		}
		
		protected internal virtual void OnEvalComplete(bool successful) 
		{
			// Eval result should be ICorDebugHandleValue so it should survive Continue()
			result = Value.CreateValue(debugger, corEval.Result);
			
			debugeeStateWhenEvaluated = debugger.DebugeeState;
			
			if (result == null) {
				evalState = EvalState.EvaluatedNoResult;
			} else {
				if (successful) {
					evalState = EvalState.EvaluatedSuccessfully;
				} else {
					evalState = EvalState.EvaluatedException;
				}
			}
			
			if (EvalComplete != null) {
				EvalComplete(this, new EvalEventArgs(this));
			}
		}
	}
}
